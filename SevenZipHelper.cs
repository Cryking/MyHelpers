using ICSharpCode.SharpZipLib.Zip;
using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YFPos.Utils
{
    /// <summary>
    /// 7ZIP帮助类
    /// </summary>
    public class SevenZipHelper
    {
        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="archiveFullName">压缩文件名称</param>
        /// <param name="directory">解压路径</param>
        public static void UnZip(string archiveFullName, string directory)
        {
            try
            {
                LogHelper.WriteLog(LogCategorys.NORMAL, "ICSharpCode.SharpZipLib开始解压-----------文件路径：" + archiveFullName);
                UnZip_ZipLib(archiveFullName, directory);
                LogHelper.WriteLog(LogCategorys.NORMAL, "ICSharpCode.SharpZipLib解压完成-----------文件路径：" + archiveFullName);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategorys.NORMAL, "ICSharpCode.SharpZipLib解压错误-----------文件路径：" + archiveFullName + "，异常信息：" + ex.ToString());
                try
                {
                    LogHelper.WriteLog(LogCategorys.NORMAL, "SevenZipSharp.dll开始解压-----------文件路径：" + archiveFullName);
                    using (var zip = new SevenZipExtractor(archiveFullName))
                    {
                        zip.ExtractArchive(directory);
                    }
                    LogHelper.WriteLog(LogCategorys.NORMAL, "SevenZipSharp.dll解压完成-----------文件路径：" + archiveFullName);
                }
                catch (Exception ex2)
                {
                    LogHelper.WriteLog(LogCategorys.NORMAL, "SevenZipSharp.dll文件解压错误-----------文件路径：" + archiveFullName + "，异常信息：" + ex2.ToString());
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="directory">被压缩文件路径</param>
        /// <param name="archiveName">压缩名称</param>
        public static void InZip(string directory, string archiveName)
        {
            try
            {
                var cmp = new SevenZipCompressor();
                cmp.ArchiveFormat = OutArchiveFormat.Zip;
                cmp.CompressDirectory(directory, archiveName);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategorys.NORMAL, "文件解压失败-----------文件路径：" + ex.ToString());
                throw ex;
            }
        }

        /// <summary>   
        /// 解压功能(解压压缩文件到指定目录)   
        /// </summary>   
        /// <param name="fileToUnZip">待解压的文件</param>   
        /// <param name="zipedFolder">指定解压目标目录</param>   
        /// <param name="password">密码</param>   
        /// <returns>解压结果</returns>
        public static void UnZip_ZipLib(string fileToUnZip, string zipedFolder)
        {
            System.IO.FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;
            if (!File.Exists(fileToUnZip))
            {
                throw new Exception(fileToUnZip + "不存在");
            }
            if (!Directory.Exists(zipedFolder))
            {
                Directory.CreateDirectory(zipedFolder);
            }
            try
            {
                zipStream = new ZipInputStream(File.OpenRead(fileToUnZip));
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = Path.Combine(zipedFolder, ent.Name);
                        fileName = fileName.Replace('/', '\\');
                        if (fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }
                        fs = File.Create(fileName);
                        int size = 1024 * 1024 * 2;
                        byte[] data = new byte[size];
                        int actualSize = 0;
                        while (true)
                        {
                            actualSize = zipStream.Read(data, 0, size);
                            if (actualSize > 0)
                            {
                                fs.Write(data, 0, actualSize);
                            }
                            else
                            {
                                fs.Close();
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                //GC.Collect();
                //GC.Collect(1);
            }
        }
    }
}
