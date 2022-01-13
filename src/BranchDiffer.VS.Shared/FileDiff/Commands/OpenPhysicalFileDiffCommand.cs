using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.ComponentModel.Design;
using BranchDiffer.VS.Shared.Utils;
using BranchDiffer.VS.Shared.BranchDiff;
using BranchDiffer.VS.Shared.Models;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;

namespace BranchDiffer.VS.Shared.FileDiff.Commands
{
    public sealed class OpenPhysicalFileDiffCommand : OpenDiffCommand
    {
        public OpenPhysicalFileDiffCommand()
        {
        }

        public bool IsVisible 
        { 
            get => OleCommandInstance.Visible; 
            set => OleCommandInstance.Visible = value; 
        }

        /// <summary>
        /// Inits the dependecies needed to execute the command, then register command in VS menu
        /// </summary>
        public async Task InitializeAndRegisterAsync(IGitBranchDifferPackage package, EnvDTE.DTE dte, IVsUIShell vsUIShell)
        {
            await this.InitializeAsync(package, dte, vsUIShell);
            this.Register(new CommandID(GitBranchDifferPackageGuids.guidFileDiffPackageCmdSet, GitBranchDifferPackageGuids.CommandIdPhysicalFileDiffMenuCommand));
        }

        protected override void OpenDiffWindow(object selectedObject)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (selectedObject is ProjectItem)
            {
                var selectedProjectItem = selectedObject as ProjectItem;
                var oldPath = BranchDiffFilterProvider.TagManager.GetOldFilePathFromRenamed(selectedProjectItem);
                var selection = new SolutionSelectionContainer<ISolutionSelection>
                {
                    Item = new SelectedProjectItem { Native = selectedProjectItem, OldFullPath = oldPath }
                };

                this.ShowFileDiffWindow(selection);
            }
        }
    }
}
