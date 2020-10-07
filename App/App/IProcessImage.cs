
namespace App
{
    /// <summary>
    /// Platform dependand image processing
    /// 
    /// SixLabors.ImageSharp 1.0.1
    /// Image img = Image.LoadPixelData<SixLabors.ImageSharp.PixelFormats.RgbaVector>(tmp, (int)_offscreenReadOut.Width, (int)_offscreenReadOut.Height);
    /// using (MemoryStream ms = new MemoryStream())
    /// {
    ///     if (flipHorizontal) { img.Mutate(x => x.Flip(FlipMode.Horizontal)); }
    ///     if (FlipVertical) { img.Mutate(x => x.Flip(FlipMode.Vertical)); }
    ///     img.SaveAsPng(ms);
    ///     return ms.ToArray();
    /// }
    /// 
    /// SkiaSharp 2.8
    /// using (SKImage img = SKImage.FromPixelCopy(new SKImageInfo((int)_offscreenReadOut.Width, (int)_offscreenReadOut.Height, SKColorType.Rgba8888), tmp))
    /// {
    ///     using (SKBitmap bmp = new SKBitmap(img.Width, img.Height))
    ///     {
    ///         using (SKCanvas surface = new SKCanvas(bmp))
    ///         {
    ///             surface.Scale(flipHorizontal? -1 : 1, FlipVertical? -1 : 1, flipHorizontal? _offscreenReadOut.Width / 2f : 0, FlipVertical? _offscreenReadOut.Height / 2f : 0);
    ///             surface.DrawImage(img, 0, 0);
    ///             return SKImage.FromBitmap(bmp).Encode(SKEncodedImageFormat.Png, 99).ToArray();
    ///         }
    ///     }
    /// }
    /// </summary>
    public interface IProcessImage
    {
        /// <summary>
        /// Convert pixelmap(byte array) RGBA 32 float/vector to a PNG image(byte array)
        /// </summary>
        /// <param name="source">A RGBA 32 Process Image</param>
        /// <param name="flipHorizontal">Flip Horizontal</param>
        /// <param name="flipVertical">Flip Vertical</param>
        /// <returns>A PNG Process Image</returns>
        ProcessImage Rgba8ToPng(ProcessImage source, bool flipHorizontal = false, bool flipVertical = false);

        /// <summary>
        /// Convert pixelmap(byte array) RGBA 16 float/vector to a PNG image(byte array)
        /// </summary>
        /// <param name="source">A RGBA 16 Process Image</param>
        /// <param name="flipHorizontal">Flip Horizontal</param>
        /// <param name="flipVertical">Flip Vertical</param>
        /// <returns>A PNG Process Image</returns>
        ProcessImage Rgba16ToPng(ProcessImage source, bool flipHorizontal = false, bool flipVertical = false);

        /// <summary>
        /// Convert pixelmap(byte array) RGBA 32 float/vector to a PNG image(byte array)
        /// </summary>
        /// <param name="source">A RGBA 32 Process Image</param>
        /// <param name="flipHorizontal">Flip Horizontal</param>
        /// <param name="flipVertical">Flip Vertical</param>
        /// <returns>A PNG Process Image</returns>
        ProcessImage Rgba32ToPng(ProcessImage source, bool flipHorizontal = false, bool flipVertical = false);

        /// <summary>
        /// Convert PNG image to JPEG image
        /// </summary>
        /// <param name="source">A PNG Process Image</param>
        /// <returns>A JPEG Process Image</returns>
        ProcessImage PngToJpeg(ProcessImage source);

        /// <summary>
        ///  Convert PNG image(byte array) to pixelmap(byte array) RGBA 32 float/vector 
        /// </summary>
        /// <param name="source">A PNG Process Image</param>
        /// <returns>A RGBA 8 Process Image</returns>
        ProcessImage PngToRgba8(ProcessImage source);

    }
    public class ProcessImage
    {
        public ProcessImage() { }
        public ProcessImage(byte[] load, ProcessImageType type) { Load = load; Type = type; }
        public ProcessImage(byte[] load, ProcessImageType type, int width, int height) : this(load, type) { Width = width; Height = height; }

        public ProcessImageType Type;
        public byte[] Load;

        public int Width;
        public int Height;
    }
    public enum ProcessImageType { RGBA32, RGBA16, RGBA8, PNG, JPEG }
}
