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
        [DisplayName("Branch To Diff Against")]
        [Description("The branch against which active HEAD will be diffed")]
        public string BaseBranchName
        {
            get { return gitBranchName; }
            set { gitBranchName = value; }
        }
    }
}
