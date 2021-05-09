using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.SolutionSelectionModels
{
    public class SelectedProject : ISolutionSelection
    {
        public EnvDTE.Project Native { get; set; }

        public string FullPath
        {
            get
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                return this.Native.FullName;
            }
        }

        public string Kind
        {
            get
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                return this.Native.Kind;
            }
        }

        public EnvDTE.Document Document => null;
    }
}
