using BranchDiffer.Git.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BranchDiffer.Git.Tests.UnitTests
{    
    public class GitRepoServiceTests
    {
        [Fact]
        public void RepoStateValidation_MustReturnError_WhenBranchNotFound()
        {
            // ARRANGE
            var serviceUnderTest = new GitRepoService();
        }
    }
}
