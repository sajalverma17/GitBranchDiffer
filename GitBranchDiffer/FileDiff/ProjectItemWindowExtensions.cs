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
    internal static class ProjectItemWindowExtensions
    {
        /// <summary>
        /// Here we iterated over all open windows in UI shell, and see if there are any window frames with: 
        /// (A) The same as the project item document fullName && (B) a DifferenceCodeWindowType.
        /// </summary>
        /// <remark>
        /// If this proves to be too slow for a large number of documents open in UIShell,
        /// just iterate over the projectItem.Document.Windows - and check if any Window.Caption.Contains("Vs. {projectItem.Name}")
        /// </remark>
        public static bool HasNoAssociatedDiffWindow(this EnvDTE.ProjectItem projectItem, IVsUIShell vsUIShell)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (projectItem.Document == null)
            {
                return true;
            }

            if (projectItem.Document.Windows.Count == 0)
            {
                // No windows at all...
                return true;
            }

            bool diffWindowExistsForDocument = false;
            foreach (IVsWindowFrame vsWindowFrame in GetAllWindowFramesFromShell(vsUIShell))
            {
                // check if the frame is (A) The same as the project item document AND (B) a DifferenceCodeWindowType
                var frameImpl = vsWindowFrame as WindowFrame;
                if (frameImpl != null
                    && frameImpl.IsDocument
                    && frameImpl.DocumentMoniker.Equals(projectItem.Document.FullName, StringComparison.OrdinalIgnoreCase)
                    && IsDiffWindowFrame(frameImpl))
                {
                    diffWindowExistsForDocument = true;
                    break;
                }
            }

            return !diffWindowExistsForDocument;
        }

        public static void FocusAssociatedDiffWindow(this EnvDTE.ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (projectItem.Document?.Windows.Count == 1)
            {
                var enumerator = projectItem.Document?.Windows.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var currentWindow = enumerator.Current as EnvDTE.Window;
                    currentWindow?.Activate();
                }
            }
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
