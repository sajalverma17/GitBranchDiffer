using BranchDiffer.Git.Configuration;
using BranchDiffer.Git.Core;
using BranchDiffer.VS.Shared.Models;
using BranchDiffer.VS.Shared.Utils;
using BranchDiffer.VS.Shared.Configuration;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace BranchDiffer.VS.Shared.FileDiff.Commands
{
    public abstract class OpenDiffCommand
    {
        private IGitBranchDifferPackage package;
        private DTE dte;
        private IVsDifferenceService vsDifferenceService;
        private IVsUIShell vsUIShell;
        private ErrorPresenter errorPresenter;
        private IMenuCommandService commandService;
        private GitFileDiffController gitFileDiffController;

        protected OpenDiffCommand()
        {
        }

        protected abstract void Execute(object sender, EventArgs e);

        /// <summary>
        /// VS Menu command instance on which visibility is switched.
        /// </summary>
        protected OleMenuCommand OleCommandInstance { get; private set; }

        protected async Task InitializeAsync(IGitBranchDifferPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.dte = await package.GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE ?? throw new ArgumentNullException(nameof(dte));
            this.vsUIShell = await package.GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell ?? throw new ArgumentNullException(nameof(vsUIShell));

            // Dependencies that can be moved to constructor and resolved via IoC...
            this.vsDifferenceService = await package.GetServiceAsync(typeof(SVsDifferenceService)) as IVsDifferenceService ?? throw new ArgumentNullException(nameof(vsDifferenceService));
            this.commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as IMenuCommandService ?? throw new ArgumentNullException(nameof(commandService));
            this.errorPresenter = VsDIContainer.Instance.GetService(typeof(ErrorPresenter)) as ErrorPresenter ?? throw new ArgumentNullException(nameof(errorPresenter));
            this.gitFileDiffController = DIContainer.Instance.GetService(typeof(GitFileDiffController)) as GitFileDiffController ?? throw new ArgumentNullException(nameof(gitFileDiffController));
        }

        protected void Register(CommandID menuCommandId)
        {
            var menuCommand = new OleMenuCommand(this.Execute, menuCommandId);
            OleCommandInstance = menuCommand;
            commandService.AddCommand(menuCommand);
        }

        protected T GetSelectedObjectInSolution<T>()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uih = (UIHierarchy)this.dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;
            Array selectedItems = (Array)uih.SelectedItems;
            if (selectedItems != null && selectedItems.Length == 1)
            {
                var selectedHierarchyItem = selectedItems.GetValue(0) as UIHierarchyItem;
                var selectedObject = selectedHierarchyItem?.Object;
                if (selectedObject != null && selectedObject is T t)
                {
                    return t;
                }
            }

            return default;
        }

        protected void ShowFileDiffWindow(SolutionSelectionContainer<ISolutionSelection> solutionSelectionContainer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!string.IsNullOrEmpty(solutionSelectionContainer.FullName))
            {
                if (solutionSelectionContainer.HasNoAssociatedDiffWindow(vsUIShell))
                {
                    // Create a new diff window if none already open
                    var absoluteSoltuionPath = this.dte.Solution.FullName;
                    var solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSoltuionPath);
                    var fileDiffProvider = new VsFileDiffProvider(this.vsDifferenceService, solutionDirectory, solutionSelectionContainer, this.errorPresenter, this.gitFileDiffController);
                    fileDiffProvider.ShowFileDiffWithBaseBranch(this.package.BranchToDiffAgainst);
                }
                else
                {
                    // Activate already open diff window
                    solutionSelectionContainer.FocusAssociatedDiffWindow(vsUIShell);
                }
            }
        }
    }
}
