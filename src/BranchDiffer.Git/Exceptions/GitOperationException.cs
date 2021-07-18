using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Exceptions
{
    public class GitOperationException : InvalidOperationException
    {
        public GitOperationException(string message) : base(message)
        {
        }

        public GitOperationException() : base()
        {
        }

        public GitOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
