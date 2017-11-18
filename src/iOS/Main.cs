using UIKit;
using System;
using Xamarin;
using WebServiceDashboard.Shared;

namespace WebServiceDashboard.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            try
            {
                //#if DEBUG
                //    // Use Insights debug mode.  Any unhandled crash reports, native crash reports, and any Insights.Track() or Insights.Identify() calls will be ignored.
                //    Insights.Initialize(Insights.DebugModeKey);
                //#else
                //    Insights.Initialize(Constants.InsightsApiKey);
                //#endif
            }
            catch(Exception ex)
            {
                // eat this exception. don't let insights initialization crash the app.  #paranoid
                Console.WriteLine("Exception caught. Xamarin Insights failed on Initialize() call. Exception message: " + ex.Message);
            }


            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}