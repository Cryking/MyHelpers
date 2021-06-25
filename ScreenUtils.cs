using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace YFPos.Utils
{
    public class ScreenUtils
    {
        public static void CaptureScreen(string picPath)
        {
            if (!System.IO.Directory.Exists(picPath))
            {
                System.IO.Directory.CreateDirectory(picPath);
            }
            try
            {
                double Left = SystemParameters.VirtualScreenLeft;
                double Top = SystemParameters.VirtualScreenTop;
                double ScreenWidth = SystemParameters.VirtualScreenWidth;
                double ScreenHeight = SystemParameters.VirtualScreenHeight;

                using (var bmpScreen = new System.Drawing.Bitmap((int)ScreenWidth, (int)ScreenHeight))
                {
                    using (var graphic = System.Drawing.Graphics.FromImage(bmpScreen))
                    {
                        graphic.CopyFromScreen((int)Left, (int)Top, 0, 0, bmpScreen.Size);
                        bmpScreen.Save(System.IO.Path.Combine(picPath,
                            $"{DateTime.Now:yyyyMMddHHmmss}.bmp"));
                    }
                }
            }
            catch(Exception e)
            {
                e.SaveLog();
            }
        }
    }
}
