using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.SolutionSelectionModels
{
    public class SelectedProjectItem : ISolutionSelection
    {
        public EnvDTE.ProjectItem Native { get; set; }

        public EnvDTE.Document Document
        {
            get 
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                return this.Native.Document;
            }
        }

        public string FullPath
        {
            get
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                return Native.Properties.Item("FullPath")?.Value.ToString();
            }
        }

        public string OldFullPath { get; set; }

        public string Kind
        {
            get
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                return this.Native.Kind;
            }
        }
    }
}
