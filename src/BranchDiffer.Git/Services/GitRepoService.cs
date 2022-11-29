using System.Linq;
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

            if (!branchesInRepo.Contains(branchOrCommitToDiffAgainst) && repo.GetCommit(branchOrCommitToDiffAgainst) == null)
            {
                message = "The Branch or Commit To Diff Against set in plugin options is not found in this repo.";
                return false;
            }
            else if (string.IsNullOrEmpty(activeBranch) || !branchesInRepo.Contains(activeBranch))
            {
                message = "There is no HEAD set in this repo.";
                return false;
            }
            else if (activeBranch.Equals(branchOrCommitToDiffAgainst))
            {
                message = "The Branch To Diff Against cannot be the same as the working branch of the repo.";
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
