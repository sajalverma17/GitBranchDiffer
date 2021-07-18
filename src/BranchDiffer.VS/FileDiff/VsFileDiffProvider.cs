using BranchDiffer.Git.Core;
using BranchDiffer.Git.Exceptions;
using BranchDiffer.Git.Models;
using BranchDiffer.VS.Models;
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
            var fileDiffController = DIContainer.Instance.GetService(typeof(GitFileDiffController)) as GitFileDiffController;
            Assumes.Present(fileDiffController);

            // Get branch pairs to diff and Get the revision of file in the branch-to-diff-against
            try
            {
                var branchPairs = fileDiffController.GetDiffBranchPair(this.solutionPath, baseBranchToDiffAgainst);
                var baseBranchFilePath = string.IsNullOrEmpty(this.OldDocumentPath) ? this.DocumentPath : this.OldDocumentPath;
                var leftFileMoniker = fileDiffController.GetBaseBranchRevisionOfFile(this.solutionPath, baseBranchToDiffAgainst, baseBranchFilePath);
                var rightFileMoniker = this.DocumentPath;

                this.PresentComparisonWindow(branchPairs, leftFileMoniker, rightFileMoniker);
            }
            catch (GitBranchException e)
            {
                ErrorPresenter.ShowError(e.Message);
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
