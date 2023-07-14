using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using BranchDiffer.Git.Core;
using BranchDiffer.VS.Shared.Utils;
using BranchDiffer.VS.Shared.BranchDiff;
using System.Windows.Forms;
using BranchDiffer.Git.Configuration;
using Task = System.Threading.Tasks.Task;
using System.Linq;

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

            var dialog = new GitReferenceObjectConfigurationDialog();
            LoadBranches(dialog);
            LoadCommits(dialog);
            LoadTags(dialog);

            _ = dialog.ShowModal();

            MessageBox.Show(dialog.SelectedReference.Name);
        }

        private void LoadTags(GitReferenceObjectConfigurationDialog dialog)
        {
            var tags = gitObjectsStore.GetRecentTags(gitBranchDifferPackage.SolutionDirectory);
            foreach (var tag in tags)
            {
                dialog.TagListData.Add(tag);
            }

            ActivityLog.LogInformation(nameof(OpenGitReferenceConfigurationCommand), $"Loaded {tags.Count()} tags for repo at {gitBranchDifferPackage.SolutionDirectory}.");
        }

        private void LoadCommits(GitReferenceObjectConfigurationDialog dialog)
        {
            var commits = gitObjectsStore.GetRecentCommits(gitBranchDifferPackage.SolutionDirectory);
            foreach (var commit in commits)
            {
                dialog.CommitListData.Add(commit);
            }

            ActivityLog.LogInformation(nameof(OpenGitReferenceConfigurationCommand), $"Loaded {commits.Count()} commits for repo at {gitBranchDifferPackage.SolutionDirectory}");
        }

        private void LoadBranches(GitReferenceObjectConfigurationDialog dialog)
        {
            var branches = gitObjectsStore.GetBranches(gitBranchDifferPackage.SolutionDirectory);

            foreach (var branch in branches)
            {
                dialog.BranchListData.Add(branch);
            }

            ActivityLog.LogInformation(nameof(OpenGitReferenceConfigurationCommand), $"Loaded {branches.Count()} branches for repo at {gitBranchDifferPackage.SolutionDirectory}");

        }
    }
}
