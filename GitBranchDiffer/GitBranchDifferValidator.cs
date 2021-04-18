using BranchDiffer.Git.DiffModels;
using BranchDiffer.Git.DiffServices;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GitBranchDiffer
{
    /// <summary>
    /// Helper to make some prelimenary validations on our VSPackage and the Solution opened in Visual Studio by user.
    /// </summary>
    public static class GitBranchDifferValidator
    {
        public static bool ValidatePackage(GitBranchDifferPackage package)
        {
            if (package is null)
            {
                MessageBox.Show(
                    "Unable to load Git Branch Differ plug-in. It is possible Visual Studio is still initializing, please wait and try again.",
                    "Git Branch Differ");

                return false;
            }

            if (package.BranchToDiff is null || package.BranchToDiff == string.Empty)
            {
                VsShellUtilities.ShowMessageBox(
                    package,
                    "Branch to diff against is not set. Go to Options -> Git Branch Differ -> Set \"Branch To Diff Against\"",
                    "Git Branch Differ",
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_CRITICAL,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                return false;
            }

            return true;
        }

        public static bool ValidateSolution(string solutionDirectory, string solutionFile, IVsHierarchyItem rootItemInSolution, GitBranchDifferPackage package)
        {
            if (string.IsNullOrEmpty(solutionDirectory))
            {
                VsShellUtilities.ShowMessageBox(
                    package,
                    "Unable to get Solution directory from Visual Studio services.\n" +
                    "If a solution has been opened, please wait it is fully loaded.",
                    "Git Branch Differ",
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_CRITICAL,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                return false;
            }

            if (string.IsNullOrEmpty(solutionFile))
            {
                VsShellUtilities.ShowMessageBox(
                    package,
                    "Unable to get Solution name from Visual Studio services.\n" +
                    "If a solution has been opened please wait until it is fully loaded.",
                    "Git Branch Differ",
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_CRITICAL,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                return false;
            }

            var solutionFileInHierarchy = System.IO.Path.GetFileName(rootItemInSolution.CanonicalName);
            if (!string.Equals(solutionFile, solutionFileInHierarchy))
            {
                VsShellUtilities.ShowMessageBox(
                    package,
                    $"The solution determined by GiBranchDiffer ({solutionFile}) does not correspond to the solution opened in Solution Explorer ({solutionFileInHierarchy}).\n" +
                    $"If you have previously closed ({solutionFile}) and just opened ({solutionFileInHierarchy}), please wait untill it is fully loaded.\n" +
                    $"If the problem persists, please restart Visual Studio with {solutionFileInHierarchy} as a start-up solution.",
                    "Git Branch Differ",
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_CRITICAL,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                return false;
            }

            return true;
        }
    }
}
