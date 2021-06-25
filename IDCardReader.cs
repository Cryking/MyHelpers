using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace YFPos.Utils
{
    /// <summary>
    /// 身份证读取数据类
    /// </summary>
    public class IDCardReader
    {
        #region 字段
        /// <summary>
        /// 编码格式
        /// </summary>
        private static Encoding encoding = Encoding.GetEncoding("GB2312");//.UTF8;//;   
        /// <summary>
        /// dll引用路径
        /// </summary>
        private const string dllPath = "CVR/termb.dll";
        #endregion

        #region 接口定义
        /// <summary>
        /// 初始化连接
        /// </summary>
        /// <param name="Port">连接串口（COM1~COM16）或USB口(1001~1016)</param>
        /// <returns>1	正确 2	端口打开失败 0	动态库加载失败</returns>
        [DllImport(dllPath, EntryPoint = "CVR_InitComm", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern int CVR_InitComm(int Port);//声明外部的标准动态库, 跟Win32API是一样的
        /// <summary>
        /// 读卡器和卡片之间的合法身份确认。卡认证循环间隔大于300ms
        /// </summary>
        /// <returns>1	正确	卡片认证成功 2	错误	寻卡失败 3	错误	选卡失败 0	错误	初始化失败</returns>
        [DllImport(dllPath, EntryPoint = "CVR_Authenticate", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern int CVR_Authenticate();
        /// <summary>
        /// 通过阅读器从第二代居民身份证中读取相应信息
        /// </summary>
        /// <param name="Active">兼容以前版本，无实际意义</param>
        /// <returns>1	正确 0	错误 99	异常</returns>
        [DllImport(dllPath, EntryPoint = "CVR_Read_Content", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern int CVR_Read_Content(int Active);
        /// <summary>
        /// 关闭PC到阅读器的连接
        /// </summary>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "CVR_CloseComm", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern int CVR_CloseComm();
        /// <summary>
        /// 姓名信息
        /// </summary>
        /// <param name="strTmp">返回的信息缓存指针</param>
        /// <param name="strLen">返回的长度指针</param>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "GetPeopleName", CharSet = CharSet.Ansi, SetLastError = false)]
        private static extern int GetPeopleName(StringBuilder sb, out int strLen);
        /// <summary>
        /// 民族信息
        /// </summary>
        /// <param name="strTmp"></param>
        /// <param name="strLen"></param>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "GetPeopleNation", CharSet = CharSet.Ansi, SetLastError = false)]
        private static extern int GetPeopleNation(StringBuilder sb, out int strLen);
        /// <summary>
        /// 出生日期
        /// </summary>
        /// <param name="strTmp"></param>
        /// <param name="strLen"></param>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "GetPeopleBirthday", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPeopleBirthday(StringBuilder sb, out int strLen);
        /// <summary>
        /// 地址信息
        /// </summary>
        /// <param name="strTmp"></param>
        /// <param name="strLen"></param>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "GetPeopleAddress", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPeopleAddress(StringBuilder sb, out int strLen);
        /// <summary>
        /// 卡号信息
        /// </summary>
        /// <param name="strTmp"></param>
        /// <param name="strLen"></param>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "GetPeopleIDCode", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPeopleIDCode(StringBuilder sb, out int strLen);
        /// <summary>
        /// 发证机关信息
        /// </summary>
        /// <param name="strTmp"></param>
        /// <param name="strLen"></param>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "GetDepartment", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetDepartment(StringBuilder sb, out int strLen);
        /// <summary>
        /// 有效开始日期
        /// </summary>
        /// <param name="strTmp"></param>
        /// <param name="strLen"></param>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "GetStartDate", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetStartDate(StringBuilder sb, out int strLen);
        /// <summary>
        /// 有效截止日期
        /// </summary>
        /// <param name="strTmp"></param>
        /// <param name="strLen"></param>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "GetEndDate", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetEndDate(StringBuilder sb, out int strLen);
        /// <summary>
        /// 性别
        /// </summary>
        /// <param name="strTmp"></param>
        /// <param name="strLen"></param>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "GetPeopleSex", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPeopleSex(StringBuilder sb, out int strLen);
        /// <summary>
        /// 安全模块号码
        /// </summary>
        /// <param name="strTmp"></param>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "CVR_GetSAMID", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        private static extern int CVR_GetSAMID(StringBuilder sb, out int strLen);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strTmp"></param>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "GetManuID", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetManuID(StringBuilder sb, out int strLen);
        #endregion

        #region 初始化接口
        /// <summary>
        /// 初始化接口
        /// </summary>
        /// <returns>初始化是否成功</returns>
        private static bool InitComm()
        {
            //USB
            for (int port = 1001; port <= 1016; port++)
            {
                //初始化连接 连接成功
                if (CVR_InitComm(port) == 1)
                {
                    return true;
                }
            }
            //串口
            for (int port = 1; port <= 4; port++)
            {
                //初始化连接 连接成功
                if (CVR_InitComm(port) == 1)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 验证卡片信息
        /// <summary>
        /// 验证卡片信息
        /// </summary>
        private static bool Authenticate()
        {
            bool result = false;
            //读卡次数
            int count = 20;
            //1	正确	卡片认证成功 
            //2	错误	寻卡失败  
            //3	错误	选卡失败 
            //0	错误	初始化失败
            int flag;
            do
            {
                Thread.Sleep(450);
                flag = CVR_Authenticate();
            }
            while (flag == 2 && --count > 0);

            switch (flag)
            {
                case 0:
                    throw new Exception("设备初始化失败");
                case 1:
                    result = true;
                    break;
                case 2:
                    throw new Exception("没发现身份证，请重新刷卡");
                default:
                    throw new Exception("身份证验证失败");
            }
            return result;
        }
        #endregion

        #region 读取卡信息
        /// <summary>
        /// 读取卡信息
        /// </summary>
        /// <returns></returns>
        public static IDCardInfo GetIDCardInfo()
        {
            IDCardInfo cardInfo = null;
            if (!InitComm())
            {
                throw new Exception("设备初始化失败");
            }
            if (Authenticate())
            {
                //1	正确 
                //0	错误 
                //99异常
                int result = CVR_Read_Content(4);
                if (result != 1)
                {
                    throw new Exception("身份证信息读取失败");
                }
                //填充数据
                FillDate(out cardInfo);
            }
            //关闭设备
            CVR_CloseComm();
            return cardInfo;
        }
        #endregion

        #region 填充数据
        /// <summary>
        /// 填充数据
        /// </summary>
        private static void FillDate(out IDCardInfo cardInfo)
        {
            cardInfo = new IDCardInfo();
            StringBuilder sb = new StringBuilder(256);
            int len;
            //获取姓名
            GetPeopleName(sb, out len);
            cardInfo.Name = sb.ToString();
            //获取性别
            GetPeopleSex(sb, out len);
            cardInfo.Sex = sb.ToString().Substring(0, 1);
            //获取卡号
            GetPeopleIDCode(sb, out len);
            cardInfo.IDCode = sb.ToString();
            //获取民族
            GetPeopleNation(sb, out len);
            cardInfo.Nation = sb.ToString();
            //获取有效开始时间
            GetStartDate(sb, out len);
            cardInfo.StartDate = sb.ToString();
            //获取有效开始时间
            GetPeopleBirthday(sb, out len);
            cardInfo.Birthday = DateTime.Parse(sb.ToString());
            //获取有效结束时间
            GetEndDate(sb, out len);
            cardInfo.EndDate = sb.ToString();
            //获取发证机关信息
            GetDepartment(sb, out len);
            cardInfo.Department = sb.ToString();
            //获取地址
            GetPeopleAddress(sb, out len);
            cardInfo.Address = sb.ToString();
        }
        #endregion

    }

    /// <summary>
    /// 身份证信息类
    /// </summary>
    public class IDCardInfo
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string IDCode { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 发证机关信息 
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 有效起始日期
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 有效截止日期
        /// </summary>
        public string EndDate { get; set; }
    }
}
