using GitBranchDiffer.SolutionSelectionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.FileDiff.Commands
{
    public abstract class OpenDiffCommand
    {
        public abstract ISolutionSelection GetSolutionSelection();
    }
}
