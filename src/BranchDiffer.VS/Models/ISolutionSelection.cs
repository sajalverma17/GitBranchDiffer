using EnvDTE;

namespace BranchDiffer.VS.Models
{
    public interface ISolutionSelection
    {
        string FullPath { get; }

        string OldFullPath { get; }

        /// <summary>
        /// One of EnvDTE.Constants.vsProjectItemKind....
        /// </summary>
        string Kind { get; }

        /// <summary>
        /// Document related to the selection item. Will be null for EnvDTE.Project
        /// </summary>
        Document Document { get; }
    }
}
