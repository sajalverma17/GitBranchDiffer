using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.Filter
{
    internal static class BranchDiffFilterCommandGuids
    {
        public const string guidGitBranchDifferPackageCmdSet = "91fdc0b8-f3f8-4820-9734-1721db207258";
        public const int CommandIdGenerateDiffAndFilter = 0x133;

        public static Guid UIContext_BranchDiffFilter = new Guid("9707060D-0A76-4063-96E0-7868E1CA2616");
        public const int CommandIdPhysicalFileDiffMenuCommand = 0x0100;
        public const int CommandIdProjectFileDiffMenuCommand = 0x0200;
    }
}
