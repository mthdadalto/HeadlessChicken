using System;

using SkiaSharp;

namespace App
{
    /// <summary>
    /// ProcessImage SkiaSharp 2.80.x Implementation
    /// </summary>
    public class ProcessImageSkiaSharpImplementation : IProcessImage
    {
        public ProcessImage PngToJpeg(ProcessImage source)
        {
            if(source.Type != ProcessImageType.PNG) { throw new FormatException("Process image should be Png"); }
            SKImage tmp = SKImage.FromBitmap(SKBitmap.Decode(source.Load));
            return new ProcessImage { Load = tmp.Encode(SKEncodedImageFormat.Jpeg, 95).ToArray(), Type = ProcessImageType.JPEG, Width = tmp.Width, Height = tmp.Height };
        }

        public ProcessImage PngToRgba8(ProcessImage source)
        {
            if (source.Type != ProcessImageType.PNG) { throw new FormatException("Process image should be Png"); }
            SKBitmap bmp = SKBitmap.Decode(source.Load);
            return new ProcessImage { Load = bmp.Bytes, Height = bmp.Height, Type = ProcessImageType.RGBA8, Width = bmp.Width };
        }

        public ProcessImage Rgba16ToPng(ProcessImage source, bool flipHorizontal, bool flipVertical)
        {
            if (source.Type != ProcessImageType.RGBA16) { throw new FormatException("Process image should be RGBA16"); }
            using SKImage img = SKImage.FromPixelCopy(new SKImageInfo(source.Width, source.Height, SKColorType.Rgba16161616), source.Load);
            using SKBitmap bmp = new SKBitmap(img.Width, img.Height);
            using SKCanvas surface = new SKCanvas(bmp);
            surface.Scale(flipHorizontal ? -1 : 1, flipVertical ? -1 : 1, flipHorizontal ? source.Width / 2f : 0, flipVertical ? source.Height / 2f : 0);
            surface.DrawImage(img, 0, 0);

            return new ProcessImage { Load = SKImage.FromBitmap(bmp).Encode(SKEncodedImageFormat.Png, 99).ToArray(), Height = source.Height, Type = ProcessImageType.PNG, Width = source.Width };
        }

        public ProcessImage Rgba32ToPng(ProcessImage source, bool flipHorizontal, bool flipVertical)
        {
            if (source.Type != ProcessImageType.RGBA32) { throw new FormatException("Process image should be RGBA32"); }
            using SKImage img = SKImage.FromPixelCopy(new SKImageInfo(source.Width, source.Height, SKColorType.RgbaF32), source.Load);
            using SKBitmap bmp = new SKBitmap(img.Width, img.Height);
            using SKCanvas surface = new SKCanvas(bmp);
            surface.Scale(flipHorizontal ? -1 : 1, flipVertical ? -1 : 1, flipHorizontal ? source.Width / 2f : 0, flipVertical ? source.Height / 2f : 0);
            surface.DrawImage(img, 0, 0);

            return new ProcessImage { Load = SKImage.FromBitmap(bmp).Encode(SKEncodedImageFormat.Png, 99).ToArray(), Height = source.Height, Type = ProcessImageType.PNG, Width = source.Width };
        }

        public ProcessImage Rgba8ToPng(ProcessImage source, bool flipHorizontal, bool flipVertical)
        {
            if (source.Type != ProcessImageType.RGBA8) { throw new FormatException("Process image should be RGBA8"); }
            using SKImage img = SKImage.FromPixelCopy(new SKImageInfo(source.Width, source.Height, SKColorType.Rgba8888), source.Load);
            using SKBitmap bmp = new SKBitmap(img.Width, img.Height);
            using SKCanvas surface = new SKCanvas(bmp);
            surface.Scale(flipHorizontal ? -1 : 1, flipVertical ? -1 : 1, flipHorizontal ? source.Width / 2f : 0, flipVertical ? source.Height / 2f : 0);
            surface.DrawImage(img, 0, 0);
            return new ProcessImage { Load = SKImage.FromBitmap(bmp).Encode(SKEncodedImageFormat.Png, 99).ToArray(), Height = source.Height, Type = ProcessImageType.PNG, Width = source.Width };
        }
    }
}
