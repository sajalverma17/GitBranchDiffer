using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.Filter
{
    [SolutionTreeFilterProvider(BranchDiffFilterCommandGuids.guidBranchDiffWindowPackageCmdSet, (uint)(BranchDiffFilterCommandGuids.CommandIdGenerateDiffAndFilter))]
    public class BranchDiffFilterProvider : HierarchyTreeFilterProvider
    {
        [ImportingConstructor]
        public BranchDiffFilterProvider()
        {
        }

        protected override HierarchyTreeFilter CreateFilter()
        {
            throw new NotImplementedException();
        }

        private sealed class BranchDiffFilter : HierarchyTreeFilter
        {
            protected override Task<IReadOnlyObservableSet> GetIncludedItemsAsync(IEnumerable<IVsHierarchyItem> rootItems)
            {
                throw new NotImplementedException();
            }
        }
    }

}
