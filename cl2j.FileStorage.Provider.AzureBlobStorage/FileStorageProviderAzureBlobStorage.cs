﻿using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using cl2j.FileStorage.Core;
using cl2j.Tooling.Exceptions;
using Microsoft.Extensions.Configuration;

namespace cl2j.FileStorage.Provider.AzureBlobStorage
{
    public class FileStorageProviderAzureBlobStorage : IFileStorageProvider
    {
        private BlobContainerClient? container;

        public void Initialize(string providerName, IConfigurationSection configuration)
        {
            Name = providerName;

            var settings = new FileStorageProviderAzureBlobStorageConfiguration();
            configuration.Bind(settings);

            if (string.IsNullOrEmpty(settings.Container))
                throw new NotFoundException("FileStorageProviderDisk: ContainerName configuration not defined.");

            container = new BlobContainerClient(settings.ConnectionString, settings.Container);
            try
            {
                container.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{providerName} -> Container '{settings.Container}'");
                Console.WriteLine(ex.ToString());
            }
        }

        public string Name { get; set; } = null!;

        public async Task<bool> ExistsAsync(string name)
        {
            try
            {
                return await GetBlob(name).ExistsAsync();
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                return false;
            }
        }

        public async Task<FileStoreFileInfo?> GetInfoAsync(string name)
        {
            var prop = await GetBlobPropertiesAsync(name);
            if (prop == null)
                return null;

            return new FileStoreFileInfo
            {
                Size = prop.ContentLength,
                LastModified = prop.LastModified.DateTime,
                Created = prop.CreatedOn.DateTime
            };
        }

        public async Task<IEnumerable<string>> ListAsync(string path)
        {
            if (container == null)
                throw new BadRequestException("Container not defined");

            var blobs = container.GetBlobs();
            var names = blobs.Select(b => b.Name);

            if (!string.IsNullOrEmpty(path))
                names = names.Where(n => n.StartsWith(path));

            await Task.CompletedTask;

            return names;
        }

        public async Task<bool> ReadAsync(string name, Stream stream)
        {
            try
            {
                await GetBlob(name).DownloadToAsync(stream);
                return true;
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                return false;
            }
        }

        public async Task WriteAsync(string name, Stream stream, string? contentType)
        {
            if (container == null)
                throw new BadRequestException("Container not defined");

            var blobClient = container.GetBlobClient(name);
            var blobHttpHeader = new BlobHttpHeaders();
            if (contentType != null)
                blobHttpHeader.ContentType = contentType;

            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeader, Conditions = null });
            stream.Seek(0, SeekOrigin.Begin);
        }

        public async Task AppendAsync(string name, Stream stream)
        {
            var appendBlobClient = container.GetAppendBlobClient(name);

            try
            {
                await appendBlobClient.AppendBlockAsync(stream);
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                await appendBlobClient.CreateIfNotExistsAsync();

                stream.Seek(0, SeekOrigin.Begin);
                await appendBlobClient.AppendBlockAsync(stream);
            }
        }

        public async Task DeleteAsync(string name)
        {
            await GetBlob(name).DeleteAsync();
        }

        #region Private

        private BlobClient GetBlob(string name)
        {
            if (container == null)
                throw new BadRequestException("Container not defined");

            return container.GetBlobClient(name);
        }

        public async Task<BlobProperties?> GetBlobPropertiesAsync(string name)
        {
            var blob = GetBlob(name);
            if (!await blob.ExistsAsync())
                return null;

            return (await blob.GetPropertiesAsync()).Value;
        }

        #endregion Private
    }
}