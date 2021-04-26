using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GitBranchDiffer
{
    public static class ErrorPresenter
    {
        public static string PackageInvalidStateError =
            "Intializing GitBranchDiffer filter failed, and it can not be re-initialized again.\n" +
            "This means GitBranchDiffer extension was loaded without a solution in Visual Studio.\n" +
            "To use GitBranchDiffer filter, please restart Visual Studio with a solution pre-selected.";

        public static string PackageNameToDisplay = "Git Branch Differ";

        public static void ShowError(string error)
        {
            ShowError(null, error);
        }

        public static void ShowError(GitBranchDifferPackage package, string errorMsg)
        {
            if (package != null)
            {
                VsShellUtilities.ShowMessageBox(
                        package,
                        errorMsg,
                        PackageNameToDisplay,
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
            else
            {
                MessageBox.Show(
                    errorMsg,
                    PackageNameToDisplay);
            }
        }
    }
}
