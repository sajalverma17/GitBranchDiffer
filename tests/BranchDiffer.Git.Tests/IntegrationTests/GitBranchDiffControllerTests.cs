using BranchDiffer.Git.Configuration;
using BranchDiffer.Git.Core;
using BranchDiffer.Git.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Tests.IntegrationTests
{
    [TestFixture]
    public class GitBranchDiffControllerTests : IntegrationTestBase
    {
        [Test]
        public void GenerateDiff_MustThrowException_When_BranchToDiffAgainst_NotFound()
        {
            // ARRANGE
            var controllerUnderTest = DIContainer.Instance.GetService(typeof(GitBranchDiffController)) as GitBranchDiffController;

            // ACT + ASSERT Should throw a git operation exception, TODO: Assert expected operation failed by checking error message of exception
            Assert.Throws<GitOperationException>(() => controllerUnderTest.GenerateDiff(this.TestGitRepoPath, null));
        }
    }
}
