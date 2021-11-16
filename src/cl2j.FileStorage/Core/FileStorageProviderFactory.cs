using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace cl2j.FileStorage.Core
{
    internal class FileStorageProviderFactory : IFileStorageProviderFactory
    {
        private static readonly object Lock = new();
        private readonly IConfigurationRoot configuration;
        private readonly ILogger<FileStorageProviderFactory> logger;
        private static Dictionary<string, IFileStorageProvider> storageProviderInstances = new Dictionary<string, IFileStorageProvider>();

        public FileStorageProviderFactory(IConfigurationRoot configuration, ILogger<FileStorageProviderFactory> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public void Register<T>(string type) where T : IFileStorageProvider, new()
        {
            var section = configuration.GetSection("cl2j:FileStorage:Storages");

            var childrenConfigs = section.GetChildren();
            foreach (var childConfig in childrenConfigs)
            {
                var providerType = childConfig["Type"];
                if (providerType == type)
                {
                    var providerName = childConfig.Key;
                    if (!storageProviderInstances.ContainsKey(providerName))
                    {
                        var provider = new T();
                        provider.Initialize(providerName, childConfig);

                        storageProviderInstances.Add(providerName, provider);

                        logger?.LogTrace($"Registered '{providerName}' with provider '{typeof(T).Name}'");
                    }
                }
            }
        }

        public IFileStorageProvider Get(string name)
        {
            //First search into the cached fileSystems
            if (storageProviderInstances.TryGetValue(name, out var fileStorageInstance))
                return fileStorageInstance;

            //Create the fileSystem
            lock (Lock)
            {
                if (storageProviderInstances.TryGetValue(name, out fileStorageInstance))
                    return fileStorageInstance;

                //if (configurations.TryGetValue(name, out var configuration))
                //{
                //    fileSystem = Create(configuration);
                //    stores.Add(name, fileSystem);
                //    return fileSystem;
                //}
            }

            //var knownFileSystems = new StringBuilder();
            //foreach (var kvp in configurations)
            //    knownFileSystems.Append($"'{kvp.Key}' ");

            //throw new Exception($"FileSystem '{name}' is not defined in the application bootstrap. Known file systems: {knownFileSystems}");
            return null;
        }
    }
}