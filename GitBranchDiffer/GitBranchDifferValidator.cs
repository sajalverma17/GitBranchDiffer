using BranchDiffer.Git.DiffModels;
using BranchDiffer.Git.DiffServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GitBranchDiffer
{
    /// <summary>
    /// Helper to make some prelimenary validations on our VSPackage and the Solution opened in Visual Studio by user.
    /// </summary>
    public static class GitBranchDifferValidator
    {
        public static bool ValidatePackage(GitBranchDifferPackage package)
        {
            if (package is null)
            {
                MessageBox.Show(
                    "Unable to load Git Branch Differ plug-in. It is possible Visual Studio is still initializing, please wait and try again.",
                    "Git Branch Differ");

                return false;
            }

            if (package.BranchToDiffAgainst is null || package.BranchToDiffAgainst == string.Empty)
            {
                VsShellUtilities.ShowMessageBox(
                    package,
                    "Branch to diff against is not set. Go to Options -> Git Branch Differ -> Set \"Branch To Diff Against\"",
                    "Git Branch Differ",
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                return false;
            }

            return true;
        }

        public static bool ValidateSolution(string solutionDirectory, string solutionFile, IVsHierarchyItem rootItemInSolution, GitBranchDifferPackage package)
        {
            if (package.PackageInitializationState == PackageInitializationState.Invalid)
            {
                ErrorPresenter.ShowError(
                        package,
                        ErrorPresenter.PackageInvalidStateError);

                return false;
            }

            if (package.PackageInitializationState == PackageInitializationState.SolutionInfoUnset)
            {
                ErrorPresenter.ShowError(
                       package,
                       "Unable to get Solution from Visual Studio services.\n" +
                       "If you closed and then opened another solution, please wait until it is finished loading.\n" +
                       "If the error persists, please restart Visual Studio with this solution as the start-up solution.");

                return false;
            }


            if (package.PackageInitializationState == PackageInitializationState.SoltuionInfoSet && string.IsNullOrEmpty(solutionDirectory))
            {
                ErrorPresenter.ShowError(
                    package,
                    "GitBranchDiffer detected a solution, but was unable to extract directory in which the .sln file resides");
                return false;

            }

            var solutionFileInHierarchy = System.IO.Path.GetFileName(rootItemInSolution.CanonicalName);
            if (!string.Equals(solutionFile, solutionFileInHierarchy))
            {
                ErrorPresenter.ShowError(
                    package,
                    $"The solution determined by GiBranchDiffer ({solutionFile}) does not correspond to the solution opened in Solution Explorer ({solutionFileInHierarchy}).\n" +
                    $"If you have previously closed ({solutionFile}) and just opened ({solutionFileInHierarchy}), please wait until it is fully loaded.\n" +
                    $"If the problem persists, please restart Visual Studio with {solutionFileInHierarchy} as a start-up solution.");

                return false;
            }

            return true;
        }        
    }
}
