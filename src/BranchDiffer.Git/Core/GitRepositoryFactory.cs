using BranchDiffer.Git.Exceptions;
using BranchDiffer.Git.Models.LibGit2SharpModels;
using LibGit2Sharp;

namespace BranchDiffer.Git.Core
{
    public interface IGitRepositoryFactory
    {
        IGitRepository Create(string directoryPath);
    }

    public class GitRepositoryFactory : IGitRepositoryFactory
    {
        public IGitRepository Create(string directoryPath)
        {
            return this.DoCreate(directoryPath);
        }

        private IGitRepository DoCreate(string directoryPath)
        {
            GitRepository createdRepository;
            try
            {
                Repository native = new Repository(directoryPath);
                createdRepository = new GitRepository(native);
            }
            catch (RepositoryNotFoundException repoNotFoundException)
            {
                throw new GitOperationException(repoNotFoundException.Message);
            }

            return createdRepository;
        }
    }
}
