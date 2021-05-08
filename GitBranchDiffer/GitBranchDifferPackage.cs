using GitBranchDiffer.Filter;
using GitBranchDiffer.FileDiff;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace GitBranchDiffer
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [ProvideOptionPage(typeof(GitBranchDifferPluginOptions),
    "Git Branch Differ", "Git Branch Differ Options", 0, 0, true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class GitBranchDifferPackage : AsyncPackage
    {
        public const string PackageGuidString = "156fcec6-25ac-4279-91cc-bbe2e4ea8c14";

        private EnvDTE.DTE dte;
        private EnvDTE.SelectionEvents selectionEvents;
        private FilterInitializationState packageInitializationState = FilterInitializationState.Invalid;
        private IVsDifferenceService vsDifferenceService;
        private string currentSelectionInSolutionExplorer;

        public GitBranchDifferPackage()
        {
            BranchDiffFilterProvider.InitializeOnce(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "VSSDK006:Check services exist", Justification = "Show custom error if DTE service doesn't exist")]
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            this.dte = await GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            this.vsDifferenceService = await GetServiceAsync(typeof(SVsDifferenceService)) as IVsDifferenceService;
            if (dte != null)
            {
                // Filter will be initialized "If and Only If" our package was initialized by VS instance having a solution pre-selected before open
                // Also hook to all relevant events here.
                if (dte.Solution.IsOpen)
                {
                    this.dte.Events.SolutionEvents.Opened += InitializeFilter;
                    this.dte.Events.SolutionEvents.BeforeClosing += UnInitializeFilter;
                    this.selectionEvents = dte.Events.SelectionEvents;
                    this.selectionEvents.OnChange += SelectionEvents_OnChange;
                    this.InitializeFilter();
                }
                else
                {
                    // User triggered our MEF component (by clicking on the Filter) without a solution being opened in VS.
                    // This unfortunately calls InitializeAsync of our package and force loads it.
                    // After this, we lose the opportunity to init filter with solution info since VS will never load our package.
                    // Hence the error message asking to restart VS with a Solution pre-selected.
                    this.FilterInitializationState = FilterInitializationState.Invalid;
                    ErrorPresenter.ShowError(this, ErrorPresenter.PackageInvalidStateError);
                }
            }
        }

        #region Public Package Members   

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

        /// <summary>
        /// The state of the filter, describes if the solution path (which becomes the git repo path) was passed to it or not.
        /// Defaults to invalid.
        /// </summary>
        public FilterInitializationState FilterInitializationState 
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

        #endregion

        #region VS Events
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
            this.FilterInitializationState = FilterInitializationState.SoltuionInfoSet;
        }

        /// <summary>
        /// Resets the filter by removing the solution info (directory path of the solution is set to string.Empty)
        /// </summary>
        private void UnInitializeFilter()
        {
            BranchDiffFilterProvider.Initialize(string.Empty, string.Empty);
            this.FilterInitializationState = FilterInitializationState.SolutionInfoUnset;
        }

        private void SelectionEvents_OnChange()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (BranchDiffFilterProvider.IsFilterApplied && this.FilterInitializationState == FilterInitializationState.SoltuionInfoSet)
            {
                var selectedProjectItem = this.GetCurrentSelectionInSolutionExplorer();

                // TODO [Feature]: Support for project files too, just allow returning EnvDTE.Project from above method
                if (selectedProjectItem != null && this.IsSelectionChangeInSolutionExplorer(selectedProjectItem))
                {
                    if (this.NoExistingWindowFor(selectedProjectItem) && this.IsVisibleInSolutionExplorer(selectedProjectItem))
                    {
                        string filePath = selectedProjectItem.Properties.Item("FullPath").Value.ToString();
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            this.ShowFileDiffWindow(filePath);
                        }
                    }
                    else
                    {
                        // else, just bring existing document window of this item to front, don't open a new one
                        SetFocusOnWindowFor(selectedProjectItem);
                    }

                    this.UpdateCurrentSelectionFromSolutionExplorer(selectedProjectItem);
                }
            }
        }
        #endregion

        #region Private methods - Solution Explorer inspection
        private bool IsVisibleInSolutionExplorer(EnvDTE.ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uih = (EnvDTE.UIHierarchy)this.dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;
            EnvDTE.UIHierarchyItem uiHierarchyItem = uih?.FindHierarchyItem(projectItem);

            // If the item is null in the UI (after our filter is applied), we must NOT generate comparison view for this file.
            return uiHierarchyItem != null;
        }

        private EnvDTE.ProjectItem GetCurrentSelectionInSolutionExplorer()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uih = (EnvDTE.UIHierarchy)this.dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;
            Array selectedItems = (Array)uih.SelectedItems;
            if (selectedItems != null && selectedItems.Length == 1)
            {
                var selectedHierarchyItem = selectedItems.GetValue(0) as EnvDTE.UIHierarchyItem;
                return selectedHierarchyItem.Object as EnvDTE.ProjectItem;
            }

            return null;
        }

        // We are only interested in change in solution Explorer items. This detects if the Selection_Change event was due to change there. 
        private bool IsSelectionChangeInSolutionExplorer(EnvDTE.ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var thisItemName = projectItem.Properties.Item("FullPath").Value.ToString();
            return thisItemName != currentSelectionInSolutionExplorer;
        }

        private void UpdateCurrentSelectionFromSolutionExplorer(EnvDTE.ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var itemCanonicalName = projectItem.Properties.Item("FullPath").Value.ToString();
            this.currentSelectionInSolutionExplorer = itemCanonicalName;
        }
        #endregion

        #region Private methods - Window manipulation
        private bool NoExistingWindowFor(EnvDTE.ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (projectItem.Document == null)
            {
                return true;
            }

            return projectItem.Document.Windows.Count == 0;
        }

        private void SetFocusOnWindowFor(EnvDTE.ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (projectItem.Document.Windows.Count == 1)
            {
                var enumerator = projectItem.Document.Windows.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var currentWindow = enumerator.Current as EnvDTE.Window;
                    currentWindow.Activate();
                }
            }
        }

        private void ShowFileDiffWindow(string selectedItemPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var absoluteSoltuionPath = this.dte.Solution.FullName;
            var solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSoltuionPath);
            var fileDiffProvider = new VsFileDiffProvider(this.vsDifferenceService, solutionDirectory, selectedItemPath);
            fileDiffProvider.ShowFileDiffWithBaseBranch(this.BranchToDiffAgainst);
        }
        #endregion
    }
}