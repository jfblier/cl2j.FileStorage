using cl2j.FileStorage.Core;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cl2j.FileStorage.Extensions
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerOptions optionsDeserialize = new();

        static JsonExtensions()
        {
            optionsDeserialize.PropertyNameCaseInsensitive = true;
            optionsDeserialize.Converters.Add(new JsonStringEnumConverter());
        }

        public static string SerializeJsonObject<T>(T obj, bool indented = false)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = indented,
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Serialize(obj, options);
        }

        public static T? DeserializeJsonObject<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, optionsDeserialize);
        }

        public static async Task<T?> ReadJsonObjectAsync<T>(this IFileStorageProvider fileStorageProvider, string fileName, Encoding? encoding = null) where T : new()
        {
            var data = await fileStorageProvider.ReadTextAsync(fileName, encoding);

            if (string.IsNullOrWhiteSpace(data))
                return new T();

            return DeserializeJsonObject<T>(data);
        }

        public static async Task WriteJsonObjectAsync<T>(this IFileStorageProvider fileStorageProvider, string fileName, T obj, bool indented = false, Encoding? encoding = null)
        {
            var data = SerializeJsonObject(obj, indented);
            await fileStorageProvider.WriteTextAsync(fileName, data, encoding);
        }
    }
}