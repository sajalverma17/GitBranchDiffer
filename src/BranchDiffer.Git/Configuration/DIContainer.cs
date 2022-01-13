using System;
using BranchDiffer.Git.Core;
using BranchDiffer.Git.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BranchDiffer.Git.Configuration
{
    /// <summary>
    /// Resolves DI dependencies in BranchDiff.Git project
    /// </summary>
    public static class DIContainer
    {
        private static IServiceProvider instance;

        public static IServiceProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = BuildProvider();
                }

                return instance;
            }
        }

        private static IServiceProvider BuildProvider()
        {
            var container = new ServiceCollection();
            container.AddScoped<GitBranchDiffController>();
            container.AddScoped<GitFileDiffController>();
            container.AddScoped<IGitRepositoryFactory, GitRepositoryFactory>();
            container.AddScoped<IGitDiffService, GitDiffService>();
            container.AddScoped<IGitFileService, GitFileService>();
            container.AddScoped<IGitRepoService, GitRepoService>();
            return container.BuildServiceProvider();
        }
    }
}
