using cl2j.FileStorage.Core;
using cl2j.FileStorage.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace cl2j.FileStorage.Tests
{
    public class FileStorageProviderDiskTests : IAsyncLifetime
    {
        private IFileStorageProvider fileStorageProvider;
        private const string Filename1 = "file1.txt";
        private const string Filename2 = "file2.txt";
        private const string Text1 = "First part of the text";
        private const string Text2 = "Second part of the text";

        #region Warn up / Cool down

        public FileStorageProviderDiskTests()
        {
        }

        public Task InitializeAsync()
        {
            var serviceProvider = ConfigurationHelpers.Configure(ConfigurationHelpers.ConfigureServices());
            fileStorageProvider = serviceProvider.GetRequiredService<IFileStorageFactory>().Get("Data");

            return fileStorageProvider.ClearDirectoryAsync("");
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
            // Setup
            await fileStorageProvider.ClearDirectoryAsync("");

            //Act
            await fileStorageProvider.WriteTextAsync(Filename1, Text1);

            //Assert
            Assert.True(await fileStorageProvider.ExistsAsync(Filename1));

            var text1Read = await fileStorageProvider.ReadTextAsync(Filename1);
            Assert.Equal(Text1, text1Read);
        }

        [Fact]
        public async Task AppendAsync_IfExists_DoNotThrow()
        {
            // Setup
            await fileStorageProvider.ClearDirectoryAsync("");

            //Act
            await fileStorageProvider.WriteTextAsync(Filename1, Text1);
            await fileStorageProvider.AppendTextAsync(Filename1, Text2);

            //Assert
            Assert.True(await fileStorageProvider.ExistsAsync(Filename1));

            var text1Read = await fileStorageProvider.ReadTextAsync(Filename1);
            Assert.Equal(Text1 + Text2, text1Read);
        }

        [Fact]
        public async Task ListAsync_IfExists_DoNotThrow()
        {
            // Setup
            await fileStorageProvider.ClearDirectoryAsync("");

            //Act
            await fileStorageProvider.WriteTextAsync(Filename1, Text1);
            await fileStorageProvider.AppendTextAsync(Filename2, Text2);

            //Assert
            var rootFiles = await fileStorageProvider.ListAsync("");
            Assert.Equal(2, rootFiles.Count());
        }

        [Fact]
        public async Task DeleteAsync_IfNotExists_DoNotThrow()
        {
            // Setup
            await fileStorageProvider.ClearDirectoryAsync("");

            //Act
            await fileStorageProvider.WriteTextAsync(Filename1, Text1);
            await fileStorageProvider.DeleteAsync(Filename1);

            //Assert
            Assert.False(await fileStorageProvider.ExistsAsync(Filename1));
        }

        #endregion Tests
    }
}