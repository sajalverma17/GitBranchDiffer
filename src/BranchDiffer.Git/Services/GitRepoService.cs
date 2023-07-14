using System;
using System.Linq;
using System.Xml;
using BranchDiffer.Git.Exceptions;
using BranchDiffer.Git.Models;
using BranchDiffer.Git.Models.LibGit2SharpModels;
using LibGit2Sharp;

namespace BranchDiffer.Git.Services
{
    public interface IGitRepoService
    {
        IGitObject GetGitObjectFromName(IGitRepository repository, string objectName);
    }

    public class GitRepoService : IGitRepoService
    {
        // TODO Refractor and use in the method below
        private bool IsRepoStateValid(IGitRepository repo, string branchOrCommitToDiffAgainst, out string message)
        {
            var branchesInRepo = repo.Branches.Select(branch => branch.FriendlyName);
            var activeBranch = repo.Head?.FriendlyName;
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
            else if (activeBranch.Equals(branchOrCommitToDiffAgainst) || repo.Head.TipSha.Equals(commitToDiffAgainst.TipSha))
            {
                message = "The Branch or Commit to diff against cannot be the same as HEAD.";
                return false;
            }

            message = null;
            return true;
        }

        public IGitObject GetGitObjectFromName(IGitRepository repository, string objectName)
        {
            IGitObject gitObject;
            if (repository.Branches.Contains(objectName))
            {
                gitObject = repository.Branches[objectName];
            }
            else
            {
                gitObject = repository.GetCommit(objectName);
                if (gitObject == null)
                {
                    gitObject = repository.GetTag(objectName);
                    if (gitObject == null)
                    {
                        throw new GitOperationException("Given branch name/commit sha/tag name not found in repo");
                    }
                }
            }

            return gitObject;
        }
    }
}
