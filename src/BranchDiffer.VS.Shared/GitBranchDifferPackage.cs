using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio;
using BranchDiffer.VS.Shared.Utils;
using BranchDiffer.VS.Shared.FileDiff.Commands;
using BranchDiffer.VS.Shared.BranchDiff;
using Microsoft.VisualStudio.Shell.Interop;
using BranchDiffer.VS.Shared.Configuration;

namespace BranchDiffer.VS.Shared
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GitBranchDifferPackageGuids.guidBranchDiffWindowPackage)]
    [ProvideOptionPage(typeof(GitBranchDifferPluginOptions),
    "Git Branch Differ", "Git Branch Differ Options", 0, 0, true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class GitBranchDifferPackage : AsyncPackage, IGitBranchDifferPackage
    {
        private EnvDTE.DTE dte;
        private OpenPhysicalFileDiffCommand openPhysicalFileDiffCommand;
        private OpenProjectFileDiffCommand openProjectFileDiffCommand;

        public GitBranchDifferPackage()
        {
            BranchDiffFilterProvider.Initialize(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "VSSDK006:Check services exist", Justification = "Show custom error if DTE service doesn't exist")]
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            this.dte = await GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            IVsUIShell uiShell = await GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;

            // Construct file diff commands, initialize dependecies in it and then register them in VS Menu Commands asynchronously
            this.openPhysicalFileDiffCommand = VsDIContainer.Instance.GetService(typeof(OpenPhysicalFileDiffCommand)) as OpenPhysicalFileDiffCommand;
            this.openProjectFileDiffCommand = VsDIContainer.Instance.GetService(typeof(OpenProjectFileDiffCommand)) as OpenProjectFileDiffCommand;
            await this.openPhysicalFileDiffCommand.InitializeAndRegisterAsync(this, this.dte, uiShell);
            await this.openProjectFileDiffCommand.InitializeAndRegisterAsync(this, this.dte, uiShell);            

            // Commands invisible in UI by default
            this.openPhysicalFileDiffCommand.IsVisible = false;
            this.openProjectFileDiffCommand.IsVisible = false;

            if (dte != null)
            {
                // Filter will be initialized on package load.
                // Also hook to all relevant events here so we can solution-info set on the Filter when they are fired
                this.dte.Events.SolutionEvents.Opened += SetSolutionInfo;
                this.dte.Events.SolutionEvents.BeforeClosing += ResetSolutionInfo;
                this.SetSolutionInfo();
            }
            else
            {
                Guid clsid = Guid.Empty;
                uiShell.ShowMessageBox(
                    0,
                    ref clsid,
                    "FirstPackage",
                    "Unable to load Git Branch Differ plug -in.Failed to get Visual Studio services",
                    string.Empty,
                    0,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                    OLEMSGICON.OLEMSGICON_INFO,
                    0,
                    out _);
            }
        }

        /// <summary>
        /// The branch against which active branch will be diffed.
        /// </summary>
        public string BranchToDiffAgainst
        {
            get
            {
                GitBranchDifferPluginOptions options = (GitBranchDifferPluginOptions)GetDialogPage(typeof(GitBranchDifferPluginOptions));
                return options.BaseBranchName;
            }
        }

        public CancellationToken CancellationToken => this.DisposalToken;

        /// <summary>
        /// Sets the Solution directory and name info on Branch Diff Filter.
        /// </summary> 
        private void SetSolutionInfo()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var absoluteSolutionPath = this.dte.Solution.FullName;
            var solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSolutionPath);
            BranchDiffFilterProvider.SetSolutionInfo(solutionDirectory);
        }

        /// <summary>
        /// Resets the filter by removing the solution info (set to string.Empty)
        /// </summary>
        private void ResetSolutionInfo()
        {
            BranchDiffFilterProvider.SetSolutionInfo(string.Empty);
        }

        public void OnFilterApplied()
        {
            this.openPhysicalFileDiffCommand.IsVisible = true;
            this.openProjectFileDiffCommand.IsVisible = true;
        }

        public void OnFilterUnapplied()
        {
            // When plugin isn't constructed, UI state of filter button can still be toggled, and user might un-apply before initialization
            if (this.openPhysicalFileDiffCommand != null)
            {
                this.openPhysicalFileDiffCommand.IsVisible = false;
            }
            if (this.openProjectFileDiffCommand != null)
            {
                this.openProjectFileDiffCommand.IsVisible = false;
            }
        }
    }
}