using Microsoft.VisualStudio.Shell;
using System.Threading;

namespace BranchDiffer.VS.Shared
{
    /// <summary>
    /// Provides access to the implementation of VSPackage in startup project.
    /// </summary>
    public interface IGitBranchDifferPackage : IAsyncServiceProvider
    {
        // Is Branch/SHA/Tag name
        string BranchToDiffAgainst { get; }

        string SolutionDirectory { get; }

        CancellationToken CancellationToken { get; }
    }
}
