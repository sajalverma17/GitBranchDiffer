using BranchDiffer.Git.DiffModels;
using BranchDiffer.Git.DiffServices;
using System;
using System.Collections.Generic;
using LibGit2Sharp;

namespace BranchDiffer.Git.Core
{
    /// <summary>
    /// Worker class that composes all other Git services and performs branch diff with the solution path and branch to diff against provided.
    /// </summary>
    public class GitBranchDiffController
    {
        private readonly IGitDiffService gitBranchDiffService;
        private readonly IGitFileService itemIdentityService;
        private readonly IGitRepoService gitRepoService;

        public GitBranchDiffController(
            IGitDiffService gitBranchDiffService,
            IGitFileService itemIdentityService,
            IGitRepoService gitRepoService)
        {
            this.gitBranchDiffService = gitBranchDiffService;
            this.itemIdentityService = itemIdentityService;
            this.gitRepoService = gitRepoService;
        }

        public bool SetupRepository(string solutionPath, string branchToDiffAgainst, out Repository repository, out string errorMsg)
        {
            if (gitRepoService.CreateGitRepository(solutionPath, out var repo, out var repoCreationException))
            {
                if (gitRepoService.IsRepoStateValid(repo, branchToDiffAgainst, out var repoStateException))
                {
                    errorMsg = string.Empty;
                    repository = repo;
                    return true;
                }

                // Return error
                repository = null;
                repo.Dispose();
                errorMsg = repoStateException.Message;
                return false;
            }

            // Return error
            repository = null;
            repo.Dispose();
            errorMsg = repoCreationException.Message;
            return false;
        }

        public HashSet<DiffResultItem> GenerateDiff(Repository repository, string branchToDiffAgainst)
        {
            var diffBranchPair = gitRepoService.GetBranchesToDiffFromRepo(repository, branchToDiffAgainst);
            return this.gitBranchDiffService.GetDiffedChangeSet(repository, diffBranchPair);
        }

        public bool HasItemInChangeSet(HashSet<DiffResultItem> changeSet, string vsItemAbsolutePath)
        {
            return itemIdentityService.HasFileInChangeSet(changeSet, vsItemAbsolutePath);
        }
    }
}
