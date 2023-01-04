﻿using System.Linq;
using System.Xml;
using BranchDiffer.Git.Models;
using BranchDiffer.Git.Models.LibGit2SharpModels;

namespace BranchDiffer.Git.Services
{
    public interface IGitRepoService
    {
        bool IsRepoStateValid(IGitRepository repo, string branchToDiffAgainst, out string message);

        DiffBranchPair GetBranchesToDiffFromRepo(IGitRepository repository, string branchNameToDiffAgainst);
    }

    public class GitRepoService : IGitRepoService
    {
        public bool IsRepoStateValid(IGitRepository repo, string branchOrCommitToDiffAgainst, out string message)
        {
            var branchesInRepo = repo.Branches.Select(branch => branch.Name);
            var activeBranch = repo.Head?.Name;
            var commitToDiffAgainst = repo.GetCommit(branchOrCommitToDiffAgainst);

            if (!branchesInRepo.Contains(branchOrCommitToDiffAgainst) && commitToDiffAgainst == null)
            {
                message = "The Branch or Commit to diff against set in plugin options is not found in this repo.";
                return false;
            }
            else if (string.IsNullOrEmpty(activeBranch) || !branchesInRepo.Contains(activeBranch))
            {
                message = "The HEAD is detached. You must checkout a branch.";
                return false;
            }
            else if (activeBranch.Equals(branchOrCommitToDiffAgainst) || repo.Head.Tip.Equals(commitToDiffAgainst))
            {
                message = "The Branch or Commit to diff against cannot be the same as HEAD.";
                return false;
            }

            message = null;
            return true;
        }

        public DiffBranchPair GetBranchesToDiffFromRepo(IGitRepository repository, string branchNameToDiffAgainst)
        {
            IGitObject gitBranchOrCommit;
            if (repository.Branches.Contains(branchNameToDiffAgainst))
            {
                gitBranchOrCommit = repository.Branches[branchNameToDiffAgainst];
            }
            else
            {
                var commit = repository.GetCommit(branchNameToDiffAgainst);
                gitBranchOrCommit = new GitBranch(commit);
            }

            return new DiffBranchPair { WorkingBranch = repository.Head, BranchToDiffAgainst = gitBranchOrCommit };
        }
    }
}
