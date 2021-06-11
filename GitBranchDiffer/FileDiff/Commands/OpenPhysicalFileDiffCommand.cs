using GitBranchDiffer.Filter;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace GitBranchDiffer.FileDiff.Commands
{
    internal sealed class OpenPhysicalFileDiffCommand : OpenDiffCommand
    {
        private OpenPhysicalFileDiffCommand(
            GitBranchDifferPackage package,
            DTE dte,
            IVsDifferenceService vsDifferenceService,
            IVsUIShell vsUIShell,
            OleMenuCommandService commandService)
            :base(package,
                 dte,
                 vsDifferenceService,
                 vsUIShell,
                 commandService,
                 new CommandID(
                     GitBranchDifferPackageGuids.guidFileDiffPackageCmdSet, 
                     GitBranchDifferPackageGuids.CommandIdPhysicalFileDiffMenuCommand))
        {
        }

        public static OpenPhysicalFileDiffCommand Instance { get; private set; }

        public bool IsVisible { get => OleCommandInstance.Visible; set => OleCommandInstance.Visible = value; }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        public static async Task InitializeAsync(GitBranchDifferPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            DTE dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            IVsDifferenceService vsDifferenceService = await package.GetServiceAsync(typeof(SVsDifferenceService)) as IVsDifferenceService;
            IVsUIShell vsUIShell = await package.GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;
            Instance = new OpenPhysicalFileDiffCommand(package, dte, vsDifferenceService, vsUIShell, commandService);
        }
    }
}
