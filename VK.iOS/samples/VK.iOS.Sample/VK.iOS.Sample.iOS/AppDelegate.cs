using Foundation;
using Prism;
using Prism.Ioc;
using UIKit;
using VKontakte;

namespace VK.iOS.Sample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public const string VKAppID = "6724423";
        public const string VKSecureKey = "t9miwWMrryaD7p1CtH8I";

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            var sdk = VKSdk.Initialize(VKAppID);
            LoadApplication(new App(new iOSInitializer()));

            return base.FinishedLaunching(app, options);
        }


        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            if (VKSdk.ProcessOpenUrl(url, sourceApplication))
                return true;

            return base.OpenUrl(application, url, sourceApplication, annotation);
        }
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }
}
