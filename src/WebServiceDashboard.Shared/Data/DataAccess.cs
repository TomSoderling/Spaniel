using System;
using WebServiceDashboard.Shared.Services;
using WebServiceDashboard.Shared.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WebServiceDashboard.Shared.Data
{
    public static class DataAccess
    {
        /// <summary>
        /// Loads the projects.xml file from device, or the Test Data xml file
        /// </summary>
        public static ObservableCollection<Project> Load(IDependencyService dependencyService)
        {
            var projectRepo = dependencyService.Get<IProjectRepository>();
            var doc = projectRepo.LoadProjectFileFromDevice();

            // a null doc will tell this method to load the test data file instead.
            var projectCollection = XDocumentHelper.LoadProjectFile(doc);

            if (doc == null)
                SaveProjects(dependencyService, projectCollection); // save the test data file to device the very first time

            return projectCollection;
        }

        /// <summary>
        /// Saves the new Project to file
        /// </summary>
        public static bool SaveNewProject(IDependencyService dependencyService, Project project)
        {
            var projectRepo = dependencyService.Get<IProjectRepository>();
            var doc = projectRepo.LoadProjectFileFromDevice();
            if (doc != null)
            {
                var success = XDocumentHelper.AddProjectToDoc(doc, project);

                if (success)
                    return projectRepo.SaveProjectsDocument(doc);
            }

            return false;
        }

        /// <summary>
        /// Saves the projects to file
        /// </summary>
        public static bool SaveProjects(IDependencyService dependencyService, IEnumerable<Project> projects)
        {
            var projectRepo = dependencyService.Get<IProjectRepository>();
            return projectRepo.SaveProjectsToFile(projects);
        }


        /// <summary>
        /// Updates the specified project in file
        /// </summary>
        public static bool UpdateProject(IDependencyService dependencyService, Project project)
        {
            var projectRepo = dependencyService.Get<IProjectRepository>();
            var doc = projectRepo.LoadProjectFileFromDevice();
            if (doc != null)
            {
                var success = XDocumentHelper.UpdateProjectInDoc(doc, project);

                if (success)
                    return projectRepo.SaveProjectsDocument(doc);
            }

            return false;
        }

        /// <summary>
        /// Deletes the specified project from file
        /// </summary>
        public static bool DeleteProject(IDependencyService dependencyService, Guid projectID)
        {
            var projectRepo = dependencyService.Get<IProjectRepository>();
            var doc = projectRepo.LoadProjectFileFromDevice();
            if (doc != null)
            {
                var success = XDocumentHelper.RemoveProjectFromDoc(doc, projectID);

                if (success)
                    return projectRepo.SaveProjectsDocument(doc);
            }

            return false;
        }

        /// <summary>
        /// Saves the Project Status to file
        /// </summary>
        public static bool SaveProjectStatus(IDependencyService dependencyService, Guid projectID, TestStatus status)
        {
            var projectRepo = dependencyService.Get<IProjectRepository>();
            var doc = projectRepo.LoadProjectFileFromDevice();
            if (doc != null)
            {
                var success = XDocumentHelper.UpdateProjectStatusInDoc(doc, projectID, status);

                if (success)
                    return projectRepo.SaveProjectsDocument(doc);
            }

            return false;
        }



        /// <summary>
        /// Saves the new EndPoint to file
        /// </summary>
        public static bool SaveNewEndPoint(IDependencyService dependencyService, Guid projectID, EndPoint endPoint)
        {
            var projectRepo = dependencyService.Get<IProjectRepository>();
            var doc = projectRepo.LoadProjectFileFromDevice();

            if (doc != null)
            {
                var success = XDocumentHelper.AddEndPointToDoc(doc, projectID, endPoint);

                if (success)
                    return projectRepo.SaveProjectsDocument(doc);
            }

            return false;
        }

        /// <summary>
        /// Updates the specified EndPoint in file
        /// </summary>
        public static bool UpdateEndPoint(IDependencyService dependencyService, EndPoint endPoint)
        {
            var projectRepo = dependencyService.Get<IProjectRepository>();
            var doc = projectRepo.LoadProjectFileFromDevice();
            if (doc != null)
            {
                var success = XDocumentHelper.UpdateEndPointInDoc(doc, endPoint);

                if (success)
                    return projectRepo.SaveProjectsDocument(doc);
            }

            return false;
        }

        /// <summary>
        /// Deletes the specified EndPoint from file
        /// </summary>
        public static bool DeleteEndPoint(IDependencyService dependencyService, Guid endPointID)
        {
            var projectRepo = dependencyService.Get<IProjectRepository>();
            var doc = projectRepo.LoadProjectFileFromDevice();
            if (doc != null)
            {
                var success = XDocumentHelper.RemoveEndPointFromDoc(doc, endPointID);

                if (success)
                    return projectRepo.SaveProjectsDocument(doc);
            }

            return false;
        }



        /// <summary>
        /// Saves the test result, any changed EndPoint details, and run dates for the Project
        /// </summary>
        public static bool SaveTestResultAndEndPoint(IDependencyService dependencyService, Guid projectID, EndPoint endPoint, Result result, TestStatus status)
        {
            var projectRepo = dependencyService.Get<IProjectRepository>();
            var doc = projectRepo.LoadProjectFileFromDevice();
            if (doc != null)
            {
                var success = XDocumentHelper.AddResultAndUpdateEndPointInDoc(doc, projectID, endPoint, result, status);

                if (success)
                    return projectRepo.SaveProjectsDocument(doc);
            }

            return false;
        }



        /// <summary>
        /// Deletes all results for the specified project
        /// </summary>
        public static bool DeleteAllResultsForProject(IDependencyService dependencyService, Guid projectID)
        {
            var projectRepo = dependencyService.Get<IProjectRepository>();
            var doc = projectRepo.LoadProjectFileFromDevice();

            if (doc != null)
            {
                var success = XDocumentHelper.RemoveAllResultsForProjectFromDoc(doc, projectID);

                if (success)
                    return projectRepo.SaveProjectsDocument(doc);
            }

            return false;
        }



        /// <summary>
        /// Returns the project XML node for the specified project ID, minus any results and with new Project and EndPoint IDs
        /// </summary>
        public static string FetchProjectXMLForExport(IDependencyService dependencyService, Guid projectID)
        {
            var projectRepo = dependencyService.Get<IProjectRepository>();
            var doc = projectRepo.LoadProjectFileFromDevice();

            if (doc != null)
            {
                // Get the entire project xml node. Returns null on exception
                var projectNode = XDocumentHelper.ExportProjectFromDoc(doc, projectID);

                if (projectNode != null)
                {
                    // Strip out any EndPoint results
                    XDocumentHelper.RemoveAllResultsForProjectFromDoc(projectNode.Document, projectID);

                    // Project and EndPoint IDs need to be re-created because they must be unique.
                    XDocumentHelper.ChangeProjectAndEndPointIDs(projectNode.Document, projectID);

                    return projectNode.ToString();
                }
            }

            return "Oops! Something went wrong retrieving the project XML data.";
        }
    }
}