using cl2j.FileStorage.Core;
using Microsoft.Extensions.Configuration;

namespace cl2j.FileStorage.Provider.Disk
{
    public class FileStorageProviderDisk : IFileStorageProvider
    {
        private DirectoryInfo directory = null!;
        private const int BufferSize = 4096;

        public void Initialize(string name, IConfigurationSection configuration)
        {
            Name = name;

            var settings = new FileStorageProviderDiskConfiguration();
            configuration.Bind(settings);

            if (string.IsNullOrEmpty(settings.Path))
                throw new Exception("FileStorageProviderDisk: Path configuration not defined.");

            if (Directory.Exists(settings.Path))
                directory = new DirectoryInfo(settings.Path);
            else
                directory = Directory.CreateDirectory(settings.Path);
        }

        public string Name { get; set; } = null!;

        public async Task<bool> ExistsAsync(string name)
        {
            var exists = File.Exists(GetName(name));
            await Task.CompletedTask;
            return exists;
        }

        public async Task<FileStoreFileInfo?> GetInfoAsync(string name)
        {
            try
            {
                await Task.CompletedTask;

                var fi = new FileInfo(GetName(name));
                if (!fi.Exists)
                    return null;

                return new FileStoreFileInfo
                {
                    Size = fi.Length,
                    LastModified = fi.LastWriteTimeUtc,
                    Created = fi.CreationTime
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<string>> ListAsync(string path)
        {
            await Task.CompletedTask;
            return Directory.GetFiles(GetName(path));
        }

        public async Task<bool> ReadAsync(string name, Stream stream)
        {
            try
            {
                var fs = new FileStream(GetName(name), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fs);
                await fs.CopyToAsync(stream);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task WriteAsync(string name, Stream stream)
        {
            var fileName = GetName(name);
            CreateDirectory(fileName);

            using var outputStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, BufferSize, true);
            var bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            var actualCount = await stream.ReadAsync(bytes);
            await outputStream.WriteAsync(bytes.AsMemory(0, actualCount));
        }

        public async Task AppendAsync(string name, Stream stream)
        {
            var fileName = GetName(name);
            CreateDirectory(fileName);

            using var outputStream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, BufferSize, true);
            var bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            var actualCount = await stream.ReadAsync(bytes);
            await outputStream.WriteAsync(bytes.AsMemory(0, actualCount));
        }

        public async Task DeleteAsync(string name)
        {
            await Task.CompletedTask;
            File.Delete(GetName(name));
        }

        #region Private

        private static void CreateDirectory(string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);
            if (directory != null && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        private string GetName(string key)
        {
            return Path.Combine(directory.FullName, key);
        }

        #endregion Private
    }
}