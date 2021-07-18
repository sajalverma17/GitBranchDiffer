using LibGit2Sharp;
using System;
using System.Linq;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{
    public interface IGitRepository : IDisposable
    {
        GitBranchCollection Branches { get; }

        GitBranch Head { get; }

        string WorkingDirectory { get; }
    }

    internal sealed class GitRepository : IGitRepository
    {
        private readonly Repository repository;
        private readonly GitBranch head;
        private readonly GitBranchCollection gitBranches;

        public GitRepository(Repository repository)
        {
            this.repository = repository;
            this.head = new GitBranch(this.repository.Head);
            this.gitBranches = new GitBranchCollection();
            repository.Branches.ToList().ForEach(x => this.gitBranches.Add(new GitBranch(x)));
        }

        public GitBranchCollection Branches => this.gitBranches;

        public GitBranch Head => this.head;

        public string WorkingDirectory => this.repository.Info.WorkingDirectory;

        public void Dispose()
        {
            // Dispose the native instance of repository
            this.repository.Dispose();
        }
    }
}
