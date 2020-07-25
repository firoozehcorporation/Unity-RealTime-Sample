using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Utils
{
    public static class BinaryUtil
    {
        public static TData Deserialize<TData>(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (TData)formatter.Deserialize(stream);
            }
        }

        public static byte[] Serialize<TData>(TData obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Flush();
                stream.Position = 0;
                return stream.GetBuffer();
            }
        }
    }
}