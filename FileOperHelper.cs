using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YFPos.Utils
{
    public class FileOperHelper
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file">文件路径</param>
        public static void DelFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// 创建新文件
        /// </summary>
        /// <param name="file">文件全路径</param>
        /// <param name="isExistDel">如已存在,是否删除</param>
        public static void CreatNewFile(string file, bool isExistDel = false)
        {
            if (File.Exists(file) && !isExistDel)
            {
                return;
            }
            string filePath = Path.GetDirectoryName(file);
            //判断文件路径是否存在
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            FileStream fs = File.Create(file);
            fs.Close();
        }

        /// <summary>
        /// 追加或覆盖写入文本
        /// </summary>
        /// <param name="file">要写入的文件</param>
        /// <param name="fileText">要写入的文本</param>
        /// <param name="isAppend">是否追加,true=是</param>
        public static void WriteFile(string file, string fileText, bool isAppend = true)
        {
            using (StreamWriter sw = new StreamWriter(file, isAppend, Encoding.Default))
            {
                sw.WriteLine(fileText);
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string GetContent(string filepath, Encoding encoding, out string errMsg)
        {
            errMsg = "";

            FileStream fs = null;
            StreamReader sr = null;
            string strLine = "";

            using (fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (sr = new StreamReader(fs, encoding))
            {
                try
                {
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    strLine = sr.ReadToEnd();
                }
                catch (Exception ex)
                {
                    //错误日志
                    errMsg = string.Format("文件{0}读取数据失败！失败原因：{1}", filepath, ex.ToString());
                }
            }

            return strLine;
        }


        /// <summary>
        /// 将文件内容读取出来的字符串转为List<>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<T> FileToList<T>(string filePath) where T : new()
        {
            var objString = System.IO.File.ReadAllText(filePath, Encoding.Default);
            T obj = new T();
            List<T> listObj = new List<T>();
            //根据|分割字符串
            List<string> dataList = objString.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //定位字符串数组位置
            if (dataList != null && dataList.Count > 0)
            {
                //获取类型T中的所有属性
                PropertyInfo[] propertyInfos = typeof(T).GetProperties();
                foreach (var item in dataList)
                {
                    List<string> data = item.Split(new[] { '\t' }).ToList();
                    obj = new T();
                    int count = 0;

                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        if (count < data.Count)
                        {
                            //属性类型的String属性
                            if (propertyInfo.PropertyType == typeof(System.String))
                            {
                                propertyInfo.SetValue(obj, data[count], null);
                            }

                            count++;
                        }
                    }

                    listObj.Add(obj);
                }
            }

            return listObj;
        }
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="filesName">文件名</param>
        /// <param name="sourcePath">源文件路径（ 路径最后面不带斜杠 ）</param>
        /// <param name="targetPath">目标文件路径（ 路径最后面不带斜杠 ）</param>
        public static void MoveFile(string[] filesName, string sourcePath, string targetPath)
        {
            if (sourcePath.EndsWith("\\"))
            {
                sourcePath = sourcePath.Substring(0, sourcePath.Length - 1);
            }
            if (targetPath.EndsWith("\\"))
            {
                targetPath = targetPath.Substring(0, targetPath.Length - 1);
            }

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            foreach (var item in filesName)
            {
                try
                {
                    var path = sourcePath + "\\" + item;
                    
                    if (File.Exists(path))
                    {
                        if (File.Exists(targetPath + "\\" + item))
                        {
                            File.Delete(targetPath + "\\" + item);
                        }
                        File.Move(path, targetPath + "\\" + item);
                    }
                }
                catch (Exception ex)
                {
                    ex.SaveLog();
                }
            }
        }
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="filesName">文件名</param>
        /// <param name="sourcePath">源文件路径（ 路径最后面不带斜杠 ）</param>
        /// <param name="targetPath">目标文件路径（ 路径最后面不带斜杠 ）</param>
        public static void CopyFile(string[] filesName, string sourcePath, string targetPath)
        {
            if (sourcePath.EndsWith("\\"))
            {
                sourcePath = sourcePath.Substring(0, sourcePath.Length - 1);
            }
            if (targetPath.EndsWith("\\"))
            {
                targetPath = targetPath.Substring(0, targetPath.Length - 1);
            }

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            foreach (var item in filesName)
            {
                try
                {
                    var path = sourcePath + "\\" + item;
                    if (File.Exists(path))
                    {
                        File.Copy(path, targetPath + "\\" + item, true);
                    }
                }
                catch (Exception ex)
                {
                    ex.SaveLog();
                }
            }
        }

    }
}
