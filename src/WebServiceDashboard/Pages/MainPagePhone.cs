using Spaniel.Shared.Services;
using Spaniel.ViewModels;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Spaniel.Pages
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