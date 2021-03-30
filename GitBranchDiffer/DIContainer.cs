using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BranchDiffer.Git.DiffServices;
using GitBranchDiffer.ViewModels;
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
            container.AddScoped<IGitBranchDiffService, GitBranchDiffService>();
            container.AddScoped<BranchDiffViewModel>();
            return container.BuildServiceProvider();
        }
    }
}
