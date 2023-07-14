using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{   
    public class GitBranch : IGitObject
    {
        public string FriendlyName { get; private set; }

        public string TipSha { get; private set; }

        public GitBranch(string name, string tipSha) 
        {
            this.FriendlyName = name;
            this.TipSha = tipSha;
        }
    }
}
