using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.DiffModels
{
    public class DiffResultObject
    {
        /// <summary>
        /// Windows file name of the diffed object.
        /// </summary>
        public string OnjectFileName { get; }

        /// <summary>
        /// Diffed object, returned by GitLib2Sharp library
        /// </summary>
        public object DiffedObject { get; set; }
    }
}
