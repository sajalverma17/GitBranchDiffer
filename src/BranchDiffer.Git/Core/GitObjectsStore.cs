using BranchDiffer.Git.Models.LibGit2SharpModels;
using BranchDiffer.Git.Services;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Core
{
    public class GitObjectsStore
    {
        private readonly IGitRepositoryFactory gitRepositoryFactory;

        public GitObjectsStore(IGitRepositoryFactory gitRepositoryFactory) 
        {
            this.gitRepositoryFactory = gitRepositoryFactory;
        }

        public IEnumerable<GitBranch> GetBranches(string solutionPath)
        {
            using (var repo = this.gitRepositoryFactory.Create(solutionPath))
            {
                return repo.Branches.Cast<GitBranch>().ToList();
            }
        }

        public IEnumerable<IGitCommit> GetRecentCommits(string solutionPath)
        {
            using (var repo = this.gitRepositoryFactory.Create(solutionPath))
            {
                return repo.GetRecentCommits();
            }
        }
    }
}
