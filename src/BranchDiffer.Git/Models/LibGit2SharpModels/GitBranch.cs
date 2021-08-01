using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{
    public interface IGitBranch
    {
        string Name { get; }

        /// <summary>
        /// TODO:
        /// Makes the code untestable...
        /// </summary>
        Commit Tip { get; }
    }

    public class GitBranch : IGitBranch
    {
        private readonly Branch branch;

        public GitBranch(Branch branch)
        {
            this.branch = branch ?? throw new ArgumentNullException(nameof(branch));
        }

        public string Name => this.branch.FriendlyName;

        public Commit Tip => this.branch.Tip;
    }
}
