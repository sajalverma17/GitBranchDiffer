using BranchDiffer.Git.Models;
using BranchDiffer.Git.Services;

namespace BranchDiffer.Git.Core
{
    /// <summary>
    /// Worker class that composes Git-services to generate a file diff of the provided document opened in VS.
    /// </summary>
    public class GitFileDiffController
    {
        private readonly IGitRepoService gitRepoService;
        private readonly IGitFileService gitFileService;
        private readonly IGitRepositoryFactory gitRepositoryFactory;

        public GitFileDiffController(
            IGitRepositoryFactory gitRepositoryFactory,
            IGitRepoService gitRepoService,
            IGitFileService gitFileService)
        {
            this.gitRepoService = gitRepoService;
            this.gitFileService = gitFileService;
            this.gitRepositoryFactory = gitRepositoryFactory;
        }

        public DiffBranchPair GetDiffBranchPair(string solutionPath, string branchToDiffAgainst)
        {
            using (var repository = this.gitRepositoryFactory.Create(solutionPath))
            {
                return this.gitRepoService.GetBranchesToDiffFromRepo(repository, branchToDiffAgainst);
            }
        }

        // Creates and returns a temp file on disk. Contents of this temp file are the contents of the VS active document file under Base Branch tree.
        public string GetBaseBranchRevisionOfFile(string solutionPath, string branchToDiffAgainst, string activeVsDocumentPath)
        {
            using (var repository = this.gitRepositoryFactory.Create(solutionPath))
            {
                var diffBranchPair = this.gitRepoService.GetBranchesToDiffFromRepo(repository, branchToDiffAgainst);
                return this.gitFileService.GetBaseBranchRevisionOfFile(repository, diffBranchPair.BranchToDiffAgainst, activeVsDocumentPath);
            }
        }
    }
}
