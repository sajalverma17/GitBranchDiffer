using BranchDiffer.Git.DiffModels;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BranchDiffer.Git.DiffServices
{
    public interface IGitBranchDiffService
    {
        IEnumerable<string> GetDiffFileNames(string repo, string currentBranch, string baseBranch);
    }

    public class GitBranchDiffService : IGitBranchDiffService
    {
        public IEnumerable<string> GetDiffFileNames(string repo, string currentBranch, string baseBranch)
        {
            var gitRepo = new Repository(repo);
            var selectedWorkingBranch = gitRepo.Branches[currentBranch];
            var selectedBaseBranch = gitRepo.Branches[baseBranch];

            // TODO : Include rename detections, diff lines
            var compareOptions = new CompareOptions
            {
                Algorithm = DiffAlgorithm.Minimal,
                IncludeUnmodified = false,
            };

            var branchDiffResult = gitRepo.Diff.Compare<TreeChanges>(selectedBaseBranch.Tip.Tree, selectedWorkingBranch.Tip.Tree, compareOptions);
            var modifiedTreeChanges = branchDiffResult.Modified;

            using (var iterator = modifiedTreeChanges.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    yield return iterator.Current.Path;
                }
            }
        }
    }
}
