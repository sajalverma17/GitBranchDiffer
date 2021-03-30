using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.ViewModels
{
    public class BranchDiffViewModel
    {
        public string ButtonTextToTest => "Test String";

        // TODO : Get everything from VisualStudio.Teams.Git 
        public const string CurrentSelectedBranchName = "feature/test-feature";
        public const string BranchToCompareWith = "master";
        public const string Repo = @"C:\Tools\ProjectUnderTest";

        // Initialize the Diff VM with branches to diff
        public void Init()
        {
        }
    }
}
