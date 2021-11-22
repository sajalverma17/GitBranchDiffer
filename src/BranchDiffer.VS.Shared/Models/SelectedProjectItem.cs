using EnvDTE;

namespace BranchDiffer.VS.Models
{
    public class SelectedProjectItem : ISolutionSelection
    {
        public ProjectItem Native { get; set; }

        public Document Document
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
                // BUG: Null Ref Exception when opening Diff for Solution Items, Properties is null, implement a different way to get their FullPath
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
