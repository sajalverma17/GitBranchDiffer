using BranchDiffer.Git.Models.LibGit2SharpModels;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace BranchDiffer.VS.Shared.FileDiff.Commands
{
	public partial class GitReferenceConfigurationDialog : DialogWindow
    {
        public ObservableCollection<GitBranch> BranchListData { get; } = new ObservableCollection<GitBranch>();
        public ObservableCollection<GitCommit> CommitListData { get; } = new ObservableCollection<GitCommit>();

        public IGitObject SelectedReference { get; set; }

        public GitReferenceConfigurationDialog()
        {
            InitializeComponent();

            DataContext = this;
            BranchList.ItemsSource = BranchListData;
            CommitList.ItemsSource = CommitListData;
            TagList.ItemsSource = TagListData;
        }

        public void SetDefaultReference(GitReference<GitCommitObject> reference)
        {
            if (reference == null)
            {
                return;
            }

            SelectedReference = reference;

            // this is not showing up selection correctly
            switch (reference)
            {
                case GitBranch branch:
                    Tabs.SelectedItem = BranchesTab;
                    BranchList.SelectedItem = branch;
                    return;
                case GitCommit commit:
                    Tabs.SelectedItem = CommitsTab;
                    CommitList.SelectedItem = commit;
                    return;
                case GitTag tag:
                    Tabs.SelectedItem = TagsTab;
                    TagList.SelectedItem = tag;
                    return;
                default:
                    return;
            }
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BranchList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (BranchList.SelectedItem != null)
            {
                SelectedReference = BranchList.SelectedItem as GitReference<GitCommitObject>;
            }
        }

        private void CommitList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CommitList.SelectedItem != null)
            {
                SelectedReference = CommitList.SelectedItem as GitReference<GitCommitObject>;
            }
        }

        private void TagList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TagList.SelectedItem != null)
            {
                SelectedReference = TagList.SelectedItem as GitReference<GitCommitObject>;
            }
        }
    }
}