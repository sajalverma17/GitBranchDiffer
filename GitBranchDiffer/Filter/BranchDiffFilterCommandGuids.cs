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

        public static Guid guidFileDiffPackageCmdSet = new Guid("2C760378-A2F7-4140-8478-9BD354E38430");
        public const int CommandIdPhysicalFileDiffMenuCommand = 0x0100;
        public const int CommandIdProjectFileDiffMenuCommand = 0x0200;
    }
}
