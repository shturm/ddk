using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace System
{
    public static class SessionExtensions
    {
        public static void SetAsObject(this ISession session, string key, object value)
        {
            string json = JsonConvert.SerializeObject(value);

            byte[] serializedResult = Encoding.UTF8.GetBytes(json);

            session.Set(key, serializedResult);
        }

        public static object GetAsObject(this ISession session, string key)
        {
            try
            {
                var value = session.Get(key);

                string json = Encoding.UTF8.GetString(value);

                return value == null ? default(object) : JsonConvert.DeserializeObject(json);
            }
            catch
            {
                return null;
            }
        }

        public static bool IsExists(this ISession session, string key)
        {
            return session.Get(key) != null;
        }
    }
}