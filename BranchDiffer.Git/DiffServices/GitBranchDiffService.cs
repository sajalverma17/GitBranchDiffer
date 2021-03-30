using BranchDiffer.Git.DiffModels;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BranchDiffer.Git.DiffServices
{
    public interface IGitBranchDiffService<TDiffedItem>
            where TDiffedItem : class
    {
        IEnumerable<TDiffedItem> GetDiffedFiles(string repo, string currentBranch, string baseBranch);
    }

    public class GitBranchDiffService : IGitBranchDiffService<VSDiffModel>
    {
        public IEnumerable<VSDiffModel> GetDiffedFiles(string repo, string currentBranch, string baseBranch)
        {
            var gitRepo = new Repository(repo);
            throw new NotImplementedException("Use Git library to do a branch diff");
        }
    }
}
