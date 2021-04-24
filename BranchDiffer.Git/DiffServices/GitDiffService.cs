using BranchDiffer.Git.DiffModels;
using LibGit2Sharp;
using System.Collections.Generic;
using System.Linq;

namespace BranchDiffer.Git.DiffServices
{
    public interface IGitDiffService
    {
        HashSet<DiffResultItem> GetDiffedChangeSet(Repository gitRepo, DiffBranchPair diffBranchPair);

        IEnumerable<HunkRangeInfo> GetFileDiff(Repository gitRepo, DiffBranchPair diffBranchPair, string filePath);
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

            var modifiedTreeChanges = branchDiffResult.Modified;
            HashSet<DiffResultItem> changedPathsSet = new HashSet<DiffResultItem>();
            foreach (var item in modifiedTreeChanges)
            {
                // Issue with LibGit2Sharp: Paths returned are *-nix format, not windows directory format.
                var itemPathWithCorrectSeparator = item.Path.Replace("/", Constants.DirectorySeperator);
                var repoPathWithCorrectSeperator = gitRepo.Info.WorkingDirectory.Replace("/", Constants.DirectorySeperator);

                var diffedObject = new DiffResultItem
                {
                    AbsoluteFilePath =
                    repoPathWithCorrectSeperator.ToLowerInvariant()
                    + itemPathWithCorrectSeparator.ToLowerInvariant()
                };

                changedPathsSet.Add(diffedObject);
            }

            return changedPathsSet;
        }

        public IEnumerable<HunkRangeInfo> GetFileDiff(Repository gitRepo, DiffBranchPair diffBranchPair, string filePath)
        {
            int contextLines = 0;
            var compareOptions = new CompareOptions
            {
                Algorithm = DiffAlgorithm.Minimal,
                IncludeUnmodified = false,
                ContextLines = contextLines,
                InterhunkLines = 0,
            };

            var fileStateInRepo = gitRepo.RetrieveStatus(filePath);

            if (fileStateInRepo == FileStatus.Ignored || fileStateInRepo == FileStatus.Nonexistent)
            {
                yield break;
            }

            var patches = gitRepo.Diff.Compare<Patch>(
                diffBranchPair.BranchToDiffAgainst.Tip.Tree, 
                diffBranchPair.WorkingBranch.Tip.Tree, 
                Enumerable.Repeat(filePath, 1), compareOptions);

            if (patches.Any())
            {
                var patch = patches.First().Patch;
                var diffParser = new GitFileDiffParser(patch, contextLines, false);
                foreach (var hunkrange in diffParser.Parse())
                {
                    yield return hunkrange;
                }
            }
        }
    }
}
