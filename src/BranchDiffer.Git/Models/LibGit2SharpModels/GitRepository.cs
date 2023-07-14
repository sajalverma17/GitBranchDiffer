using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{
    public interface IGitRepository : IDisposable
    {
        string WorkingDirectory { get; }

        Diff Diff { get; }

        GitBranch Head { get; }

        GitBranchCollection Branches { get; }

        IEnumerable<GitCommit> GetRecentCommits(int number = 50);

        IEnumerable<GitTag> GetRecentTags(int number = 50);

        GitCommit GetCommit(string commitSha);
    }

    internal sealed class GitRepository : IGitRepository
    {
        private readonly IRepository repository;
        private readonly GitBranch head;
        private readonly GitBranchCollection gitBranches;

        public GitRepository(IRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.gitBranches = new GitBranchCollection();

            this.head = new GitBranch(this.repository.Head.FriendlyName, new GitReference(this.repository.Head.Tip));
            repository.Branches.Where(b => !b.IsRemote).ToList().ForEach(x => this.gitBranches.Add(new GitBranch(x.FriendlyName, new GitReference(x.Tip))));
        }

        public GitBranchCollection Branches => this.gitBranches;

        public GitBranch Head => this.head;

        public Diff Diff => this.repository.Diff;

        public string WorkingDirectory => this.repository.Info.WorkingDirectory;

        public IEnumerable<GitCommit> GetRecentCommits(int number = 50)
        {
            return this.repository.Commits
                .QueryBy(new CommitFilter()
                {
                    FirstParentOnly = true,
                    SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time,
                    IncludeReachableFrom = repository.Head.Tip
                })
                .Take(number)
                .Select(x => new GitCommit(x.Message, new GitReference(x)))
                .ToList();
        }

        public IEnumerable<GitTag> GetRecentTags(int number = 50)
        {
            return repository.Tags                 
                 .Take(number)
                 .Select(x =>
                 {
                     try
                     {
                         var commit = repository.Lookup<Commit>(x.Target.Id);
                         return new GitTag(x.FriendlyName, new GitReference(commit));
                     }
                     catch
                     {
                         return null;
                     }
                 })
                 .Where(x => x != null)
                 .ToList();
        }

        public GitCommit GetCommit(string commitSha)
        {
            Commit commit = null;
            try
            {
                commit = this.repository.Lookup<Commit>(commitSha);
            }
            catch (AmbiguousSpecificationException) { }
            catch (InvalidSpecificationException) { }
            
            return commit != null ? new GitCommit(commit.Message, new GitReference(commit)) : null;
        }

        public void Dispose()
        {
            // Dispose the native instance of repository
            this.repository.Dispose();
        }
    }
}
