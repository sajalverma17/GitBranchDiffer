using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.Filter
{
    /// <summary>
    /// The state of our Package (depending on VS initialization and solution events). 
    /// The initialization of our package decides if our Filter can work or not.
    /// Either VS initalized it with a solution (SolutionInfoSet), or without it (Invalid state). 
    /// If user closes a solution in VS, it changes state of our package to SolutionInfoUnset.
    /// </summary>
    /// <remarks>
    /// TODO: We can get rid of keeping track of solution info this way if we figure how to HIDE our filter button 
    /// based on Solution existing in Solution Explorer or not.
    /// </remarks>
    public enum FilterInitializationState
    {
        /// <summary>
        /// This is a filter state when VS was opened along with a pre-selected solution.
        /// When the filter is in this state, a diff can be generated.
        /// </summary>
        SoltuionInfoSet,

        /// <summary>
        /// This is the state of our plugin when VS was opened along with a pre-selected solution, 
        /// but a solution closed event was triggered by user and there is no solution currently opened in VS at this state.
        /// </summary>
        SolutionInfoUnset,

        /// <summary>
        /// Package was initalized without a pre-selected solution. After this state, the package will never be initialized again by VS.
        /// WHY DOES VS initializes our VSPackage component when our MEF component (BranchDiffFilter) is triggered?
        /// </summary>
        Invalid,
    }
}
