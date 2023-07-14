using BranchDiffer.Git.Models.LibGit2SharpModels;
using Microsoft.VisualStudio.Shell;
using System.Threading;

namespace BranchDiffer.VS.Shared
{
    /// <summary>
    /// Provides access to the implementation of VSPackage in startup project.
    /// </summary>
    public interface IGitBranchDifferPackage : IAsyncServiceProvider
    {
        IGitObject BranchToDiffAgainst { get; set; }

        string SolutionDirectory { get; }

        CancellationToken CancellationToken { get; }
    }
}
