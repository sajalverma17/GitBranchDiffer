using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.FileDiff
{
    public class ItemTagManager
    {
        public const string OldFilePathTagName = "RenamedFileOldPath";

        /// <summary>
        /// Gets old path from Tag, if any. Will be non-empty string if the file was renamed in base branch.
        /// </summary>
        public string GetOldFilePathFromTag(IVsHierarchy vsHierarchy, string itemCanonicalName)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var buildPropertyStorage = vsHierarchy as IVsBuildPropertyStorage;            
            vsHierarchy.ParseCanonicalName(itemCanonicalName, out uint selectedItemId);
            buildPropertyStorage.GetItemAttribute(selectedItemId, ItemTagManager.OldFilePathTagName, out string oldPath);
            return oldPath;
        }

        public void SetOldFilePathInTag(IVsHierarchy vsHierarchy, string itemCanonicalName, string oldPath)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            vsHierarchy.ParseCanonicalName(itemCanonicalName, out uint itemId);
            var buildItemStorage = vsHierarchy as IVsBuildPropertyStorage;
            if (buildItemStorage != null)
            {
                buildItemStorage.SetItemAttribute(itemId, ItemTagManager.OldFilePathTagName, oldPath);
            }
        }
    }
}
