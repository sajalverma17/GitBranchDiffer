using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using BranchDiffer.Git.Core;
using BranchDiffer.VS.Shared.Utils;
using BranchDiffer.VS.Shared.BranchDiff;
using BranchDiffer.Git.Configuration;
using Task = System.Threading.Tasks.Task;
using System.Linq;
using BranchDiffer.Git.Exceptions;
using BranchDiffer.Git.Models.LibGit2SharpModels;

namespace BranchDiffer.VS.Shared.FileDiff.Commands
{
    internal class OpenGitReferenceConfigurationCommand : OpenDiffCommand
    {
        private GitObjectsStore gitObjectsStore;

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

            _ = dialog.ShowModal();

            IGitObject gitObject;
            if (dialog.IsReferenceUserDefined)
            {
                try
                {
                    gitObject = this.gitObjectsStore.FindReferenceObjectByName(this.package.SolutionDirectory, dialog.UserDefinedReferenceName);
                }
                catch (GitOperationException ex)
                {
                    this.errorPresenter.ShowError(ex.Message);
                    return;
                }
            }
            else
            {
                gitObject = dialog.SelectedReference;
            }

            this.package.BranchToDiffAgainst = gitObject;
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
    }
}
