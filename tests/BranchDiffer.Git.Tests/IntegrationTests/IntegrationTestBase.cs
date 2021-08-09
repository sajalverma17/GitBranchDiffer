using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;

namespace BranchDiffer.Git.Tests.IntegrationTests
{    
    [TestFixture]
    public class IntegrationTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            // Path to test VS solution
            this.TestSolutionPath = @"..\Assets\GitBranchDiffer-TestAsset\ProjectUnderTest.sln";

            // Path to Git repo
            this.TestGitRepoPath = TestContext.CurrentContext.TestDirectory + @"\..\..\..\Assets\GitBranchDiffer-TestAsset\";
        }

        protected string TestSolutionPath { get; private set; }

        protected string TestGitRepoPath { get; private set; }

    }
}
