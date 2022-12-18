using BranchDiffer.Git.Configuration;
using BranchDiffer.Git.Core;
using BranchDiffer.Git.Exceptions;
using NUnit.Framework;
 
namespace BranchDiffer.Git.Tests.IntegrationTests
{
    [TestFixture]
    public class GitRepoValidationTests : IntegrationTestBase
    {
        [Test]
        public void GenerateDiff_MustThrowException_When_BranchToDiffAgainst_NotFound()
        {
            // ARRANGE
            var controllerUnderTest = DIContainer.Instance.GetService(typeof(GitBranchDiffController)) as GitBranchDiffController;

            // ACT + ASSERT Should throw a git operation exception when 
            Assert.Throws<GitOperationException>(() => controllerUnderTest.GenerateDiff(this.TestGitRepoPath, "develop"));
        }

        [Test]
        public void GenerateDiff_MustThrowException_When_InvalidSpecification()
        {
            // ARRANGE
            var controllerUnderTest = DIContainer.Instance.GetService(typeof(GitBranchDiffController)) as GitBranchDiffController;

            // ACT + ASSERT Should throw a git operation exception when 
            Assert.Throws<GitOperationException>(() => controllerUnderTest.GenerateDiff(this.TestGitRepoPath, "Bob Lawblaw"));
        }

        [Test]
        public void GenerateDiff_MustThrowException_When_CommitToDiffAgainst_NotFound()
        {
            // ARRANGE
            var controllerUnderTest = DIContainer.Instance.GetService(typeof(GitBranchDiffController)) as GitBranchDiffController;

            // ACT + ASSERT Should throw a git operation exception when 
            Assert.Throws<GitOperationException>(() => controllerUnderTest.GenerateDiff(this.TestGitRepoPath, "ae55e0f"));
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
