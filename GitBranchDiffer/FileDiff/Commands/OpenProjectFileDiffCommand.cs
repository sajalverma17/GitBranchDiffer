using EnvDTE;
using GitBranchDiffer.Filter;
using GitBranchDiffer.SolutionSelectionModels;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace GitBranchDiffer.FileDiff.Commands
{
    internal sealed class OpenProjectFileDiffCommand : OpenDiffCommand
    {
        private readonly AsyncPackage package;

        private OpenProjectFileDiffCommand(GitBranchDifferPackage package, DTE dte, OleMenuCommandService commandService)
            : base(package,
                 dte,
                 commandService,
                 new CommandID(
                     GitBranchDifferPackageGuids.guidFileDiffPackageCmdSet,
                     GitBranchDifferPackageGuids.CommandIdProjectFileDiffMenuCommand))
        {
        }

        public static OleMenuCommand Instance => OpenDiffCommand.OleCommandInstance;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        public static async Task InitializeAsync(GitBranchDifferPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            DTE dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
            new OpenProjectFileDiffCommand(package, dte, commandService);
        }
    }
}
