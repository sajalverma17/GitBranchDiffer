using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Exceptions
{
    ///<summary>
    /// Clone of LibGit2Sharp's GitRepostioryNotFoundException.
    /// This was added so that tests can assert an error in this specific error case without directly referencing LibGit2Sharp
    /// </summary>
    public class GitRepoNotFoundException : GitOperationException
    {
        public GitRepoNotFoundException(string message) : base(message)
        {
        }

        public GitRepoNotFoundException() : base()
        {
        }

        public GitRepoNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
