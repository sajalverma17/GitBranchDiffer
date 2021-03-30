using BranchDiffer.Git.DiffServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.ViewModels
{
    public class BranchDiffViewModel
    {
        private IGitBranchDiffService gitBranchDiffService;
        public string ButtonTextToTest => "Test String";

        // TODO : Get everything from VisualStudio.Teams.Git 
        public const string CurrentSelectedBranchName = "feature/test-feature";
        public const string BranchToCompareWith = "master";
        public const string Repo = @"C:\Tools\ProjectUnderTest";

        public IEnumerable<string> ModifiedFileList { get; set; }

        // Initialize the Diff VM with branches to diff
        public void Init()
        {
            this.gitBranchDiffService = new GitBranchDiffService();
        }

        public void Generate()
        {
            var changeList = this.gitBranchDiffService.GetDiffFileNames(Repo, BranchToCompareWith, CurrentSelectedBranchName);
            this.ModifiedFileList = changeList;
        }
    }
}
