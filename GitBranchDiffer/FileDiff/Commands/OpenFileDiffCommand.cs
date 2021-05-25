using EnvDTE;
using GitBranchDiffer.Filter;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.FileDiff.Commands
{
    public class OpenFileDiffCommand
    {
        private readonly Package package;
        private readonly DTE dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseTabsToLeftCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private OpenFileDiffCommand(Package package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.package = package;
            var serviceProvider = this.package as IServiceProvider;
            this.dte = serviceProvider.GetService(typeof(DTE)) as DTE;

            if (serviceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var physicalFileDiffCommandId = new CommandID(BranchDiffFilterCommandGuids.guidFileDiffPackageCmdSet, BranchDiffFilterCommandGuids.CommandIdPhysicalFileDiffMenuCommand);
                var physicalFileDiffMenuCommand = new MenuCommand(this.OpenFileDiff, physicalFileDiffCommandId);
                commandService.AddCommand(physicalFileDiffMenuCommand);

                var projectFileDiffCommandId = new CommandID(BranchDiffFilterCommandGuids.guidFileDiffPackageCmdSet, BranchDiffFilterCommandGuids.CommandIdProjectFileDiffMenuCommand);
                var projectFileDiffMenuCommand = new MenuCommand(this.OpenFileDiff, projectFileDiffCommandId);
                commandService.AddCommand(projectFileDiffMenuCommand);
            }
        }

        public static void Initialize(Package package)
        {
            CommandInstance = new OpenFileDiffCommand(package);
        }

        private static OpenFileDiffCommand CommandInstance
        {
            get;
            set;
        }

        private void OpenFileDiff(object sender, EventArgs e)
        {
            ErrorPresenter.ShowError("Clicked file diff menu command");
        }
    }
}
