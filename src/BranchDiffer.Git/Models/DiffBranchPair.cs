using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BranchDiffer.Git.Models.LibGit2SharpModels;

namespace BranchDiffer.Git.Models
{
    public class DiffBranchPair
    {
        /// <summary>
        /// The working branch of Git Repo
        /// </summary>
        public IGitBranch WorkingBranch { get; set; }

        /// <summary>
        /// The branch against which a diff will be run.
        /// </summary>
        public IGitBranch BranchToDiffAgainst { get; set; }

    }
}
