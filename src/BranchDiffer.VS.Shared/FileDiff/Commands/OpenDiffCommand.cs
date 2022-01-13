using BranchDiffer.Git.Configuration;
using BranchDiffer.Git.Core;
using BranchDiffer.VS.Shared.BranchDiff;
using BranchDiffer.VS.Shared.Configuration;
using BranchDiffer.VS.Shared.Models;
using BranchDiffer.VS.Shared.Utils;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace BranchDiffer.VS.Shared.FileDiff.Commands
{
    public abstract class OpenDiffCommand
    {
        private readonly IGitBranchDifferPackage package;
        private readonly DTE dte;
        private readonly IVsDifferenceService vsDifferenceService;
        private readonly IVsUIShell vsUIShell;
        private readonly ErrorPresenter errorPresenter;
        private readonly IMenuCommandService commandService;
        private readonly GitFileDiffController gitFileDiffController;

        public OpenDiffCommand(IGitBranchDifferPackage package, CommandID menuCommandId)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.vsDifferenceService = package.GetService(typeof(SVsDifferenceService)) as IVsDifferenceService ?? throw new ArgumentNullException(nameof(vsDifferenceService));
            this.commandService = package.GetService(typeof(IMenuCommandService)) as IMenuCommandService ?? throw new ArgumentNullException(nameof(commandService));
            this.dte = package.GetService(typeof(DTE)) as DTE ?? throw new ArgumentNullException(nameof(dte));
            this.vsUIShell = package.GetService(typeof(SVsUIShell)) as IVsUIShell ?? throw new ArgumentNullException(nameof(vsUIShell));
            this.errorPresenter = VsDIContainer.Instance.GetService(typeof(ErrorPresenter)) as ErrorPresenter ?? throw new ArgumentNullException(nameof(errorPresenter));
            this.gitFileDiffController = DIContainer.Instance.GetService(typeof(GitFileDiffController)) as GitFileDiffController ?? throw new ArgumentNullException(nameof(gitFileDiffController));

            var menuCommand = new OleMenuCommand(this.Execute, menuCommandId);
            OleCommandInstance = menuCommand;
            commandService.AddCommand(menuCommand);
        }

        /// <summary>
        /// VS Menu command instance on which visibility is switched.
        /// </summary>
        protected OleMenuCommand OleCommandInstance { get; private set; }

        protected abstract void OpenDiffWindow(object selectedObject);

        protected void ShowFileDiffWindow(SolutionSelectionContainer<ISolutionSelection> solutionSelectionContainer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!string.IsNullOrEmpty(solutionSelectionContainer.FullName))
            {
                if (solutionSelectionContainer.HasNoAssociatedDiffWindow(this.vsUIShell))
                {
                    // Create a new diff window if none already open
                    var absoluteSoltuionPath = this.dte.Solution.FullName;
                    var solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSoltuionPath);
                    var fileDiffProvider = new VsFileDiffProvider(this.vsDifferenceService, solutionDirectory, solutionSelectionContainer, this.errorPresenter, this.gitFileDiffController);
                    fileDiffProvider.ShowFileDiffWithBaseBranch(package.BranchToDiffAgainst);
                }
                else
                {
                    // Activate already open diff window
                    solutionSelectionContainer.FocusAssociatedDiffWindow(this.vsUIShell);
                }
            }
        }

        // TODO: We don't need to get selected items if there is a way to capture menu button's target item and it's info in VS API
        private void Execute(object sender, EventArgs e)
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
                    this.OpenDiffWindow(selectedObject);
                }
            }
        }
    }
}
