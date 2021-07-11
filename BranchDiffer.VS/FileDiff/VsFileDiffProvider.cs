using BranchDiffer.Git.Core;
using BranchDiffer.Git.DiffModels;
using BranchDiffer.VS.SolutionSelectionModels;
using BranchDiffer.VS.Utils;
using Microsoft;
using Microsoft.VisualStudio.Shell.Interop;

namespace BranchDiffer.VS.FileDiff
{
    /// <summary>
    /// TODO: Make injectible
    /// </summary>
    public class VsFileDiffProvider
    {
        private readonly IVsDifferenceService vsDifferenceService;
        private readonly string DocumentPath;
        private readonly string OldDocumentPath;
        private readonly string solutionPath;

        public VsFileDiffProvider(IVsDifferenceService vsDifferenceService, string solutionPath, SolutionSelectionContainer<ISolutionSelection> selectionContainer)
        {
            this.vsDifferenceService = vsDifferenceService;
            this.solutionPath = solutionPath;
            this.DocumentPath = selectionContainer.FullName;
            this.OldDocumentPath = selectionContainer.OldFullName;
        }

        public void ShowFileDiffWithBaseBranch(string baseBranchToDiffAgainst)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var fileDiffService = DIContainer.Instance.GetService(typeof(GitFileDiffController)) as GitFileDiffController;
            Assumes.Present(fileDiffService);

            if (fileDiffService.SetupRepository(this.solutionPath, baseBranchToDiffAgainst, out var repo, out string error))
            {
                var branchPairs = fileDiffService.GetDiffBranchPair(repo, baseBranchToDiffAgainst);
                var baseBranchFilePath = string.IsNullOrEmpty(this.OldDocumentPath) ? this.DocumentPath : this.OldDocumentPath;

                var leftFileMoniker = fileDiffService.GetBaseBranchRevisionOfFile(repo, baseBranchToDiffAgainst, baseBranchFilePath);
                var rightFileMoniker = this.DocumentPath;
                repo.Dispose();

                this.PresentComparisonWindow(branchPairs, leftFileMoniker, rightFileMoniker);
            }
            else
            {
                ErrorPresenter.ShowError(error);
            }
        }

        private void PresentComparisonWindow(DiffBranchPair branchDiffPair, string leftFileMoniker, string rightFileMoniker)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();            
            var filename = System.IO.Path.GetFileName(this.DocumentPath);
            string leftLabel = $"{filename}@{branchDiffPair.BranchToDiffAgainst.FriendlyName}";
            string rightLabel = $"{filename}@{branchDiffPair.WorkingBranch.FriendlyName}";
            string caption = $"{System.IO.Path.GetFileName(leftFileMoniker)} Vs. {System.IO.Path.GetFileName(rightFileMoniker)}";
            string tooltip = string.Empty;
            string inlineLabel = string.Empty;
            string roles = string.Empty;
            __VSDIFFSERVICEOPTIONS diffServiceOptions = __VSDIFFSERVICEOPTIONS.VSDIFFOPT_LeftFileIsTemporary;
            vsDifferenceService.OpenComparisonWindow2(leftFileMoniker, rightFileMoniker, caption, tooltip, leftLabel, rightLabel, inlineLabel, roles, (uint)diffServiceOptions);
            System.IO.File.Delete(leftFileMoniker);
        }
    }
}
