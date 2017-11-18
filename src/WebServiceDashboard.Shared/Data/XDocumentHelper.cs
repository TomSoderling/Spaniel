using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using WebServiceDashboard.Shared.Models;
using WebServiceDashboard.Shared.Data;
using System.Linq;
using System.Collections.Generic;
using Xamarin;

namespace WebServiceDashboard.Shared.Data
{
    public static class XDocumentHelper 
    {
        /// <summary>
        /// Creates a collection of Projects with its hierarchy of EndPoints and Results
        /// </summary>
        public static ObservableCollection<Project> LoadProjectFile(XDocument doc)
        {
            if (doc == null)
            {
                // if the projects.xml file doesn't exist on the device, load up some test data.
                doc = XDocument.Parse(LoadTestDataXml.TestData);
            }

            var projectList = new ObservableCollection<Project>();

            // Iterate over each element in the xml file and create the appropriate object hierarchy
            if (doc.Root != null) 
            {
                var projectStatusIsAllSuccessful = true;

                // Create a Project object for each one in the collection
                // Sort on reading in xml file - show projects that have most recently been tested on top
                foreach (var project in doc.Root.Elements("project").OrderByDescending(p => string.IsNullOrEmpty(p.Attribute(LAST_TEST).Value) ? DateTime.MinValue : DateTime.Parse(p.Attribute(LAST_TEST).Value)))
                {
                    var newProject = new Project()
                        {
                            ID = Guid.Parse(project.Attribute(ID).Value),
                            Name = project.Attribute(NAME).Value,
                            Description = project.Attribute(DESCRIPTION).Value,
                            Username = project.Attribute(USERNAME).Value,
                            Password = project.Attribute(PASSWORD).Value,
                            BaseURL = project.Attribute(BASE_URL).Value,
                            TestStatus = (TestStatus)int.Parse(project.Attribute(STATUS).Value),
                            LastTestRun = string.IsNullOrWhiteSpace(project.Attribute(LAST_TEST).Value) ? null : (DateTime?)DateTime.Parse(project.Attribute(LAST_TEST).Value)
                        };

                    // Create an EndPoint object for each one in this current Project
                    foreach (var endpoint in project.Elements("endpoints").Elements("endpoint"))
                    {
                        var newEndPoint = new EndPoint()
                            {
                                ID = Guid.Parse(endpoint.Attribute(ID).Value),
                                Name = endpoint.Attribute(NAME).Value,
                                Status = (TestStatus)int.Parse(endpoint.Attribute(STATUS).Value),
                                EndPointURI = endpoint.Attribute(URL).Value,
                                HttpVerb = endpoint.Attribute(VERB).Value,
                                ParameterFillIn = endpoint.Attribute(PARAM_FILL).Value,
                                FilterDefinition = endpoint.Attribute(FILTER_DEF).Value,
                                LastTested = string.IsNullOrWhiteSpace(endpoint.Attribute(LAST_TEST).Value) ? null : (DateTime?)DateTime.Parse(endpoint.Attribute(LAST_TEST).Value)
                            };

                        if (newEndPoint.Status != TestStatus.Successful)
                            projectStatusIsAllSuccessful = false;

                        Icons.SetEndPointIcon(newEndPoint);
                        newProject.EndPoints.Add(newEndPoint);

                        // Create a Result object for each one in this current EndPoint
                        foreach (var result in endpoint.Elements("results").Elements("result"))
                        {
                            var newResult = new Result()
                                {
                                    RunDate = string.IsNullOrWhiteSpace(result.Attribute(RUN_DATE).Value) ? null : (DateTime?)DateTime.Parse(result.Attribute(RUN_DATE).Value),
                                    HttpCode = (System.Net.HttpStatusCode)int.Parse(result.Attribute(HTTP_CODE).Value),
                                    ResponseTime = string.IsNullOrWhiteSpace(result.Attribute(RESPONSE_TIME).Value) ? null : (TimeSpan?)TimeSpan.Parse(result.Attribute(RESPONSE_TIME).Value)
                                };

                            Icons.SetResultIcon(newResult);
                            newEndPoint.Results.Add(newResult);
                        }
                    }

                    Icons.SetProjectIcon(newProject);
                    projectList.Add(newProject);
                }
            }

            return projectList;
        }

