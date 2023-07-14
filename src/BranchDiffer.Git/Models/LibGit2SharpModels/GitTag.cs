using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{
    public class GitTag : IGitObject
    {
        public string Name { get; private set; }

        public IGitReference Tip { get; private set; }

        public GitTag(string name, IGitReference gitReference)
        {
            Name = name;
            this.Tip = gitReference;
        }
    }
}
