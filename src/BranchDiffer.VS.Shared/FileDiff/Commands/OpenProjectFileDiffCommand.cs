using BranchDiffer.VS.Shared.BranchDiff;
using BranchDiffer.VS.Shared.Models;
using BranchDiffer.VS.Shared.Utils;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;

namespace BranchDiffer.VS.Shared.FileDiff.Commands
{
    public sealed class OpenProjectFileDiffCommand : OpenDiffCommand
    {
        private OpenProjectFileDiffCommand(
            IGitBranchDifferPackage package)
            : base(package, new CommandID(GitBranchDifferPackageGuids.guidFileDiffPackageCmdSet, GitBranchDifferPackageGuids.CommandIdProjectFileDiffMenuCommand))
        {
        }


        public static OpenProjectFileDiffCommand Instance { get; private set; }

        public bool IsVisible 
        { 
            get => OleCommandInstance.Visible; 
            set => OleCommandInstance.Visible = value; 
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        public static void Initialize(IGitBranchDifferPackage package)
        {
            Instance = new OpenProjectFileDiffCommand(package);
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
