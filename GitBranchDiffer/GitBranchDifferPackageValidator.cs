using BranchDiffer.Git.DiffModels;
using BranchDiffer.Git.DiffServices;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GitBranchDiffer
{
    /// <summary>
    /// Helper to validate initial state of plugin's package
    /// </summary>
    public static class GitBranchDifferPackageValidator
    {
        public static bool Validate(GitBranchDifferPackage package)
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

            /* TODO: Get Git Repo instance here (via a service or VS SourceControl info provider?) and make more Git-specific validations
             * For eg. (1) Branch set in Plugin options should not be identical as the Git HEAD
             *         (2) Below validation:
            if (this.branchToDiffWith is not found in the active repo)
            {
                VsShellUtilities.ShowMessageBox(
                    this.gitBranchDifferPackage,
                    $"{this.branchToDiffWith} branch is not found in this Repo", 
                    "Git Branch Differ")
            }*/

            return true;
        }
    }
}
