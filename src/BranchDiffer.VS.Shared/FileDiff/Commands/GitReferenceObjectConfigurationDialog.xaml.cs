using BranchDiffer.Git.Models.LibGit2SharpModels;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BranchDiffer.VS.Shared.FileDiff.Commands
{
    public sealed partial class GitReferenceObjectConfigurationDialog : DialogWindow, INotifyPropertyChanged
    {
        private string _userDefinedReferenceName = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<GitBranch> BranchListData { get; } = new ObservableCollection<GitBranch>();

        public ObservableCollection<GitCommit> CommitListData { get; } = new ObservableCollection<GitCommit>();

        public ObservableCollection<GitTag> TagListData { get; } = new ObservableCollection<GitTag>();

        public IGitObject SelectedReference { get; set; }

        public string UserDefinedReferenceName
        {
            get => _userDefinedReferenceName.Trim();

            set 
            {
                _userDefinedReferenceName = value;
                this.OnPropertyChanged(nameof(UserDefinedReferenceName));
            }
        }

        public bool IsReferenceUserDefined { get; private set; }

        public GitReferenceObjectConfigurationDialog()
        {
            this.InitializeComponent();

            DataContext = this;
            SearchBoxRow.Height = new GridLength(22, GridUnitType.Pixel);
            BranchList.ItemsSource = BranchListData;
            CommitList.ItemsSource = CommitListData;
            TagList.ItemsSource = TagListData;
        }

        public void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SetDefaultReference(IGitObject reference)
        {
            if (reference == null)
            {
                return;
            }

            SelectedReference = reference;
            UserDefinedReferenceName = reference.FriendlyName;

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

            if (BranchListData.Any(x => x.FriendlyName == UserDefinedReferenceName) ||
               CommitListData.Any(x => UserDefinedReferenceName.StartsWith(x.FriendlyName)) ||
               TagListData.Any(x => x.FriendlyName == UserDefinedReferenceName))
            {
                IsReferenceUserDefined = false;
                SelectedReference =
                    BranchListData.FirstOrDefault(x => x.FriendlyName == UserDefinedReferenceName) 
                    ?? CommitListData.FirstOrDefault(x => UserDefinedReferenceName.StartsWith(x.FriendlyName))
                    ?? TagListData.FirstOrDefault<IGitObject>(x => x.FriendlyName == UserDefinedReferenceName)
                    ?? throw new InvalidOperationException();
            }
            else
            {
                IsReferenceUserDefined = true;
            }

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
                UserDefinedReferenceName = SelectedReference.FriendlyName;
            }
        }

        private void CommitList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CommitList.SelectedItem != null)
            {
                SelectedReference = CommitList.SelectedItem as IGitObject;
                UserDefinedReferenceName = SelectedReference.FriendlyName;
            }
        }

        private void TagList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TagList.SelectedItem != null)
            {
                SelectedReference = TagList.SelectedItem as IGitObject;
                UserDefinedReferenceName = SelectedReference.FriendlyName;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(BranchListData);
            if (e.Changes.Any())
            {
                var textToSearch = ((TextBox)sender).Text;
                view.Filter = (branch) => { return ((GitBranch)branch).FriendlyName.Contains(textToSearch); };
            }
            else
            {
                view.Filter = null;
            }
        }
    }
}
