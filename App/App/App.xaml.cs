using System;
using Xamarin.Forms;
using App.Views;
using Xamarin.Essentials;

namespace App
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            MainPage = new RenderTestPage();
        }

        public static void RunOnMainThread(Action action)
        {
            if (MainThread.IsMainThread)
            {
                action.Invoke();
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(action);
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
