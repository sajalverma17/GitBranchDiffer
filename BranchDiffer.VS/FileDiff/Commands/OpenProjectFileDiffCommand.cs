using BranchDiffer.VS.Utils;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace BranchDiffer.VS.FileDiff.Commands
{
    public sealed class OpenProjectFileDiffCommand : OpenDiffCommand
    {
        private OpenProjectFileDiffCommand(
            IGitBranchDifferPackage package,
            DTE dte, 
            IVsDifferenceService vsDifferenceService,
            IVsUIShell vsUIShell,
            OleMenuCommandService commandService)
            : base(package,
                 dte,
                 vsDifferenceService,
                 vsUIShell,
                 commandService,
                 new CommandID(
                     GitBranchDifferPackageGuids.guidFileDiffPackageCmdSet,
                     GitBranchDifferPackageGuids.CommandIdProjectFileDiffMenuCommand))
        {
        }


        public static OpenProjectFileDiffCommand Instance { get; private set; }

        public bool IsVisible { get => OleCommandInstance.Visible; set => OleCommandInstance.Visible = value; }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        public static async Task InitializeAsync(IGitBranchDifferPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.CancellationToken);

            DTE dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            IVsDifferenceService vsDifferenceService = await package.GetServiceAsync(typeof(SVsDifferenceService)) as IVsDifferenceService;
            IVsUIShell vsUIShell = await package.GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;
            Instance = new OpenProjectFileDiffCommand(package, dte, vsDifferenceService, vsUIShell, commandService);
        }
    }
}
