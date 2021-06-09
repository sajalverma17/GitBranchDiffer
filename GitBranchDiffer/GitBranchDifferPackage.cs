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
using GitBranchDiffer.FileDiff.Commands;

namespace GitBranchDiffer
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
    public sealed class GitBranchDifferPackage : AsyncPackage
    {
        private EnvDTE.DTE dte;

        public GitBranchDifferPackage()
        {
            BranchDiffFilterProvider.Initialize(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "VSSDK006:Check services exist", Justification = "Show custom error if DTE service doesn't exist")]
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            this.dte = await GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            // Init file diff commands, default invisilble
            await OpenPhysicalFileDiffCommand.InitializeAsync(this);
            await OpenProjectFileDiffCommand.InitializeAsync(this);
            OpenPhysicalFileDiffCommand.Instance.Visible = false;
            OpenProjectFileDiffCommand.Instance.Visible = false;

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

        #region Private methods - Solution Explorer inspection

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

        public void OnFilterApplied()
        {
            OpenPhysicalFileDiffCommand.Instance.Visible = true;
            OpenProjectFileDiffCommand.Instance.Visible = true;
        }

        public void OnFilterUnapplied()
        {
            OpenPhysicalFileDiffCommand.Instance.Visible = false;
            OpenProjectFileDiffCommand.Instance.Visible = false;
        }

        #endregion
    }
}