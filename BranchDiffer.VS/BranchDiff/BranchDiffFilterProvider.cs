using BranchDiffer.Git.Core;
using BranchDiffer.Git.Models;
using BranchDiffer.VS.FileDiff;
using BranchDiffer.VS.Utils;
using Microsoft;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace BranchDiffer.VS.BranchDiff
{
    [SolutionTreeFilterProvider(GitBranchDifferPackageGuids.guidGitBranchDifferPackageCmdSet, (uint)(GitBranchDifferPackageGuids.CommandIdGenerateDiffAndFilter))]    
    public class BranchDiffFilterProvider : HierarchyTreeFilterProvider
    {
        private readonly SVsServiceProvider serviceProvider;
        private readonly IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider;
        private static IGitBranchDifferPackage Package;
        private static string SolutionDirectory;
        private static string SolutionFile;

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
        public static void SetSolutionInfo(string solutionDirectory, string solutionFile)
        {
            SolutionDirectory = solutionDirectory;
            SolutionFile = solutionFile;
        }

        protected override HierarchyTreeFilter CreateFilter()
        {
            return new BranchDiffFilter(Package, SolutionDirectory, SolutionFile, this.serviceProvider, this.vsHierarchyItemCollectionProvider);
        }

        private sealed class BranchDiffFilter : HierarchyTreeFilter
        {
            private readonly SVsServiceProvider serviceProvider;
            private readonly IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider;            
            private readonly IGitBranchDifferPackage package;
            private readonly string solutionDirectory;
            private readonly string solutionFile;

            private readonly GitBranchDiffController branchDiffWorker;
            private HashSet<DiffResultItem> changeSet;

            public BranchDiffFilter(
                IGitBranchDifferPackage package,
                string solutionPath,
                string solutionName,
                SVsServiceProvider serviceProvider, 
                IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider)
            {
                this.package = package;
                this.solutionDirectory = solutionPath;
                this.solutionFile = solutionName;
                this.serviceProvider = serviceProvider;
                this.vsHierarchyItemCollectionProvider = vsHierarchyItemCollectionProvider;
                this.Initialized += BranchDiffFilter_Initialized;
                this.branchDiffWorker = DIContainer.Instance.GetService(typeof(GitBranchDiffController)) as GitBranchDiffController;
                Assumes.Present(this.branchDiffWorker);
            }

            protected override async Task<IReadOnlyObservableSet> GetIncludedItemsAsync(IEnumerable<IVsHierarchyItem> rootItems)
            {
                if (BranchDiffFilterValidator.ValidateBranch(this.package))
                {
                    // Create new tag tables everytime the filter is applied 
                    BranchDiffFilterProvider.TagManager.CreateTagTables();
                    IVsHierarchyItem root = HierarchyUtilities.FindCommonAncestor(rootItems);

                    if (BranchDiffFilterValidator.ValidateSolution(this.solutionDirectory, this.solutionFile))
                    {
                        var setupOk = this.branchDiffWorker.SetupRepository(this.solutionDirectory, this.package.BranchToDiffAgainst, out var repo, out var error);
                        if (setupOk)
                        {
                            this.changeSet = this.branchDiffWorker.GenerateDiff(repo, this.package.BranchToDiffAgainst);

                            IReadOnlyObservableSet<IVsHierarchyItem> sourceItems = await this.vsHierarchyItemCollectionProvider.GetDescendantsAsync(
                                                root.HierarchyIdentity.NestedHierarchy,
                                                CancellationToken);

                            IFilteredHierarchyItemSet includedItems = await this.vsHierarchyItemCollectionProvider.GetFilteredHierarchyItemsAsync(
                                sourceItems,
                                ShouldIncludeInFilter,
                                CancellationToken);

                            return includedItems;
                        }
                        else
                        {
                            ErrorPresenter.ShowError(error);
                        }
                    }
                }

                return null;
            }

            private bool ShouldIncludeInFilter(IVsHierarchyItem hierarchyItem)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                if (hierarchyItem == null)
                {
                    return false;
                }
               
                if (HierarchyUtilities.IsPhysicalFile(hierarchyItem.HierarchyIdentity)
                    || HierarchyUtilities.IsProject(hierarchyItem.HierarchyIdentity))
                {
                    var absoluteFilePath = string.Empty;
                    if (HierarchyUtilities.IsPhysicalFile(hierarchyItem.HierarchyIdentity))
                    {
                        absoluteFilePath = hierarchyItem.CanonicalName;
                    }
                    else if (HierarchyUtilities.IsProject(hierarchyItem.HierarchyIdentity))
                    {
                        hierarchyItem.HierarchyIdentity.Hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out var prjObject);
                        if (prjObject is EnvDTE.Project project)
                        {
                            absoluteFilePath = project.FullName;
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(absoluteFilePath) 
                        && this.branchDiffWorker.HasItemInChangeSet(this.changeSet, hierarchyItem.CanonicalName, out var diffResultItem))
                    {
                        // Tag the old path so we find the Base branch version of file using the Old Path (for files renamed in the working branch)
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

                return false;
            }

            private void BranchDiffFilter_Initialized(object sender, EventArgs e)
            {
                Package.OnFilterApplied();
            }

            // We override this method to use it as a life-cycle hook to mark that our filter was un-applied
            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();
                Package?.OnFilterUnapplied();
            }
        }
    }

}
