using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;
using VK.Android.Sample.Models;
using VK.Android.Sample.Services.Contracts;
using VKontakte;
using VKontakte.API;

namespace VK.Android.Sample.Droid
{
    [IntentFilter(new[] { Intent.ActionView }, DataScheme = "vk6724423", Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault })]
    [Activity(Label = "VK.Android.Sample", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static VKService VKService;
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(new AndroidInitializer()));
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            bool vkResult;
            var task = VKSdk.OnActivityResultAsync(requestCode, resultCode, data, out vkResult);

            if (!vkResult)
            {
                base.OnActivityResult(requestCode, resultCode, data);
                VKService.OnLoginComplete?.Invoke(null, "VK Login failed");
                return;
            }

            try
            {
                var token = await task;
                var vkUserModel = new VKUserModel()
                {
                    Email = token.Email,
                    UserID = token.UserId
                };

                await GetUserInfo(vkUserModel);
                VKService.OnLoginComplete?.Invoke(vkUserModel, string.Empty);
            }
            catch (VKException vkEx)
            {
                if (vkEx.Error.ErrorCode == VKError.VkCanceled)
                    VKService.OnLoginComplete?.Invoke(null, "Cancelled by User");
                else
                    VKService.OnLoginComplete?.Invoke(null, "VK login failed");
            }
            catch (Exception ex)
            {
                VKService.OnLoginComplete?.Invoke(null, "VK Login unknown error");
            }
        }

        async Task GetUserInfo(VKUserModel vkUserModel)
        {
            var request = VKApi.Users.Get(VKParameters.From(VKApiConst.Fields, @"photo_400_orig"));
            var response = await request.ExecuteAsync();
            var jsonArray = response.Json.OptJSONArray(@"response");
            var account = jsonArray?.GetJSONObject(0);
            vkUserModel.Name = account.OptString(@"first_name") + " " + account.OptString(@"last_name");
            if (!string.IsNullOrEmpty(account.OptString(@"photo_400_orig")))
                vkUserModel.Picture = new Uri(account.OptString(@"photo_400_orig"));
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }
}

