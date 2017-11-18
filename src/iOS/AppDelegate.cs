using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Foundation;
using UIKit;
using WebServiceDashboard.Pages;
using WebServiceDashboard.Shared.Data;
using WebServiceDashboard.Shared.Services;
using WebServiceDashboard.ViewModels;
using Xamarin.Forms;

namespace WebServiceDashboard.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        const string runAllEndpointsShortcutItemType = "com.tomsoderling.Spaniel.000";


        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Get possible 3D Touch shortcut items
            // This is used if you define static Home screen quick actions (UIApplicationShortcutItems) in your info.plist
            //if (options != null) 
                //LaunchedShortcutItem = options[UIApplication.LaunchOptionsShortcutItemKey] as UIApplicationShortcutItem;


            Forms.Init();


            // Used for Xamarin.UITest - Copy any StyleId to the native property
            Forms.ViewInitialized += (object sender, ViewInitializedEventArgs e) => 
                {
                    if (null != e.View.StyleId) 
                        e.NativeView.AccessibilityIdentifier = e.View.StyleId;
                };

            // Code for starting up the Xamarin Test Cloud Agent
            #if ENABLE_TEST_CLOUD
			    Xamarin.Calabash.Start();
            #endif


            // trying out the continuous code plugin
//            #if DEBUG
//                new Continuous.Server.HttpServer(this).Run();
//            #endif


            // iOS project plist is set to hide status bar on app load, this will display it again
            UIApplication.SharedApplication.StatusBarHidden = false;

            LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }

        // This is used if you define static Home screen quick actions (UIApplicationShortcutItems) in your info.plist
        //public UIApplicationShortcutItem LaunchedShortcutItem { get; set; }

        public async Task<bool> HandleShortcutItemAsync(UIApplicationShortcutItem shortcutItem)
        {
            var handled = false;

            // Anything to process?
            if (shortcutItem != null)
            {
                switch (shortcutItem.Type)
                {
                    case runAllEndpointsShortcutItemType:
                        
                        handled = true;

                        // In hindsight, this may be a good place to use Xamarin.Forms IMessagingCenter
                        // then I wouldn't need the global static DependencyServiceWrapperProperty

                        var ds = App.DependencyServiceWrapperProperty; // this may not be a good idea to have this global property hanging off of the app
                        var navService = ds.Get<INavigationService>();

                        // Find the right Project
                        var nameOfProjectToFind = shortcutItem.LocalizedTitle;
                        var projects = DataAccess.Load(ds);
                        var project = projects.FirstOrDefault(p => p.Name == nameOfProjectToFind);

                        if (project != null)
                        {
                            // Always start out on the ProjectList page
                            var navPage = App.Current.MainPage as NavigationPage;
                            if (navPage.CurrentPage.GetType() != typeof(ProjectListPage))
                                await navService.GoToPageAsync(AppPage.ProjectListPage);
                            
                            // Navigate to the EndpointList page using this as the selected project
                            await navService.GoToPageAsync(AppPage.EndPointListPage, project, ds);

                            // get the ViewModel and execute command to run all tests for this project
                            var endpointVM = navPage.CurrentPage.BindingContext as EndPointViewModel;
                            endpointVM.RunAllTests.Execute(null);
                        }

                        break;
                }
            }

            return handled;
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            // I guess this is you handle static Home screen quick actions.
            
            // Handle any shortcut item being selected
            //HandleShortcutItemAsync(LaunchedShortcutItem);

            // Clear shortcut after it's been handled
            //LaunchedShortcutItem = null;
        }

        public override void DidEnterBackground(UIApplication uiApplication)
        {
            // TODO: small issue with these quick actions
            // the results aren't being displayed on the ProjectList page or when you go back to the EndPointList page from visiting the ProjectListPage

            // Add a dynamic quick action for each project
            var ds = App.DependencyServiceWrapperProperty;
            var projects = DataAccess.Load(ds);

            var shortcutList = projects.Select(x => new UIApplicationShortcutItem(
                runAllEndpointsShortcutItemType,
                x.Name,
                $"Run all {x.EndPoints.Count} endpoints",
                UIApplicationShortcutIcon.FromType(UIApplicationShortcutIconType.Play),
                new NSDictionary<NSString, NSObject>()
            ));

            uiApplication.ShortcutItems = shortcutList.ToArray();
        }

        public override async void PerformActionForShortcutItem(UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
        {
            // Perform action
            completionHandler(await HandleShortcutItemAsync(shortcutItem));
        }
            

        /// <summary>
        /// Handle incoming Spaniel files
        /// </summary>
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var fileData = File.ReadAllText(url.Path); // iOS will place the file at this location: /private/var/mobile/Containers/Data/Application/[App ID]/Documents/Inbox/[File name]-[number].spaniel

            try 
            {
                var element = XElement.Parse(fileData);

                var projRepo = new ProjectRepository();
                var projectsDoc = projRepo.LoadProjectFileFromDevice(); // load main in projects.xml file from device

                if (projectsDoc != null)
                {
                    projectsDoc.Root.Add(element); // add this new node to the projects.xml file
                    projRepo.SaveProjectsDocument(projectsDoc); // save back to device

                    // Send message to tell the ProjectViewModel to reload the Project List
                    MessagingCenter.Send<IProjectRepository>(projRepo, "Reload");

                    return true;
                }
            }
            catch(Exception ex)
            {
                var alert = new UIAlertView("Error Parsing Spaniel File", "Message: " + ex.Message, null, "Okay");
                alert.Show();
            }

            return false;
        }
    }
}

