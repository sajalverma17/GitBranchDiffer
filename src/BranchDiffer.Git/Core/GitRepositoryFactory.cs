using BranchDiffer.Git.Exceptions;
using BranchDiffer.Git.Models.LibGit2SharpModels;
using LibGit2Sharp;
using System.IO;

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
                // locate .git repo upwards
                while(!Directory.Exists(Path.Combine(directoryPath, ".git")) && !File.Exists(Path.Combine(directoryPath, ".git")) && Path.GetPathRoot(directoryPath) != directoryPath)
				{
                    directoryPath = Path.GetDirectoryName(directoryPath);
				}
                Repository native = new Repository(directoryPath);
                createdRepository = new GitRepository(native);
            }
            catch (RepositoryNotFoundException nativeException)
            {
                throw new GitRepoNotFoundException(nativeException.Message);
            }

            return createdRepository;
        }
    }
}
