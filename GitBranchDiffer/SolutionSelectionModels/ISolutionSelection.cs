﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.SolutionSelectionModels
{
    public interface ISolutionSelection
    {
        string FullPath { get; }

        /// <summary>
        /// One of EnvDTE.Constants.vsProjectItemKind....
        /// </summary>
        string Kind { get; }

        /// <summary>
        /// Document related to the selection item. Will be null for EnvDTe.Project
        /// </summary>
        EnvDTE.Document Document { get; }
    }
}