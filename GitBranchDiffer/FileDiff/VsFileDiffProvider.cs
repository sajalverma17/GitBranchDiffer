using BranchDiffer.Git.Core;
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
                var leftFileMoniker = fileDiffService.GetBaseBranchRevisionOfFile(repo, baseBranchToDiffAgainst, this.activeDocumentPath);
                var rightFileMoniker = this.activeDocumentPath;

                // TODO :1) Labels on left and right files (branch revision name). 2) Comparison window must be brought to focus: window.Show() doesnt work 
                var vsWindowsFrame = vsDifferenceService.OpenComparisonWindow(leftFileMoniker, rightFileMoniker);
                vsWindowsFrame.Show();
            }
            else
            {
                ErrorPresenter.ShowError(error);                
            }
        }
    }
}
