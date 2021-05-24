using GitBranchDiffer.Filter;
using GitBranchDiffer.FileDiff;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using GitBranchDiffer.SolutionSelectionModels;
using System.Runtime.CompilerServices;
using Microsoft;

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
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class GitBranchDifferPackage : AsyncPackage
    {
        public const string PackageGuidString = "156fcec6-25ac-4279-91cc-bbe2e4ea8c14";

        private EnvDTE.DTE dte;
        private EnvDTE.SelectionEvents selectionEvents;
        private IVsDifferenceService vsDifferenceService;
        private IVsUIShell vsUIShell;

        public GitBranchDifferPackage()
        {
            BranchDiffFilterProvider.Initialize(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "VSSDK006:Check services exist", Justification = "Show custom error if DTE service doesn't exist")]
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            this.dte = await GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            this.vsDifferenceService = await GetServiceAsync(typeof(SVsDifferenceService)) as IVsDifferenceService;
            this.vsUIShell = await GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;

            if (dte != null)
            {
                // Filter will be initialized on package load.
                // Also hook to all relevant events here so we can solution-info set on the Filter when they are fired
                this.dte.Events.SolutionEvents.Opened += SetSolutionInfo;
                this.dte.Events.SolutionEvents.BeforeClosing += ResetSolutionInfo;
                this.selectionEvents = dte.Events.SelectionEvents;
                this.selectionEvents.OnChange += SelectionEvents_OnChange;
                this.SetSolutionInfo();
            }
            else
            {
                ErrorPresenter.ShowError(this, "Unable to load Git Branch Differ plug-in. Failed to get Visual Studio services.");
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

        #endregion

        #region VS Events
        /// <summary>
        /// Sets the Solution directory and name info on Branch Diff Filter.
        /// </summary> 
        private void SetSolutionInfo()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var absoluteSolutionPath = this.dte.Solution.FullName;
            var solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSolutionPath);
            var solutionFile = System.IO.Path.GetFileName(absoluteSolutionPath);
            BranchDiffFilterProvider.SetSolutionInfo(solutionDirectory, solutionFile);
        }

        /// <summary>
        /// Resets the filter by removing the solution info (set to string.Empty)
        /// </summary>
        private void ResetSolutionInfo()
        {
            BranchDiffFilterProvider.SetSolutionInfo(string.Empty, string.Empty);
        }

        private void SelectionEvents_OnChange()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (BranchDiffFilterProvider.IsFilterApplied)
            {
                var selectionContainer = this.GetCurrentSelectionInSolutionExplorer();

                if (selectionContainer.Item != null && selectionContainer.IsSupported() && this.IsSelectionChangeInSolutionExplorer(selectionContainer))
                {
                    // Only open diff window for item if no diff window already associated
                    if (selectionContainer.HasNoAssociatedDiffWindow(this.vsUIShell) && this.IsVisibleInSolutionExplorer(selectionContainer))
                    {
                        this.ShowFileDiffWindow(selectionContainer);
                    }
                    else
                    {
                        // else, just bring existing diff window of this item to front, don't open a new one
                        selectionContainer.FocusAssociatedDiffWindow(this.vsUIShell);
                    }
                }

                this.UpdateCurrentSelectionFromSolutionExplorer(selectionContainer);
            }
        }

        private void ShowFileDiffWindow(SolutionSelectionContainer<ISolutionSelection> solutionSelectionContainer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!string.IsNullOrEmpty(solutionSelectionContainer.FullName))
            {
                var absoluteSoltuionPath = this.dte.Solution.FullName;
                var solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSoltuionPath);
                var fileDiffProvider = new VsFileDiffProvider(this.vsDifferenceService, solutionDirectory, solutionSelectionContainer);
                fileDiffProvider.ShowFileDiffWithBaseBranch(this.BranchToDiffAgainst);
            }
        }
        #endregion

        #region Private methods - Solution Explorer inspection
        private bool IsVisibleInSolutionExplorer(SolutionSelectionContainer<ISolutionSelection> solutionSelectionContainer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uih = (EnvDTE.UIHierarchy)this.dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;
            EnvDTE.UIHierarchyItem uiHierarchyItem = uih?.FindHierarchyItem(solutionSelectionContainer);

            // If the item is null in the UI (after our filter is applied), we must NOT generate comparison view for this file.
            return uiHierarchyItem != null;
        }

        // We are only interested in change in solution Explorer items. This detects if the Selection_Change event was due to change there. 
        private bool IsSelectionChangeInSolutionExplorer(SolutionSelectionContainer<ISolutionSelection> solutionSelectionContainer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var thisItemName = solutionSelectionContainer.FullName;
            return thisItemName != BranchDiffFilterProvider.CurrentSelectionInFilter;
        }

        private SolutionSelectionContainer<ISolutionSelection> GetCurrentSelectionInSolutionExplorer()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uih = (EnvDTE.UIHierarchy)this.dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;
            Array selectedItems = (Array)uih.SelectedItems;
            if (selectedItems != null && selectedItems.Length == 1)
            {
                var selectedHierarchyItem = selectedItems.GetValue(0) as EnvDTE.UIHierarchyItem;
                var selectedObject = selectedHierarchyItem?.Object;

                if (selectedObject != null)
                {
                    if (selectedObject is EnvDTE.Project selectedProject)
                    {
                        var oldPath = BranchDiffFilterProvider.TagManager.GetOldFilePathFromRenamed(selectedProject);
                        return new SolutionSelectionContainer<ISolutionSelection>
                        {
                            Item = new SelectedProject { Native = selectedProject, OldFullPath = oldPath }
                        };
                    }

                    if (selectedObject is EnvDTE.ProjectItem selectedProjectItem)
                    {
                        var oldPath = BranchDiffFilterProvider.TagManager.GetOldFilePathFromRenamed(selectedProjectItem);
                        return new SolutionSelectionContainer<ISolutionSelection>
                        {
                            Item = new SelectedProjectItem { Native = selectedProjectItem, OldFullPath = oldPath }
                        };
                    }
                }
            }

            return new SolutionSelectionContainer<ISolutionSelection>
            {
                Item = null
            }; 
        }

        private void UpdateCurrentSelectionFromSolutionExplorer(SolutionSelectionContainer<ISolutionSelection> solutionSelectionContainer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (solutionSelectionContainer.Item != null)
            {
                var itemCanonicalName = solutionSelectionContainer.FullName;
                BranchDiffFilterProvider.CurrentSelectionInFilter = itemCanonicalName;
                return;
            }

            BranchDiffFilterProvider.CurrentSelectionInFilter = string.Empty;
        }
        #endregion
    }
}