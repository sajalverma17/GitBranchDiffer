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
        /// Windows file path of the diffed item.
        /// </summary>
        /// <remarks>
        /// This path is used to determine uniqueness of an item in both the VS side and the Git side
        /// and is used to compare if an item in VS solution exists in the changeset returned by Git diff service.
        /// </remarks>
        public string AbsoluteFilePath { get; set; }

        /// <summary>
        /// Diffed item, returned by GitLib2Sharp library
        /// </summary>
        public object DiffedObject { get; set; }

        /// <summary>
        /// The uniquness of a DiffResultItem is determined by the hashcode of it's absolute path.
        /// </summary>
        public override int GetHashCode()
        {
            return this.AbsoluteFilePath.GetHashCode();
        }

        /// <summary>
        /// Checks if two result items are equal by comparing hash codes.
        /// </summary>
        /// <param name="obj"></param>
        public override bool Equals(object obj)
        {
            if (obj is DiffResultItem diffResultItem)
            {
                return this.GetHashCode() == diffResultItem.GetHashCode();
            }

            return false;
        }
    }
}
