using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ToByteArray(this Stream stream)
        {
            using (stream)
            {
                using (MemoryStream byteStream = new MemoryStream())
                {
                    await stream.CopyToAsync(byteStream);
                    return byteStream.ToArray();
                }
            }
        }
    }
}
