using BranchDiffer.VS.BranchDiff;
using BranchDiffer.VS.SolutionSelectionModels;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;

namespace BranchDiffer.VS.FileDiff.Commands
{
    public abstract class OpenDiffCommand
    {
        private readonly IGitBranchDifferPackage package;
        private readonly DTE dte;
        private readonly IVsDifferenceService vsDifferenceService;
        private readonly IVsUIShell vsUIShell;

        public OpenDiffCommand(
            IGitBranchDifferPackage package,
            DTE dte, 
            IVsDifferenceService vsDifferenceService,
            IVsUIShell vsUIShell,
            OleMenuCommandService commandService,
            CommandID menuCommandId)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.dte = dte ?? throw new ArgumentNullException(nameof(dte));
            this.vsDifferenceService = vsDifferenceService ?? throw new ArgumentNullException(nameof(vsDifferenceService));
            this.vsUIShell = vsUIShell ?? throw new ArgumentNullException(nameof(vsUIShell));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommand = new OleMenuCommand(this.Execute, menuCommandId);
            OleCommandInstance = menuCommand;
            commandService.AddCommand(menuCommand);
        }

        protected OleMenuCommand OleCommandInstance { get; private set; }

        // TODO: We don't need to get selected items if there is a way to capture menu button's target item and it's info
        protected virtual void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uih = (UIHierarchy)this.dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;
            Array selectedItems = (Array)uih.SelectedItems;
            if (selectedItems != null && selectedItems.Length == 1)
            {
                var selectedHierarchyItem = selectedItems.GetValue(0) as UIHierarchyItem;
                var selectedObject = selectedHierarchyItem?.Object;

                if (selectedObject != null)
                {
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
                    else if (selectedObject is Project)
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

        private void ShowFileDiffWindow(SolutionSelectionContainer<ISolutionSelection> solutionSelectionContainer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!string.IsNullOrEmpty(solutionSelectionContainer.FullName))
            {
                if (solutionSelectionContainer.HasNoAssociatedDiffWindow(this.vsUIShell))
                {
                    // Create a new diff window if none already open
                    var absoluteSoltuionPath = this.dte.Solution.FullName;
                    var solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSoltuionPath);
                    var fileDiffProvider = new VsFileDiffProvider(this.vsDifferenceService, solutionDirectory, solutionSelectionContainer);
                    fileDiffProvider.ShowFileDiffWithBaseBranch(package.BranchToDiffAgainst);
                }
                else
                {
                    // Activate already open diff window
                    solutionSelectionContainer.FocusAssociatedDiffWindow(this.vsUIShell);
                }
            }
        }
    }
}
