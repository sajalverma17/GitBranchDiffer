using BranchDiffer.Git.Configuration;
using BranchDiffer.Git.Core;
using BranchDiffer.Git.Exceptions;
using BranchDiffer.Git.Models;
using BranchDiffer.VS.Shared.Configuration;
using BranchDiffer.VS.Shared.FileDiff;
using BranchDiffer.VS.Shared.Utils;
using Microsoft;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace BranchDiffer.VS.Shared.BranchDiff
{
    [SolutionTreeFilterProvider(GitBranchDifferPackageGuids.guidGitBranchDifferPackageCmdSet, (uint)(GitBranchDifferPackageGuids.CommandIdGenerateDiffAndFilter))]    
    public class BranchDiffFilterProvider : HierarchyTreeFilterProvider
    {
        private readonly SVsServiceProvider serviceProvider;
        private readonly IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider;
        private static IGitBranchDifferPackage Package;
        private static string SolutionDirectory;

        [ImportingConstructor]
        public BranchDiffFilterProvider(
            SVsServiceProvider serviceProvider,
            IVsHierarchyItemCollectionProvider hierarchyCollectionProvider)
        {
            this.serviceProvider = serviceProvider;
            this.vsHierarchyItemCollectionProvider = hierarchyCollectionProvider;
        }

        internal static ItemTagManager TagManager { get; private set; }

        /// <summary>
        /// One time initialization of the Solution Explorer filter, happens once per-Visual-Studio-startup.
        /// </summary>
        /// <param name="package">
        /// The package becomes a static dependency of this filter,
        /// and is required in order to get the plugin option <see cref="GitBranchDifferPackage.BranchToDiffAgainst"/> set by user.</param>
        public static void Initialize(IGitBranchDifferPackage package)
        {
            Package = package;
            BranchDiffFilterProvider.TagManager = new ItemTagManager();
        }

        /// <summary>
        /// Initalizes the Solution Explorer filter with Solution info once per-solution-load in Visual Studio
        /// </summary>
        public static void SetSolutionInfo(string solutionDirectory)
        {
            SolutionDirectory = solutionDirectory;
        }

        protected override HierarchyTreeFilter CreateFilter()
        {
            return new BranchDiffFilter(Package, SolutionDirectory, this.vsHierarchyItemCollectionProvider);
        }

        private sealed class BranchDiffFilter : HierarchyTreeFilter
        {
            private readonly IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider;            
            private readonly IGitBranchDifferPackage package;
            private readonly string solutionDirectory;
            private readonly GitBranchDiffController branchDiffWorker;
            private readonly BranchDiffFilterValidator branchDiffValidator;
            private readonly ErrorPresenter errorPresenter;

            private HashSet<DiffResultItem> changeSet;

            public BranchDiffFilter(
                IGitBranchDifferPackage package,
                string solutionPath,
                IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider)
            {
                this.package = package;
                this.solutionDirectory = solutionPath;
                this.vsHierarchyItemCollectionProvider = vsHierarchyItemCollectionProvider;
                this.Initialized += BranchDiffFilter_Initialized;


                // Dependencies that can be moved to constructor and resolved via IoC...
                this.branchDiffWorker = DIContainer.Instance.GetService(typeof(GitBranchDiffController)) as GitBranchDiffController;
                this.branchDiffValidator = VsDIContainer.Instance.GetService(typeof(BranchDiffFilterValidator)) as BranchDiffFilterValidator;
                this.errorPresenter = VsDIContainer.Instance.GetService(typeof(ErrorPresenter)) as ErrorPresenter;
                Assumes.Present(this.branchDiffWorker);
                Assumes.Present(this.branchDiffValidator);
                Assumes.Present(this.errorPresenter);
            }


            // We override this method to use it as a life-cycle hook to mark that our filter was un-applied
            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();
                Package?.OnFilterUnapplied();
            }

            protected override async Task<IReadOnlyObservableSet> GetIncludedItemsAsync(IEnumerable<IVsHierarchyItem> rootItems)
            {
                if (this.branchDiffValidator.ValidateBranch(this.package))
                {                    
                    // Create new tag tables everytime the filter is applied 
                    BranchDiffFilterProvider.TagManager.CreateTagTables();
                    IVsHierarchyItem root = HierarchyUtilities.FindCommonAncestor(rootItems);

                    if (this.branchDiffValidator.ValidateSolution(this.solutionDirectory))
                    {
                        try
                        {
                            this.changeSet = this.branchDiffWorker.GenerateDiff(this.solutionDirectory, this.package.BranchToDiffAgainst);
                        }
                        catch (GitOperationException e)
                        {
                            this.errorPresenter.ShowError(e.Message);
                            return null;
                        }

                        IReadOnlyObservableSet<IVsHierarchyItem> sourceItems = await this.vsHierarchyItemCollectionProvider.GetDescendantsAsync(
                                            root.HierarchyIdentity.NestedHierarchy,
                                            CancellationToken);

                        IFilteredHierarchyItemSet includedItems = await this.vsHierarchyItemCollectionProvider.GetFilteredHierarchyItemsAsync(
                            sourceItems,
                            ShouldIncludeInFilter,
                            CancellationToken);

                        return includedItems;
                    }
                }

                return null;
            }

            private bool ShouldIncludeInFilter(IVsHierarchyItem hierarchyItem)
            {
                if (hierarchyItem == null)
                {
                    return false;
                }

                if (HierarchyUtilities.IsPhysicalFile(hierarchyItem.HierarchyIdentity)
                    || HierarchyUtilities.IsProject(hierarchyItem.HierarchyIdentity))
                {
                    if (!string.IsNullOrEmpty(hierarchyItem.CanonicalName))
                    {
                        DiffResultItem diffResultItem = this.branchDiffWorker.GetItemFromChangeSet(this.changeSet, hierarchyItem.CanonicalName);

                        if (diffResultItem != null)
                        {
                            // *WIP* - If the changed physical file shows up under "External Dependencies" folder of a C++ project, always ignore
                            if (HierarchyUtilities.IsPhysicalFile(hierarchyItem.HierarchyIdentity) && IsCPPExternalDependencyFile(hierarchyItem))
                            {
                                return false;
                            }

                            // If files renamed in working branch, Tag the old path so we find the Base branch version of file using the Old Path
                            if (!string.IsNullOrEmpty(diffResultItem.OldAbsoluteFilePath))
                            {
                                    BranchDiffFilterProvider.TagManager.SetOldFilePathOnRenamedItem(
                                        hierarchyItem.HierarchyIdentity.Hierarchy,
                                        hierarchyItem.CanonicalName,
                                        diffResultItem.OldAbsoluteFilePath);
                            }

                            return true;
                        }
                    }
                }

                return false;
            }

            private bool IsCPPExternalDependencyFile(IVsHierarchyItem hierarchyItem)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                hierarchyItem.HierarchyIdentity.Hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out var prjObject);
                if (prjObject is EnvDTE.Project containingProject)
                {
                    if (containingProject.Kind == "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}")
                    {
                        // Check if is leaf node AND parent is a named "External Dependencies" AND parent is NOT a VSX filter added to vcsx project
                        if (hierarchyItem.Parent.Text.Equals("External Dependencies"))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            private void BranchDiffFilter_Initialized(object sender, EventArgs e)
            {
                Package.OnFilterApplied();
            }
        }
    }

}
