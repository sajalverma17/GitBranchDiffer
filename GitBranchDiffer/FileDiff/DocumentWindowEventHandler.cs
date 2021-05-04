using GitBranchDiffer.Filter;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.FileDiff
{
    public class DocumentWindowEventHandler : IVsRunningDocTableEvents//, IVsRunningDocTableEvents2 //, IVsRunningDocTableEvents3, IVsRunningDocTableEvents4
    {
        private RunningDocumentTable runningDocumentTable;
        private uint openedDocumentId;
        private uint openedDocumentLockType;

        public DocumentWindowEventHandler(RunningDocumentTable runningDocumentTable)
        {
            this.runningDocumentTable = runningDocumentTable;
        }

        int IVsRunningDocTableEvents.OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            this.openedDocumentId = docCookie;
            this.openedDocumentLockType = dwRDTLockType;
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterSave(uint docCookie)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            var documentInfo = this.runningDocumentTable.GetDocumentInfo(this.openedDocumentId);
            ThreadHelper.ThrowIfNotOnUIThread();
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        /*
        private static bool IsVisibleInSolutionHierarchy(EnvDTE.ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uih = (EnvDTE.UIHierarchy)dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;
            EnvDTE.UIHierarchyItem uiHierarchyItem = uih?.FindHierarchyItem(projectItem);

            // If the item is null in the UI (after our filter is applied), we must NOT generate comparison view for this file.
            return uiHierarchyItem != null;
        }

        private static void ShowFileDiffWindow(string openedDocumentFullPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var absoluteSoltuionPath = this.dte.Solution.FullName;
            var solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSoltuionPath);
            var fileDiffProvider = new VsFileDiffProvider(this.vsDifferenceService, solutionDirectory, openedDocumentFullPath);
            fileDiffProvider.ShowFileDiffWithBaseBranch(this.BranchToDiffAgainst);
        }

        // TODO [Bug]: On document open, close the one VS opens as we only want to show comparison. Might be solved by Running Document Table.
        private static void TryFocusDiffWindow(EnvDTE.Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                foreach (EnvDTE.Window window in document.Windows)
                {
                    var caption = window.Caption;
                    if (caption.Contains("Vs."))
                    {
                        window.Activate();
                    }
                    else
                    {
                        window.Close();
                    }
                }
                // document.ActiveWindow.Close(EnvDTE.vsSaveChanges.vsSaveChangesYes);
            }
            catch
            {
            }
        }
        */
    }
}
