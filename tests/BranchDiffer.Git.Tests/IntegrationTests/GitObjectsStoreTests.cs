using BranchDiffer.Git.Configuration;
using BranchDiffer.Git.Core;
using BranchDiffer.Git.Exceptions;
using BranchDiffer.Git.Services;
using NUnit.Framework;

namespace BranchDiffer.Git.Tests.IntegrationTests
{
    [TestFixture]
    public class GitObjectsStoreTests : IntegrationTestBase
    {
        [Test]
        public void ObjectsStore_MustReturnNull_When_InvalidBranchName()
        {
            var sut = DIContainer.Instance.GetService(typeof(GitObjectsStore)) as GitObjectsStore;

            var gitObject = sut.FindGitReferenceByUserDefinedName(this.TestGitRepoPath, "develop");

            Assert.That(gitObject, Is.Null);
        }

        [Test]
        public void ObjectsStore_MustReturnGitObject_When_ValidBranchName()
        {
            var sut = DIContainer.Instance.GetService(typeof(GitObjectsStore)) as GitObjectsStore;

            var gitObject = sut.FindGitReferenceBySha(this.TestGitRepoPath, "feature/test-feature");

            Assert.That(gitObject, Is.Not.Null);
        }

        [Test]
        public void ObjectsStore_MustThrowException_When_InvalidSpecification()
        {
            var sut = DIContainer.Instance.GetService(typeof(GitObjectsStore)) as GitObjectsStore;

            var gitObject = sut.FindGitReferenceBySha(this.TestGitRepoPath, "00ecaa4");

            Assert.That(gitObject, Is.Not.Null);
            Assert.That(gitObject.TipSha.StartsWith("00ecaa4"), Is.True);
        }
    }
}
