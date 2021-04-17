using BranchDiffer.Git.DiffModels;
using BranchDiffer.Git.DiffServices;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GitBranchDiffer.ViewModels
{
    public class BranchDiffViewModel
    {
        private readonly IGitBranchDiffService gitBranchDiffService;
        private readonly IItemIdentityService itemIdentityService;

        public BranchDiffViewModel(
            IGitBranchDiffService gitBranchDiffService,
            IItemIdentityService itemIdentityService)
        {
            this.gitBranchDiffService = gitBranchDiffService;
            this.itemIdentityService = itemIdentityService;
        }

        public HashSet<DiffResultItem> GenerateDiff(string branchToDiff)
        {
            return this.gitBranchDiffService.GetDiffFileNames(@"C:\Tools\ProjectUnderTest", branchToDiff);
        }

        public bool HasItemInChangeSet(HashSet<DiffResultItem> changeSet, string vsItemAbsolutePath)
        {
            return this.itemIdentityService.HasItemInChangeSet(changeSet, vsItemAbsolutePath);
        }
    }
}
