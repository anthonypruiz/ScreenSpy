using System;
using System.Drawing;
using System.Drawing.Imaging;
namespace ScreenSpy
{
    class GreyScale : IDisposable
    {
        public static unsafe Bitmap ToGrayscale(Bitmap colorBitmap)
        {
            int width = colorBitmap.Width;
            int height = colorBitmap.Height;

            Bitmap grayscaleBitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            grayscaleBitmap.SetResolution(colorBitmap.HorizontalResolution,
                colorBitmap.VerticalResolution);

            ColorPalette colorPalette = grayscaleBitmap.Palette;
            for (int i = 0; i < colorPalette.Entries.Length; i++)
            {
                colorPalette.Entries[i] = Color.FromArgb(i, i, i);
            }
            grayscaleBitmap.Palette = colorPalette;

            BitmapData bitmapData = grayscaleBitmap.LockBits(
                new Rectangle(Point.Empty, grayscaleBitmap.Size),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            byte* pPixel = (byte*)bitmapData.Scan0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color clr = colorBitmap.GetPixel(x, y);

                    Byte byPixel = (byte)((30 * clr.R + 59 * clr.G + 11 * clr.B) / 100);

                    pPixel[x] = byPixel;
                }

                pPixel += bitmapData.Stride;
            }

            grayscaleBitmap.UnlockBits(bitmapData);

            return grayscaleBitmap;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
