using BranchDiffer.Git.DiffModels;
using BranchDiffer.Git.DiffServices;
using GitBranchDiffer.Filter;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GitBranchDiffer.Filter
{
    /// <summary>
    /// Helper to make some prelimenary validations done by Filter on Package info and the Solution info set on it.
    /// </summary>
    public static class BranchDiffFilterValidator
    {
        public static bool ValidatePackage(GitBranchDifferPackage package)
        {
            if (package is null)
            {
                MessageBox.Show(
                    "Unable to load Git Branch Differ plug-in. It is possible Visual Studio is still initializing, please wait and try again.",
                    ErrorPresenter.PackageNameToDisplay);

                return false;
            }

            if (package.BranchToDiffAgainst is null || package.BranchToDiffAgainst == string.Empty)
            {
                ErrorPresenter.ShowError(package, "Branch to diff against is not set. Go to Options -> Git Branch Differ -> Set \"Branch To Diff Against\"");
                return false;
            }

            return true;
        }

        public static bool ValidateSolution(string solutionDirectory, string solutionFile, GitBranchDifferPackage package)
        {
            // We set solution info from solution load event on the filter, and display the filter button at the same time,
            // so just to be safe, check if info was set before user clicked the button
            if (string.IsNullOrEmpty(solutionDirectory) || string.IsNullOrEmpty(solutionFile))
            {
                ErrorPresenter.ShowError(
                       package,
                       "Unable to get Solution from Visual Studio services.\n" +                       
                       "If the error persists, please restart Visual Studio with this solution as the start-up solution.");

                return false;
            }

            return true;
        }
    }
}
