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

        public GitBranchDifferPackage()
        {

            // When VS sites the package, pass package object to our plugin's Filter. We use package to read PluginOptions evertime user triggers filter.
            BranchDiffFilterProvider.InitializeOnce(this);
        }


        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "VSSDK006:Check services exist", Justification = "Show custom error if DTE service doesn't exist")]
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            dte = await GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            if (dte != null)
            {
                // TODO: Reset/Clear solution info from filter OnSolutionClose event???
                // In that case, we can make a FilterState enum to provide better feedback to user:
                // FilterState.SolutionInfoSet (set this On package load, and on SolutionEvents.Open),
                // FilterState.SolutionInfoReset (set this when solution closed),
                // FilterState.SolutionInfoNotSet (set this when Package is first sited, this state should never actually be possible to encounter)
                // etc.
                dte.Events.SolutionEvents.Opened += InitializeFilter;
                this.InitializeFilter();
            }
            else
            {
                VsShellUtilities.ShowMessageBox(
                    this, 
                    "Failed intializing GitBranchDiffer. Unable to fetch Visual Studio services.",
                    "Git Branch Differ",
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        #region Package Members        
        /// <summary>
        /// The branch againt which active HEAD will be diffed
        /// </summary>
        public string BranchToDiff
        {
            get
            {
                GitBranchDifferPluginOptions options = (GitBranchDifferPluginOptions)GetDialogPage(typeof(GitBranchDifferPluginOptions));
                return options.BaseBranchName;
            }
        }

        /// <summary>
        /// Initializes Branch Diff Filter with Solution directory and name info.
        /// </summary> 
        private void InitializeFilter()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var absoluteSolutionPath = dte.Solution.FullName;
            var solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSolutionPath);
            var solutionFile = System.IO.Path.GetFileName(absoluteSolutionPath);
            BranchDiffFilterProvider.Initialize(solutionDirectory, solutionFile);
        }

        #endregion
    }
}
