using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YFPos.Utils
{
    /// <summary>
    /// Excel操作
    /// </summary>
    public class ExcelUtils
    {
        /// <summary>
        /// 导出list对象到excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="subPathName"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool SaveToExcel<T>(List<T> list,string subPathName, out string msg)
        {
            msg = "";
            object misValue = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook xlWorkBook = xlApp.Workbooks.Add(misValue);
            int iSheet = 1;
            Worksheet xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(iSheet);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), string.Format("excel\\{0}\\{1}.xls", subPathName, DateTime.Now.ToString("yyyyMMddHHssmm")));
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            bool isSuceess = true;

            PropertyInfo[] props = typeof(T).GetProperties();
            for (int j = 0; j < list.Count; j++)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    if (0 == j)
                    {
                        //如果属性有描述,则取描述为标题
                        if (props[i].IsDefined(typeof(DescriptionAttribute), true))
                        {
                            object[] descObjs = props[i].GetCustomAttributes(typeof(DescriptionAttribute), true);
                            xlWorkSheet.Cells[1, i + 1] = ((DescriptionAttribute)descObjs[0]).Description;
                        }
                        else
                        {
                            xlWorkSheet.Cells[1, i + 1] = props[i].Name;
                        }
                    }
                    object value = props[i].GetValue(list[j], null) ?? string.Empty;
                    xlWorkSheet.Cells[j + 2, i + 1] = value;
                }
                if (j > 0 && 0 == j % 60000)//office2003 一个Sheet不能超过65535行
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(++iSheet);
                }
            }
            try
            {
                xlWorkBook.SaveAs(Filename: filePath, FileFormat: XlFileFormat.xlWorkbookNormal);
                xlWorkBook.Close(true, misValue, misValue);
                xlWorkBook = null;
                xlApp.Quit();
                xlApp = null;
                msg = filePath;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                isSuceess = false;
            }
            finally
            {
                GC.Collect();//垃圾回收
                GC.WaitForPendingFinalizers();
            }

            return isSuceess;
        }

        public static bool SaveToCSVFile<T>(List<T> list, string subPathName,out string filePath, out string msg)
        {
            msg = "";
            filePath = Path.Combine(Directory.GetCurrentDirectory(), string.Format("excel\\{0}\\{1}.csv", subPathName, DateTime.Now.ToString("yyyyMMddHHssmm")));
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            var strColumn = new StringBuilder();//列名
            var strValue = new StringBuilder();//列名
            bool isSuceess = true;

            PropertyInfo[] props = typeof(T).GetProperties();

            using (var sw = new StreamWriter(filePath,false,Encoding.Default))
            {
                for (int j = 0; j < list.Count; j++)
                {
                    for (int i = 0; i < props.Length; i++)
                    {
                        if (0 == j)
                        {
                            //如果属性有描述,则取描述为标题
                            if (props[i].IsDefined(typeof(DescriptionAttribute), true))
                            {
                                object[] descObjs = props[i].GetCustomAttributes(typeof(DescriptionAttribute), true);
                                strColumn.Append(((DescriptionAttribute)descObjs[0]).Description);
                            }
                            else
                            {
                                strColumn.Append(props[i].Name);
                            }
                            strColumn.Append(",");
                        }
                        object value = props[i].GetValue(list[j], null) ?? string.Empty;
                        if (value != string.Empty)
                        {
                            strValue.Append("'");
                        }
                        strValue.Append(value);
                        strValue.Append(",");
                    }
                    if (0 == j)
                    {
                        sw.WriteLine(strColumn);
                    }
                    sw.WriteLine(strValue);
                    strValue.Remove(0, strValue.Length);
                }
            }

            return isSuceess;
        }
    }
}
