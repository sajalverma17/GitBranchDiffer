using BranchDiffer.Git.Models;
using BranchDiffer.Git.Services;
using System;
using System.Collections.Generic;
using LibGit2Sharp;
using BranchDiffer.Git.Exceptions;

namespace BranchDiffer.Git.Core
{
    /// <summary>
    /// Worker class that composes all other Git services and performs branch diff with the solution path and branch to diff against provided.
    /// </summary>
    public class GitBranchDiffController : ControllerBase
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

        public HashSet<DiffResultItem> GenerateDiff(string solutionPath, string branchToDiffAgainst)
        {
            using (var repository = this.SetupRepository(solutionPath))
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
