using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AzureStorage
{
    public static class Extensions
    {
         

        public static byte[] ToByteArray(this Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                return br.ReadBytes((int)stream.Length);
            }
        }

        public static MemoryStream GetStreamFromFile(string path)
        {
            var bytes = System.IO.File.ReadAllBytes(path);

            System.IO.File.Delete(path);
            return new MemoryStream(bytes);

        }

        public static List<string> ReadLines(this System.IO.Stream stream)
        {
            List<string> lineas = new List<string> { };
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineas.Add(line);
                }
            }
            return lineas;
        }
    }
}
