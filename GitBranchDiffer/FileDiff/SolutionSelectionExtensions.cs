using GitBranchDiffer.SolutionSelectionModels;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Platform.WindowManagement;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.FileDiff
{
    internal static class SolutionSelectionExtensions
    {
        /// <summary>
        /// Here we iterate over all open window frames in UI Shell, and check if a frame has the same name as this projectItem, and
        /// if that frame is of type DifferenceCodeWindow.
        /// </summary>
        /// <param name="projectItem"></param>
        /// <param name="vsUIShell"></param>
        /// <remarks>
        /// Note that if this method proves to be too slow for large number of document frames opened in UI shell: 
        /// We can just checks for Window captions of the windows in given projectItem: projectItem.Document.Windows.
        /// Any window with caption that contains the string: "Vs. {projectItem.Document.Name}" has got to be a diff window associated with this item.
        /// </remarks>
        public static bool HasNoAssociatedDiffWindow(this SolutionSelectionContainer<ISolutionSelection> selectedItem, IVsUIShell vsUIShell)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Specific checks for EnvDTE.ProjectItem so it becomes easy to return early
            if (selectedItem.IsProjectItem)
            {
                if (selectedItem.Item.Document == null)
                {
                    return true;
                }

                if (selectedItem.Item.Document.Windows.Count == 0)
                {
                    // No windows at all...
                    return true;
                }
            }

            bool diffWindowExistsForDocument = false;
            foreach (IVsWindowFrame vsWindowFrame in GetAllWindowFramesFromShell(vsUIShell))
            {
                // check if the frame is (A) The same as the selected item name AND (B) is a DifferenceCodeWindowType
                var frameImpl = vsWindowFrame as WindowFrame;
                if (frameImpl != null
                    && frameImpl.IsDocument
                    && frameImpl.DocumentMoniker.Equals(selectedItem.FullName, StringComparison.OrdinalIgnoreCase)
                    && IsDiffWindowFrame(frameImpl))
                {
                    diffWindowExistsForDocument = true;
                    break;
                }
            }

            return !diffWindowExistsForDocument;
        }

        public static void FocusAssociatedDiffWindow(this SolutionSelectionContainer<ISolutionSelection> selectedItem, IVsUIShell vsUIShell)
        {
            ThreadHelper.ThrowIfNotOnUIThread(); 
            foreach (IVsWindowFrame vsWindowFrame in GetAllWindowFramesFromShell(vsUIShell))
            {
                // check if the frame is (A) The same as the selected item name AND (B) is a DifferenceCodeWindowType
                var frameImpl = vsWindowFrame as WindowFrame;
                if (frameImpl != null
                    && frameImpl.IsDocument
                    && frameImpl.DocumentMoniker.Equals(selectedItem.FullName, StringComparison.OrdinalIgnoreCase)
                    && IsDiffWindowFrame(frameImpl))
                {
                    frameImpl.Show();
                    break;
                }
            }
        }

        /// <summary>
        /// Only show diff for these ProjectItems and Projects as of now.
        /// </summary>
        /// <param name="projectItem"></param>
        /// <returns></returns>
        public static bool IsSupported(this SolutionSelectionContainer<ISolutionSelection> selectedItemContainer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return selectedItemContainer.IsProject
                || selectedItemContainer.VsItemKind == EnvDTE.Constants.vsProjectItemKindPhysicalFile;
        }

        private static List<IVsWindowFrame> GetAllWindowFramesFromShell(IVsUIShell vsUIShell)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var allWindowFrames = new List<IVsWindowFrame>();
            ErrorHandler.ThrowOnFailure(vsUIShell.GetDocumentWindowEnum(out var windowEnumerator));
            if (windowEnumerator.Reset() != VSConstants.S_OK)
            {
                // If error reseting enumerator, open a diff window regardless
                return allWindowFrames;
            }

            var tempFrameList = new IVsWindowFrame[1];
            bool hasMorewindows;
            do
            {
                hasMorewindows = windowEnumerator.Next(1, tempFrameList, out var fetched) == VSConstants.S_OK && fetched == 1;

                if (!hasMorewindows || tempFrameList[0] == null)
                    continue;

                allWindowFrames.Add(tempFrameList[0]);

            } while (hasMorewindows);

            return allWindowFrames;
        }

        private static bool IsDiffWindowFrame(IVsWindowFrame frame)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ErrorHandler.Succeeded(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out object docView));
            return docView is IVsDifferenceCodeWindow;
        }
    }
}
