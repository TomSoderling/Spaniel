using System;
using Spaniel.Shared.Services;
using Xamarin.Forms;
using Spaniel.Droid.Services;
using Android.Content;
using System.IO;
using Android.Webkit;

[assembly: Dependency(typeof(ShareServiceDroid))]

namespace Spaniel.Droid.Services
{
    public class ShareServiceDroid : IShareService
    {
        readonly Context localContext = Android.App.Application.Context;

        public void Share(string xmlData, string filename, string projectName)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var fullFilename = Path.Combine(documents, filename);

            // TODO: is there a better way to get a file than writing to device?
            File.WriteAllText(fullFilename, xmlData);


            var projectFile = new Java.IO.File(fullFilename);
            var uri = Android.Net.Uri.FromFile(projectFile);
            projectFile.SetReadable(true, false);

            // Also known as the "share" intent, you should use this in an intent with startActivity() when you have some data that the user can share through 
            // another app, such as an email app or social sharing app.
            var myIntent = new Intent(Intent.ActionSend); 

            myIntent.SetType("text/xml");
            //myIntent.SetType("message/rfc822"); // causes the mail application to launch. If multiple applications are capable of handling mail, the user will get a list to choose from.

            //myIntent.PutExtra(Intent.ExtraSubject, projectName + " Project File.spaniel"); // set the email subject
            // 1/1/18 - for whatever reason, this is what gets used as the filename when saved to Google Drive, NOT what gets passed in.

            // Set the data
            myIntent.PutExtra(Intent.ExtraStream, uri);


            var chooser = Intent.CreateChooser(myIntent, "Send Project File");

            // This fixes an error on API 22: "Calling startActivity() from outside of an Activity context requires the FLAG_ACTIVITY_NEW_TASK flag. Is this really what you want?"
            chooser.AddFlags(ActivityFlags.NewTask);

            //if (myIntent.ResolveActivity(Forms.Context.PackageManager) != null)
            //{
                localContext.StartActivity(chooser);
            //}
            //else
            //{
            //    // the intent can't resolve to any activities
            //}
        }
    }
}

