using BranchDiffer.Git.DiffModels;
using BranchDiffer.Git.DiffServices;
using System;
using System.Collections.Generic;
using LibGit2Sharp;

namespace GitBranchDiffer.ViewModels
{
    /// <summary>
    /// Worker class that composes all other Git services and performs operations that the plugin needs.
    /// </summary>
    public class GitBranchDifferService
    {
        private readonly IGitDiffService gitBranchDiffService;
        private readonly IGitItemIdentityService itemIdentityService;
        private readonly IGitRepoService gitRepoService;

        public GitBranchDifferService(
            IGitDiffService gitBranchDiffService,
            IGitItemIdentityService itemIdentityService,
            IGitRepoService gitRepoService)
        {
            this.gitBranchDiffService = gitBranchDiffService;
            this.itemIdentityService = itemIdentityService;
            this.gitRepoService = gitRepoService;
        }

        public bool SetupRepository(string solutionPath, string branchToDiffAgainst, out Repository repository, out string errorMsg)
        {
            if (this.gitRepoService.CreateGitRepository(solutionPath, out var repo, out var repoCreationException))
            {
                if (this.gitRepoService.IsRepoStateValid(repo, branchToDiffAgainst, out var repoStateException))
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
            var diffBranchPair = this.gitRepoService.GetBranchesToDiffFromRepo(repository, branchToDiffAgainst);
            return this.gitBranchDiffService.GetDiffedChangeSet(repository, diffBranchPair);            
        }

        public bool HasItemInChangeSet(HashSet<DiffResultItem> changeSet, string vsItemAbsolutePath)
        {
            return this.itemIdentityService.HasItemInChangeSet(changeSet, vsItemAbsolutePath);
        }
    }
}
