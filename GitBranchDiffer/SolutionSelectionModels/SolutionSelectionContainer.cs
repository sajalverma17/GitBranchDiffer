using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.SolutionSelectionModels
{
    public class SolutionSelectionContainer<T>
        where T : ISolutionSelection
    {
        public bool IsProject
        {
            get
            {
                return this.Item is SelectedProject;
            }
        }

        public bool IsProjectItem
        {
            get
            {
                return this.Item is SelectedProjectItem;
            }
        }

        public string FullName
        {
            get
            {
                return this.Item.FullPath;
            }
        }

        public string Kind
        {
            get
            {
                return this.Item.Kind;
            }
        }

        public T Item { get; set; }
    }
}