        /// <summary>
        /// Creates an XDocument from a list of Projects
        /// </summary>
        public static XDocument BuildProjectDocument(IEnumerable<Project> projects)
        {
            var projectDoc = new XDocument(
                new XElement("projects",
                    projects.Select(p =>
                        new XElement("project", 
                            new XAttribute(ID,          p.ID.ToString()),
                            new XAttribute(NAME,        p.Name ?? string.Empty),
                            new XAttribute(DESCRIPTION, p.Description ?? string.Empty),
                            new XAttribute(USERNAME,    p.Username ?? string.Empty),
                            new XAttribute(PASSWORD,    p.Password ?? string.Empty),
                            new XAttribute(BASE_URL,    p.BaseURL ?? string.Empty),
                            new XAttribute(STATUS,      p.TestStatus == null ? (int)TestStatus.Untested : (int)p.TestStatus), // save the status as an int
                            new XAttribute(LAST_TEST,   p.LastTestRun == null ? string.Empty : p.LastTestRun.ToString()), // save string.Empty if date is null
                            new XElement("endpoints",
                                p.EndPoints.Select(e => 
                                    new XElement("endpoint",
                                        new XAttribute(ID,         e.ID.ToString()),
                                        new XAttribute(NAME,       e.Name ?? string.Empty),
                                        new XAttribute(STATUS,     e.Status == null ? (int)TestStatus.Untested : (int)e.Status),
                                        new XAttribute(URL,        e.EndPointURI ?? string.Empty),
                                        new XAttribute(VERB,       e.HttpVerb ?? string.Empty),
                                        new XAttribute(PARAM_FILL, e.ParameterFillIn ?? string.Empty),
                                        new XAttribute(FILTER_DEF, e.FilterDefinition ?? string.Empty),
                                        new XAttribute(LAST_TEST,  e.LastTested == null ? string.Empty : e.LastTested.ToString()),
                                        new XElement("results",
                                            e.Results.Select(r => 
                                                new XElement("result",
                                                    new XAttribute(RUN_DATE,      r.RunDate == null ? string.Empty : r.RunDate.ToString()),
                                                    new XAttribute(HTTP_CODE,     r.HttpCode == null ? 0 : (int)r.HttpCode),
                                                    new XAttribute(RESPONSE_TIME, r.ResponseTime == null ? string.Empty : r.ResponseTime.ToString())

                                                    // It's not smart or useful to save all response bodies.  A single iFactr customers xml response body is 195,000 characters!
                                                    //{
                                                    //    Value = r.ResponseBody
                                                    //}
                                                )))
                                    )))

                        ))));

            return projectDoc;
        }



        /// <summary>
        /// Adds the project to the project document
        /// </summary>
        public static bool AddProjectToDoc(XDocument doc, Project project)
        {
            try
            {
            // Add a new Project node to the root Projects node
            doc.Root.Add(
                new XElement("project", 
                    new XAttribute(ID,          project.ID.ToString()),
                    new XAttribute(NAME,        project.Name ?? string.Empty),
                    new XAttribute(DESCRIPTION, project.Description ?? string.Empty),
                    new XAttribute(USERNAME,    project.Username ?? string.Empty),
                    new XAttribute(PASSWORD,    project.Password ?? string.Empty),
                    new XAttribute(BASE_URL,    project.BaseURL ?? string.Empty),
                    new XAttribute(STATUS,      project.TestStatus == null ? (int)TestStatus.Untested : (int)project.TestStatus), // save the status as an int
                    new XAttribute(LAST_TEST,   project.LastTestRun == null ? string.Empty : project.LastTestRun.ToString()), // save string.Empty if date is null
                    new XElement("endpoints")
                ));
            }
            catch(Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", "Unable to new add project to project.xml document" }
                    });
                return false;
            }

