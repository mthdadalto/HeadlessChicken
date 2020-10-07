using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.iOS;
using App.Render;
using Foundation;
using Metal;
using UIKit;
using Veldrid;

[assembly: Xamarin.Forms.Dependency(typeof(RenderBase))]
namespace App.iOS
{
    public class RenderBase : IGetRender
    {

        public IRenderBase GetRender()
        {
            MetalInfo();
            TaskCompletionSource<VeldridRender> tcs = new TaskCompletionSource<VeldridRender>();
            App.RunOnMainThread(() =>
            {
                //GPU won't always load without these
                Veldrid.MetalBindings.MTLDevice.MTLCreateSystemDefaultDevice();
                MTLDevice.SystemDefault.Dispose();

                //Thread.Sleep(50);

                UIView view = new UIView();
                SwapchainSource scs = SwapchainSource.CreateUIView(new UIView().Handle);//Apparently necessary to not native crash when creating Color buffer on A10 processors(A12 won't need)
                //SwapchainSource scs = SwapchainSource.CreateUIView(UIApplication.SharedApplication.KeyWindow.RootViewController.View.Handle);//Apparently necessary to not native crash when creating Color buffer on A10 processors(A12 won't need)

                //UIApplication.SharedApplication.KeyWindow.Add(view);
                /*
                                Thread.Sleep(50);

                                try
                                {
                                    ViewPtr = UIApplication.SharedApplication.Windows[0].InputViewController.View.Handle;
                                }
                                catch { }*/

                // IntPtr Handler = UIApplication.SharedApplication.Windows[0].RootViewController.View.Handle;


                //tcs.TrySetResult(VeldridRender.InitFromMetal(AppDelegate.RootViewControllerHandle));
                tcs.SetResult(VeldridRender.InitFromMetal(scs));
            });

            var render = tcs.Task.Result;

            MetalInfo();
            return render;
        }
        public void MetalInfo()
        {
            //MTLDevice.SystemDefault.Dispose();
            Debug.WriteLine("IOS GPU " + MTLDevice.SystemDefault.Name);
            //Debug.WriteLine("IOS GPU AllocHeap:" + MTLDevice.SystemDefault.CreateHeap(new MTLHeapDescriptor() { Size = 5000000, CpuCacheMode = MTLCpuCacheMode.DefaultCache }).GetCurrentAllocatedSize());
            Debug.WriteLine("IOS GPU GetCurrentAllocatedSize:" + MTLDevice.SystemDefault.GetCurrentAllocatedSize());
            Debug.WriteLine("IOS GPU GetMaxBufferLength:" + MTLDevice.SystemDefault.GetMaxBufferLength());
        }

    }
}