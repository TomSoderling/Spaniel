﻿using System;
using System.Collections.Generic;
using Spaniel.Shared.Models;
using Spaniel.Shared.Data;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;
using Xamarin;
using System.Diagnostics;
using Spaniel.Shared;
using Spaniel.Droid;
using Spaniel.Shared.Services;

[assembly: Dependency(typeof(ProjectRepository))]

namespace Spaniel.Droid
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
                        // TODO: log this with App Center
                        // Report to Xamarin Insights
                        // doc was loaded, but root is null
                        //Insights.Report(null, new Dictionary<string, string>
                            //{
                            //    { "Issue", "projects.xml file was loaded as an XDocument, but the document.Root was null" },
                            //    { "Where", "Android Project, ProjectRepository.cs, LoadProjectFileFromDevice() method" },
                            //    { "Filename", filename }
                            //});
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Getting a "sharing violation" exception when attempting to load the document the first time.
                    // Didn't save any project results & can't delete a Project. Galaxy S8


                    // TODO: log this with App Center
                    // file exists, but can't load it
                    //Insights.Report(ex, new Dictionary<string, string>
                        //{
                        //    { "Issue", "projects.xml file exists on device, but unable to load as an XDocument" },
                        //    { "Filename", filename }
                        //});
                }
            }
            else
            {
                // This file won't exist the first time. Don't want to report this on the first failure.
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
                // TODO: log this with App Center
                //Insights.Report(ex, new Dictionary<string, string>
                    //{
                    //    { "Issue", "Unable to save projects.xml document to device" }
                    //});
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
                var sw = new StreamWriter(filename);
                doc.Save(sw);

                // Need to dispose of this after the file is created. Otherwise we get a "Sharing violation on path /data/user/0/com.TomSoderling.Spaniel/files/projects.xml" 
                // exception when trying to load the document the first time after it's created.
                sw.Dispose(); 
            }
            catch (Exception ex)
            {
                // TODO: log this with App Center
                //Insights.Report(ex, new Dictionary<string, string>
                    //{
                    //    { "Issue", "Unable to save projects.xml file to device" },
                    //    { "Filename", filename }
                    //});
                return false;
            }

            return true;
        }
    }
}

