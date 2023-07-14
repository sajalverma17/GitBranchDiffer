using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{
    // Represents a Branch/Tag/Commit. Contains a reference to the commit SHA it points to.
    public interface IGitObject
    {
        string FriendlyName { get; }

        string TipSha { get; }
    }
}
