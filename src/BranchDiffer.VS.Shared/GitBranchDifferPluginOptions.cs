using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace BranchDiffer.VS.Shared
{
    public class GitBranchDifferPluginOptions : DialogPage
    {
        // Default base is master
        private string gitBranchName = "master";

        [Category("Git Branch Differ")]
        [DisplayName("Branch or Commit To Diff Against")]
        [Description("The branch/commit against which HEAD will be diffed")]
        public string BaseBranchName
        {
            get { return gitBranchName; }
            set { gitBranchName = value; }
        }
    }
}
