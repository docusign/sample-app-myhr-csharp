using System.IO;

namespace DocuSign.MyHR.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ReadAsBytes(this Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