            return true;
        }

        /// <summary>
        /// Updates the project in the project document
        /// </summary>
        public static bool UpdateProjectInDoc(XDocument doc, Project project)
        {
            try
            {
                // Find the Project
                var updatedProject = doc.Root.Descendants("project").First(e => e.Attribute(ID).Value == project.ID.ToString());

                // Update any changed Project details
                if (updatedProject.Attribute(NAME).Value != project.Name)
                    updatedProject.Attribute(NAME).SetValue(project.Name);
                if (updatedProject.Attribute(DESCRIPTION).Value != project.Description)
                    updatedProject.Attribute(DESCRIPTION).SetValue(project.Description);
                if (updatedProject.Attribute(USERNAME).Value != project.Username)
                    updatedProject.Attribute(USERNAME).SetValue(project.Username);
                if (updatedProject.Attribute(PASSWORD).Value != project.Password)
                    updatedProject.Attribute(PASSWORD).SetValue(project.Password);
                if (updatedProject.Attribute(BASE_URL).Value != project.BaseURL)
                    updatedProject.Attribute(BASE_URL).SetValue(project.BaseURL);
                if (updatedProject.Attribute(STATUS).Value != ((int)project.TestStatus).ToString())
                    updatedProject.Attribute(STATUS).SetValue((int)project.TestStatus);
                if (updatedProject.Attribute(LAST_TEST).Value != project.LastTestRun.ToString())
                    updatedProject.Attribute(LAST_TEST).SetValue(project.LastTestRun.ToString());

                // replace the existing Project node with this updated one
                doc.Root.Descendants("project").First(p => p.Attribute(ID).Value == project.ID.ToString()).ReplaceWith(updatedProject);
            }
            catch(Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", "Unable to update project in project.xml document" }
                    });
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes the specified project from the project document
        /// </summary>
        public static bool RemoveProjectFromDoc(XDocument doc, Guid projectID)
        {
            try
            {
                doc.Root.Descendants("project").First(p => p.Attribute(ID).Value == projectID.ToString()).Remove();
            }
            catch(Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", "Unable to remove project from project.xml document" }
                    });
                return false;
            }

            return true;
        }

        /// <summary>
        /// Updates the test status in the project document for the given Project ID
        /// </summary>
        public static bool UpdateProjectStatusInDoc(XDocument doc, Guid projectID, TestStatus status)
        {
            try
            {
                // Find the Project
                var updatedProject = doc.Root.Descendants("project").First(p => p.Attribute(ID).Value == projectID.ToString());
                updatedProject.Attribute(STATUS).SetValue((int)status);

                // replace the existing project node with this updated one
                doc.Root.Descendants("project").First(p => p.Attribute(ID).Value == projectID.ToString()).ReplaceWith(updatedProject);
            }
            catch(Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", "Unable to update project status in project.xml document" }
                    });
                return false;
            }

            return true;
        }



        /// <summary>
        /// Adds the EndPoint to the project document for the given Project ID
        /// </summary>
        public static bool AddEndPointToDoc(XDocument doc, Guid projectID, EndPoint endPoint)
        {
            try
            {
                // Find the Project
                var updatedProject = doc.Root.Descendants("project").First(p => p.Attribute(ID).Value == projectID.ToString());

                // Find the list of EndPoints and add new one to top of collection
                updatedProject.Descendants("endpoints").First().Add(
                    new XElement("endpoint",
                        new XAttribute(ID,         endPoint.ID.ToString()),
                        new XAttribute(NAME,       endPoint.Name ?? string.Empty),
                        new XAttribute(STATUS,     endPoint.Status == null ? (int)TestStatus.Untested : (int)endPoint.Status),
                        new XAttribute(URL,        endPoint.EndPointURI ?? string.Empty),
                        new XAttribute(VERB,       endPoint.HttpVerb ?? string.Empty),
                        new XAttribute(PARAM_FILL, endPoint.ParameterFillIn ?? string.Empty),
                        new XAttribute(FILTER_DEF, endPoint.FilterDefinition ?? string.Empty),
                        new XAttribute(LAST_TEST,  endPoint.LastTested == null ? string.Empty : endPoint.LastTested.ToString()),
                        new XElement("results")
                    ));

                // replace the existing project node with this updated one
                doc.Root.Descendants("project").First(p => p.Attribute(ID).Value == projectID.ToString()).ReplaceWith(updatedProject);
            }
            catch(Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", "Unable to add endpoint to project.xml document" }
                    });
                return false;
            }

            return true;
        }

        /// <summary>
        /// Updates the EndPoint in the project document for the given EndPoint ID
        /// </summary>
        public static bool UpdateEndPointInDoc(XDocument doc, EndPoint endPoint)
        {
            try
            {
                // Find the EndPoint
                var updatedEndPoint = doc.Root.Descendants("endpoint").First(e => e.Attribute(ID).Value == endPoint.ID.ToString());

                // Update any changed EndPoint details
                if (updatedEndPoint.Attribute(NAME).Value != endPoint.Name)
                    updatedEndPoint.Attribute(NAME).SetValue(endPoint.Name);
                if (updatedEndPoint.Attribute(URL).Value != endPoint.EndPointURI)
                    updatedEndPoint.Attribute(URL).SetValue(endPoint.EndPointURI);
                if (updatedEndPoint.Attribute(PARAM_FILL).Value != endPoint.ParameterFillIn)
                    updatedEndPoint.Attribute(PARAM_FILL).SetValue(endPoint.ParameterFillIn);
                if (updatedEndPoint.Attribute(FILTER_DEF).Value != endPoint.FilterDefinition)
                    updatedEndPoint.Attribute(FILTER_DEF).SetValue(endPoint.FilterDefinition);

                // replace the existing EndPoint node with this updated one
                doc.Root.Descendants("endpoint").First(p => p.Attribute(ID).Value == endPoint.ID.ToString()).ReplaceWith(updatedEndPoint);
            }
            catch(Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", "Unable to update endpoint in project.xml document" }
                    });
                return false;
            }

            return true;
        }
            
        /// <summary>
        /// Removes the specified EndPoint from the project document
        /// </summary>
        public static bool RemoveEndPointFromDoc(XDocument doc, Guid endPointID)
        {
            try
            {
                // remove the first EndPoint with this ID from the file
                doc.Root.Descendants("endpoint").First(p => p.Attribute(ID).Value == endPointID.ToString()).Remove();
            }
            catch(Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", "Unable to remove endpoint from project.xml document" }
                    });
                return false;
            }

            return true;

        }



        /// <summary>
        /// Adds the Result to the project document, and updates the Project and EndPoint Test Date, EndPoint Status, and any changed EndPoint details
        /// </summary>
        public static bool AddResultAndUpdateEndPointInDoc(XDocument doc, Guid projectID, EndPoint endPoint, Result result, TestStatus status)
        {
            try
            {
                // Find the Project
                var updatedProject = doc.Root.Descendants("project").First(p => p.Attribute(ID).Value == projectID.ToString());
                updatedProject.Attribute(LAST_TEST).SetValue(result.RunDate.ToString()); // Update the Project last tested date

                // Find the EndPoint
                var updatedEndPoint = updatedProject.Descendants("endpoint").First(e => e.Attribute(ID).Value == endPoint.ID.ToString());
                updatedEndPoint.Attribute(LAST_TEST).SetValue(result.RunDate.ToString()); // Update the EndPoint run time
                updatedEndPoint.Attribute(STATUS).SetValue((int)status); // Update the EndPoint status. Overall project status is driven by these EndPoint statuses.

                // Save any changed EndPoint details while we're at it - why not.
                if (updatedEndPoint.Attribute(NAME).Value != endPoint.Name)
                    updatedEndPoint.Attribute(NAME).SetValue(endPoint.Name);
                if (updatedEndPoint.Attribute(URL).Value != endPoint.EndPointURI)
                    updatedEndPoint.Attribute(URL).SetValue(endPoint.EndPointURI);
                if (updatedEndPoint.Attribute(PARAM_FILL).Value != endPoint.ParameterFillIn)
                    updatedEndPoint.Attribute(PARAM_FILL).SetValue(endPoint.ParameterFillIn);
                if (updatedEndPoint.Attribute(FILTER_DEF).Value != endPoint.FilterDefinition)
                    updatedEndPoint.Attribute(FILTER_DEF).SetValue(endPoint.FilterDefinition);

                // Find the list of Results and add new one to top of collection
                updatedEndPoint.Descendants("results").First().AddFirst(
                    new XElement("result",
                        new XAttribute(RUN_DATE,      result.RunDate == null ? string.Empty : result.RunDate.ToString()),
                        new XAttribute(HTTP_CODE,     result.HttpCode == null ? 0 : (int)result.HttpCode),
                        new XAttribute(RESPONSE_TIME, result.ResponseTime == null ? string.Empty : result.ResponseTime.ToString())
                    ));

                // replace the existing project node with this updated one
                doc.Root.Descendants("project").First(p => p.Attribute(ID).Value == projectID.ToString()).ReplaceWith(updatedProject);
            }
            catch(Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", "Unable to add result and update endpoint in project.xml document" }
                    });
                return false;
            }

            return true;
        }


        /// <summary>
        /// Removes every result node from the project document for the given Project ID, resets status and test dates
        /// </summary>
        public static bool RemoveAllResultsForProjectFromDoc(XDocument doc, Guid projectID)
        {
            try 
            {
                var updatedProject = doc.Root.Descendants("project").First(e => e.Attribute(ID).Value == projectID.ToString());

                // clear project test status and last test date
                updatedProject.Attribute(STATUS).SetValue(((int)TestStatus.Untested).ToString());
                updatedProject.Attribute(LAST_TEST).SetValue(string.Empty);

                // clear each endPoint status and last test date
                foreach (var endPoint in updatedProject.Descendants("endpoint"))
                {
                    endPoint.Attribute(STATUS).SetValue(((int)TestStatus.Untested).ToString());
                    endPoint.Attribute(LAST_TEST).SetValue(string.Empty);
                }

                // remove all the result nodes from the project
                updatedProject.Descendants("result").Remove();
            }
            catch(Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", "Unable to remove results from XML document" }
                    });
                return false;
            }

            return true;
        }


        /// <summary>
        /// Returns the entire project node for the given Project ID
        /// </summary>
        public static XElement ExportProjectFromDoc(XDocument doc, Guid projectID)
        {
            try 
            {
                var projectElement = doc.Root.Descendants("project").First(p => p.Attribute(ID).Value == projectID.ToString());

                return projectElement;
            }
            catch(Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", $"Unable to export project (ID: {projectID}) from project.xml document" }
                    });
                return null;
            }
        }


        /// <summary>
        /// Change the Project and EndPoint IDs for export. These all need to be unique
        /// </summary>
        public static bool ChangeProjectAndEndPointIDs(XDocument doc, Guid oldProjectID)
        {
            try 
            {
                // Change the Project ID
                var updatedProject = doc.Root.Descendants("project").First(e => e.Attribute(ID).Value == oldProjectID.ToString());
                var newProjectID = Guid.NewGuid().ToString();
                updatedProject.Attribute(ID).SetValue(newProjectID);

                // Change all the EndPoint IDs
                foreach (var endPoint in updatedProject.Descendants("endpoint"))
                    endPoint.Attribute(ID).SetValue(Guid.NewGuid().ToString());

                return true;
            }
            catch(Exception ex)
            {
                Insights.Report(ex, new Dictionary<string, string>
                    {
                        { "Issue", $"Unable to change IDs in exported project node. Project ID: {oldProjectID}" }
                    });
                return false;
            }
        }


        #region XAttribute constants

        private const string ID = "ID";
        private const string NAME = "name";
        private const string DESCRIPTION = "desc";
        private const string USERNAME = "user";
        private const string PASSWORD = "pass";
        private const string BASE_URL = "baseURL";
        private const string STATUS = "status";
        private const string LAST_TEST = "lastDt";
        private const string URL = "url";
        private const string VERB = "verb";
        private const string PARAM_FILL = "pFill";
        private const string FILTER_DEF = "fltrD";
        private const string RUN_DATE = "runDt";
        private const string HTTP_CODE = "httpCd";
        private const string RESPONSE_TIME = "respTime";

        #endregion XAttribute constants

    }
}

