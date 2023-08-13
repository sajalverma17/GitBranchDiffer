using BranchDiffer.Git.Models.LibGit2SharpModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Tests.IntegrationTests
{
    internal class FakeGitObject(string friendlyName, string sha) : IGitObject
    {
        public string FriendlyName => friendlyName;

        public string TipSha => sha;
    }
}
