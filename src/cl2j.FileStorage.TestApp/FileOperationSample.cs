using cl2j.FileStorage.Core;
using cl2j.FileStorage.Extensions;
using System.Threading.Tasks;

namespace cl2j.FileStorage.TestApp
{
    internal class FileOperationSample
    {
        private IFileStorageProvider fileStorageProvider;

        public FileOperationSample(IFileStorageProviderFactory fileStorageFactory)
        {
            fileStorageProvider = fileStorageFactory.Get("Data");
        }

        public async Task ExecuteAsync()
        {
            //Create a file with the specified text
            await fileStorageProvider.WriteTextAsync("file1.txt", "First part of the text");

            //Read the file content
            var text = await fileStorageProvider.ReadTextAsync("file1.txt");

            //Append text to the existing file
            await fileStorageProvider.AppendTextAsync("file1.txt", "Second part of the text");

            //Delete the file
            await fileStorageProvider.DeleteAsync("file1.txt");
        }
    }
}