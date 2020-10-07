using System;
using System.Diagnostics;

using Veldrid;
using App.Render;
using App.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(RenderBase))]
namespace App.Droid
{
    public class RenderBase : IGetRender
    {

        public IRenderBase GetRender()
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                try
                {
                    if (GraphicsDevice.IsBackendSupported(GraphicsBackend.Vulkan))
                    {
                        return VeldridRender.InitFromVulkan();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            return null;//new Droid.Render.RenderDriver();//old openGL compat
        }

    }

}