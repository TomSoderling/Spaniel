using Spaniel.Shared.Services;
using Xamarin.Forms;
using Spaniel.iOS.Services;
using UIKit;
using Foundation;
using System;
using System.IO;
using CoreGraphics;

[assembly: Dependency(typeof(ShareServiceiOS))]

namespace Spaniel.iOS.Services
{
    public class ShareServiceiOS : IShareService
    {
        public void Share(string xmlData, string filename, string projectName)
        {
            // Write the file to device, and create an NSUrl to be used by the UIActivityViewController
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fullFilename = Path.Combine(documents, filename); // use our custom UTI with a ".spaniel" extension
            File.WriteAllText(fullFilename, xmlData);

            var url = new NSUrl(fullFilename, false);

            var activityItems = new NSObject[] { url };
            var activityController = new UIActivityViewController(activityItems, null);

            // TODO: can we use the projectName as the email subject? : projectName + " Project File"

            // don't need to post this to any Social channels, or send via messages
            activityController.ExcludedActivityTypes = new NSString[] 
                { 
                    UIActivityType.PostToFacebook, 
                    UIActivityType.PostToTwitter,
                    UIActivityType.PostToWeibo,
                    UIActivityType.PostToTencentWeibo,
                    UIActivityType.Message
                };

            var popover = activityController.PopoverPresentationController;
            if (popover != null)
            {
                // this is for when the app is running on an iPad
                var self = UIApplication.SharedApplication.KeyWindow.RootViewController;

                popover.SourceView = self.View;
                var frame = UIScreen.MainScreen.Bounds;
                frame.Height /= 4;
                popover.SourceRect = frame;
                popover.PermittedArrowDirections = UIPopoverArrowDirection.Unknown;
            }

            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(activityController, true, null);
        }
    }
}