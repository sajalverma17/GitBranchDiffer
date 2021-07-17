using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BranchDiffer.Git.Models;
using LibGit2Sharp;

namespace BranchDiffer.Git.DiffServices
{
    public interface IGitRepoService
    {
        bool CreateGitRepository(string directoryPath, out Repository repository, out Exception exception);

        bool IsRepoStateValid(Repository repo, string branchToDiffAgainst, out InvalidOperationException exception);

        DiffBranchPair GetBranchesToDiffFromRepo(Repository repository, string branchNameToDiffAgainst);
    }

    public class GitRepoService : IGitRepoService
    {
        public bool CreateGitRepository(string directoryPath, out Repository repository, out Exception exception)
        {
            Repository createdRepository;
            try
            {
                createdRepository = new Repository(directoryPath);
            }
            catch (RepositoryNotFoundException repoNotFoundException)
            {
                exception = repoNotFoundException;
                repository = null;
                return false;
            }

            repository = createdRepository;
            exception = null;
            return true;
        }

        public bool IsRepoStateValid(Repository repo, string branchToDiffAgainst, out InvalidOperationException exception)
        {
            var branchesInRepo = repo.Branches.Select(branch => branch.FriendlyName);
            var activeBranch = repo.Head?.FriendlyName;

            if (!branchesInRepo.Contains(branchToDiffAgainst))
            {
                exception = new InvalidOperationException("The Branch To Diff Against set in plugin options is not found in this repo.");
                return false;
            }
            else if (string.IsNullOrEmpty(activeBranch) || !branchesInRepo.Contains(activeBranch))
            {
                exception = new InvalidOperationException("There is no HEAD set in this repo.");
                return false;
            }
            else if (activeBranch.Equals(branchToDiffAgainst))
            {
                exception = new InvalidOperationException("The Branch To Diff Against cannot be the same as the working branch of the repo.");
                return false;
            }

            exception = null;
            return true;
        }

        public DiffBranchPair GetBranchesToDiffFromRepo(Repository repository, string branchNameToDiffAgainst)
        {
            var branchToDiffAgainst = repository.Branches[branchNameToDiffAgainst];            
            return new DiffBranchPair { WorkingBranch = repository.Head, BranchToDiffAgainst = branchToDiffAgainst };
        }
    }
}
