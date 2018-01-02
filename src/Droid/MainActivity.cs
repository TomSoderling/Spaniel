using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using System;
using Xamarin;
using Spaniel.Shared;
using System.IO;
using System.Xml.Linq;
using Xamarin.Forms;
using Spaniel.Shared.Services;

namespace Spaniel.Droid
{
    [Activity(Label = "Spaniel", Icon = "@drawable/icn_launcher", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionSend },
                  Categories = new[] { Intent.CategoryDefault },
                  DataMimeType = "text/xml",
                  DataPathPattern = "*.spaniel")] // not sure this does what I want it to
                  //DataScheme = "*.spaniel")] // not sure what this does
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);


            global::Xamarin.Forms.Forms.Init(this, bundle);

            // map the StyleID to the navtive controls.  This is for Xamarin.UITest
            global::Xamarin.Forms.Forms.ViewInitialized += (sender, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.View.StyleId))
                        e.NativeView.ContentDescription = e.View.StyleId;
                };

            LoadApplication(new App());


            // remove the ridiculous app icon on the action bar
            this.ActionBar.SetIcon(Android.Resource.Color.Transparent);


            if (Intent.Action == Intent.ActionSend)
            {
                // This is just an example of the data stored in the extras 
                //var uriFromExtras = Intent.GetParcelableExtra(Android.Content.Intent.ExtraStream) as Android.Net.Uri;
                //var subject = Intent.GetStringExtra(Android.Content.Intent.ExtraSubject);

                // Get the info from ClipData 
                var spanielFile = Intent.ClipData.GetItemAt(0);

                // Open a stream from the URI 
                var stream = ContentResolver.OpenInputStream(spanielFile.Uri);

                // Save it over 
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                var docsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(docsPath, "incoming.spaniel"); // Saves the file to: /data/user/0/com.TomSoderling.Spaniel/files/incoming.spaniel

                File.WriteAllBytes(filePath, memoryStream.ToArray());
                memoryStream.Dispose();


                try
                {
                    // Import the projects from the file
                    var fileData = File.ReadAllText(filePath);

                    var element = XElement.Parse(fileData);

                    var projRepo = new ProjectRepository();
                    var projectsDoc = projRepo.LoadProjectFileFromDevice(); // load main in projects.xml file from device

                    if (projectsDoc != null)
                    {
                        projectsDoc.Root.Add(element); // add this new node to the projects.xml file
                        projRepo.SaveProjectsDocument(projectsDoc); // save back to device

                        // Send message to tell the ProjectViewModel to reload the Project List
                        MessagingCenter.Send<IProjectRepository>(projRepo, "Reload");
                    }
                }
                catch (Exception ex)
                {
                    AlertDialog.Builder dialog = new AlertDialog.Builder(this);  
                    AlertDialog alert = dialog.Create();  
                    alert.SetTitle("Error Parsing Spaniel File");  
                    alert.SetMessage("Message: " + ex.Message);
                    alert.SetButton("Rats", (sender, e) => { }); 
                    alert.Show();  
                }

            }
        }
    }
}

