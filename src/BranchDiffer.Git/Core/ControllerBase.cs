using BranchDiffer.Git.Exceptions;
using LibGit2Sharp;

namespace BranchDiffer.Git.Core
{
    public class ControllerBase
    {
        protected Repository SetupRepository(string directoryPath)
        {
            Repository createdRepository;
            try
            {
                createdRepository = new Repository(directoryPath);
            }
            catch (RepositoryNotFoundException repoNotFoundException)
            {
                throw new GitBranchException(repoNotFoundException.Message);
            }

            return createdRepository;
        }
    }
}
