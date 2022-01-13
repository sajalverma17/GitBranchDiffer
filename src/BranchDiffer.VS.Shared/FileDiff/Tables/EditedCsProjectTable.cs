using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BranchDiffer.VS.Shared.FileDiff.Tables
{
    /// <summary>
    /// Holds a table of .csproj files that were added/edited.
    /// We lookup this table and then make "Open Diff With Base" menu command visible for Project node in Solution Explorer.
    /// </summary>
    internal class EditedCsProjectTable : IItemTable<EnvDTE.Project, string>
    {
        private ConditionalWeakTable<EnvDTE.Project, string> conditionalWeakTable;

        public EditedCsProjectTable()
        {
            conditionalWeakTable = new ConditionalWeakTable<EnvDTE.Project, string>();
        }

        public string Select(EnvDTE.Project key)
        {
            this.conditionalWeakTable.TryGetValue(key, out string value);
            return value;
        }

        public void Insert(EnvDTE.Project key, string value)
        {
            this.conditionalWeakTable.Add(key, value);
        }

        /// <summary>
        /// I meant to clear all values of the weak table everytime the branch diff filter is un-applied, 
        /// but ConditionalWeakTable does not have Clear() method in .NET framework, so we dispose try force garbage-collect the object
        /// </summary>
        public void Dispose()
        {
            this.conditionalWeakTable = null;
            GC.Collect();
        }
    }
}
