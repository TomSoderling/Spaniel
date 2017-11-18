using Xamarin.UITest;
using WebServiceDashboard.Shared;

namespace WebServiceDashboard.UITests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            
            if (platform == Platform.Android)
            {
                #if DEBUG
                    // for debug builds, allow test automation on devices
                    return ConfigureApp.Android.ApiKey(Constants.TestCloudApiKey).StartApp();
                #else
                    return ConfigureApp.Android.StartApp();
                #endif
            }

            #if DEBUG
                // for debug builds, allow test automation on devices
                return ConfigureApp.iOS.ApiKey(Constants.TestCloudApiKey).StartApp();
            #else
                return ConfigureApp.iOS.StartApp();
            #endif
        }
    }
}

