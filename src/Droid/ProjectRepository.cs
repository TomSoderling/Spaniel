using System;
using System.Collections.Generic;
using WebServiceDashboard.Shared.Models;
using WebServiceDashboard.Shared.Data;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;
using Xamarin;
using System.Diagnostics;
using WebServiceDashboard.Shared;
using WebServiceDashboard.Droid;
using WebServiceDashboard.Shared.Services;

[assembly: Dependency(typeof(ProjectRepository))]

namespace WebServiceDashboard.Droid
{
    /// <summary>
    /// Class to handle file access for each platform
    /// </summary>
    public class ProjectRepository : IProjectRepository
    {
        private static string DocumentsFolderPath
        {
            get 
            { 
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
        }

        private static string GetFilename()
        {
            return Path.Combine(DocumentsFolderPath, Constants.ProjectFileName);
        }



        /// <summary>
        /// Load the existing projects.xml file on device
        /// </summary>
        public XDocument LoadProjectFileFromDevice()
        {
            string filename = GetFilename();

            if (File.Exists(filename))
            {
                try
                {
                    Debug.WriteLine(string.Format("Loading projects.xml file: {0}", filename));
                    var doc = XDocument.Load(filename); // load existing projects.xml file

                    if (doc.Root != null)
                    {
                        return doc;
                    }
                    else
                    {
                        // Report to Xamarin Insights
                        // doc was loaded, but root is null
                        Insights.Report(null, new Dictionary<string, string>
                            {
                                { "Issue", "projects.xml file was loaded as an XDocument, but the document.Root was null" },
                                { "Where", "Android Project, ProjectRepository.cs, LoadProjectFileFromDevice() method" },
                                { "Filename", filename }
                            });
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Getting a "sharing violation" on Android Sim. Nexus 4 Lollipop
                    // seems like it happens this first time the app is run. Didn't save any project results


                    // file exists, but can't load it
                    Insights.Report(ex, new Dictionary<string, string>
                        {
                            { "Issue", "projects.xml file exists on device, but unable to load as an XDocument" },
                            { "Filename", filename }
                        });
                }
            }
            else
            {
                // TODO: this file won't exist the first time. Don't want to report this on the first failure.

                // expected file not on device
                Insights.Report(null, new Dictionary<string, string>
                    {
                        { "Issue", "Expected projects.xml file does not exist on device" },
                        { "Where", "Android Project, ProjectRepository.cs, LoadProjectFileFromDevice() method" },
                        { "Filename", filename }
                    });
            }

            return null;
        }

        /// <summary>
        /// Saves the projects.xml to device storage
        /// </summary>
        public bool SaveProjectsDocument(XDocument doc)
        {
            try
            {
                doc.Save(GetFilename());
            }
            catch(Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", "Unable to save projects.xml document to device" }
                    });
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves a list of project data to xml file on device.  Deletes and rewrites entire file.
        /// </summary>
        public bool SaveProjectsToFile(IEnumerable<Project> projects)
        {
            string filename = GetFilename();

            if (File.Exists(filename))
                File.Delete(filename);

            var doc = XDocumentHelper.BuildProjectDocument(projects);

            try
            {
                doc.Save(new StreamWriter(filename));
            }
            catch (Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", "Unable to save projects.xml file to device" },
                        { "Filename", filename }
                    });
                return false;
            }

            return true;
        }
    }
}

