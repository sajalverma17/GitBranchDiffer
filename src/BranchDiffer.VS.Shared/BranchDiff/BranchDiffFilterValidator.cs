using BranchDiffer.VS.Shared.Utils;

namespace BranchDiffer.VS.Shared.BranchDiff
{
    public class BranchDiffFilterValidator
    {
        private readonly ErrorPresenter errorPresenter;

        public BranchDiffFilterValidator(ErrorPresenter errorPresenter)
        { 
            this.errorPresenter = errorPresenter;
        }

        public bool ValidatePackage(IGitBranchDifferPackage package)
        {
            if (package is null)
            {
                this.errorPresenter.ShowError("Unable to load Git Branch Differ plug-in. It is possible Visual Studio is still initializing, please wait and try again.");

                return false;
            }

            return true;
        }

        public bool ValidateSolution(string solutionDirectory)
        {
            // We set solution info from solution load event on the filter, and display the filter button at the same time,
            // so just to be safe, check if info was set before user clicked the button
            if (string.IsNullOrEmpty(solutionDirectory))
            {
                this.errorPresenter.ShowError(
                       "Unable to get Solution from Visual Studio services.\n" +
                       "If the error persists, please restart Visual Studio with this solution as the start-up solution.");

                return false;
            }

            return true;
        }
    }
}
