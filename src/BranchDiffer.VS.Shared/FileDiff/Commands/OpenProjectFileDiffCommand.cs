using BranchDiffer.VS.Shared.BranchDiff;
using BranchDiffer.VS.Shared.Models;
using BranchDiffer.VS.Shared.Utils;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace BranchDiffer.VS.Shared.FileDiff.Commands
{
    public sealed class OpenProjectFileDiffCommand : OpenDiffCommand
    {
        public OpenProjectFileDiffCommand()
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
            this.Register(new CommandID(GitBranchDifferPackageGuids.guidFileDiffPackageCmdSet, GitBranchDifferPackageGuids.CommandIdProjectFileDiffMenuCommand));
        }

        protected override void OpenDiffWindow(object selectedObject)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (selectedObject is Project)
            {
                var selectedProject = selectedObject as Project;
                var oldPath = BranchDiffFilterProvider.TagManager.GetOldFilePathFromRenamed(selectedProject);
                var selection = new SolutionSelectionContainer<ISolutionSelection>
                {
                    Item = new SelectedProject { Native = selectedProject, OldFullPath = oldPath }
                };

                this.ShowFileDiffWindow(selection);
            }
        }
    }
}
