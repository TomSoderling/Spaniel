using WebServiceDashboard.Pages;
using WebServiceDashboard.Shared.Services;
using WebServiceDashboard.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace WebServiceDashboard.Pages
{
    public class MainPage : MasterDetailPage
    {
        public MainPage(IDependencyService dependencyService)
        {
            MasterBehavior = MasterBehavior.Split;

            var masterVM = new ProjectViewModel(dependencyService);
            var masterPage = new ProjectListPage() { BindingContext = masterVM };
            var masterNavPage = new Xamarin.Forms.NavigationPage(masterPage) 
            { 
                Title = "Projects", 
                BarBackgroundColor = SpanielColors.NavBarBackground,
                BarTextColor = SpanielColors.NavBarText
            };

            // set option to use large navigation page titles where desired on iOS 11
            masterNavPage.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersLargeTitles(true);

            Master = masterNavPage;


            var detailVM = new PlaceHolderPageViewModel();
            var detailPage = new PlaceholderPage() { BindingContext = detailVM };
            Detail = new Xamarin.Forms.NavigationPage(detailPage)
                {
                    BarBackgroundColor = SpanielColors.NavBarBackground,
                    //BarTextColor = Color.White // for iOS, this alllows the Status Bar text color (network, time, battery percentage) to be light colored, but also forces the Nav Bar text color to be white
                    BarTextColor = SpanielColors.NavBarText // for iOS, this shows the Navigation Bar text as the correct color (yellow), but the status bar text is dark colored
                };
        }
    }
}

