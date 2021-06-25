using System.IO;
using System.IO.Compression;
using System.Text;

namespace YFPos.Utils
{
    public class GZipHelper
    {
        #region 字段属性
        /// <summary>
        /// 读入大小
        /// </summary>
        const int SIZE = 10240;
        /// <summary>
        /// 编码方式
        /// </summary>
        public readonly static Encoding GZipEncoding = Encoding.UTF8;
        #endregion
         
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Decompress(byte[] compressBuffer)
        { 
            using (MemoryStream msDecompress = new MemoryStream())
            {
                var gzip = new GZipStream(new MemoryStream(compressBuffer), CompressionMode.Decompress);
                int len = 0;
                byte[] buffer = new byte[SIZE];
                while ((len = gzip.Read(buffer, 0, buffer.Length)) != 0)
                {
                    msDecompress.Write(buffer, 0, len);
                }
                gzip.Close();
                return GZipEncoding.GetString(msDecompress.ToArray());
            }
        } 

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="buffer">要压缩的字节</param>
        /// <returns></returns>
        public static MemoryStream Compress(byte[] buffer)
        {
            var ms = new MemoryStream();
            var compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(buffer, 0, buffer.Length);
            compressedzipStream.Close();
            return ms;
        }         
    }
}
