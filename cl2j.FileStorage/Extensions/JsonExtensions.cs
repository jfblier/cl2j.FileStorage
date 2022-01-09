using cl2j.FileStorage.Core;
using Newtonsoft.Json;
using System.Text;

namespace cl2j.FileStorage.Extensions
{
    public static class JsonExtensions
    {
        public static async Task<T> ReadJsonObjectAsync<T>(this IFileStorageProvider fileStorageProvider, string fileName, Encoding? encoding = null, bool specifyType = false) where T : new()
        {
            var data = await fileStorageProvider.ReadTextAsync(fileName, encoding);
            if (string.IsNullOrWhiteSpace(data))
                return new T();

            var options = CreateSerializerOptions(specifyType);

            var value = JsonConvert.DeserializeObject<T>(data, options);
            if (value == null)
                throw new Exception($"Unable to deserialize {typeof(T).Name} from value '{data}'");
            return value;
        }

        public static async Task WriteJsonObjectAsync<T>(this IFileStorageProvider fileStorageProvider, string fileName, T obj, bool indented = false, Encoding? encoding = null, bool specifyType = false)
        {
            var data = SerializeJsonObject(obj, indented, specifyType);
            await fileStorageProvider.WriteTextAsync(fileName, data, encoding);
        }

        private static string SerializeJsonObject<T>(T obj, bool indented, bool specifyType)
        {
            var options = CreateSerializerOptions(specifyType);
            options.Formatting = indented ? Formatting.Indented : Formatting.None;

            return JsonConvert.SerializeObject(obj, options);
        }

        private static JsonSerializerSettings CreateSerializerOptions(bool specifyType)
        {
            var options = new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
            };
            options.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            if (specifyType)
                options.TypeNameHandling = TypeNameHandling.Objects;
            return options;
        }
    }
}