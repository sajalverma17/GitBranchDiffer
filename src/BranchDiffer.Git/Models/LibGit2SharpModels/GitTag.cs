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
        public string FriendlyName { get; private set; }

        public string TipSha { get; private set; }

        public string ShortSha => TipSha.Substring(0 , 7);

        public GitTag(string name, string tipSha)
        {
            FriendlyName = name;
            TipSha = tipSha;
        }
    }
}
