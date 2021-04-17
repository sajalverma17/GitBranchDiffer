using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.DiffModels
{
    public class DiffResultItem

    {
        /// <summary>
        /// Windows file name of the diffed object.
        /// This is a concatenation of {Git Repo path} + {diff object's new path} and must be unqiue.
        /// </summary>
        public string AbsoluteFilePath { get; set; }

        /// <summary>
        /// Diffed object, returned by GitLib2Sharp library
        /// </summary>
        public object DiffedObject { get; set; }
    }
}
