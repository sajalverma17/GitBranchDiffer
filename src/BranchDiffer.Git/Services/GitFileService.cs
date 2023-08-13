using BranchDiffer.Git.Models;
using BranchDiffer.Git.Models.LibGit2SharpModels;
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

        /// <summary>
        /// Finds the file from the given changeset, note that the changeset contains paths of solution items of vs which are always lowercase.
        /// </summary>
        /// <param name="gitChangeSet"></param>
        /// <param name="vsSolutionItemPath"></param>
        /// <returns>The DiffResultItem from the change set, or null if nothing found.</returns>
        DiffResultItem GetFileFromChangeSet(HashSet<DiffResultItem> gitChangeSet, string absoluteItemPath);

        /// <summary>
        /// Creates a temp file on disk having content of this file but from branch against which user wants to diff.       
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="filePath"></param>
        /// <returns>Path to a temp file, which is the Base-branch version of this file</returns>
        string GetBaseBranchRevisionOfFile(IGitRepository repository, IGitObject referenceBranch, string filePath);
    }

    public class GitFileService : IGitFileService
    {
        public string GetBaseBranchRevisionOfFile(IGitRepository repository, IGitObject referenceBranch, string filePath)
        {
            string workingDirectory = repository.WorkingDirectory;
            string relativePathInRepo = filePath;

            if (relativePathInRepo.StartsWith(workingDirectory, StringComparison.OrdinalIgnoreCase))
            {
                relativePathInRepo = relativePathInRepo.Substring(workingDirectory.Length);
            }

            var treeEntryAtTipOfBase = repository.GetCommitTree(referenceBranch.TipSha)[relativePathInRepo.Replace(Constants.DirectorySeperator, "/")];
            if (treeEntryAtTipOfBase != null)
            {
                return GetBaseBranchPathOfFile(treeEntryAtTipOfBase, relativePathInRepo, filePath);
            }
            
            return Path.GetTempFileName();
        }

        public DiffResultItem GetFileFromChangeSet(HashSet<DiffResultItem> gitChangeSet, string vsSolutionItemPath)
        {
            var searchItemByAbsolutePath = new DiffResultItem { AbsoluteFilePath = vsSolutionItemPath.ToLowerInvariant() };
            if (gitChangeSet.TryGetValue(searchItemByAbsolutePath, out DiffResultItem resultItem))
            {
                return resultItem;
            }
            return null;
        }

        private static string GetBaseBranchPathOfFile(TreeEntry treeEntryAtTipOfBase, string pathInRepo, string filePath)
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
                File.WriteAllText(tempFileName, treeEntryblob.GetContentText(new FilteringOptions(pathInRepo)), GetEncoding(filePath));
                return tempFileName;
            }
            else
            {
                // File is not a blob in Git. Unsupported.
                return Path.GetTempFileName();
            }
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
