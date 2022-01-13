using BranchDiffer.VS.Shared.FileDiff.Tables;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BranchDiffer.VS.Shared.FileDiff
{
    /// <summary>
    /// Holds tables that contain extra info on SolutionExplorer nodes (if the file corresponding that node was renamed, or if the .csproj file behind the proj node was edited).
    /// The tables are filled in when filter is applied.
    /// </summary>
    public class ItemTagManager
    {
        private RenamedPathTable<EnvDTE.ProjectItem> renamedProjectItemTable;
        private RenamedPathTable<EnvDTE.Project> renamedCsProjectTable;
        private EditedCsProjectTable editedCsProjectTable;

        public void CreateTagTables()
        {
            // Does not support Clear(), so we dispose the tables, create new
            this.renamedProjectItemTable?.Dispose();
            this.renamedCsProjectTable?.Dispose();
            this.editedCsProjectTable?.Dispose();
            this.renamedProjectItemTable = new RenamedPathTable<EnvDTE.ProjectItem>();
            this.renamedCsProjectTable = new RenamedPathTable<EnvDTE.Project>();
            this.editedCsProjectTable = new EditedCsProjectTable();
        }

        /*
         * Methods to manage csproj objects that were added/edited/renamed in working branch
         */
        public void MarkProjAsChanged(IVsHierarchyItem vsHierarchyItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var vsHierarchy = vsHierarchyItem.HierarchyIdentity.Hierarchy;
            vsHierarchy.ParseCanonicalName(vsHierarchyItem.CanonicalName, out uint itemId);
            vsHierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object itemObject);
            var project = itemObject as EnvDTE.Project;
            this.editedCsProjectTable.Insert(project, project.FullName);
        }

        public bool IsCsProjEdited(EnvDTE.Project project)
        {            
            return this.editedCsProjectTable.Select(project) is null ? false : true; 
        }

        /*
         * Methods to manage files that were renamed in working branch
         */
        public string GetOldFilePathFromRenamed(EnvDTE.Project project)
        {
            return this.renamedCsProjectTable.Select(project);
        }

        public string GetOldFilePathFromRenamed(EnvDTE.ProjectItem projectItem)
        {
            return this.renamedProjectItemTable.Select(projectItem);
        }

        public void SetOldFilePathOnRenamedItem(IVsHierarchyItem vsHierarchyItem, string oldPath)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var vsHierarchy = vsHierarchyItem.HierarchyIdentity.Hierarchy;            
            vsHierarchy.ParseCanonicalName(vsHierarchyItem.CanonicalName, out uint itemId);
            vsHierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object itemObject);
            var projectItem = itemObject as EnvDTE.ProjectItem;
            if (projectItem != null)
            {
                this.renamedProjectItemTable.Insert(projectItem, oldPath);
            }

            var project = itemObject as EnvDTE.Project;
            if (project != null)
            {
                this.renamedCsProjectTable.Insert(project, oldPath);
            }
        }
    }
}
