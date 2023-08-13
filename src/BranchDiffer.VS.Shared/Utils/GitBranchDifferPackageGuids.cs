using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.VS.Shared.Utils
{
    public static class GitBranchDifferPackageGuids
    {
        // GUID of package. For registration with VS
        public const string guidBranchDiffWindowPackage = "156fcec6-25ac-4279-91cc-bbe2e4ea8c14";

        // GUID of branch diff filter
        public const string guidGitBranchDifferPackageCmdSet = "91fdc0b8-f3f8-4820-9734-1721db207258";
        public const int CommandIdGenerateDiffAndFilter = 0x133;

        // GUID for UI context of filter applied/un-applied
        public static Guid UIContext_BranchDiffFilter = new Guid("9707060D-0A76-4063-96E0-7868E1CA2616");

        public const uint SelectReferenceObjectButtonId = 0x1022;

        // GUID for open file diff command
        public static Guid guidFileDiffPackageCmdSet = new Guid("2C760378-A2F7-4140-8478-9BD354E38430");
        public const int CommandIdPhysicalFileDiffMenuCommand = 0x0100;
        public const int CommandIdProjectFileDiffMenuCommand = 0x0200;
        public const int CommandIdSelectReferenceObjectCommand = 0x1022;
    }
}
