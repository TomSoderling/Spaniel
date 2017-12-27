using System;
using System.Threading.Tasks;
using Spaniel.Pages;
using Spaniel.Shared.Models;
using Spaniel.Shared.Services;
using Spaniel.ViewModels;
using Xamarin.Forms;

namespace Spaniel.Services
{
    /// <summary>
    /// Implementation of a lightweight NavigationService for Xamarin.Forms project
    /// </summary>
    public class NavigationService : INavigationService
    {
        /// <summary>
        /// Based on the AppPage specified, navigate to the appropriate app page
        /// </summary>
        public async Task GoToPageAsync(AppPage page, BaseModel selectedModel = null, IDependencyService dependencyService = null, BaseModel otherSelectedModel = null)
        {
            if (Application.Current.MainPage != null) // don't navigate anywhere until the MainPage is set
            {
                switch (page)
                {
                    case AppPage.ProjectListPage:

                        if (Device.Idiom == TargetIdiom.Phone)
                        {
                            await Application.Current.MainPage.Navigation.PopToRootAsync();
                        }
                        else
                        {
                            var masterDetailPage = (MasterDetailPage)Application.Current.MainPage;
                            masterDetailPage.IsPresented = true;
                        }
                        break;

                    case AppPage.ResultDetailPage:

                        var resultVM = new ResultViewModel((Result)selectedModel); // create instance of the ViewModel
                        var resultDetailPage = new ResultDetailPage() { BindingContext = resultVM }; // set the Binding Context here

                        if (Device.Idiom == TargetIdiom.Phone)
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(resultDetailPage);
                        }
                        else
                        {
                            var masterDetailPage = (MasterDetailPage)Application.Current.MainPage;
                            await masterDetailPage.Detail.Navigation.PushAsync(resultDetailPage);
                        }
                        break;

                    case AppPage.ProjectDetailPage:

                        var projectVM = new ProjectViewModel(dependencyService, (Project)selectedModel);
                        var projectDetailPage = new ProjectDetailPage() { BindingContext = projectVM };

                        if (Device.Idiom == TargetIdiom.Phone)
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(projectDetailPage);
                        }
                        else
                        {
                            var masterDetailPage = (MasterDetailPage)Application.Current.MainPage;
                            masterDetailPage.Master.Title = "Projects";
                            NavigationPage.SetHasBackButton(projectDetailPage, false); // hide the back button

                            await masterDetailPage.Detail.Navigation.PushAsync(projectDetailPage);
                        }
                        break;

                    case AppPage.EndPointListPage:

                        var endPointVM = new EndPointViewModel(dependencyService, (Project)selectedModel);
                        var endPointListPage = new EndPointListPage() { BindingContext = endPointVM };

                        if (Device.Idiom == TargetIdiom.Phone)
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(endPointListPage);
                        }
                        else
                        {
                            var masterDetailPage = (MasterDetailPage)Application.Current.MainPage;
                            masterDetailPage.Master.Title = "EndPoints";

                            await masterDetailPage.Master.Navigation.PushAsync(endPointListPage);
                            await GoToRootDetailPageAsync(); // clear whatever may be on the detail page

                            // clear whatever may be on the detail page when leaving
                            endPointListPage.Disappearing += async (object sender, EventArgs e) => SetPlaceholderDetailPage();
                        }
                        break;

                    case AppPage.EndPointDetailPage:

                        var endPointViewModel = new EndPointViewModel(dependencyService, (Project)selectedModel, (EndPoint)otherSelectedModel);
                        var endPointDetailPage = new EndPointDetailPage() { BindingContext = endPointViewModel };

                        if (Device.Idiom == TargetIdiom.Phone)
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(endPointDetailPage);
                        }
                        else
                        {
                            // Don't push the detail page like the others, set it.
                            var masterDetailPage = (MasterDetailPage)Application.Current.MainPage;
                            masterDetailPage.Detail = new NavigationPage(endPointDetailPage)
                            {
                                BarBackgroundColor = SpanielColors.NavBarBackground,
                                BarTextColor = SpanielColors.NavBarText
                            };
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("page", "Unable to find the specified AppPage in NavigationService: " + page);
                }
            }
        }

        /// <summary>
        /// Pops to the root page on the Detail Pane
        /// </summary>
        public async Task GoToRootDetailPageAsync()
        {
            var currentDetailPage = ((MasterDetailPage)Application.Current.MainPage).Detail;

            if (currentDetailPage.Navigation.NavigationStack.Count > 1) // check if there's something to pop
                await currentDetailPage.Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Sets the Detail Pane to the Spaniel Placeholder page
        /// </summary>
        public void SetPlaceholderDetailPage()
        {
            var masterDetailPage = (MasterDetailPage)Application.Current.MainPage;

            masterDetailPage.Detail = new NavigationPage(new PlaceholderPage() { BindingContext = new PlaceHolderPageViewModel() })
            {
                BarBackgroundColor = SpanielColors.NavBarBackground,
                BarTextColor = SpanielColors.NavBarText
            };
        }
    }
}

