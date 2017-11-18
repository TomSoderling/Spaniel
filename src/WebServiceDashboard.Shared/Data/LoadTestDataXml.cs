using System.Reflection;
using System.IO;
using System;

namespace WebServiceDashboard.Shared.Data
{
    /// <summary>
    /// Static class to load in the embedded xml resource file (TestData.xml)
    /// </summary>
    public static class LoadTestDataXml
    {
        // embedded resource file

        #if DEBUG
            private const string testDataFilename = "WebServiceDashboard.Shared.Data.TestData.xml"; // test data. contains the iFactr and HttpBin projects
        #else
            private const string testDataFilename = "WebServiceDashboard.Shared.Data.StartingData.xml"; // starting test data for release builds. contains only the HttpBin project
        #endif
        
        public static string TestData
        {
            get
            {
                var assembly = typeof(LoadTestDataXml).GetTypeInfo().Assembly;

                try 
                {
                    var stream = assembly.GetManifestResourceStream(testDataFilename);
                    var testData = new StreamReader(stream).ReadToEnd();

                    return testData;
                }
                catch (Exception)
                {
                    return string.Empty; // if this test data file doesn't exist for some reason, return an empty string.
                }
            }
        }
    }
}

