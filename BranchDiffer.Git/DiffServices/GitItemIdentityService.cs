using BranchDiffer.Git.DiffModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.DiffServices
{
    public interface IGitItemIdentityService
    {
        bool HasItemInChangeSet(HashSet<DiffResultItem> gitChangeSet, string absoluteItemPath);
    }

    public class GitItemIdentityService : IGitItemIdentityService
    {
        /// <summary>
        /// This method searches for item in the git change set by it's path. 
        /// </summary>
        /// <param name="gitChangeSet"></param>
        /// <param name="vsSolutionItemPath"></param>
        public bool HasItemInChangeSet(HashSet<DiffResultItem> gitChangeSet, string vsSolutionItemPath)
        {
            var vsItem = new DiffResultItem { AbsoluteFilePath = vsSolutionItemPath.ToLowerInvariant() };
            return gitChangeSet.Contains(vsItem);
        }
    }
}
