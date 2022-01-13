using System;
using System.Collections.Generic;
using System.Text;

namespace BranchDiffer.VS.Shared.FileDiff.Tables
{
    // TKey is the Solution object (either ProjectItem or Project)
    internal interface IItemTable<TKey, TValue> : IDisposable
    {
        void Insert(TKey item, TValue value);

        TValue Select(TKey item);
    }
}
