using System.Linq;
using BranchDiffer.Git.Models.LibGit2SharpModels;

namespace BranchDiffer.Git.Services
{
    public interface IGitRepoService
    {
        IGitObject GetGitObjectFromName(IGitRepository repository, string friendlyName);

        IGitObject GetGitObjectFromSha(IGitRepository repository, string sha);
    }

    public class GitRepoService : IGitRepoService
    {
        public IGitObject GetGitObjectFromName(IGitRepository repository, string friendlyName)
        {
            IGitObject gitObject;
            if (repository.Branches.Contains(friendlyName))
            {
                gitObject = repository.Branches[friendlyName];
            }
            else
            {
                gitObject = repository.GetCommit(friendlyName);
                if (gitObject == null)
                {
                    gitObject = repository.GetTag(friendlyName);
                    if (gitObject == null)
                    {
                        return null;
                    }
                }
            }

            return gitObject;
        }

        // Searches branch's tip and commits, no need to search tags by SHA as you'll get the same commit either way
        public IGitObject GetGitObjectFromSha(IGitRepository repository, string sha)
        {
            IGitObject gitObject;
            if (repository.Branches.Any(x => x.TipSha == sha))
            {
                gitObject = repository.Branches.FirstOrDefault(x => x.TipSha == sha);
            }
            else
            {
                gitObject = repository.GetCommit(sha);
                if (gitObject == null)
                {
                    return null;
                }
            }

            return gitObject;
        }

        /*
        // TODO Refractor and use in the method below to validate?
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
        */
    }
}
