using System;
using BranchDiffer.Git.Core;
using BranchDiffer.Git.DiffServices;
using Microsoft.Extensions.DependencyInjection;

namespace GitBranchDiffer
{
    /// <summary>
    /// Singleton DI container of the plugin project.
    /// Not thread safe, not sure if async working of plugin could cause problems here.
    /// </summary>
    public static class DIContainer
    {
        private static IServiceProvider instance;

        internal static IServiceProvider Instance
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
            container.AddScoped<GitBranchDifferService>();
            container.AddScoped<GitFileDifferService>();
            container.AddScoped<IGitDiffService, GitDiffService>();
            container.AddScoped<IGitItemIdentityService, GitItemIdentityService>();
            container.AddScoped<IGitRepoService, GitRepoService>();
            return container.BuildServiceProvider();
        }
    }
}
