using cl2j.FileStorage.Provider.AzureBlobStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace cl2j.FileStorage.Tests
{
    internal class ConfigurationHelpers
    {
        public static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            services.AddSingleton(configuration);

            services.AddLogging(builder => { builder.AddDebug().SetMinimumLevel(LogLevel.Trace); });

            services.AddFileStorage();

            return services.BuildServiceProvider();
        }

        public static IServiceProvider Configure(IServiceProvider serviceProvider)
        {
            //Add the Local provider
            serviceProvider.UseFileStorageDisk();

            //Add the Azure provider
            serviceProvider.UseFileStorageAzureBlobStorage();

            return serviceProvider;
        }
    }
}