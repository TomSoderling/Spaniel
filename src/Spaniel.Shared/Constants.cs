
namespace Spaniel.Shared
{
    public static class Constants
    {
        // API Keys
        public const string InsightsApiKey = "d99f9c0e3c9740463602a17773a5f24a2fa4373a";
        public const string TestCloudApiKey = "07f728d967e1fdc85bf75810e611d5c0"; // this enables the app to be tested on device as well as in test could


        public const string BaseURLPlaceholder = "http://www.";
        public const string ProjectFileName = "projects.xml";
        public const string ProjectExportFileExtension = ".spaniel";


        // Colors
        public const string SpanielDarkGrey = "2E2E2E";  // dark grey 
        public const string SpanielLightGrey = "E4E4E4"; // light grey 

        // due to the way Xamarin.Forms handles this, I need to add 1 to the Blue in order to get the status bar on iOS to have white text
        //public const string SpanielYellow = "FFD200";    // yellow
        public const string SpanielYellow = "FFD201";     


    }
}

