using LibGit2Sharp;
using System;
using System.Linq;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{
    public interface IGitRepository : IDisposable
    {
        GitBranchCollection Branches { get; }

        IGitBranch Head { get; }

        string WorkingDirectory { get; }

        Diff Diff { get; }
    }

    internal sealed class GitRepository : IGitRepository
    {
        private readonly IRepository repository;
        private readonly IGitBranch head;
        private readonly GitBranchCollection gitBranches;

        public GitRepository(IRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.head = new GitBranch(this.repository.Head);
            this.gitBranches = new GitBranchCollection();
            repository.Branches.ToList().ForEach(x => this.gitBranches.Add(new GitBranch(x)));
        }

        public GitBranchCollection Branches => this.gitBranches;

        public IGitBranch Head => this.head;

        public Diff Diff => this.repository.Diff;

        public string WorkingDirectory => this.repository.Info.WorkingDirectory;

        public void Dispose()
        {
            // Dispose the native instance of repository
            this.repository.Dispose();
        }
    }
}
