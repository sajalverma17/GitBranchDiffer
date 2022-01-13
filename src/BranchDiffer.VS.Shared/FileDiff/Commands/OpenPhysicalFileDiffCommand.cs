using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.ComponentModel.Design;
using BranchDiffer.VS.Shared.Utils;
using BranchDiffer.VS.Shared.BranchDiff;
using BranchDiffer.VS.Shared.Models;

namespace BranchDiffer.VS.Shared.FileDiff.Commands
{
    public sealed class OpenPhysicalFileDiffCommand : OpenDiffCommand
    {
        private OpenPhysicalFileDiffCommand(IGitBranchDifferPackage package)
            : base(package, new CommandID(GitBranchDifferPackageGuids.guidFileDiffPackageCmdSet, GitBranchDifferPackageGuids.CommandIdPhysicalFileDiffMenuCommand))
        {
        }

        public static OpenPhysicalFileDiffCommand Instance { get; private set; }

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
            Instance = new OpenPhysicalFileDiffCommand(package);
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
