using cl2j.FileStorage.Core;
using cl2j.FileStorage.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace cl2j.FileStorage.TestApp
{
    internal class Program
    {
        private static async Task Main()
        {
            ServiceProvider serviceProvider = ConfigureServices();

            //Retreive the
            var fileStorageProvider = serviceProvider.GetRequiredService<IFileStorageProviderFactory>().Get("Data");

            //Create a file
            await fileStorageProvider.WriteTextAsync("file1.txt", "First part of the text");

            //Read the file content
            var text = await fileStorageProvider.ReadTextAsync("file1.txt");

            //Append text to the existing file
            await fileStorageProvider.AppendTextAsync("file1.txt", "Second part of the text");

            //Delete the file
            await fileStorageProvider.DeleteAsync("file1.txt");
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            services.AddSingleton(configuration);

            services.AddLogging(builder => { builder.AddDebug().SetMinimumLevel(LogLevel.Trace); });

            //Configure the FileStorage DI
            services.AddFileStorage();

            var serviceProvider = services.BuildServiceProvider();

            //Add the FileStorgae Disk provider
            serviceProvider.UseFileStorageDisk();

            return serviceProvider;
        }
    }
}