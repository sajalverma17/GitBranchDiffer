using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BranchDiffer.VS.Shared.Utils
{
    public class ErrorPresenter
    {
        private const string PackageNameToDisplay = "Git Branch Differ";

        public void ShowError(string error)
        {
            MessageBox.Show(
                error,
                PackageNameToDisplay);
        }
    }
}
