﻿using BranchDiffer.Git.DiffModels;
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

            // TODO: Include diffResult.Added and Renamed. Should work since the file will already be present in working branch.
            // Can not include delete items. Even if you want to get the deleted files in changeset, how would you add it in Solution Explorer hierarchy to display it?
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
    }
}
