using System;
using EdFi.Ods.AdminApp.Web;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Azure.IntegrationTests
{
    public static class Testing
    {
        private static readonly IServiceScopeFactory ScopeFactory;

        static Testing()
        {
            var serviceProvider = Program.CreateHostBuilder(new string[] { })
                .ConfigureServices((context, services) =>
                {
                    // Test-specific IoC modifications here.
                })
                .Build()
                .Services;

            ScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        public static void EnsureInitialized()
        {
            ScopeFactory.ShouldNotBeNull();
        }

        public static void Scoped<TService>(Action<TService> action)
        {
            using (var scope = ScopeFactory.CreateScope())
                action(scope.ServiceProvider.GetRequiredService<TService>());
        }

        public static TResult Scoped<TService, TResult>(Func<TService, TResult> func)
        {
            var result = default(TResult);

            Scoped<TService>(service =>
            {
                result = func(service);
            });

            return result;
        }
    }
}
