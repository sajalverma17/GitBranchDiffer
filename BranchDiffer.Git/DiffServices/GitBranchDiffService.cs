using BranchDiffer.Git.DiffModels;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BranchDiffer.Git.DiffServices
{
    public interface IGitBranchDiffService
    {
        IList<string> GetDiffFileNames(string repo, string baseBranch);
    }

    public class GitBranchDiffService : IGitBranchDiffService
    {
        public IList<string> GetDiffFileNames(string repo, string baseBranchName)
        {
            var gitRepo = new Repository(repo);
            var workingBranchName = gitRepo.Head.FriendlyName;
            var workingBranch = gitRepo.Branches[workingBranchName];
            var baseBranch = gitRepo.Branches[baseBranchName];

            var compareOptions = new CompareOptions
            {
                Algorithm = DiffAlgorithm.Minimal,
                IncludeUnmodified = false,
            };

            var branchDiffResult = gitRepo.Diff.Compare<TreeChanges>(baseBranch.Tip.Tree, workingBranch.Tip.Tree, compareOptions);
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
