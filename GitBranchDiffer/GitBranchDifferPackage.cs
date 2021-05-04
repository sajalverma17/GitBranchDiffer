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
using System.Collections.Generic;
using Microsoft.VisualStudio.Platform.WindowManagement;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Linq;
using Microsoft.VisualStudio.Text.Differencing;
using Microsoft.VisualStudio.Editor;

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
        private EnvDTE.DocumentEvents documentEvents;
        private EnvDTE.SelectionEvents selectionEvents;
        private FilterInitializationState packageInitializationState = FilterInitializationState.Invalid;
        private IVsDifferenceService vsDifferenceService;
        private IVsUIShell vsUIShell;
        private IVsMonitorSelection vsMonitorSelectionService;
        private RunningDocumentTable vsRunningDocumentTable;

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
            this.vsUIShell = await GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;
            this.vsMonitorSelectionService = await GetServiceAsync(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;

            IOleServiceProvider sp = GetGlobalService(typeof(IOleServiceProvider)) as IOleServiceProvider;
            this.vsRunningDocumentTable = new RunningDocumentTable(new ServiceProvider(sp));
            if (sp == null) return;
            if (dte != null)
            {
                // Filter will be initialized "If and Only If" our package was initialized by VS instance having a solution pre-selected before open
                // Also hook to all relevant events here.
                if (dte.Solution.IsOpen)
                {
                    this.dte.Events.SolutionEvents.Opened += InitializeFilter;
                    this.dte.Events.SolutionEvents.BeforeClosing += UnInitializeFilter;                                    
                    this.documentEvents = dte.Events.DocumentEvents;
                    this.selectionEvents = dte.Events.SelectionEvents;
                    this.selectionEvents.OnChange += SelectionEvents_OnChange;
                    // documentEvents.DocumentOpening += DocumentEvents_DocumentOpening;
                    // documentEvents.DocumentOpened += DocumentEvents_DocumentOpened;
                    // var documentWindowEventHandler = new DocumentWindowEventHandler(vsRunningDocumentTable);
                    // uint clientIdToUnregister = this.vsRunningDocumentTable.Advise(documentWindowEventHandler);
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

        /// <summary>
        /// Initializes Branch Diff Filter with Solution directory and name info.
        /// TODO: Move hooking and unhooking from Document Events in this method and the UnInitializeFilter.
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

        /*
        private void DocumentEvents_DocumentOpening(string DocumentPath, bool ReadOnly)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var projectItem = this.dte.Solution.FindProjectItem(DocumentPath);
            // TODO [Bug]: Event triggered even if document is already opened and we switch to it. Might be solved by using Running Document Table.
            // TODO [Feature]: Support Showing diff for project files as well (project item is null for them, but they are physical files too.
            if (projectItem is null || !projectItem.Kind.Equals(EnvDTE.Constants.vsProjectItemKindPhysicalFile))
            {
                return;
            }

            if (this.FilterInitializationState == FilterInitializationState.SoltuionInfoSet)
            {
                // We generate diff window if the DocumentOpened event is triggered by a Project Item visible in our filter.
                if (BranchDiffFilterProvider.IsFilterApplied && this.IsVisibleInSolutionHierarchy(projectItem))
                {
                    // Prevent stack overflow: Unhook so opening comparison document does not trigger this event again.
                    documentEvents.DocumentOpening -= DocumentEvents_DocumentOpening;

                    var filePathToDiff = DocumentPath;
                    this.TryCloseWindow(DocumentPath);
                    this.ShowFileDiffWindow(filePathToDiff);

                    documentEvents.DocumentOpening += DocumentEvents_DocumentOpening;
                }
            }
        }
        */

        private void DocumentEvents_DocumentOpened(EnvDTE.Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // TODO [Bug]: Event triggered even if document is already opened and we switch to it. Might be solved by using Running Document Table.
            // TODO [Feature]: Support Showing diff for project files as well (project item is null for them, but they are physical files too.
            if (document.ProjectItem is null || !document.ProjectItem.Kind.Equals(EnvDTE.Constants.vsProjectItemKindPhysicalFile))
            {
                return;
            }
            
            if (this.FilterInitializationState == FilterInitializationState.SoltuionInfoSet)
            {
                // We generate diff window if the DocumentOpened event is triggered by a Project Item visible in our filter.
                if (BranchDiffFilterProvider.IsFilterApplied && this.IsVisibleInSolutionHierarchy(document.ProjectItem))
                {
                    // Prevent stack overflow: Unhook so opening comparison document does not trigger this event again.
                    documentEvents.DocumentOpened -= DocumentEvents_DocumentOpened;

                    var filePathToDiff = document.FullName;
                    var diffWindowFrame = this.ShowFileDiffWindow(filePathToDiff);
                    this.TryCloseWindow(diffWindowFrame);

                    documentEvents.DocumentOpened += DocumentEvents_DocumentOpened;
                }
            }

        }

        private void SelectionEvents_OnChange()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IntPtr hierarchyPointer, selectionContainerPointer;
            Object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint projectItemId;

            this.vsMonitorSelectionService.GetCurrentSelection(out hierarchyPointer,
                                                 out projectItemId,
                                                 out multiItemSelect,
                                                 out selectionContainerPointer);

            IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(
                                                hierarchyPointer,
                                                typeof(IVsHierarchy)) as IVsHierarchy;
            if (selectedHierarchy != null)
            {
                ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(
                                                  projectItemId,
                                                  (int)__VSHPROPID.VSHPROPID_ExtObject,
                                                  out selectedObject));
            }

            EnvDTE.ProjectItem selectedProjectItem = selectedObject as EnvDTE.ProjectItem;

            if (selectedProjectItem != null)
            {
                if (this.FilterInitializationState == FilterInitializationState.SoltuionInfoSet)
                {
                    // We generate diff window if the DocumentOpened event is triggered by a Project Item visible in our filter.
                    if (BranchDiffFilterProvider.IsFilterApplied && this.IsVisibleInSolutionHierarchy(selectedProjectItem))
                    {
                        var projectPath = System.IO.Path.GetDirectoryName(selectedProjectItem.ContainingProject.FullName);
                        var filePath = projectPath + "\\" + selectedProjectItem.Name;
                        this.ShowFileDiffWindow(filePath);
                    }
                }
            }
        }

        private bool IsVisibleInSolutionHierarchy(EnvDTE.ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uih = (EnvDTE.UIHierarchy)this.dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;
            EnvDTE.UIHierarchyItem uiHierarchyItem = uih?.FindHierarchyItem(projectItem);

            // If the item is null in the UI (after our filter is applied), we must NOT generate comparison view for this file.
            return uiHierarchyItem != null;
        }

        private IVsWindowFrame ShowFileDiffWindow(string openedDocumentFullPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var absoluteSoltuionPath = this.dte.Solution.FullName;
            var solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSoltuionPath);
            var fileDiffProvider = new VsFileDiffProvider(this.vsDifferenceService, solutionDirectory, openedDocumentFullPath);
            return fileDiffProvider.ShowFileDiffWithBaseBranch(this.BranchToDiffAgainst);
        }

        // TODO [Bug]: On document open, close the one VS opens as we only want to show comparison. Might be solved by Running Document Table.
        private void TryCloseWindow(IVsWindowFrame diffWindowFrame)
        {         
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                this.vsUIShell.GetDocumentWindowEnum(out IEnumWindowFrames windowFramesEnumerator);

                var allDocumentWindowFrames = new List<IVsWindowFrame>();
                var frames = new IVsWindowFrame[1];
                var hasMorewindows = true;
                do
                {
                    hasMorewindows = windowFramesEnumerator.Next(1, frames, out var fetched) == VSConstants.S_OK && fetched == 1;

                    if (!hasMorewindows || frames[0] == null)
                        continue;

                    allDocumentWindowFrames.Add(frames[0]);

                } while (hasMorewindows);

                foreach (IVsWindowFrame windowFrame in allDocumentWindowFrames)
                {
                    var windowFrameImpl = windowFrame as WindowFrame;
                    if (windowFrameImpl != null)
                    {
                        var documentGroup = this.GetDocumentGroup(windowFrameImpl);
                        var documentViews = documentGroup.Children.Where(c => c != null && c.GetType() == typeof(DocumentView)).Select(c => c as DocumentView);                       
                        

                        if (!windowFrameImpl.AnnotatedTitle.Contains("Vs."))
                        {
                            windowFrameImpl.CloseFrame(__FRAMECLOSE.FRAMECLOSE_NoSave);
                        }
                    }
                }
                // document.ActiveWindow.Close(EnvDTE.vsSaveChanges.vsSaveChangesYes);
            }
            catch (Exception e)
            {
            }
        }
        private DocumentGroup GetDocumentGroup(WindowFrame windowFrame)
        {
            return Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<DocumentGroup, ViewElement>(
                windowFrame.FrameView, e => e.Parent as ViewElement);
        }
        private IDifferenceViewer GetDiffViewer(IVsWindowFrame frame)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return ErrorHandler.Succeeded(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out object docView))
                ? (docView as IVsDifferenceCodeWindow)?.DifferenceViewer : null;
        }
    }
}
