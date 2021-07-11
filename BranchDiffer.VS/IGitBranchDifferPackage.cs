using Microsoft.VisualStudio.Shell;
using System.Threading;

namespace BranchDiffer.VS
{
    public interface IGitBranchDifferPackage : IAsyncServiceProvider
    {
        string BranchToDiffAgainst { get; }

        CancellationToken CancellationToken { get; }

        void OnFilterApplied();

        void OnFilterUnapplied();
    }
}
