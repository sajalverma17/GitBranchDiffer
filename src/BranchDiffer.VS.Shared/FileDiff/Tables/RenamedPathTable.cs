using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BranchDiffer.VS.Shared.FileDiff.Tables
{
    /// <summary>
    /// Holds solution object that were renamed in working branch. The table contains the path of the same object in base branch.
    /// </summary>
    public class RenamedPathTable<TKey> : IItemTable<TKey, string>
        where TKey : class
    {
        private ConditionalWeakTable<TKey, string> conditionalWeakTable;

        public RenamedPathTable()
        {
            conditionalWeakTable = new ConditionalWeakTable<TKey, string>();
        }

        public string Select(TKey key)
        {
            this.conditionalWeakTable.TryGetValue(key, out string value);
            return value;
        }

        public void Insert(TKey key, string value)
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
