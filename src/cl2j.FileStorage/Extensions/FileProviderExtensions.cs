using cl2j.FileStorage.Core;
using System.Linq;
using System.Threading.Tasks;

namespace cl2j.FileStorage.Extensions
{
    public static class FileProviderExtensions
    {
        public static async Task<int> ClearDirectoryAsync(this IFileStorageProvider fileStorageProvider, string path)
        {
            var files = await fileStorageProvider.ListAsync(path);
            foreach (var file in files)
                await fileStorageProvider.DeleteAsync(file);
            return files.Count();
        }
    }
}