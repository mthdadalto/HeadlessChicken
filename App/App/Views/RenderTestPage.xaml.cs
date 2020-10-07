using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using App.Render;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RenderTestPage : ContentPage
    {
        public RenderTestPage()
        {
            InitializeComponent();
            SwipeGestureRecognizer Event = new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Down,
                Threshold = 20
            };
            Event.Swiped += (o, e) => UpdateRender();
            RenderBody.GestureRecognizers.Add(Event);

            render = DependencyService.Get<IGetRender>().GetRender();
        }
        IRenderBase render;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateRender();
        }

        void UpdateRender() => App.RunOnMainThread(() =>
        {
            try
            {

                Stopwatch watch = new Stopwatch();
                StringBuilder log = new StringBuilder("Log:");
                Logger.Added += (o, e) => { log.AppendLine(e.Message); if (e.Exception != null) { log.AppendLine(e.Exception.Message); log.AppendLine(e.Exception.StackTrace); } };
                StringBuilder sb = new StringBuilder("Times:");

                byte[] vbo = Helpers.GetAssetByteArray("App.vbo.raw");

                IntPtr source = Marshal.AllocHGlobal(vbo.Length);
                Marshal.Copy(vbo, 0, source, vbo.Length);

                float[] dest = new float[vbo.Length / 4];
                Marshal.Copy(source, dest, 0, dest.Length);
                Marshal.FreeHGlobal(source);

                render.UpdateConfigs(RenderConfig.Default);

                watch.Start();
                byte[] res = render.VboToPng(dest, dest.Length / 24, false);
                watch.Stop();

                sb.AppendLine("Convert to PNG: " + watch.ElapsedMilliseconds + "ms");


                StackLayout stack = new StackLayout() { Orientation = StackOrientation.Vertical };
                stack.Children.Add(new Image
                {
                    Source = ImageSource.FromStream(() => new MemoryStream(res)),
                    WidthRequest = 250,
                    HeightRequest = 200,
                    Margin = 5
                });
                sb.AppendLine("PNG to ImageSource: " + watch.ElapsedMilliseconds + "ms " + "Size:" + res?.Length);

                stack.Children.Add(new Label { Text = sb.ToString() });

                stack.Children.Add(new Label { Text = log.ToString(), FontSize = 8 });

                RenderBody.Content = stack;
                Debug.WriteLine(log);
                Debug.WriteLine(sb);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("UpdateRender:" + ex.Message);
            }
        });
    }


}
