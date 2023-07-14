using BranchDiffer.Git.Models.LibGit2SharpModels;
using System.Collections.ObjectModel;

namespace BranchDiffer.Git.Models
{
    public class GitBranchCollection : KeyedCollection<string, IGitObject>
    {
        /// <summary>
        /// Git branches have unique names and are case-insensitive, and branch names can be keys
        /// </summary>
        /// <param name="item"></param>
        protected override string GetKeyForItem(IGitObject item)
        {
            return item.FriendlyName;
        }
    }
}
