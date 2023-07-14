using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{
    public class GitCommit : IGitObject
    {
        public string FriendlyName { get; private set; }

        public string TipSha { get; private set; }

        public string Message { get; private set; }

        public GitCommit(string message, string commitSha) 
        {
            Message = message;
            FriendlyName = commitSha.Substring(0, 7);
            TipSha = commitSha;
        }
    }
}
