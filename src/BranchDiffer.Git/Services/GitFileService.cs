using BranchDiffer.Git.Models;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Services
{
    public interface IGitFileService
    {
        DiffResultItem GetFileFromChangeSet(HashSet<DiffResultItem> gitChangeSet, string absoluteItemPath);

        string GetBaseBranchRevisionOfFile(Repository repository, string baseBranchName, string filePath);
    }

    public class GitFileService : IGitFileService
    {
        /// <summary>
        /// Creates a temp file on disk having content of this file but from branch against which user wants to diff.       
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="filePath"></param>
        /// <returns>Path to a temp file, which is the Base-branch version of this file</returns>
        public string GetBaseBranchRevisionOfFile(Repository repository, string branchToDiffAgainst, string filePath)
        {
            // Get the file path relative to repo 
            string workingDirectory = repository.Info.WorkingDirectory;
            string relativePathInRepo = filePath;
            if (relativePathInRepo.StartsWith(workingDirectory, StringComparison.OrdinalIgnoreCase))
                relativePathInRepo = relativePathInRepo.Substring(workingDirectory.Length);

            // Find the file path in base branch.
            var baseBranch = repository.Branches[branchToDiffAgainst];
            var treeEntryAtTipOfBase = baseBranch.Tip[relativePathInRepo.Replace(Constants.DirectorySeperator, "/")];
            if (treeEntryAtTipOfBase != null)
            {
                if (treeEntryAtTipOfBase.TargetType is TreeEntryTargetType.Blob)
                {
                    var treeEntryblob = treeEntryAtTipOfBase.Target as Blob;
                    if (treeEntryblob.IsBinary)
                    {
                        // File content binary. Unsupported.
                        return Path.GetTempFileName();
                    }

                    var tempFileName = Path.GetTempFileName();
                    File.WriteAllText(tempFileName, treeEntryblob.GetContentText(new FilteringOptions(relativePathInRepo)), GetEncoding(filePath));
                    return tempFileName;
                }
                else
                {
                    // File is not a blob in Git. Unsupported.
                    return Path.GetTempFileName();
                }
            }

            // File was not found in BranchToDiff, this file must have been ADDED in working branch. 
            return Path.GetTempFileName();
        }

        /// <summary>
        /// Finds the file from the given changeset, note that the changeset contains paths of solution items of vs which are always lowercase.
        /// </summary>
        /// <param name="gitChangeSet"></param>
        /// <param name="vsSolutionItemPath"></param>
        /// <returns>The DiffResultItem from the change set, or null if nothing found.</returns>
        public DiffResultItem GetFileFromChangeSet(HashSet<DiffResultItem> gitChangeSet, string vsSolutionItemPath)
        {
            var searchItemByAbsolutePath = new DiffResultItem { AbsoluteFilePath = vsSolutionItemPath.ToLowerInvariant() };
            if (gitChangeSet.TryGetValue(searchItemByAbsolutePath, out DiffResultItem resultItem))
            {
                return resultItem;
            }
            return null;
        }

        private static Encoding GetEncoding(string file)
        {
            if (File.Exists(file))
            {
                var encoding = Encoding.UTF8;
                if (HasPreamble(file, encoding))
                {
                    return encoding;
                }
            }

            return Encoding.Default;
        }

        private static bool HasPreamble(string file, Encoding encoding)
        {
            using (var stream = File.OpenRead(file))
            {
                foreach (var b in encoding.GetPreamble())
                {
                    if (b != stream.ReadByte())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

    }
}
