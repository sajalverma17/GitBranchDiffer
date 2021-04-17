using BranchDiffer.Git.DiffModels;
using BranchDiffer.Git.DiffServices;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitBranchDiffer.ViewModels
{
    public class BranchDiffViewModel
    {
        private readonly IGitBranchDiffService gitBranchDiffService;
        private GitBranchDifferPackage gitBranchDifferPackage;

        public BranchDiffViewModel(IGitBranchDiffService gitBranchDiffService)
        {
            this.gitBranchDiffService = gitBranchDiffService;
        }

        public void Init(GitBranchDifferPackage package)
        {
            this.gitBranchDifferPackage = package;
        }

        public bool Validate()
        {
            /* TODO: Get Git Repo instance here and make more Git-specific validations
             * For eg. Branch set in Plugin options should not be identical as the Git HEAD
             *         Below validation:
            if (this.branchToDiffWith is not found in the active repo)
            {
                VsShellUtilities.ShowMessageBox(
                    this.gitBranchDifferPackage,
                    $"{this.branchToDiffWith} branch is not found in this Repo", 
                    "Git Branch Differ")
            }*/

            if (this.gitBranchDifferPackage is null)
            {
                MessageBox.Show(
                    "Unable to load Git Branch Differ extension package. It is possible the soltuion is not completely loaded, please wait and try again.",
                    "Git Branch Differ");

                return false;
            }

            if (this.gitBranchDifferPackage.BranchToDiff is null || this.gitBranchDifferPackage.BranchToDiff == string.Empty)
            {
                VsShellUtilities.ShowMessageBox(
                    this.gitBranchDifferPackage,
                    "Branch to diff against is not set. Go to Options -> Git Branch Differ -> Set \"Branch To Diff Against\"",
                    "Git Branch Differ",
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_CRITICAL,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                return false;
            } 

            return true;
        }

        // TODO : Make Async, or directly call the serivice from BranchDiffFilterProvider
        public HashSet<DiffResultItem> Generate()
        {
            var branchToDiffWith = this.gitBranchDifferPackage.BranchToDiff;
            var changeList = this.gitBranchDiffService.GetDiffFileNames(@"C:\Tools\ProjectUnderTest", branchToDiffWith);
            return changeList;
        }
    }
}
