using Newtonsoft.Json;

namespace Source.Constants
{
    public class SerializerSettings
    {
        public static readonly JsonSerializerSettings Default = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
            };
    }
}