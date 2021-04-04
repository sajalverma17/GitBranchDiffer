using EnvDTE80;
using GitBranchDiffer.ViewModels;
using Microsoft;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.Filter
{
    [SolutionTreeFilterProvider(BranchDiffFilterCommandGuids.guidGitBranchDifferPackageCmdSet, (uint)(BranchDiffFilterCommandGuids.CommandIdGenerateDiffAndFilter))]
    public class BranchDiffFilterProvider : HierarchyTreeFilterProvider
    {
        private readonly SVsServiceProvider serviceProvider;
        private readonly IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider;
        private static GitBranchDifferPackage Package;

        [ImportingConstructor]
        public BranchDiffFilterProvider(
            SVsServiceProvider serviceProvider,
            IVsHierarchyItemCollectionProvider hierarchyCollectionProvider)
        {
            this.serviceProvider = serviceProvider;
            this.vsHierarchyItemCollectionProvider = hierarchyCollectionProvider;
        }

        public static void Init(GitBranchDifferPackage package)
        {
            Package = package;
        }

        protected override HierarchyTreeFilter CreateFilter()
        {
            return new BranchDiffFilter(Package, this.serviceProvider, this.vsHierarchyItemCollectionProvider);            
        }

        private sealed class BranchDiffFilter : HierarchyTreeFilter
        {
            private readonly SVsServiceProvider serviceProvider;
            private readonly IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider;
            private readonly GitBranchDifferPackage package;
            private IList<string> changeList;

            public BranchDiffFilter(GitBranchDifferPackage package, SVsServiceProvider serviceProvider, IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider)
            {
                this.package = package;
                this.serviceProvider = serviceProvider;
                this.vsHierarchyItemCollectionProvider = vsHierarchyItemCollectionProvider;
            }

            protected override async Task<IReadOnlyObservableSet> GetIncludedItemsAsync(IEnumerable<IVsHierarchyItem> rootItems)
            {
                var vm = DIContainer.Instance.GetService(typeof(BranchDiffViewModel)) as BranchDiffViewModel;
                Assumes.Present(vm);

                vm.Init(this.package);
                if (vm.Validate())
                {
                    changeList = vm.Generate();

                    IVsHierarchyItem root = HierarchyUtilities.FindCommonAncestor(rootItems);
                    IReadOnlyObservableSet<IVsHierarchyItem> sourceItems;

                    sourceItems = await this.vsHierarchyItemCollectionProvider.GetDescendantsAsync(
                                        root.HierarchyIdentity.NestedHierarchy,
                                        CancellationToken);

                    IFilteredHierarchyItemSet includedItems = await this.vsHierarchyItemCollectionProvider.GetFilteredHierarchyItemsAsync(
                        sourceItems,
                        ShouldIncludeInFilter,
                        CancellationToken);

                    return includedItems;
                }

                return null;
            }

            private bool ShouldIncludeInFilter(IVsHierarchyItem hierarchyItem)
            {
                if (hierarchyItem == null)
                {
                    return false;
                }
                return this.changeList.Contains(hierarchyItem.Text);
            }
        }
    }

}
