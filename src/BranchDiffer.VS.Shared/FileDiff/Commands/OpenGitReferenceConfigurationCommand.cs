using Microsoft.VisualStudio.Shell;
using System;
using System.Threading.Tasks;
using System.ComponentModel.Design;
using BranchDiffer.Git.Core;
using BranchDiffer.VS.Shared.Utils;
using BranchDiffer.VS.Shared.BranchDiff;
using System.Windows.Forms;
using BranchDiffer.Git.Configuration;
using Task = System.Threading.Tasks.Task;

namespace BranchDiffer.VS.Shared.FileDiff.Commands
{
    internal class OpenGitReferenceConfigurationCommand : OpenDiffCommand
    {
        private GitObjectsStore gitObjectsStore;
        private IGitBranchDifferPackage gitBranchDifferPackage;

        public OpenGitReferenceConfigurationCommand()
        {
        }

        public bool IsVisible
        {
            get => OleCommandInstance.Visible;
            set => OleCommandInstance.Visible = value;
        }

        /// <summary>
        /// Inits the dependecies needed to execute the command, then register command in VS menu
        /// </summary>
        public async Task InitializeAndRegisterAsync(IGitBranchDifferPackage package)
        {
            await this.InitializeAsync(package);
            this.gitObjectsStore = DIContainer.Instance.GetService(typeof(GitObjectsStore)) as GitObjectsStore;
            this.gitBranchDifferPackage = package;

            this.Register(new CommandID(GitBranchDifferPackageGuids.guidFileDiffPackageCmdSet, GitBranchDifferPackageGuids.CommandIdSelectReferenceObjectCommand));
            OleCommandInstance.BeforeQueryStatus += OleCommandInstance_BeforeQueryStatus;
        }

        private void OleCommandInstance_BeforeQueryStatus(object sender, EventArgs e)
        {
            if (BranchDiffFilterProvider.IsFilterApplied)
            {
                OleCommandInstance.Visible = true;
                return;
            }

            OleCommandInstance.Visible = false;
        }

        protected override void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var branches = gitObjectsStore.GetBranches(gitBranchDifferPackage.SolutionDirectory);
            var recentCommits = gitObjectsStore.GetRecentCommits(gitBranchDifferPackage.SolutionDirectory);
            var recentTags = gitObjectsStore.GetRecentTags(gitBranchDifferPackage.SolutionDirectory);

            MessageBox.Show(gitBranchDifferPackage.SolutionDirectory);
        }
    }
}
