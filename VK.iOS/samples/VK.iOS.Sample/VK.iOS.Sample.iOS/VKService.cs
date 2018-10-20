using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using VK.iOS.Sample.iOS;
using VK.iOS.Sample.Models;
using VK.iOS.Sample.Services.Contracts;
using VKontakte;
using VKontakte.API;
using VKontakte.API.Methods;
using VKontakte.API.Models;
using VKontakte.Core;
using VKontakte.Views;
using Xamarin.Forms;

[assembly: Dependency(typeof(VKService))]
namespace VK.iOS.Sample.iOS
{
    public class VKService : NSObject, IVKService, IVKSdkDelegate, IVKSdkUIDelegate
    {
        readonly string[] _permission = {
            VKPermissions.Email,
            VKPermissions.Offline
        };
        UIViewController _viewController;

        Action<VKUserModel, string> _onLoginComplete;

        public VKService()
        {
            VKSdk.Instance.RegisterDelegate(this);
            VKSdk.Instance.UiDelegate = this;
        }

        public void Login(Action<VKUserModel, string> onLoginComplete)
        {
            _onLoginComplete = onLoginComplete;

            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            _viewController = vc;

            VKSdk.Authorize(_permission);
        }

        public void Logout()
        {
            VKSdk.ForceLogout();
        }

        public async void AccessAuthorizationFinished(VKAuthorizationResult result)
        {
            if (result?.Token == null)
                _onLoginComplete?.Invoke(null, result?.Error?.LocalizedDescription ?? "VK login unknown error");
            else
            {
                VKUserModel vkUserModel = new VKUserModel
                {
                    Email = result.Token.Email,
                    UserID = result.Token.UserId
                };

                await GetUserInfo(vkUserModel);
                _onLoginComplete?.Invoke(vkUserModel, string.Empty);
            }
        }

        async Task GetUserInfo(VKUserModel vkUserModel)
        {
            var request = VKApi.Users.Get(NSDictionary.FromObjectAndKey((NSString)@"photo_400_orig", VKApiConst.Fields));
            var response = await request.ExecuteAsync();
            var users = response.ParsedModel as VKUsersArray;
            var account = users?.FirstObject as VKUser;
            if (account != null)
            {
                vkUserModel.Name = account.first_name + " " + account.last_name;
                if (!string.IsNullOrEmpty(account.photo_400_orig))
                    vkUserModel.Picture = new Uri(account.photo_400_orig);
            }

            //Depends user image quality is the User profile of the VK account, the photo might be in different fields
            //var request100 = VKApi.Users.Get(NSDictionary.FromObjectAndKey((NSString)@"photo_100", VKApiConst.Fields));
            //var response100 = await request100.ExecuteAsync();
            //var account100 = (response100.ParsedModel as VKUsersArray)?.FirstObject as VKUser;

            //var request200 = VKApi.Users.Get(NSDictionary.FromObjectAndKey((NSString)@"photo_200", VKApiConst.Fields));
            //var response200 = await request200.ExecuteAsync();
            //var account200 = (response200.ParsedModel as VKUsersArray)?.FirstObject as VKUser;

            //var request201 = VKApi.Users.Get(NSDictionary.FromObjectAndKey((NSString)@"photo_200_orig", VKApiConst.Fields));
            //var response201 = await request201.ExecuteAsync();
            //var account201 = (response201.ParsedModel as VKUsersArray)?.FirstObject as VKUser;

            //var request50 = VKApi.Users.Get(NSDictionary.FromObjectAndKey((NSString)@"photo_50", VKApiConst.Fields));
            //var response50 = await request50.ExecuteAsync();
            //var account50 = (response50.ParsedModel as VKUsersArray)?.FirstObject as VKUser;

        }

        public void NeedCaptchaEnter(VKError captchaError)
        {
            if (_viewController != null)
                Device.BeginInvokeOnMainThread(() => VKCaptchaViewController.Create(captchaError)
                                               .PresentIn(_viewController));
        }

        public void ShouldPresentViewController(UIViewController controller)
        {
            Device.BeginInvokeOnMainThread(() => _viewController?.PresentViewController(controller, true, null));
        }

        public void UserAuthorizationFailed()
        {
            _onLoginComplete?.Invoke(null, "VK login unknown error");
        }
    }
}
