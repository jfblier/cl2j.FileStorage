using cl2j.FileStorage.Core;
using cl2j.FileStorage.Provider.Disk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace cl2j.FileStorage
{
    public static class FileStorageExtensions
    {
        public static void AddFileStorage(this IServiceCollection services)
        {
            services.TryAddSingleton<IFileStorageProviderFactory, FileStorageProviderFactory>();
        }

        public static void UseFileStorageDisk(this IServiceProvider serviceProvider)
        {
            var fileStorageProviderFactory = serviceProvider.GetRequiredService<IFileStorageProviderFactory>();
            fileStorageProviderFactory.Register<FileStorageProviderDisk>("Disk");
        }
    }
}