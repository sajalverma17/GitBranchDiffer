using BranchDiffer.Git.DiffModels;
using LibGit2Sharp;
using System.Collections.Generic;
using System.IO;

namespace BranchDiffer.Git.DiffServices
{
    public interface IGitBranchDiffService
    {
        HashSet<DiffResultItem> GetDiffFileNames(string repo, string baseBranch);
    }

    public class GitBranchDiffService : IGitBranchDiffService
    {
        public HashSet<DiffResultItem> GetDiffFileNames(string repoPath, string baseBranchName)
        {
            var gitRepo = new Repository(repoPath);
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
            HashSet<DiffResultItem> changedPathsSet = new HashSet<DiffResultItem>();

            foreach (var item in modifiedTreeChanges)
            {
                var itemPathWithCorrectSeperator = item.Path.Replace("/", Constants.DirectorySeperator);
                var diffedObject = new DiffResultItem
                {
                    AbsoluteFilePath = 
                    repoPath.ToLowerInvariant() 
                    + Constants.DirectorySeperator 
                    + itemPathWithCorrectSeperator.ToLowerInvariant()
                };

                changedPathsSet.Add(diffedObject);
            }

            return changedPathsSet;
        }
    }
}
