using BranchDiffer.Git.Models.LibGit2SharpModels;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BranchDiffer.VS.Shared.FileDiff.Commands
{
    public sealed partial class GitReferenceObjectConfigurationDialog : DialogWindow
    {
        public ObservableCollection<GitBranch> BranchListData { get; } = new ObservableCollection<GitBranch>();

        public ObservableCollection<GitReference> CommitListData { get; } = new ObservableCollection<GitReference>();

        public ObservableCollection<GitTag> TagListData { get; } = new ObservableCollection<GitTag>();

        public IGitObject SelectedReference { get; set; }

        public GitReferenceObjectConfigurationDialog()
        {
            this.InitializeComponent();
        }

        public void SetDefaultReference(IGitObject reference)
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
                case GitReference commit:
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
                SelectedReference = BranchList.SelectedItem as IGitObject;
            }
        }

        private void CommitList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CommitList.SelectedItem != null)
            {
                SelectedReference = CommitList.SelectedItem as IGitObject;
            }
        }

        private void TagList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TagList.SelectedItem != null)
            {
                SelectedReference = TagList.SelectedItem as IGitObject;
            }
        }
    }
}
