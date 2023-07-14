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
using BranchDiffer.Git.Configuration;
using BranchDiffer.Git.Core;
using BranchDiffer.Git.Models.LibGit2SharpModels;

namespace BranchDiffer.VS.Shared
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GitBranchDifferPackageGuids.guidBranchDiffWindowPackage)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class GitBranchDifferPackage : AsyncPackage, IGitBranchDifferPackage
    {
        private EnvDTE.DTE dte;
        private OpenPhysicalFileDiffCommand openPhysicalFileDiffCommand;
        private OpenProjectFileDiffCommand openProjectFileDiffCommand;
        private OpenGitReferenceConfigurationCommand openGitReferenceConfigurationCommand;
        private GitObjectsStore gitObjectsStore;
        private string solutionDirectory;

        public GitBranchDifferPackage()
        {
            BranchDiffFilterProvider.Initialize(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "VSSDK006:Check services exist", Justification = "Show custom error if DTE service doesn't exist")]
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            this.dte = await GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            this.gitObjectsStore = DIContainer.Instance.GetService(typeof(GitObjectsStore)) as GitObjectsStore;


            await this.RegisterCommandsAsync();

            if (dte != null)
            {
                // Hook on to events to set/reset solution path on BranchDiffFilter
                this.dte.Events.SolutionEvents.Opened += SetSolutionPathOnFilter;
                this.dte.Events.SolutionEvents.BeforeClosing += ClearSolutionPathFromFilter;

                // When a document is opened separate from a solution, VS loads a "dummy" Solution1, this leads to our extension initialized due to ProvideAutoLoad.
                // In this case, don't set solution path now, let it happen when SolutionEvents.Opened triggers it on some "real" solution.
                if (!string.IsNullOrEmpty(this.dte.Solution.FullName))
                {
                    this.SetSolutionPathOnFilter();
                }
                
            }
            else
            {
                IVsUIShell uiShell = await GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;

                Guid clsid = Guid.Empty;
                uiShell.ShowMessageBox(
                    0,
                    ref clsid,
                    "FirstPackage",
                    "Unable to load Git Branch Differ plug-in. Failed to get Visual Studio services.",
                    string.Empty,
                    0,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                    OLEMSGICON.OLEMSGICON_INFO,
                    0,
                    out _);
            }
        }

        public IGitObject BranchToDiffAgainst { get; set; }

        public string SolutionDirectory => this.solutionDirectory;

        public CancellationToken CancellationToken => this.DisposalToken;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "VSSDK006:Check services exist", Justification = "Show custom error if DTE service doesn't exist")]
        private async Task RegisterCommandsAsync()
        {
            // Construct file diff commands, initialize dependecies in it and then register them in VS Menu Commands asynchronously
            this.openPhysicalFileDiffCommand = VsDIContainer.Instance.GetService(typeof(OpenPhysicalFileDiffCommand)) as OpenPhysicalFileDiffCommand;
            this.openProjectFileDiffCommand = VsDIContainer.Instance.GetService(typeof(OpenProjectFileDiffCommand)) as OpenProjectFileDiffCommand;
            this.openGitReferenceConfigurationCommand = VsDIContainer.Instance.GetService(typeof(OpenGitReferenceConfigurationCommand)) as OpenGitReferenceConfigurationCommand;

            await this.openPhysicalFileDiffCommand.InitializeAndRegisterAsync(this);
            await this.openProjectFileDiffCommand.InitializeAndRegisterAsync(this);
            await this.openGitReferenceConfigurationCommand.InitializeAndRegisterAsync(this);
        }

        private void SetSolutionPathOnFilter()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var absoluteSolutionPath = this.dte.Solution.FullName;
            this.solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSolutionPath);            
            BranchDiffFilterProvider.SetSolutionInfo(this.solutionDirectory);
            this.BranchToDiffAgainst = this.gitObjectsStore.GetDefaultGitReferenceObject(this.solutionDirectory);
        }

        private void ClearSolutionPathFromFilter()
        {
            BranchDiffFilterProvider.SetSolutionInfo(string.Empty);
        }
    }
}