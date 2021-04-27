using BranchDiffer.Git.Core;
using BranchDiffer.Git.DiffModels;
using Microsoft;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.FileDiff
{
    public class VsFileDiffProvider
    {
        private readonly IVsDifferenceService vsDifferenceService;
        private readonly string activeDocumentPath;
        private readonly string solutionPath;

        public VsFileDiffProvider(IVsDifferenceService vsDifferenceService, string solutionPath, string activeDocumentPath)
        {
            this.vsDifferenceService = vsDifferenceService;
            this.solutionPath = solutionPath;
            this.activeDocumentPath = activeDocumentPath;
        }

        public void ShowFileDiffWithBaseBranch(string baseBranchToDiffAgainst)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var fileDiffService = DIContainer.Instance.GetService(typeof(GitFileDiffController)) as GitFileDiffController;
            Assumes.Present(fileDiffService);

            if (fileDiffService.SetupRepository(this.solutionPath, baseBranchToDiffAgainst, out var repo, out string error))
            {
                var branchPairs = fileDiffService.GetDiffBranchPair(repo, baseBranchToDiffAgainst);
                var leftFileMoniker = fileDiffService.GetBaseBranchRevisionOfFile(repo, baseBranchToDiffAgainst, this.activeDocumentPath);                
                var rightFileMoniker = this.activeDocumentPath;
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
            var filename = System.IO.Path.GetFileName(this.activeDocumentPath);
            string leftLabel = $"{filename}@{branchDiffPair.BranchToDiffAgainst.FriendlyName}";
            string rightLabel = $"{filename}@{branchDiffPair.WorkingBranch.FriendlyName}";
            string caption = $"{System.IO.Path.GetFileName(leftFileMoniker)} Vs. {System.IO.Path.GetFileName(leftFileMoniker)}";
            string tooltip = string.Empty;
            string inlineLabel = string.Empty;
            string roles = string.Empty;
            __VSDIFFSERVICEOPTIONS diffServiceOptions = __VSDIFFSERVICEOPTIONS.VSDIFFOPT_DetectBinaryFiles;
            var vsWindowsFrame = vsDifferenceService.OpenComparisonWindow2(leftFileMoniker, rightFileMoniker, caption, tooltip, leftLabel, rightLabel, inlineLabel, roles, (uint)diffServiceOptions);
            vsWindowsFrame.Show();
        }
    }
}
