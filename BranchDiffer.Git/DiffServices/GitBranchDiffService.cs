using BranchDiffer.Git.DiffModels;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BranchDiffer.Git.DiffServices
{
    public interface IGitBranchDiffService
    {
        IList<string> GetDiffFileNames(string repo, string currentBranch, string baseBranch);
    }

    public class GitBranchDiffService : IGitBranchDiffService
    {
        public IList<string> GetDiffFileNames(string repo, string currentBranch, string baseBranch)
        {
            var gitRepo = new Repository(repo);
            var selectedWorkingBranch = gitRepo.Branches[currentBranch];
            var selectedBaseBranch = gitRepo.Branches[baseBranch];

            var compareOptions = new CompareOptions
            {
                Algorithm = DiffAlgorithm.Minimal,
                IncludeUnmodified = false,
            };

            var branchDiffResult = gitRepo.Diff.Compare<TreeChanges>(selectedBaseBranch.Tip.Tree, selectedWorkingBranch.Tip.Tree, compareOptions);
            var modifiedTreeChanges = branchDiffResult.Modified;

            // could get WAY TOO big on large diffs (if merge only diff?)
            List<string> resultList = new List<string>();

            foreach (var item in modifiedTreeChanges)
            {
                resultList.Add(item.Path);
            }

            return resultList;
        }
    }
}
