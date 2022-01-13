using System;
using BranchDiffer.VS.Shared.BranchDiff;
using BranchDiffer.VS.Shared.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace BranchDiffer.VS.Shared.Configuration
{
    /// <summary>
    /// Resolves DI dependencies used in BranchDiffer.VS projects
    /// </summary>
    internal class VsDIContainer
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
            var services = new ServiceCollection();
            services.AddSingleton<ErrorPresenter>();
            services.AddSingleton<BranchDiffFilterValidator>();
            return services.BuildServiceProvider();
        }
    }
}
