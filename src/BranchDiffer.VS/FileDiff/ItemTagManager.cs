using Microsoft.VisualStudio.Shell.Interop;

namespace BranchDiffer.VS.FileDiff
{
    /// <summary>
    /// Tags an item in the SolutionExplorer hierarchy with the old path (the path it has in the branch against which we diff working branch)
    /// </summary>
    public class ItemTagManager
    {
        private RenamedPathTable<EnvDTE.ProjectItem> renamedProjectItemTable;
        private RenamedPathTable<EnvDTE.Project> renamedCsProjectTable;

        public void CreateTagTables()
        {
            // Does not support Clear(), so we dispose the tables, create new
            this.renamedProjectItemTable?.Dispose();
            this.renamedCsProjectTable?.Dispose();
            this.renamedProjectItemTable = new RenamedPathTable<EnvDTE.ProjectItem>();
            this.renamedCsProjectTable = new RenamedPathTable<EnvDTE.Project>();
        }

        public string GetOldFilePathFromRenamed(EnvDTE.Project project)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return this.renamedCsProjectTable.GetValue(project);
        }

        public string GetOldFilePathFromRenamed(EnvDTE.ProjectItem projectItem)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return this.renamedProjectItemTable.GetValue(projectItem);
        }

        public void SetOldFilePathOnRenamedItem(IVsHierarchy vsHierarchy, string itemCanonicalName, string oldPath)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            vsHierarchy.ParseCanonicalName(itemCanonicalName, out uint itemId);
            vsHierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object itemObject);
            var projectItem = itemObject as EnvDTE.ProjectItem;
            if (projectItem != null)
            {
                this.renamedProjectItemTable.Set(projectItem, oldPath);
            }

            var project = itemObject as EnvDTE.Project;
            if (project != null)
            {
                this.renamedCsProjectTable.Set(project, oldPath);
            }
        }
    }
}
