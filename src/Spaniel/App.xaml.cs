using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Spaniel.Pages;
using Spaniel.Services;
using Spaniel.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;


[assembly: XamlCompilation(XamlCompilationOptions.Compile)] // use compiled XAML for the entire app
namespace Spaniel
{
    public partial class App : Application
    {
        static App()
        {
            // Register app dependencies
            DependencyService.Register<NavigationService>();
            DependencyService.Register<MessageVisualizerService>();
            DependencyService.Register<ActionSheetVisualizerService>();
        }

        public App()
        {
            InitializeComponent();


            // Start the VS App Center SDK
            #if !DEBUG
                AppCenter.Start(ApiKeys.AppCenterAppSecret_iOS + ApiKeys.AppCenterAppSecret_Android, typeof(Analytics), typeof(Crashes));
            #endif


            // This may not be good practice - it's only here to help me get the iOS 3D touch quick actions to work
            DependencyServiceWrapperProperty = new DependencyServiceWrapper();

            // Display App as Master/Detail only on non-phone devices
            if (Xamarin.Forms.Device.Idiom == TargetIdiom.Phone)
            {
                // pass the dependency service into the Main page
                //var phoneMainPage = new MainPagePhone(new DependencyServiceWrapper()); // before DependencyServiceWrapperProperty
                var phoneMainPage = new MainPagePhone(DependencyServiceWrapperProperty);

                var navPage = new Xamarin.Forms.NavigationPage(phoneMainPage)
                {
                    BarBackgroundColor = SpanielColors.NavBarBackground,
                    BarTextColor = SpanielColors.NavBarText
                };

                // set option to use large navigation page titles where desired on iOS 11
                navPage.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersLargeTitles(true);

                MainPage = navPage;
            }
            else
            {
                // pass the dependency service into the Main page
                //MainPage = new MainPage(new DependencyServiceWrapper()); // before DependencyServiceWrapperProperty
                MainPage = new MainPage(DependencyServiceWrapperProperty);
            }
        }

        // This may not be good practice - it's only here to help me get the iOS 3D touch quick actions to work
        public static DependencyServiceWrapper DependencyServiceWrapperProperty { get; set; }


        protected override void OnSleep()
        {
            base.OnSleep();

            // Clean up response bodies when app is backgrounded
            if (Xamarin.Forms.Device.Idiom == TargetIdiom.Tablet)
            {
                // Look at the Master page for the ViewModel in the BindingContext
                var mainPage = Application.Current.MainPage as MasterDetailPage;
                var navPage = mainPage.Master as Xamarin.Forms.NavigationPage;
                var bc = navPage.CurrentPage.BindingContext;

                // based on the ViewModel found, execute the appropriate cleanup command
                if (bc.GetType() == typeof(EndPointViewModel))
                {
                    ((EndPointViewModel)bc).CleanUpResponseBodies.Execute(null);
                }
                else if (bc.GetType() == typeof(ProjectViewModel))
                {
                    ((ProjectViewModel)bc).CleanUpResponseBodies.Execute(null);
                }
            }
            else if (Xamarin.Forms.Device.Idiom == TargetIdiom.Phone)
            {
                var navPage = Application.Current.MainPage as Xamarin.Forms.NavigationPage;
                var bc = navPage.CurrentPage.BindingContext;

                if (bc.GetType() == typeof(EndPointViewModel))
                {
                    ((EndPointViewModel)bc).CleanUpResponseBodies.Execute(null);
                }
                else if (bc.GetType() == typeof(ProjectViewModel))
                {
                    ((ProjectViewModel)bc).CleanUpResponseBodies.Execute(null);
                }
            }
                
        }

        protected override void OnResume()
        {
            base.OnResume();

            // TODO: is there a way to Track how many Projects and Endpoints here?
        }
    }
}

