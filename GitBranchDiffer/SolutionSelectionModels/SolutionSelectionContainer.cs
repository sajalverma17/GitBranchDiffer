﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.SolutionSelectionModels
{
    public class SolutionSelectionContainer<T>
        where T : ISolutionSelection
    {
        public string FullName
        {
            get
            {
                return this.Item.FullPath;
            }
        }

        public string OldFullName
        {
            get
            {
                return this.Item.OldFullPath; 
            }
        }

        public string VsItemKind
        {
            get
            {
                return this.Item.Kind;
            }
        }

        public T Item { get; set; }
    }
}
