using GitBranchDiffer.SolutionSelectionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.FileDiff.Commands
{
    public class OpenDiffEventArgs : EventArgs
    {
        public OpenDiffEventArgs(SolutionSelectionContainer<ISolutionSelection> solutionSelectionContainer)
        {
            this.SolutionSelectionContainer = solutionSelectionContainer;
        } 

        public SolutionSelectionContainer<ISolutionSelection> SolutionSelectionContainer { get; }
    }
}
