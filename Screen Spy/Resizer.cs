using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ScreenSpy
{
    class Resizer
    {
        internal void Resizing(string fullpath, int w, int h)
        {
            Bitmap Resize(Image image)
            {
                using (Bitmap bmp = new Bitmap(w, h))
                {
                    int width = image.Width;
                    int height = image.Height;
                    using (Graphics graphic = Graphics.FromImage(bmp))
                    {
                        graphic.SmoothingMode = SmoothingMode.HighQuality;
                        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphic.DrawImage(image, 0, 0, w, h);
                    }
                    return GreyScale.ToGrayscale(bmp);
                    //return bmp;
                }
            }
            try
            {
                byte[] bytes = File.ReadAllBytes(fullpath);
                using (MemoryStream stream = new MemoryStream(bytes))
                using (Bitmap image = new Bitmap(stream))
                using (Bitmap resized = Resize(image))
                    resized.Save(fullpath, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}
