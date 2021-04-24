using BranchDiffer.Git.DiffModels;
using GitBranchDiffer.ViewModels;
using Microsoft;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace GitBranchDiffer.Filter
{
    [SolutionTreeFilterProvider(BranchDiffFilterCommandGuids.guidGitBranchDifferPackageCmdSet, (uint)(BranchDiffFilterCommandGuids.CommandIdGenerateDiffAndFilter))]    
    public class BranchDiffFilterProvider : HierarchyTreeFilterProvider
    {
        private readonly SVsServiceProvider serviceProvider;
        private readonly IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider;
        private static GitBranchDifferPackage Package;
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
        
        /// <summary>
        /// True if the BranchDiffFilter is applied on the Solution Explorer.
        /// False otherwise.
        /// </summary>
        internal static bool IsFilterApplied { get; private set; }

        /// <summary>
        /// Initializes the Solution Explorer filter once per-Visual-Studio-startup.
        /// </summary>
        /// <param name="package">
        /// The package becomes a static dependency of this filter,
        /// and is required in order to get the plugin option <see cref="GitBranchDifferPackage.BranchToDiffAgainst"/> set by user.</param>
        internal static void InitializeOnce(GitBranchDifferPackage package)
        {
            Package = package;
        }

        /// <summary>
        /// Initalizes the Solution Explorer filter with Solution info once per-solution-load in Visual Studio
        /// </summary>
        /// <param name="solutionPath"></param>
        internal static void Initialize(string solutionDirectory, string solutionFile)
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
            private readonly GitBranchDifferPackage package;
            private readonly string solutionDirectory;
            private readonly string solutionFile;

            private GitBranchDifferService branchDiffService;
            private HashSet<DiffResultItem> changeSet;

            public BranchDiffFilter(GitBranchDifferPackage package, string solutionPath, string solutionName, SVsServiceProvider serviceProvider, IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider)
            {
                this.package = package;
                this.solutionDirectory = solutionPath;
                this.solutionFile = solutionName;
                this.serviceProvider = serviceProvider;
                this.vsHierarchyItemCollectionProvider = vsHierarchyItemCollectionProvider;
                this.Initialized += BranchDiffFilter_Initialized;
            }

            protected override async Task<IReadOnlyObservableSet> GetIncludedItemsAsync(IEnumerable<IVsHierarchyItem> rootItems)
            {
                this.branchDiffService = DIContainer.Instance.GetService(typeof(GitBranchDifferService)) as GitBranchDifferService;
                Assumes.Present(this.branchDiffService);

                if (GitBranchDifferValidator.ValidatePackage(this.package))
                {
                    IVsHierarchyItem root = HierarchyUtilities.FindCommonAncestor(rootItems);
                    if (GitBranchDifferValidator.ValidateSolution(this.solutionDirectory, this.solutionFile, root, this.package))
                    {
                        var setupOk = this.branchDiffService.SetupRepository(this.solutionDirectory, this.package.BranchToDiffAgainst, out var repo, out var error);
                        if (setupOk)
                        {
                            this.changeSet = this.branchDiffService.GenerateDiff(repo, this.package.BranchToDiffAgainst);
                            repo.Dispose();

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
                            ErrorPresenter.ShowError(this.package, error);
                        }
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

                // Only support showing diffed Physical Files (leaf nodes on soltuion explorer)
                if (HierarchyUtilities.IsPhysicalFile(hierarchyItem.HierarchyIdentity))
                {
                    return this.branchDiffService.HasItemInChangeSet(this.changeSet, hierarchyItem.CanonicalName);
                }

                return false;
            }

            private void BranchDiffFilter_Initialized(object sender, EventArgs e)
            {
                BranchDiffFilterProvider.IsFilterApplied = true;
            }

            // We override this method to use it as a life-cycle hook to mark that our filter was un-applied.
            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();
                BranchDiffFilterProvider.IsFilterApplied = false;
            }          
        }
    }

}
