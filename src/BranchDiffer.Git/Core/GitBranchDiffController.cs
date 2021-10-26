using BranchDiffer.Git.Models;
using BranchDiffer.Git.Services;
using System.Collections.Generic;
using BranchDiffer.Git.Exceptions;

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
        private readonly IGitRepositoryFactory gitRepositoryFactory;

        public GitBranchDiffController(
            IGitRepositoryFactory gitRepositoryFactory,
            IGitDiffService gitBranchDiffService,
            IGitFileService itemIdentityService,
            IGitRepoService gitRepoService)
        {
            this.gitBranchDiffService = gitBranchDiffService;
            this.itemIdentityService = itemIdentityService;
            this.gitRepoService = gitRepoService;
            this.gitRepositoryFactory = gitRepositoryFactory;
        }

        public HashSet<DiffResultItem> GenerateDiff(string repoPath, string branchToDiffAgainst)
        {
            using (var repository = this.gitRepositoryFactory.Create(repoPath))
            {
                if (!gitRepoService.IsRepoStateValid(repository, branchToDiffAgainst, out string userError))
                {
                    throw new GitOperationException(userError);
                }

                var diffBranchPair = gitRepoService.GetBranchesToDiffFromRepo(repository, branchToDiffAgainst);
                return this.gitBranchDiffService.GetDiffedChangeSet(repository, diffBranchPair);
            }
        }

        public DiffResultItem GetItemFromChangeSet(HashSet<DiffResultItem> changeSet, string vsItemAbsolutePath)
        {
            return itemIdentityService.GetFileFromChangeSet(changeSet, vsItemAbsolutePath);
        }
    }
}
