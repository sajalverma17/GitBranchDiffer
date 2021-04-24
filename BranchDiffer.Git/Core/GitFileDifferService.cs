using BranchDiffer.Git.DiffModels;
using BranchDiffer.Git.DiffServices;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Core
{
    /// <summary>
    /// Worker class that composes Git-services to generate a file diff of the provided document opened in VS.
    /// </summary>
    public class GitFileDifferService
    {
        private readonly IGitDiffService gitDiffService;
        private readonly IGitRepoService gitRepoService;

        public GitFileDifferService(
            IGitDiffService gitDiffService,
            IGitRepoService gitRepoService)
        {
            this.gitDiffService = gitDiffService;
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

        public IEnumerable<HunkRangeInfo> GenerateDiff(Repository repository, string branchToDiffAgainst, string filePath)
        {
            var diffBranchPair = gitRepoService.GetBranchesToDiffFromRepo(repository, branchToDiffAgainst);
            return this.gitDiffService.GetFileDiff(repository, diffBranchPair, filePath);
        }
    }
}
