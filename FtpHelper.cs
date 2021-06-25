using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace YFPos.Utils
{
    public class FtpHelper
    {
        private static string LastFileName
        {
            get;
            set;
        }

        private static int TryCount
        {
            get;
            set;
        }

        public static void Download(string sourceFilePath, string desFilePath, string fileName,string ftpUser,string ftpPwd)
        {
            //新文件计数清零
            if (LastFileName != fileName)
            {
                TryCount = 0;
            }
            FtpWebRequest reqFTP;
            try
            {
                LastFileName = fileName;
                if (!Directory.Exists(desFilePath))
                {
                    Directory.CreateDirectory(desFilePath);
                }
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(string.Format("{0}/{1}", sourceFilePath, fileName)));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUser, ftpPwd);
                reqFTP.Timeout = 40000;
                using (var response = (FtpWebResponse)reqFTP.GetResponse())
                using (Stream ftpStream = response.GetResponseStream())
                {
                    long cl = response.ContentLength;
                    int bufferSize = 4096;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];
                    using (FileStream outputStream = new FileStream(desFilePath + "\\" + fileName, FileMode.Create))
                    {
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                        while (readCount > 0)
                        {
                            outputStream.Write(buffer, 0, readCount);
                            readCount = ftpStream.Read(buffer, 0, bufferSize);
                        }
                    }
                }
                // response.Close();
            }
            catch (Exception ex)
            {
                TryCount++;
                LogHelper.WriteLog(LogCategorys.EXCEPTION, "FTP下载文件出错:{0} {1}",
                    string.Format("{0}/{1}", sourceFilePath, fileName),ex.ToString());
                //if (outputStream != null)
                //{
                //    outputStream.Close();
                //}
                if (TryCount < 2)
                {
                    Download(sourceFilePath, desFilePath, fileName, ftpUser, ftpPwd);
                }
            }
        }
    }
}
