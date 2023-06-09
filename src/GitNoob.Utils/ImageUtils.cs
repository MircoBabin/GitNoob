using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace GitNoob.Utils
{
    public static class ImageUtils
    {
        public static Bitmap LoadImageAsBitmap(string imageFilename, int width, int height, Color background)
        {
            Bitmap bitmap = null;
            try
            {
                if (System.IO.File.Exists(imageFilename))
                {
                    var image = System.Drawing.Image.FromFile(imageFilename);
                    return ImageToBitmapOfSize(image, width, height, background);
                }
            }
            catch { }

            if (bitmap == null)
            {
                bitmap = CreateEmptyBitmap(width, height, background);
            }

            return bitmap;
        }

        public static Bitmap ImageToBitmapOfSize(Image image, int width, int height, Color background)
        {
            int originalWidth = image.Size.Width;
            int originalHeight = image.Size.Height;

            if (originalWidth == width && originalHeight == height)
            {
                return new Bitmap(image);
            }

            if (originalWidth > width || originalHeight > height)
            {
                //shrink
                float ratioWidth = (float)width / (float)originalWidth;
                float ratioHeight = (float)height / (float)originalHeight;
                float ratio = Math.Min(ratioWidth, ratioHeight);
                int resizedWidth = Convert.ToInt32(originalWidth * ratio);
                int resizedHeight = Convert.ToInt32(originalHeight * ratio);

                int posX = 0;
                int posY = 0;
                if (resizedWidth < width)
                    posX = Convert.ToInt32((float)(width - resizedWidth) / 2f);
                else
                    posY = Convert.ToInt32((float)(height - resizedHeight) / 2f);

                Bitmap resized = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                resized.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(resized))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    graphics.Clear(background);

                    using (var attributes = new ImageAttributes())
                    {
                        attributes.SetWrapMode(WrapMode.TileFlipXY);

                        graphics.DrawImage(image,
                                    new Rectangle(posX, posY, resizedWidth, resizedHeight),
                                    0, 0, originalWidth, originalHeight,
                                    GraphicsUnit.Pixel, attributes);
                    }
                }

                return resized;
            }
            else
            {
                //position in middle with background
                int posX = Convert.ToInt32((float)(width - originalWidth) / 2f);
                int posY = Convert.ToInt32((float)(height - originalHeight) / 2f);

                Bitmap resized = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                resized.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                using (var graphics = Graphics.FromImage(resized))
                {
                    graphics.Clear(background);

                    graphics.DrawImage(image, new Point(posX, posY));
                }

                return resized;
            }
        }

        public static Icon LoadIconForFile(string filename)
        {
            try
            {
                string iconfilename = FileUtils.GetIconFilenameFor(filename);
                if (Path.GetExtension(iconfilename).ToLowerInvariant() == ".ico")
                {
                    return new Icon(filename);
                }

                var extractor = new IconExtractor.IconExtractor(iconfilename);
                return extractor.GetIcon(0);
            }
            catch { }

            return null;
        }

        public static Bitmap IconToBitmapOfSize(Icon icon, int width, int height, Color background)
        {
            if (icon == null)
            {
                return CreateEmptyBitmap(width, height, background);
            }

            var sized = new Icon(icon, new Size(width, height));
            return sized.ToBitmap();
        }

        private static Bitmap CreateEmptyBitmap(int width, int height, Color background)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(background);
            }

            return bitmap;
        }
    }
}
