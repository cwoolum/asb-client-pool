using System.Text.Json;

namespace System
{
    public static class SerializationsExtensions
    {
        public static T FromByteArray<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(byteArray));
        }

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