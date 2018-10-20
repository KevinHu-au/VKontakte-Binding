using System;
using Android.App;
using VK.Android.Sample.Droid;
using VK.Android.Sample.Models;
using VK.Android.Sample.Services.Contracts;
using VKontakte;
using Xamarin.Forms;

[assembly: Dependency(typeof(VKService))]
namespace VK.Android.Sample.Droid
{
    public class VKService : Java.Lang.Object, IVKService
    {
        readonly string[] _permission = {
            VKScope.Email,
            VKScope.Offline
        };

        public Action<VKUserModel, string> OnLoginComplete { get; set; }

        public VKService()
        {
            MainActivity.VKService = this;
        }

        public void Login(Action<VKUserModel, string> onLoginComplete)
        {
            OnLoginComplete = onLoginComplete;
            VKSdk.Login(Forms.Context as Activity, _permission);
        }

        public void Logout()
        {
            VKSdk.Logout();
        }
    }
}
