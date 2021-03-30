using GitBranchDiffer.Git.DiffModels;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.Git.Services
{
    public interface IGitBranchDiffService<TDiffedItem>
        where TDiffedItem : class
    {
        public IEnumerable<TDiffedItem> GetDiffedFiles(string repo, string currentBranch, string baseBranch);
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
