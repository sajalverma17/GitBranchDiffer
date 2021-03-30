using GitBranchDiffer.PackageCommands;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace GitBranchDiffer.View
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("e85482ac-ec1c-4763-bffd-b8534a079e76")]
    public class BranchDiffWindow : ToolWindowPane
    {
        public BranchDiffWindowControl BranchDiffWindowControl { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchDiffWindow"/> class.
        /// </summary>
        public BranchDiffWindow() : base(null)
        {
            this.Caption = "BranchDiffWindow";
            this.ToolBar = new CommandID(new Guid(BranchDiffWindowCommand.guidBranchDiffWindowPackageCmdSet), BranchDiffWindowCommand.BranchDiffToolbarId);
            this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            var control = new BranchDiffWindowControl();
            this.BranchDiffWindowControl = control;
            this.Content = control;
        }
    }
}
