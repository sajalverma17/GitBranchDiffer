using System;
using System.Linq;
using System.Xml;
using BranchDiffer.Git.Exceptions;
using BranchDiffer.Git.Models;
using BranchDiffer.Git.Models.LibGit2SharpModels;

namespace BranchDiffer.Git.Services
{
    public interface IGitRepoService
    {
        DiffBranchPair GetBranchesToDiffFromRepo(IGitRepository repository, string branchNameToDiffAgainst);
    }

    public class GitRepoService : IGitRepoService
    {
        // TODO Refractor and use in the method below
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

        public DiffBranchPair GetBranchesToDiffFromRepo(IGitRepository repository, string branchOrSha)
        {
            IGitObject gitObject;
            if (repository.Branches.Contains(branchOrSha))
            {
                gitObject = repository.Branches[branchOrSha];
            }
            else
            {
                gitObject = repository.GetCommit(branchOrSha);
                if (gitObject == null)
                {
                    gitObject = repository.GetTag(branchOrSha);
                    if (gitObject == null)
                    {
                        throw new GitOperationException("Given branch name/commit sha/tag name not found in repo");
                    }
                }
            }

            return new DiffBranchPair { WorkingBranch = repository.Head, BranchToDiffAgainst = gitObject };
        }
    }
}
