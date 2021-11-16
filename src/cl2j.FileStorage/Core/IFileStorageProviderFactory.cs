namespace cl2j.FileStorage.Core
{
    public interface IFileStorageProviderFactory
    {
        void Register<T>(string type) where T : IFileStorageProvider, new();

        IFileStorageProvider Get(string name);
    }
}