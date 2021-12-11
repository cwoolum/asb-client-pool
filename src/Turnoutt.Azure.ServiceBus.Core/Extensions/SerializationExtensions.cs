using System.Text.Json;

namespace System
{
    public static class SerializationsExtensions
    {
        /// <summary>
        /// Uses the JSON Serializer to serialize to a byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static T FromByteArray<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(byteArray));
        }

        /// <summary>
        /// Uses the JSON Serializer to serialize from a byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            return JsonSerializer.SerializeToUtf8Bytes(obj,
                    new JsonSerializerOptions { WriteIndented = false, IgnoreNullValues = true });
        }
    }
}