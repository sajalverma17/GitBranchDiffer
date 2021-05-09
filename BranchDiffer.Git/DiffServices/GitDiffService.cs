using BranchDiffer.Git.DiffModels;
using LibGit2Sharp;
using System.Collections.Generic;
using System.Linq;

namespace BranchDiffer.Git.DiffServices
{
    public interface IGitDiffService
    {
        HashSet<DiffResultItem> GetDiffedChangeSet(Repository gitRepo, DiffBranchPair diffBranchPair);
    }

    public class GitDiffService : IGitDiffService
    {
        public HashSet<DiffResultItem> GetDiffedChangeSet(Repository gitRepo, DiffBranchPair diffBranchPair)
        {
            var compareOptions = new CompareOptions
            {
                Algorithm = DiffAlgorithm.Minimal,
                IncludeUnmodified = false,
            };

            var branchDiffResult = gitRepo.Diff.Compare<TreeChanges>(
                diffBranchPair.BranchToDiffAgainst.Tip.Tree,
                diffBranchPair.WorkingBranch.Tip.Tree,
                compareOptions);

            // Can not include delete items, no way to show it in Solution Explorer
            // No special handling to Conflicts/Copied change kinds.
            var modifiedTreeChanges = branchDiffResult.Modified;
            var addedTreeChanges = branchDiffResult.Added;
            var renamedTreeChanges = branchDiffResult.Renamed;
            var allChanges = modifiedTreeChanges
                .Concat(addedTreeChanges)
                .Concat(renamedTreeChanges);

            HashSet<DiffResultItem> changedPathsSet = new HashSet<DiffResultItem>();
            foreach (var treeEntryChange in allChanges)
            {
                // Issue with LibGit2Sharp: Paths returned are *-nix format, not windows directory format.
                var itemPathWithCorrectSeparator = treeEntryChange.Path.Replace("/", Constants.DirectorySeperator);
                var repoPathWithCorrectSeperator = gitRepo.Info.WorkingDirectory.Replace("/", Constants.DirectorySeperator);

                var diffedObject = new DiffResultItem
                {
                    AbsoluteFilePath =
                    repoPathWithCorrectSeperator.ToLowerInvariant()
                    + itemPathWithCorrectSeparator.ToLowerInvariant(),

                    OldAbsoluteFilePath = treeEntryChange.Status == ChangeKind.Renamed ? treeEntryChange.OldPath.Replace("/", Constants.DirectorySeperator) : string.Empty,
                };

                changedPathsSet.Add(diffedObject);
            }

            return changedPathsSet;
        }
    }
}
