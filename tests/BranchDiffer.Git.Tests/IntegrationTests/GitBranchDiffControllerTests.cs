using BranchDiffer.Git.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BranchDiffer.Git.Tests.IntegrationTests
{
    public class GitBranchDiffControllerTests
    {
        [Fact]
        public void GenerateDiff_MustThrowException_When_BranchToDiffAgainst_NotFound()
        {
            // ARRANGE
            // var controllerUnderTest = new GitBranchDiffController();
        }
    }
}
