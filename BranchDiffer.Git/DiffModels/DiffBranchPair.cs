using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace BranchDiffer.Git.DiffModels
{
    public class DiffBranchPair
    {
        /// <summary>
        /// The working branch of Git Repo
        /// </summary>
        public Branch WorkingBranch { get; set; }

        /// <summary>
        /// The branch against which a diff will be run.
        /// </summary>
        public Branch BranchToDiffAgainst { get; set; }

    }
}
