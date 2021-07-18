using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Exceptions
{
    public class GitBranchException : InvalidOperationException
    {
        public GitBranchException(string message) : base(message)
        {
        }
    }
}
