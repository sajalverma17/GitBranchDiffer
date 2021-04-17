using BranchDiffer.Git.DiffModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git
{
    public static class FileIdentityManager
    {
        /// <summary>
        /// File paths in git change set are relative to the root directory of repo.
        /// File paths in vs Soltuion Explorer are absolute (not relative to anything)
        /// This method converts vs Soltion item's absolute path relative to the repo
        /// </summary>
        /// <param name="gitChangeSet"></param>
        /// <param name="vsSolutionItemPath"></param>
        /// <returns></returns>
        public static bool HasFileInChangeSet(HashSet<DiffResultItem> gitChangeSet, string vsSolutionItemPath)
        {
            throw new NotImplementedException();
        }
    }
}
