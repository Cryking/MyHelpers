using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace YFPos.Utils
{
    public class ImageUtility
    {
        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="imgPath">要加载的图片路径</param>
        /// <returns></returns>
        public static BitmapImage LoadImage(string imgPath)
        {
            BitmapImage bitmapImage = null;
            try
            {
                if (!string.IsNullOrEmpty(imgPath) && File.Exists(imgPath))
                {
                    bitmapImage = new BitmapImage();
                    using (var fs = new FileStream(imgPath, FileMode.Open))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = fs;
                        bitmapImage.EndInit();
                    }
                    bitmapImage.Freeze();
                }
            }
            catch (Exception ex)
            {
                bitmapImage = null;
                ex.SaveLog($"图像读取失败:{imgPath}");
            }
            return bitmapImage;
        }

        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="imgPath">图像路径</param>
        /// <param name="cutRect"></param>
        /// <returns></returns>
        //public static System.Drawing.Bitmap CutImageInRectangle(string imgPath, System.Drawing.Rectangle cutRect)
        //{
        //    System.Drawing.Bitmap cutBitmap;
        //    using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imgPath))
        //    {
        //        cutBitmap = bitmap.Clone(cutRect, bitmap.PixelFormat);
        //    }
        //    return cutBitmap;
        //}

        /// <summary>
        /// base64编码的文本 转为   图片
        /// </summary>
        /// <param name="basestr">base64字符串</param>
        /// <returns>转换后的Bitmap对象</returns>
        public static string Base64StringToImage(string base64str,string fileName)
        {
            var filePath = "";
            try
            {
                byte[] arr = Convert.FromBase64String(base64str);
                using (MemoryStream ms = new MemoryStream(arr))
                {
                    Bitmap bmp = new Bitmap(ms);
                    bmp.Save(fileName, ImageFormat.Jpeg);
                }
                filePath = fileName;
            }
            catch (Exception ex)
            {
                ex.SaveLog();
            }

            return filePath;
        }

        public static Bitmap CutImageInRectangle(string imgPath, Rectangle cutRect,double ImgControlHeight,double ImgControlWidth)
        {
            using (var srcImg = new Bitmap(imgPath))
            {
                double xRatio = srcImg.Width / ImgControlWidth;
                double yRatio = srcImg.Height / ImgControlHeight;
                Rectangle destRect= new Rectangle(0, 0, (int)(cutRect.Width * xRatio), (int)(cutRect.Height * yRatio));
                Rectangle srcRect = new Rectangle((int)(cutRect.X * xRatio), (int)(cutRect.Y * yRatio), (int)(cutRect.Width * xRatio), (int)(cutRect.Height * yRatio));
                var bitmap = new Bitmap(destRect.Width, destRect.Height, PixelFormat.Format24bppRgb);
                bitmap.SetResolution(96, 96);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(srcImg, destRect, srcRect, GraphicsUnit.Pixel);
                }
                return bitmap;
            }           
        }

        ///// <summary>
        ///// 将位图按指定质量保存为JPG文件
        ///// </summary>
        ///// <param name="ImagePath">JPG文件完整文件名</param>
        ///// <param name="ImageSource">位图对象</param>
        ///// <param name="ImageQuality">质量参数</param>
        //private void SaveBitmap(string ImagePath, System.Drawing.Bitmap ImageSource, long ImageQuality)
        //{
        //    EncoderParameter parameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, ImageQuality);
        //    ImageCodecInfo encoder = this.getEncoderInfo("image/jpeg");
        //    if (encoder == null)
        //    {
        //        MessageBox.Show("系统找不到JPEG图像解码器！");
        //    }
        //    else
        //    {
        //        EncoderParameters encoderParams = new EncoderParameters(1);
        //        encoderParams.Param[0] = parameter;
        //        ImageSource.Save(ImagePath, encoder, encoderParams);
        //    }
        //}
    }
}
