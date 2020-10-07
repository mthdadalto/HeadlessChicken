using System;
using System.IO;
using System.Reflection;


namespace App
{
    public static class Helpers
    {

        static IProcessImage _imageProcessing = new ProcessImageSkiaSharpImplementation();//= null;
        public static IProcessImage ImageProcessing { get { if (_imageProcessing == null) { throw new NotImplementedException("Implementation for Helpers.ImageProcessing is missing."); } return _imageProcessing; } set => _imageProcessing = value; }


        /// <summary>
        /// Transfrorms mm into Fractional 
        /// </summary>
        /// <param name="value">mm</param>
        /// <returns>Inch fraction of 32</returns>
        public static int MmToInchFractionOf32(double value)
        {
            return (int)Math.Round(value / 25.4d * 32d);
        }

        /// <summary>
        /// Transfrorms mm into Fractional 
        /// </summary>
        /// <param name="value">mm</param>
        /// <returns>Inch fraction of 32</returns>
        public static string MmToInchFractionOf32Text(double value)
        {
            return ((int)Math.Round(value / 25.4d * 32d)) + "/32";
        }

        /// <summary>
        /// Transfrorms Fractional Inch into mm
        /// </summary>
        /// <param name="value">Fractional Inch</param>
        /// <returns>mm result</returns>
        public static double InchFractionOf32ToMm(int value)
        {
            return (double)Math.Round((decimal)(value * 25.4d / 32d), 1);
        }

        #region Assets Remote
        public static string[] GetAssetList(this Assembly assembly) => assembly.GetManifestResourceNames();

        public static Stream GetAssetStream(this Assembly assembly, string name) => assembly.GetManifestResourceStream(name);

        public static byte[] GetAssetByteArray(this Assembly assembly, string name)
        {
            using (Stream stream = GetAssetStream(assembly, name))
            {
                byte[] buf = new byte[(int)stream.Length];
                stream.Read(buf, 0, (int)stream.Length);
                return buf;
            }
        }
        public static string GetAssetText(this Assembly assembly, string name)
        {
            using (StreamReader stream = new StreamReader(GetAssetStream(assembly, name)))
            {
                return stream.ReadToEnd();
            }
        }
        #endregion

        #region Assets Local
        public static Stream GetAssetStream(string name) => Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        public static byte[] GetAssetByteArray(string name)
        {
            using (Stream stream = GetAssetStream(name))
            {
                byte[] buf = new byte[(int)stream.Length];
                stream.Read(buf, 0, (int)stream.Length);
                return buf;
            }
        }

        public static string GetAssetText(string name)
        {
            using (StreamReader stream = new StreamReader(GetAssetStream(name)))
            {
                return stream.ReadToEnd();
            }
        }
        #endregion
    }
}
