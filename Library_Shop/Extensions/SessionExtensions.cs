using System.Text.Json;

namespace Library_Shop.Extensions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve // Додаємо ReferenceHandler
            };
            string str = JsonSerializer.Serialize(value, options);
            session.SetString(key, str);
        }

        public static T? Get<T>(this ISession session, string key)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve // Додаємо ReferenceHandler
            };
            string? str = session.GetString(key);
            T? res = default;
            if (str is not null)
            {
                res = JsonSerializer.Deserialize<T>(str, options);
            }
            return res;
        }
    }

}
