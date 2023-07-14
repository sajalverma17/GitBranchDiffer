using BranchDiffer.Git.Core;
using BranchDiffer.Git.Exceptions;
using BranchDiffer.Git.Models;
using BranchDiffer.Git.Models.LibGit2SharpModels;
using BranchDiffer.VS.Shared.Models;
using BranchDiffer.VS.Shared.Utils;
using Microsoft.VisualStudio.Shell.Interop;

namespace BranchDiffer.VS.Shared.FileDiff
{
    // TODO: Make this injectible
    public class VsFileDiffProvider
    {
        private readonly string DocumentPath;
        private readonly string OldDocumentPath;
        private readonly string solutionPath;

        // Resolvable dependencies...
        private readonly ErrorPresenter errorPresenter;
        private readonly IVsDifferenceService vsDifferenceService;
        private readonly GitFileDiffController gitFileDiffController;

        public VsFileDiffProvider(
            IVsDifferenceService vsDifferenceService, 
            string solutionPath, 
            SolutionSelectionContainer<ISolutionSelection> selectionContainer, 
            ErrorPresenter errorPresenter, 
            GitFileDiffController gitFileDiffController)
        {
            this.vsDifferenceService = vsDifferenceService;
            this.solutionPath = solutionPath;
            this.errorPresenter = errorPresenter;
            this.gitFileDiffController = gitFileDiffController;
            this.DocumentPath = selectionContainer.FullName;
            this.OldDocumentPath = selectionContainer.OldFullName;
        }

        public void ShowFileDiffWithBaseBranch(IGitObject gitObjectToDiffAgainst)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            // Get branch pairs to diff and Get the revision of file in the branch-to-diff-against
            try
            {
                var branchPairs = this.gitFileDiffController.GetDiffBranchPair(this.solutionPath, gitObjectToDiffAgainst);
                var baseBranchFilePath = string.IsNullOrEmpty(this.OldDocumentPath) ? this.DocumentPath : this.OldDocumentPath;
                var leftFileMoniker = this.gitFileDiffController.GetBaseBranchRevisionOfFile(this.solutionPath, gitObjectToDiffAgainst, baseBranchFilePath);
                var rightFileMoniker = this.DocumentPath;

                this.PresentComparisonWindow(branchPairs, leftFileMoniker, rightFileMoniker);
            }
            catch (GitOperationException e)
            {
                this.errorPresenter.ShowError(e.Message);
            }
        }

        // TODO: When file are renamed in working branch, left-file should be labled as with old-file-name@base-branch.
        private void PresentComparisonWindow(DiffBranchPair branchDiffPair, string leftFileMoniker, string rightFileMoniker)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();            
            var filename = System.IO.Path.GetFileName(this.DocumentPath);
            string leftLabel = $"{filename}@{branchDiffPair.BranchToDiffAgainst.Name}";
            string rightLabel = $"{filename}@{branchDiffPair.WorkingBranch.Name}";
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
