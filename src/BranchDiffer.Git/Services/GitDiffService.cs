using BranchDiffer.Git.Models;
using BranchDiffer.Git.Models.LibGit2SharpModels;
using LibGit2Sharp;
using System.Collections.Generic;
using System.Linq;

namespace BranchDiffer.Git.Services
{
    public interface IGitDiffService
    {
        HashSet<DiffResultItem> GetDiffedChangeSet(IGitRepository gitRepo, DiffBranchPair diffBranchPair);
    }

    public class GitDiffService : IGitDiffService
    {
        public HashSet<DiffResultItem> GetDiffedChangeSet(IGitRepository gitRepo, DiffBranchPair diffBranchPair)
        {
            var compareOptions = new CompareOptions
            {
                Algorithm = DiffAlgorithm.Minimal,
                IncludeUnmodified = false,
            };

            var branchDiffResult = gitRepo.Diff.Compare<TreeChanges>(
                gitRepo.GetCommitTree(diffBranchPair.BranchToDiffAgainst.TipSha),
                gitRepo.GetCommitTree(diffBranchPair.WorkingBranch.TipSha),
                compareOptions);

            HashSet<DiffResultItem> changedPathsSet = new HashSet<DiffResultItem>();
            foreach (var treeEntryChange in GetRelevantChanges(branchDiffResult))
            {                
                changedPathsSet.Add(CreateDiffedObject(gitRepo, treeEntryChange));
            }

            return changedPathsSet;
        }

        private static IEnumerable<TreeEntryChanges> GetRelevantChanges(TreeChanges branchDiffResult)
        {
            var modifiedTreeChanges = branchDiffResult.Modified;
            var addedTreeChanges = branchDiffResult.Added;
            var renamedTreeChanges = branchDiffResult.Renamed;
            var allChanges = modifiedTreeChanges
                .Concat(addedTreeChanges)
                .Concat(renamedTreeChanges);
            return allChanges;
        }

        private static DiffResultItem CreateDiffedObject(IGitRepository gitRepo, TreeEntryChanges treeEntryChange)
        {
            var itemPathWithCorrectSeparator = treeEntryChange.Path.Replace("/", Constants.DirectorySeperator);
            var repoPathWithCorrectSeperator = gitRepo.WorkingDirectory.Replace("/", Constants.DirectorySeperator);

            var diffedObject = new DiffResultItem
            {
                AbsoluteFilePath =
                repoPathWithCorrectSeperator.ToLowerInvariant()
                + itemPathWithCorrectSeparator.ToLowerInvariant(),

                OldAbsoluteFilePath = treeEntryChange.Status == ChangeKind.Renamed ? treeEntryChange.OldPath.Replace("/", Constants.DirectorySeperator) : string.Empty,
            };

            return diffedObject;
        }
    }
}
