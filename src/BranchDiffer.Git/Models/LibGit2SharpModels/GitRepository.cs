using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{
    public interface IGitRepository : IDisposable
    {
        GitBranchCollection Branches { get; }

        IGitObject Head { get; }

        string WorkingDirectory { get; }

        Diff Diff { get; }

        IEnumerable<IGitCommit> GetRecentCommits(int number = 50);

        IGitCommit GetCommit(string commitSha);
    }

    internal sealed class GitRepository : IGitRepository
    {
        private readonly IRepository repository;
        private readonly IGitObject head;
        private readonly GitBranchCollection gitBranches;

        public GitRepository(IRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.head = new GitBranch(this.repository.Head);
            this.gitBranches = new GitBranchCollection();
            repository.Branches.ToList().ForEach(x => this.gitBranches.Add(new GitBranch(x)));
        }

        public GitBranchCollection Branches => this.gitBranches;

        public IGitObject Head => this.head;

        public Diff Diff => this.repository.Diff;

        public string WorkingDirectory => this.repository.Info.WorkingDirectory;

        public IEnumerable<IGitCommit> GetRecentCommits(int number = 50)
        {
            return this.repository.Commits
                .QueryBy(new CommitFilter()
                {
                    FirstParentOnly = true,
                    SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time,
                    IncludeReachableFrom = repository.Head.Tip
                })
                .Take(number)
                .Select(x => new GitCommit(x))
                .ToList();
        }

        public IGitCommit GetCommit(string commitSha)
        {
            Commit commit = null;
            try
            {
                commit = this.repository.Lookup<Commit>(commitSha);
            }
            catch (AmbiguousSpecificationException) { }
            catch (InvalidSpecificationException) { }
            
            return commit != null ? new GitCommit(commit) : null;
        }

        public void Dispose()
        {
            // Dispose the native instance of repository
            this.repository.Dispose();
        }
    }
}
