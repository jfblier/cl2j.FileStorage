using cl2j.FileStorage.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace cl2j.FileStorage.Provider.AzureBlobStorage
{
    public static class FileStorageAzureExtensions
    {
        public static void UseFileStorageAzureBlobStorage(this IServiceProvider serviceProvider)
        {
            var fileStorageProviderFactory = serviceProvider.GetRequiredService<IFileStorageFactory>();
            fileStorageProviderFactory.Register<FileStorageProviderAzureBlobStorage>("AzureBlobStorage");
        }
    }
}