using BranchDiffer.Git.DiffServices;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.ViewModels
{
    public class BranchDiffViewModel
    {
        private readonly IGitBranchDiffService gitBranchDiffService;
        private string branchToDiffWith;
        private GitBranchDifferPackage gitBranchDifferPackage;

        public const string Repo = @"C:\Tools\ProjectUnderTest";

        public ObservableCollection<string> ModifiedFileList
        {
            get;
            set;
        }

        public BranchDiffViewModel(IGitBranchDiffService gitBranchDiffService)
        {
            this.gitBranchDiffService = gitBranchDiffService;
        }

        public void Init(GitBranchDifferPackage package, string branchToDiffWith)
        {
            this.branchToDiffWith = branchToDiffWith;
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

            if (this.branchToDiffWith is null || this.branchToDiffWith == string.Empty)
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

        public void Generate()
        {
            var changeList = this.gitBranchDiffService.GetDiffFileNames(Repo, branchToDiffWith);
            this.ModifiedFileList = new ObservableCollection<string>(changeList);
        }
    }
}
