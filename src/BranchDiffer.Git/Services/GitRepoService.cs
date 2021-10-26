using System.Linq;
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
        public bool IsRepoStateValid(IGitRepository repo, string branchToDiffAgainst, out string message)
        {
            var branchesInRepo = repo.Branches.Select(branch => branch.Name);
            var activeBranch = repo.Head?.Name;

            if (!branchesInRepo.Contains(branchToDiffAgainst))
            {
                message = "The Branch To Diff Against set in plugin options is not found in this repo.";
                return false;
            }
            else if (string.IsNullOrEmpty(activeBranch) || !branchesInRepo.Contains(activeBranch))
            {
                message = "There is no HEAD set in this repo.";
                return false;
            }
            else if (activeBranch.Equals(branchToDiffAgainst))
            {
                message = "The Branch To Diff Against cannot be the same as the working branch of the repo.";
                return false;
            }

            message = null;
            return true;
        }

        public DiffBranchPair GetBranchesToDiffFromRepo(IGitRepository repository, string branchNameToDiffAgainst)
        {
            // Git branches are case-insensitive, you can't create a new branch named "MASTER" if "master" is already present.
            var branchToDiffAgainst = repository.Branches[branchNameToDiffAgainst];
            return new DiffBranchPair { WorkingBranch = repository.Head, BranchToDiffAgainst = branchToDiffAgainst };
        }
    }
}
