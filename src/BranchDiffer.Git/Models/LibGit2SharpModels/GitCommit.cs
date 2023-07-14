using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{
    public class GitCommit : IGitObject
    {
        public string Name { get; private set; }

        public IGitReference Tip { get; private set; }

        public string ShortSha => Tip.Sha.Substring(0, 7);

        public GitCommit(string message, IGitReference gitReference) 
        {
            Name = message;
            Tip = gitReference;
        }
    }
}
