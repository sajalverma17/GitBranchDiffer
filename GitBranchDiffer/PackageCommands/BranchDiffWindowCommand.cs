using GitBranchDiffer.View;
using GitBranchDiffer.ViewModels;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace GitBranchDiffer.PackageCommands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class BranchDiffWindowCommand
    {
        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        
        public const string guidBranchDiffWindowPackageCmdSet = "91fdc0b8-f3f8-4820-9734-1721db207258";
        public const int CommandIdGenerateDiff = 0x132;
        public const int BranchDiffToolbarId = 0x1000;


        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static BranchDiffWindowCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return package;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchDiffWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private BranchDiffWindowCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            // Add generate diff handler
            var generateDiffToolbarCommandId = new CommandID(new Guid(guidBranchDiffWindowPackageCmdSet), CommandIdGenerateDiff);
            var generateDiffCommandItem = new MenuCommand(new EventHandler(
                Execute), generateDiffToolbarCommandId);
            commandService.AddCommand(generateDiffCommandItem);
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in BranchDiffWindowCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;           
            Instance = new BranchDiffWindowCommand(package, commandService);
        }

        /// <summary>
        /// Shows the actual UI in our plugin's Tools window when user clicks toolbar.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            package.JoinableTaskFactory.RunAsync(async delegate
            {
                ToolWindowPane window = await package.ShowToolWindowAsync(typeof(BranchDiffWindow), 0, true, package.DisposalToken);
                var branchDiffWindow = window as BranchDiffWindow;
                if (null == window || null == window.Frame)
                {
                    throw new NotSupportedException("Cannot create tool window");
                }

                var vm = DIContainer.Instance.GetService(typeof(BranchDiffViewModel)) as BranchDiffViewModel;
                Assumes.Present(vm);
                vm.Generate();

                branchDiffWindow.BranchDiffWindowControl.TextBlockUnderText.Text = "Generated Diff list";
            });
        }

    }
}
