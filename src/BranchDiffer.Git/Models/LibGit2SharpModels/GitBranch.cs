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
        public string Name { get; private set; }

        public IGitReference Tip { get; private set; }

        public GitBranch(string name, IGitReference gitReference) 
        {
            this.Name = name;
            this.Tip = gitReference;
        }
    }
}
