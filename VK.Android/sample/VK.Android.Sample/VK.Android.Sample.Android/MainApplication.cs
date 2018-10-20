using System;
using Android.App;
using Android.Runtime;
using VKontakte;

namespace VK.Android.Sample.Droid
{
    /// <summary>
    /// Custom extension of the base Android.App.Application class, in order to add properties which
    /// are global to the application, but not persisted as settings.
    /// </summary>
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          :base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            VKSdk.Initialize(this);
        }
    }
}
