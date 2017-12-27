using System;
using Spaniel.Shared.Services;
using Xamarin.Forms;
using Spaniel.Droid.Services;
using Android.Content;
using System.IO;

[assembly: Dependency(typeof(ShareServiceDroid))]

namespace Spaniel.Droid.Services
{
    public class ShareServiceDroid : IShareService
    {
        public void Share(string xmlData, string filename, string projectName)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var fullFilename = Path.Combine(documents, filename);
            File.WriteAllText(fullFilename, xmlData);

            // TODO: is there a better way to get a file than writing to device?

            var projectFile = new Java.IO.File(fullFilename);
            var uri = Android.Net.Uri.FromFile(projectFile);
            projectFile.SetReadable(true, false);

            var myIntent = new Intent(Intent.ActionSend); // Also known as the "share" intent, you should use this in an intent with startActivity() when you have some data that the user can share through another app, such as an email app or social sharing app.
            myIntent.SetType("text/plain");       // this allows the user to pick email or bluetooth (which could be nice). Don't need to lock them into Email only
            //myIntent.SetType("message/rfc822"); // this causes the mail application to launch
            myIntent.PutExtra(Intent.ExtraSubject, projectName + " Project File"); // set the email subject
            myIntent.PutExtra(Intent.ExtraStream, uri); // add the .spaniel file as email attachment
            var chooser = Intent.CreateChooser(myIntent, "Send Project File");


//            if (myIntent.ResolveActivity(Forms.Context.PackageManager) != null)
//            {
                Forms.Context.StartActivity(chooser);
//            }
//            else
//            {
//                // the intent can't resolve to any activities
//            }
        }
    }
}

