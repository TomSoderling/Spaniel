using Android.App;
using Android.Content.PM;
using Android.OS;
using System;
using Xamarin;
using Spaniel.Shared;

namespace Spaniel.Droid
{
    [Activity(Label = "Spaniel", Icon = "@drawable/icn_launcher", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                //#if DEBUG
                //// Use Insights debug mode.  Any unhandled crash reports, native crash reports, and any Insights.Track() or Insights.Identify() calls will be ignored.
                //    Insights.Initialize(Insights.DebugModeKey, Application.Context); 
                //#else
                //    Insights.Initialize(Constants.InsightsApiKey, Application.Context);
                //#endif
            }
            catch(Exception ex)
            {
                // eat this exception. don't let insights initialization crash the app.  #paranoid
                Console.WriteLine("Exception caught. Xamarin Insights failed on Initialize() call. Exception message: " + ex.Message);
            }



            base.OnCreate(bundle);


            global::Xamarin.Forms.Forms.Init(this, bundle);

            // map the StyleID to the navtive controls.  This is for Xamarin.UITest
            global::Xamarin.Forms.Forms.ViewInitialized += (sender, e) => 
                {
                    if (!string.IsNullOrWhiteSpace(e.View.StyleId))
                        e.NativeView.ContentDescription = e.View.StyleId;
                };

            LoadApplication(new App());
        }
    }
}

