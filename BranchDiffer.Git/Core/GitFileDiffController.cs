using BranchDiffer.Git.Models;
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
    public class GitFileDiffController
    {
        private readonly IGitDiffService gitDiffService;
        private readonly IGitRepoService gitRepoService;
        private readonly IGitFileService gitFileService;

        public GitFileDiffController(
            IGitDiffService gitDiffService,
            IGitRepoService gitRepoService,
            IGitFileService gitFileService)
        {
            this.gitDiffService = gitDiffService;
            this.gitRepoService = gitRepoService;
            this.gitFileService = gitFileService;
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

        public DiffBranchPair GetDiffBranchPair(Repository repository, string branchNameToDiffAgainst)
        {
            return this.gitRepoService.GetBranchesToDiffFromRepo(repository, branchNameToDiffAgainst);
        }

        // Creates and returns a temp file on disk. Contents of this temp file are the contents of the VS active document file under Base Branch tree.
        public string GetBaseBranchRevisionOfFile(Repository repository, string branchToDiffAgainst, string activeVsDocumentPath)
        {
            var tempFilePath = this.gitFileService.GetBaseBranchRevisionOfFile(repository, branchToDiffAgainst, activeVsDocumentPath);            
            return tempFilePath;
        }
    }
}
