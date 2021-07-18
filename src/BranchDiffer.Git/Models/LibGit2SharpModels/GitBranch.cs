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
    }

    public class GitBranch : IGitBranch
    {
        private readonly Branch branch;

        public GitBranch(Branch branch)
        {
            this.branch = branch;
        }

        public string Name => this.branch.FriendlyName;
    }
}
