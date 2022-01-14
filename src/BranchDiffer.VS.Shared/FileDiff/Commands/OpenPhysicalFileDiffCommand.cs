using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.ComponentModel.Design;
using BranchDiffer.VS.Shared.Utils;
using BranchDiffer.VS.Shared.BranchDiff;
using BranchDiffer.VS.Shared.Models;
using Task = System.Threading.Tasks.Task;
using System;

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
        public async Task InitializeAndRegisterAsync(IGitBranchDifferPackage package)
        {
            await this.InitializeAsync(package);
            this.Register(new CommandID(GitBranchDifferPackageGuids.guidFileDiffPackageCmdSet, GitBranchDifferPackageGuids.CommandIdPhysicalFileDiffMenuCommand));
            OleCommandInstance.BeforeQueryStatus += OleCommandInstance_BeforeQueryStatus;
        }

        private void OleCommandInstance_BeforeQueryStatus(object sender, EventArgs e)
        {
            if (BranchDiffFilterProvider.IsFilterApplied)
            {
                OleCommandInstance.Visible = true;
                return;
            }

            OleCommandInstance.Visible = false;
        }

        protected override void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var selectedProjectItem = this.GetSelectedObjectInSolution<ProjectItem>();
            if (selectedProjectItem != null)
            {                
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
