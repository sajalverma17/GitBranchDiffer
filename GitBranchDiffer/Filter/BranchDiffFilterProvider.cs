using EnvDTE80;
using GitBranchDiffer.ViewModels;
using Microsoft;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.Filter
{
    [SolutionTreeFilterProvider(BranchDiffFilterCommandGuids.guidBranchDiffWindowPackageCmdSet, (uint)(BranchDiffFilterCommandGuids.CommandIdGenerateDiffAndFilter))]
    public class BranchDiffFilterProvider : HierarchyTreeFilterProvider
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider;
        private static GitBranchDifferPackage Package;

        [ImportingConstructor]
        public BranchDiffFilterProvider(
            IServiceProvider serviceProvider,
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
            // Check if filter was initialized. 
            Assumes.Present(Package);
            return new BranchDiffFilter(this.serviceProvider, this.vsHierarchyItemCollectionProvider);
        }

        private sealed class BranchDiffFilter : HierarchyTreeFilter
        {
            private readonly IServiceProvider serviceProvider;
            private readonly IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider;
            private IList<string> changeList;

            public BranchDiffFilter(IServiceProvider serviceProvider, IVsHierarchyItemCollectionProvider vsHierarchyItemCollectionProvider)
            {
                this.serviceProvider = serviceProvider;
                this.vsHierarchyItemCollectionProvider = vsHierarchyItemCollectionProvider;
            }

            protected override async Task<IReadOnlyObservableSet> GetIncludedItemsAsync(IEnumerable<IVsHierarchyItem> rootItems)
            {
                var vm = DIContainer.Instance.GetService(typeof(BranchDiffViewModel)) as BranchDiffViewModel;
                Assumes.Present(vm);
                vm.Init(Package);
                if (vm.Validate())
                {
                    changeList = vm.Generate();

                    IVsHierarchyItem root = HierarchyUtilities.FindCommonAncestor(rootItems);
                    IReadOnlyObservableSet<IVsHierarchyItem> sourceItems;

                    sourceItems = await this.vsHierarchyItemCollectionProvider.GetDescendantsAsync(
                                        root.HierarchyIdentity.NestedHierarchy,
                                        CancellationToken);

                    /*
                    var dte2 = await ServiceProvider.GetGlobalServiceAsync(typeof(SDTE)) as DTE2;
                    var selectedItems = dte2.;
                    */

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
