﻿using Microsoft.VisualStudio.Shell;
using System.Threading;

namespace BranchDiffer.VS
{
    /// <summary>
    /// Provides access to the implementation of VSPackage in startup project.
    /// </summary>
    public interface IGitBranchDifferPackage : IAsyncServiceProvider
    {
        string BranchToDiffAgainst { get; }

        CancellationToken CancellationToken { get; }

        void OnFilterApplied();

        void OnFilterUnapplied();
    }
}
