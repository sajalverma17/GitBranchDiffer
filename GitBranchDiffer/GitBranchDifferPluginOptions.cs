using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer
{
    class GitBranchDifferPluginOptions : DialogPage
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
