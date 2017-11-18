using WebServiceDashboard.Shared.Services;
using WebServiceDashboard.ViewModels;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace WebServiceDashboard.Pages
{
    public class MainPagePhone : ProjectListPage
    {
        public MainPagePhone(IDependencyService dependencyService)
        {
            Title = "Projects";
            BindingContext = new ProjectViewModel(dependencyService);

            // turn large navigation page titles on for this page. iOS 11 only.
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetLargeTitleDisplay(LargeTitleDisplayMode.Always);         }
    }
}