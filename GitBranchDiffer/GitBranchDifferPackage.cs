using GitBranchDiffer.Filter;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio;
using Microsoft;
using Microsoft.VisualStudio.Shell.Interop;

namespace GitBranchDiffer
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [ProvideOptionPage(typeof(GitBranchDifferPluginOptions),
    "Git Branch Differ", "Git Branch Differ Options", 0, 0, true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class GitBranchDifferPackage : AsyncPackage
    {
        /// <summary>
        /// BranchDiffWindowPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "156fcec6-25ac-4279-91cc-bbe2e4ea8c14";

        private EnvDTE.DTE dte;
        private PackageInitializationState packageInitializationState = PackageInitializationState.Invalid;

        public GitBranchDifferPackage()
        {
            // When VS sites the package, pass package object to our plugin's Filter.
            // We use package to read PluginOptions evertime user triggers filter.
            BranchDiffFilterProvider.InitializeOnce(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "VSSDK006:Check services exist", Justification = "Show custom error if DTE service doesn't exist")]
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            dte = await GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            if (dte != null)
            {
                // Filter will be initialized "If and Only If" our package was initialized by VS instance
                // And VS was opened with a solution pre-selected
                if (dte.Solution.IsOpen)
                {
                    dte.Events.SolutionEvents.Opened += InitializeFilter;
                    dte.Events.SolutionEvents.BeforeClosing += UnInitializeFilter;
                    dte.Events.DocumentEvents.DocumentOpened += DocumentEvents_DocumentOpened;
                    this.InitializeFilter();
                }
                else
                {
                    // User triggered our MEF component (the Filter) without a solution being opened in VS.
                    // This unfortunatley Initializes our package and we lose the opportunity to init the Filter with solution info.
                    // Nothing can be done, and we ask user to restart VS with a preselected solution. 
                    this.PackageInitializationState = PackageInitializationState.Invalid;
                    ErrorPresenter.ShowError(this, ErrorPresenter.PackageInvalidStateError);
                }
            }
        }

        #region Package Members        
        /// <summary>
        /// The branch against which active branch will be diffed
        /// </summary>
        public string BranchToDiffAgainst
        {
            get
            {
                GitBranchDifferPluginOptions options = (GitBranchDifferPluginOptions)GetDialogPage(typeof(GitBranchDifferPluginOptions));
                return options.BaseBranchName;
            }
        }

        /// <summary>
        /// The state of the package set whenever VS chooses to initialize it.
        /// Defaults to invalid.
        /// </summary>
        public PackageInitializationState PackageInitializationState 
        {
            get
            {
                return this.packageInitializationState;
            }
            set
            {
                this.packageInitializationState = value;
            }
        }

        /// <summary>
        /// Initializes Branch Diff Filter with Solution directory and name info.
        /// </summary> 
        private void InitializeFilter()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var absoluteSolutionPath = this.dte.Solution.FullName;
            var solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSolutionPath);
            var solutionFile = System.IO.Path.GetFileName(absoluteSolutionPath);
            BranchDiffFilterProvider.Initialize(solutionDirectory, solutionFile);
            this.PackageInitializationState = PackageInitializationState.SoltuionInfoSet;
        }

        /// <summary>
        /// Resets the filter by removing the solution info (directory path of the solution is set to string.Empty)
        /// </summary>
        private void UnInitializeFilter()
        {
            BranchDiffFilterProvider.Initialize(string.Empty, string.Empty);
            this.PackageInitializationState = PackageInitializationState.SolutionInfoUnset;
        }

        private void DocumentEvents_DocumentOpened(EnvDTE.Document Document)
        {
            if (this.PackageInitializationState == PackageInitializationState.SoltuionInfoSet)
            {
                // TODO : Find a way to inspect if the solution explorer has out filter applied.
                // If yes, documnent opened must open the diff view.
            }
        }

        #endregion
    }
}
