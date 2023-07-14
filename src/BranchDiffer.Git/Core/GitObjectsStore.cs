using BranchDiffer.Git.Exceptions;
using BranchDiffer.Git.Models.LibGit2SharpModels;
using BranchDiffer.Git.Services;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BranchDiffer.Git.Core
{
    public class GitObjectsStore
    {
        private readonly IGitRepositoryFactory gitRepositoryFactory;
        private readonly IGitRepoService gitRepoService;

        public GitObjectsStore(IGitRepositoryFactory gitRepositoryFactory, IGitRepoService gitRepoService) 
        {
            this.gitRepositoryFactory = gitRepositoryFactory;
            this.gitRepoService = gitRepoService;
        }

        public IGitObject GetDefaultGitReferenceObject(string solutionPath)
        {
            using (var repo = this.gitRepositoryFactory.Create(solutionPath))
            {
                var gitObject = repo.Branches.Cast<IGitObject>().FirstOrDefault(x => x.FriendlyName == "master" || x.FriendlyName == "main") ?? repo.GetRecentCommits().FirstOrDefault();
                return gitObject;
            }
        }

        public IGitObject FindReferenceObjectByFriendlyName(string solutionPath, string name)
        {
            using (var repo = this.gitRepositoryFactory.Create(solutionPath))
            {
                return this.gitRepoService.GetGitObjectFromName(repo, name);
            }
        }

        public IEnumerable<GitBranch> GetBranches(string solutionPath)
        {
            using (var repo = this.gitRepositoryFactory.Create(solutionPath))
            {
                return repo.Branches.Cast<GitBranch>().ToList();
            }
        }

        public IEnumerable<GitCommit> GetRecentCommits(string solutionPath)
        {
            using (var repo = this.gitRepositoryFactory.Create(solutionPath))
            {
                return repo.GetRecentCommits();
            }
        }

        public IEnumerable<GitTag> GetRecentTags(string solutionPath) 
        {
            using (var repo = this.gitRepositoryFactory.Create(solutionPath))
            {
                return repo.GetRecentTags();
            }
        }
    }
}
