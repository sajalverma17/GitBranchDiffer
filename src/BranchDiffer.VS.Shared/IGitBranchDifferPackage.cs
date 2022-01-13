using System;
using System.Threading;

namespace BranchDiffer.VS.Shared
{
    /// <summary>
    /// Provides access to the implementation of VSPackage in startup project.
    /// </summary>
    public interface IGitBranchDifferPackage : IServiceProvider
    {
        string BranchToDiffAgainst { get; }

        CancellationToken CancellationToken { get; }

        void OnFilterApplied();

        void OnFilterUnapplied();
    }
}
