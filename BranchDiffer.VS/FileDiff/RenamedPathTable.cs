using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BranchDiffer.VS.FileDiff
{
    /// <summary>
    /// TKey can be either ProjectItem or Project
    /// </summary>
    public class RenamedPathTable<TKey> : IDisposable
        where TKey : class
    {
        private ConditionalWeakTable<TKey, string> conditionalWeakTable;

        public RenamedPathTable()
        {
            conditionalWeakTable = new ConditionalWeakTable<TKey, string>();
        }

        public string GetValue(TKey key)
        {
            this.conditionalWeakTable.TryGetValue(key, out string value);
            return value;
        }

        public void Set(TKey key, string value)
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
