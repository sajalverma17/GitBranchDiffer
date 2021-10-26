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
    public class GitRepoValidationTests : IntegrationTestBase
    {
        [Test]
        public void GenerateDiff_MustThrowException_When_BranchToDiffAgainst_Null()
        {
            // ARRANGE
            var controllerUnderTest = DIContainer.Instance.GetService(typeof(GitBranchDiffController)) as GitBranchDiffController;

            // ACT + ASSERT Should throw a git operation exception
            Assert.Throws<GitOperationException>(() => controllerUnderTest.GenerateDiff(this.TestGitRepoPath, null));
        }

        [Test]
        public void GenerateDiff_MustThrowException_When_BranchToDiffAgainst_NotFound()
        {
            // ARRANGE
            var controllerUnderTest = DIContainer.Instance.GetService(typeof(GitBranchDiffController)) as GitBranchDiffController;

            // ACT + ASSERT Should throw a git operation exception
            Assert.Throws<GitOperationException>(() => controllerUnderTest.GenerateDiff(this.TestGitRepoPath, "Bob Loblaw"));
        }

        [Test]
        public void GenerateDiff_MustThrowException_When_BranchToDiffAgainst_Same_As_Head()
        {
            // ARRANGE
            var controllerUnderTest = DIContainer.Instance.GetService(typeof(GitBranchDiffController)) as GitBranchDiffController;

            // ACT + ASSERT Should throw a git operation exception
            Assert.Throws<GitOperationException>(() => controllerUnderTest.GenerateDiff(this.TestGitRepoPath, this.TestRepoHEAD));
        }
    }
}
