using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace YFPos.Utils
{
    public class WindowOwnerUtils
    {
        public static void SetWindowOwner(Window win, DependencyObject owner)
        {
            //问题修改：无法将 Owner 属性设置为已关闭的 Window
            HwndSource winformWindow = (HwndSource.FromDependencyObject(owner) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(win) { Owner = winformWindow.Handle };
            }
        }

        /// <summary>
        /// 多线程时,跨线程同步调用(UI线程去调)
        /// </summary>
        /// <param name="method"></param>
        public static void OverThreadProcess(Action method)
        {
            Application.Current.Dispatcher?.Invoke(method);
        }
    }
}
