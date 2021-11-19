using cl2j.FileStorage.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace cl2j.FileStorage.Tests
{
    public class FileStorageProviderAzureBlobStorage : IAsyncLifetime
    {
        private FileStorageTests fileStorageTests;

        #region Warn up / Cool down

        public Task InitializeAsync()
        {
            var serviceProvider = ConfigurationHelpers.Configure(ConfigurationHelpers.ConfigureServices());
            fileStorageTests = new FileStorageTests("Azure", serviceProvider.GetRequiredService<IFileStorageFactory>().Get("Azure"));

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        #endregion Warn up / Cool down

        #region Tests

        [Fact]
        public async Task WriteAsync_IfExists_DoNotThrow()
        {
            await fileStorageTests.WriteAsync_IfExists_DoNotThrow();
        }

        [Fact]
        public async Task AppendAsync_IfExists_DoNotThrow()
        {
            await fileStorageTests.AppendAsync_IfExists_DoNotThrow();
        }

        [Fact]
        public async Task ListAsync_IfExists_DoNotThrow()
        {
            await fileStorageTests.ListAsync_IfExists_DoNotThrow();
        }

        [Fact]
        public async Task DeleteAsync_IfNotExists_DoNotThrow()
        {
            await fileStorageTests.DeleteAsync_IfNotExists_DoNotThrow();
        }

        #endregion Tests
    }
}