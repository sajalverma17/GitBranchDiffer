using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BranchDiffer.Git.Models;
using LibGit2Sharp;

namespace BranchDiffer.Git.Services
{
    public interface IGitRepoService
    {
        bool IsRepoStateValid(Repository repo, string branchToDiffAgainst, out string message);

        DiffBranchPair GetBranchesToDiffFromRepo(Repository repository, string branchNameToDiffAgainst);
    }

    public class GitRepoService : IGitRepoService
    {
        public bool IsRepoStateValid(Repository repo, string branchToDiffAgainst, out string message)
        {
            var branchesInRepo = repo.Branches.Select(branch => branch.FriendlyName);
            var activeBranch = repo.Head?.FriendlyName;

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

        public DiffBranchPair GetBranchesToDiffFromRepo(Repository repository, string branchNameToDiffAgainst)
        {
            var branchToDiffAgainst = repository.Branches[branchNameToDiffAgainst];            
            return new DiffBranchPair { WorkingBranch = repository.Head, BranchToDiffAgainst = branchToDiffAgainst };
        }
    }
}
