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

        public HashSet<DiffResultItem> GenerateDiff(string solutionPath, string branchToDiff)
        {
            // TODO : Add new service which gets a Git repo from solutionPath.
            /* Do below validations in that service:
             * For eg. (1) Branch set in Plugin options should not be identical as the Git HEAD
             *         (2) Below validation:
            if (this.branchToDiffWith is not found in the active repo)
            {
                VsShellUtilities.ShowMessageBox(
                    this.gitBranchDifferPackage,
                    $"{this.branchToDiffWith} branch is not found in this Repo", 
                    "Git Branch Differ")
            }*/
            return this.gitBranchDiffService.GetDiffFileNames(solutionPath, branchToDiff);
        }

        public bool HasItemInChangeSet(HashSet<DiffResultItem> changeSet, string vsItemAbsolutePath)
        {
            return this.itemIdentityService.HasItemInChangeSet(changeSet, vsItemAbsolutePath);
        }
    }
}
