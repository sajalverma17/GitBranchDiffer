﻿using EnvDTE;
using GitBranchDiffer.Filter;
using GitBranchDiffer.SolutionSelectionModels;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchDiffer.FileDiff.Commands
{
    public abstract class OpenDiffCommand
    {
        private readonly GitBranchDifferPackage package;
        private readonly DTE dte;
        private readonly IVsDifferenceService vsDifferenceService;

        public OpenDiffCommand(GitBranchDifferPackage package, DTE dte, OleMenuCommandService commandService, CommandID menuCommandId)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.dte = dte ?? throw new ArgumentNullException(nameof(dte));
            this.vsDifferenceService = this.package.GetServiceAsync(typeof(SVsDifferenceService)) as IVsDifferenceService;
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommand = new OleMenuCommand(this.Execute, menuCommandId);
            OleCommandInstance = menuCommand;
            commandService.AddCommand(menuCommand);
        }

        protected static OleMenuCommand OleCommandInstance { get; private set; }

        protected virtual void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uih = (UIHierarchy)this.dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;
            Array selectedItems = (Array)uih.SelectedItems;
            if (selectedItems != null && selectedItems.Length == 1)
            {
                var selectedHierarchyItem = selectedItems.GetValue(0) as UIHierarchyItem;
                var selectedObject = selectedHierarchyItem?.Object;

                if (selectedObject != null)
                {
                    if (selectedObject is ProjectItem)
                    {
                        var selectedProjectItem = selectedObject as ProjectItem;
                        var oldPath = BranchDiffFilterProvider.TagManager.GetOldFilePathFromRenamed(selectedProjectItem);
                        var selection = new SolutionSelectionContainer<ISolutionSelection>
                        {
                            Item = new SelectedProjectItem { Native = selectedProjectItem, OldFullPath = oldPath }
                        };

                        this.ShowFileDiffWindow(selection);
                    }
                    else if (selectedObject is Project)
                    {
                        var selectedProject = selectedObject as Project;
                        var oldPath = BranchDiffFilterProvider.TagManager.GetOldFilePathFromRenamed(selectedProject);
                        var selection = new SolutionSelectionContainer<ISolutionSelection>
                        {
                            Item = new SelectedProject { Native = selectedProject, OldFullPath = oldPath }
                        };

                        this.ShowFileDiffWindow(selection);
                    }
                }
            }
        }

        private void ShowFileDiffWindow(SolutionSelectionContainer<ISolutionSelection> solutionSelectionContainer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!string.IsNullOrEmpty(solutionSelectionContainer.FullName))
            {
                var absoluteSoltuionPath = this.dte.Solution.FullName;
                var solutionDirectory = System.IO.Path.GetDirectoryName(absoluteSoltuionPath);
                var fileDiffProvider = new VsFileDiffProvider(this.vsDifferenceService, solutionDirectory, solutionSelectionContainer);
                fileDiffProvider.ShowFileDiffWithBaseBranch(package.BranchToDiffAgainst);
            }
        }
    }
}
