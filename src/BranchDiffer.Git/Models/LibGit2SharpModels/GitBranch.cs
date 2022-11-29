using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{
    // Represents either a Branch or a Commit.
    public interface IGitObject
    {
        string Name { get; }

        IGitCommit Tip { get; set; }
    }

    public class GitBranch : IGitObject
    {
        private readonly Branch branch;

        public GitBranch(IGitCommit gitCommit) 
        {
            this.Tip = gitCommit;
        }

        public GitBranch(Branch branch)
        {
            this.branch = branch ?? throw new ArgumentNullException(nameof(branch));
            this.Tip = new GitCommit(branch.Tip);
        }

        public string Name => this.branch?.FriendlyName ?? this.Tip?.Sha ?? throw new InvalidOperationException("Invalid state of Git Branch Or Commit.");

        public IGitCommit Tip { get; set; }
    }
}
