using BranchDiffer.VS.Shared.BranchDiff;
using BranchDiffer.VS.Shared.Models;
using BranchDiffer.VS.Shared.Utils;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
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
        public async Task InitializeAndRegisterAsync(IGitBranchDifferPackage package)
        {
            await this.InitializeAsync(package);
            this.Register(new CommandID(GitBranchDifferPackageGuids.guidFileDiffPackageCmdSet, GitBranchDifferPackageGuids.CommandIdProjectFileDiffMenuCommand));
            OleCommandInstance.BeforeQueryStatus += OleCommandInstance_BeforeQueryStatus;
        }

        // Check if filter is applied, and if Project node was actually edited in working branch and tagged as "changed", only then make command visibile.
        private void OleCommandInstance_BeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (BranchDiffFilterProvider.IsFilterApplied)
            {
                var selectedProject = this.GetSelectedObjectInSolution<Project>();
                if (selectedProject != null)
                {
                    OleCommandInstance.Visible = BranchDiffFilterProvider.TagManager.IsCsProjEdited(selectedProject);
                    return;
                }
            }

            OleCommandInstance.Visible = false;
        }

        protected override void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var selectedProject = this.GetSelectedObjectInSolution<Project>();
            if (selectedProject != null)
            {                
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
