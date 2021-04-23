using BranchDiffer.Git.DiffModels;
using GitBranchDiffer.ViewModels;
using Microsoft;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
        /// Initializes the Solution Explorer filter once per-Visual-Studio-startup.
        /// </summary>
        /// <param name="package">
        /// The package becomes a static dependency of this filter,
        /// and is required in order to get the plugin option <see cref="GitBranchDifferPackage.BranchToDiffAgainst"/> set by user.</param>
        public static void InitializeOnce(GitBranchDifferPackage package)
        {
            Package = package;
        }

        /// <summary>
        /// Initalizes the Solution Explorer filter with Solution info once per-solution-load in Visula Studio
        /// </summary>
        /// <param name="solutionPath"></param>
        public static void Initialize(string solutionDirectory, string solutionFile)
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
            private string solutionDirectory;
            private string solutionFile;

            private GitBranchDifferService branchDiffService;
            private HashSet<DiffResultItem> changeSet;

            public BranchDiffFilter(GitBranchDifferPackage package, string solutionPath, string solutionName, SVsServiceProvider serviceProvider, IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider)
            {
                this.package = package;
                this.solutionDirectory = solutionPath;
                this.solutionFile = solutionName;
                this.serviceProvider = serviceProvider;
                this.vsHierarchyItemCollectionProvider = vsHierarchyItemCollectionProvider;
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
        }
    }

}
