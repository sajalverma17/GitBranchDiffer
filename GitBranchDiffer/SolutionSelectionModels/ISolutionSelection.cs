using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.SolutionSelectionModels
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
        EnvDTE.Document Document { get; }
    }
}
