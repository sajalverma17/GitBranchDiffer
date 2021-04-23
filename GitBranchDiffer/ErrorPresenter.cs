using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer
{
    public static class ErrorPresenter
    {
        public static string PackageInvalidStateError =
            "Intializing GitBranchDiffer filter failed, and it can not be re-initialized again.\n" +
            "This means GitBranchDiffer extension was loaded without a solution in Visual Studio.\n" +
            "To use GitBranchDiffer filter, please restart Visual Studio with a solution pre-selected.";

        public static void ShowError(GitBranchDifferPackage package, string errorMsg)
        {
            VsShellUtilities.ShowMessageBox(
                    package,
                    errorMsg,
                    "Git Branch Differ",
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
