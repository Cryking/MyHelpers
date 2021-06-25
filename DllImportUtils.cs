using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace YFPos.Utils
{
    public class DllImportUtils
    {

        private delegate int Transaction(string parameters, StringBuilder returnValue);


        public static string DllPath(string paymentModeCode)
        {
            var result = "";
            switch (paymentModeCode)
            {
                case "PC0"://江苏连云港
                    result = AppDomain.CurrentDomain.BaseDirectory + "Pay\\PC0\\SiInterface.dll";
                    break;
                case "PC6"://连云港灌云县
                    result = AppDomain.CurrentDomain.BaseDirectory + "Pay\\PC6\\SiInterface.dll";
                    break;
                case "PCZ"://宿迁特药
                    result = AppDomain.CurrentDomain.BaseDirectory + "Pay\\PCZ\\SiInterface.dll";
                    break;
                case "PC8"://江苏宿迁
                    result = AppDomain.CurrentDomain.BaseDirectory + "Pay\\PC8\\SiInterface.dll";
                    break;
                default:
                    break;
            }
            //result = AppDomain.CurrentDomain.BaseDirectory + "YB_JSLYG\\SiInterface.dll";
            var path = System.IO.Path.GetDirectoryName(result);
            Directory.SetCurrentDirectory(path); //AppDomain.CurrentDomain.BaseDirectory + "\\YB_JSLYG");//dll路径
            return result;
        }


        public static int Invoke(string inputData, out StringBuilder outputData, string paymentModeCode, string funName = "BUSINESS_HANDLE")
        {
            outputData = new StringBuilder();
            DllWrap myfun = new DllWrap();
            //StringBuilder outputSb = new StringBuilder(1024);
            try
            {
                LogHelper.WriteLog(LogCategorys.PAY, string.Format("医保请求：{0}", inputData));
                IntPtr MLib = myfun.LoadDll(DllPath(paymentModeCode)); //加载dll
                IntPtr api = myfun.LoadFun(funName); // 调入函数 BUSINESS_HANDLE
                Transaction _Business_handle = Marshal.GetDelegateForFunctionPointer(api, typeof(Transaction)) as Transaction;//非托管函数指针转换为委托
                int res = _Business_handle(inputData, outputData);//传参，调用dll中的函数
                //outputData = outputSb == null ? "" : outputSb.ToString();//出参
                // DllWrap.FreeLibrary(MLib);//释放
                LogHelper.WriteLog(LogCategorys.PAY, string.Format("医保返回：{0}", outputData.ToString()));
                return res;
            }
            catch (Exception ex)
            {
                ex.SaveLog("医保交易异常");
                outputData.AppendLine(ex.Message);
                return -1;
            }
        }

        public delegate int Initializ(StringBuilder returnValue);
        public static bool InvokeInit(out string outputData, string paymentModeCode)
        {
            outputData = "";
            DllWrap myfun = new DllWrap();
            StringBuilder outputSb = new StringBuilder(1024);
            try
            {
                IntPtr MLib = myfun.LoadDll(DllPath(paymentModeCode));
                IntPtr api = myfun.LoadFun("INIT");
                Initializ _Init = Marshal.GetDelegateForFunctionPointer(api, typeof(Initializ)) as Initializ;
                int res = _Init(outputSb);
                outputData = outputSb == null ? "" : outputSb.ToString();

                // DllWrap.FreeLibrary(MLib);
                return res == 0;
            }
            catch (Exception ex)
            {
                ex.SaveLog("医保初始化异常");
                outputData = ex.ToString();
                return false;
            }
        }
    }

    public sealed class DllWrap
    {
        /// <summary>
        /// 参数传递方式枚举 ,ByValue 表示值传递 ,ByRef 表示址传递
        /// </summary>
        public enum ModePass
        {
            ByValue = 0x0001,
            ByRef = 0x0002
        }
        /// <summary>
        /// 原型是 :HMODULE LoadLibrary(LPCTSTR lpFileName);
        /// </summary>
        /// <param name="lpFileName">DLL 文件名 </param>
        /// <returns> 函数库模块的句柄 </returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32.dll")]
        public static extern IntPtr SetCurrentDirectory(string lpFileName);

        /// <summary>
        /// 原型是 : FARPROC GetProcAddress(HMODULE hModule, LPCWSTR lpProcName);
        /// </summary>
        /// <param name="hModule"> 包含需调用函数的函数库模块的句柄 </param>
        /// <param name="lpProcName"> 调用函数的名称 </param>
        /// <returns> 函数指针 </returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        /// <summary>
        /// 原型是 : BOOL FreeLibrary(HMODULE hModule);
        /// </summary>
        /// <param name="hModule"> 需释放的函数库模块的句柄 </param>
        /// <returns> 是否已释放指定的 Dll</returns>
        [DllImport("kernel32", EntryPoint = "FreeLibrary", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// Loadlibrary 返回的函数库模块的句柄
        /// </summary>
        private IntPtr hModule = IntPtr.Zero;
        /// <summary>
        /// GetProcAddress 返回的函数指针
        /// </summary>
        private IntPtr farProc = IntPtr.Zero;
        /// <summary>
        /// 装载 Dll
        /// </summary>
        /// <param name="lpFileName">DLL 文件名 </param>
        public IntPtr LoadDll(string lpFileName)
        {
            SetCurrentDirectory(Path.GetDirectoryName(lpFileName));
            hModule = LoadLibrary(lpFileName);
            //if (hModule == IntPtr.Zero)
            //{
            //    hModule = LoadLibraryEx(lpFileName);
            //}
            if (hModule == IntPtr.Zero)
            {
                var errMsg = string.Format("没有找到:{0}", lpFileName);
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory.ToLower();
                var path = System.IO.Path.GetDirectoryName(lpFileName).ToLower();

                if (baseDirectory.Equals(path))
                {
                    errMsg += "请确实是否有少部分dll需要部署到" + AppDomain.CurrentDomain.BaseDirectory + "？";
                }
                throw (new Exception(errMsg));
            }
            return hModule;
        }

        //public IntPtr LoadDll(IntPtr HMODULE)
        //{
        //    if (HMODULE == IntPtr.Zero)
        //        throw (new Exception(" 所传入的函数库模块的句柄 HMODULE 为空 ."));
        //    hModule = HMODULE;
        //    return hModule;
        //}

        /// <summary>
        /// 获得函数指针
        /// </summary>
        /// <param name="lpProcName"> 调用函数的名称 </param>
        public IntPtr LoadFun(string lpProcName)
        { // 若函数库模块的句柄为空，则抛出异常
            if (hModule == IntPtr.Zero)
                throw (new Exception(" 函数库模块的句柄为空 , 请确保已进行 LoadDll 操作 !"));
            // 取得函数指针
            farProc = GetProcAddress(hModule, lpProcName);
            // 若函数指针，则抛出异常
            if (farProc == IntPtr.Zero)
                throw (new Exception(" 没有找到 :" + lpProcName + " 这个函数的入口点 "));
            return farProc;
        }

        /// <summary>
        /// 卸载 Dll
        /// </summary>
        public void UnLoadDll()
        {
            FreeLibrary(hModule);
            hModule = IntPtr.Zero;
            farProc = IntPtr.Zero;
        }

        /// <summary>
        /// 调用所设定的函数
        /// </summary>
        /// <param name="ObjArray_Parameter"> 实参 </param>
        /// <param name="TypeArray_ParameterType"> 实参类型 </param>
        /// <param name="ModePassArray_Parameter"> 实参传送方式 </param>
        /// <param name="Type_Return"> 返回类型 </param>
        /// <returns> 返回所调用函数的 object</returns>
        public object Invoke(object[] ObjArray_Parameter, Type[] TypeArray_ParameterType, ModePass[] ModePassArray_Parameter, Type Type_Return)
        {
            // 下面 3 个 if 是进行安全检查 , 若不能通过 , 则抛出异常
            if (hModule == IntPtr.Zero)
                throw (new Exception(" 函数库模块的句柄为空 , 请确保已进行 LoadDll 操作 !"));
            if (farProc == IntPtr.Zero)
                throw (new Exception(" 函数指针为空 , 请确保已进行 LoadFun 操作 !"));
            if (ObjArray_Parameter.Length != ModePassArray_Parameter.Length)
                throw (new Exception(" 参数个数及其传递方式的个数不匹配 ."));
            // 下面是创建 MyAssemblyName 对象并设置其 Name 属性
            AssemblyName MyAssemblyName = new AssemblyName();
            MyAssemblyName.Name = "InvokeFun";
            // 生成单模块配件
            AssemblyBuilder MyAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(MyAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder MyModuleBuilder = MyAssemblyBuilder.DefineDynamicModule("InvokeDll");
            // 定义要调用的方法 , 方法名为“ MyFun ”，返回类型是“ Type_Return ”参数类型是“ TypeArray_ParameterType ”
            MethodBuilder MyMethodBuilder = MyModuleBuilder.DefineGlobalMethod("MyFun", MethodAttributes.Public | MethodAttributes.Static, Type_Return, TypeArray_ParameterType);
            // 获取一个 ILGenerator ，用于发送所需的 IL
            ILGenerator IL = MyMethodBuilder.GetILGenerator();
            int i;
            for (i = 0; i < ObjArray_Parameter.Length; i++)
            {// 用循环将参数依次压入堆栈
                switch (ModePassArray_Parameter[i])
                {
                    case ModePass.ByValue:
                        IL.Emit(OpCodes.Ldarg, i);
                        break;
                    case ModePass.ByRef:
                        IL.Emit(OpCodes.Ldarga, i);
                        break;
                    default:
                        throw (new Exception(" 第 " + (i + 1).ToString() + " 个参数没有给定正确的传递方式 ."));
                }
            }
            if (IntPtr.Size == 4)
            {// 判断处理器类型
                IL.Emit(OpCodes.Ldc_I4, farProc.ToInt32());
            }
            else if (IntPtr.Size == 8)
            {
                IL.Emit(OpCodes.Ldc_I8, farProc.ToInt64());
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
            IL.EmitCalli(OpCodes.Calli, CallingConvention.StdCall, Type_Return, TypeArray_ParameterType);
            IL.Emit(OpCodes.Ret); // 返回值
            MyModuleBuilder.CreateGlobalFunctions();
            // 取得方法信息
            MethodInfo MyMethodInfo = MyModuleBuilder.GetMethod("MyFun");
            return MyMethodInfo.Invoke(null, ObjArray_Parameter);// 调用方法，并返回其值
        }
    }
}
