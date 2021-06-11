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
        private OpenPhysicalFileDiffCommand(GitBranchDifferPackage package, DTE dte, OleMenuCommandService commandService)
            :base(package,
                 dte,
                 commandService,
                 new CommandID(
                     GitBranchDifferPackageGuids.guidFileDiffPackageCmdSet, 
                     GitBranchDifferPackageGuids.CommandIdPhysicalFileDiffMenuCommand))
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
            new OpenPhysicalFileDiffCommand(package, dte, commandService);
        }
    }
}
