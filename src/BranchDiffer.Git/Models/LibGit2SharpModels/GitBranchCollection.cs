using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{
    public class GitBranchCollection : KeyedCollection<string, GitBranch>
    {
        /// <summary>
        /// Git branches have unique names and are case-insensitive, and branch names can be keys
        /// </summary>
        /// <param name="item"></param>
        protected override string GetKeyForItem(GitBranch item)
        {
            return item.Name;
        }
    }
}
