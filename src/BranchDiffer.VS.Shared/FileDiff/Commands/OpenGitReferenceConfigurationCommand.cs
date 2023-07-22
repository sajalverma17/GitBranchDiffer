using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using BranchDiffer.Git.Core;
using BranchDiffer.VS.Shared.Utils;
using BranchDiffer.VS.Shared.BranchDiff;
using Task = System.Threading.Tasks.Task;
using System.Linq;
using BranchDiffer.Git.Models.LibGit2SharpModels;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Settings;
using LibGit2Sharp;

namespace BranchDiffer.VS.Shared.FileDiff.Commands
{
    internal class OpenGitReferenceConfigurationCommand : OpenDiffCommand
    {
        private GitObjectsStore gitObjectsStore;
        private ShellSettingsManager shellSettingsManager;

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
        public async Task InitializeAndRegisterAsync(IGitBranchDifferPackage package, ShellSettingsManager shellSettingsManager, GitObjectsStore gitObjectsStore)
        {
            await this.InitializeAsync(package);
            this.gitObjectsStore = gitObjectsStore;
            this.shellSettingsManager = shellSettingsManager;
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
            dialog.SetDefaultReference(this.package.BranchToDiffAgainst);

            var result = dialog.ShowModal();

            if (result != null && result == false)
            {
                return;
            }

            IGitObject gitObject;
            if (!dialog.IsReferenceUserDefined)
            {
                gitObject = dialog.SelectedReference;
                SaveGitReference(gitObject);
                return;
            }

            gitObject = this.gitObjectsStore.FindGitReferenceByUserDefinedName(this.package.SolutionDirectory, dialog.UserDefinedReferenceName);
            if (gitObject == null)
            {
                this.errorPresenter.ShowError("The branch name/tag name/commit SHA you typed in was not found in this repository. Git reference was not changed.");
                return;
            }

            SaveGitReference(gitObject);
        }

        private void LoadTags(GitReferenceObjectConfigurationDialog dialog)
        {
            var tags = this.gitObjectsStore.GetRecentTags(this.package.SolutionDirectory);
            foreach (var tag in tags)
            {
                dialog.TagListData.Add(tag);
            }

            ActivityLog.LogInformation(nameof(OpenGitReferenceConfigurationCommand), $"Loaded {tags.Count()} tags for repo at {this.package.SolutionDirectory}.");
        }

        private void LoadCommits(GitReferenceObjectConfigurationDialog dialog)
        {
            var commits = this.gitObjectsStore.GetRecentCommits(this.package.SolutionDirectory);
            foreach (var commit in commits)
            {
                dialog.CommitListData.Add(commit);
            }

            ActivityLog.LogInformation(nameof(OpenGitReferenceConfigurationCommand), $"Loaded {commits.Count()} commits for repo at {this.package.SolutionDirectory}");
        }

        private void LoadBranches(GitReferenceObjectConfigurationDialog dialog)
        {
            var branches = this.gitObjectsStore.GetBranches(this.package.SolutionDirectory);

            foreach (var branch in branches)
            {
                dialog.BranchListData.Add(branch);
            }

            ActivityLog.LogInformation(nameof(OpenGitReferenceConfigurationCommand), $"Loaded {branches.Count()} branches for repo at {this.package.SolutionDirectory}");
        }

        private void SaveGitReference(IGitObject gitObject)
        {
            var writeSettingsStore = this.shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            if (!writeSettingsStore.CollectionExists(StorageKeys.StorageCollectionName))
            {
                writeSettingsStore.CreateCollection(StorageKeys.StorageCollectionName);
            }

            writeSettingsStore.SetString(StorageKeys.StorageCollectionName, StorageKeys.StoredPropertyName, gitObject.TipSha);
            this.package.BranchToDiffAgainst = gitObject;
        }
    }
}
