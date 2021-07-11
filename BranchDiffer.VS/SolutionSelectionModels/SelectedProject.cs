using EnvDTE;

namespace BranchDiffer.VS.SolutionSelectionModels
{
    public class SelectedProject : ISolutionSelection
    {
        public Project Native { get; set; }

        public string FullPath
        {
            get
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                return this.Native.FullName;
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

        public Document Document => null;
    }
}
