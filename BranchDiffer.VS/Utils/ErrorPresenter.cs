using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BranchDiffer.VS.Utils
{
    public static class ErrorPresenter
    {
        private static string PackageNameToDisplay = "Git Branch Differ";

        public static void ShowError(string error)
        {
            ShowError(null, error);
        }

        public static void ShowError(IServiceProvider package, string errorMsg)
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
