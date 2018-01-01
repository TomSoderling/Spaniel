using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using ModernHttpClient;
using Plugin.Connectivity;
using Spaniel.Shared;
using Spaniel.Shared.Data;
using Spaniel.Shared.Infrastructure;
using Spaniel.Shared.Models;
using Spaniel.Shared.Services;
using Spaniel.Shared.ViewModels;
using Xamarin.Forms;

namespace Spaniel.ViewModels
{
    public class EndPointViewModel : BaseViewModel
    {
        // dependency service for DI.
        readonly IDependencyService dependencyService;

        /// <summary>
        /// Constructor for the EndPoint Master page
        /// </summary>
        public EndPointViewModel(IDependencyService dependencyService, Project selectedProject)
        {
            // Show message when disconnected
            if (IsDisconnected)
                DisplayErrorMessage("No Network Connection");

            // Inform UI to get new connection status when the connectivity changes
            CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
            {
                if (IsDisconnected)
                    DisplayErrorMessage("No Network Connection");
                else
                    ClearErrorMessage();
            };


            // inject the passed in dependency service
            this.dependencyService = dependencyService;

            _selectedProject = selectedProject;
            EndPoints = selectedProject.EndPoints;

            Icons.SetEndPointIcons(EndPoints);

            // Implement the Commands generically
            ActionMenu = new Command(async () => await OnActionMenu());
            DeleteEndPoint = new Command<EndPoint>(async ep => await OnDeleteEndPoint(ep));
            RunAllTests = new Command(async () => await OnRunAllTests());
            Search = new Command(SearchEndPointList);
            DuplicateEndPoint = new Command<EndPoint>(OnDuplicateEndPoint);
            CleanUpResponseBodies = new Command(OnCleanUpResponseBodies);
        }

        /// <summary>
        /// Constructor for the EndPoint Detail page
        /// </summary>
        public EndPointViewModel(IDependencyService dependencyService, Project selectedProject, EndPoint selectedEndPoint)
        {
            // inject the passed in dependency service
            this.dependencyService = dependencyService;

            _selectedEndPoint = selectedEndPoint;
            _selectedProject = selectedProject;

            // set the header box icon as we go to the detail page
            Icons.SetEndPointStatusIcon(SelectedEndPoint);

            if (selectedEndPoint.ID == Guid.Empty) // indicates a newly created EndPoint
                AreDetailsDirty = true; // this will cause the "Save" toolbar button to show

            // Implement the Commands generically
            RunOrSave = new Command(async () => await OnRunOrSave());
            CleanUpResponseBodies = new Command(OnCleanUpResponseBodies);
        }


        #region Properties

        // Command properties
        public ICommand ActionMenu { get; }
        public ICommand DeleteEndPoint { get; }
        public ICommand RunOrSave { get; }
        public ICommand RunAllTests { get; }
        public ICommand Search { get; }
        public ICommand DuplicateEndPoint { get; }
        public ICommand CleanUpResponseBodies { get; }

        public ObservableCollection<EndPoint> FilteredEndPoints
        {
            get
            {
                if (_searchText.Length > 2) // search when 3 or more characters have been entered in search box
                {
                    var endPointList = new ObservableCollection<EndPoint>();

                    if (EndPoints != null)
                    {
                        // use search text to filter list of EndPoints by name
                        var filteredList =
                            (
                                from e in EndPoints
                                where e.Name.ToLower().Contains(_searchText.ToLower())
                                select e
                            ).ToList<EndPoint>();

                        if (filteredList != null && filteredList.Any())
                            endPointList = new ObservableCollection<EndPoint>(filteredList);
                    }

                    return endPointList;
                }
                else
                {
                    return _endPoints;
                }
            }
        }

        private ObservableCollection<EndPoint> _endPoints;
        public ObservableCollection<EndPoint> EndPoints
        {
            get { return _endPoints; }
            set
            {
                if (_endPoints != value)
                {
                    _endPoints = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Project _selectedProject;
        public Project SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                if (_selectedProject != value)
                {
                    _selectedProject = value;
                    RaisePropertyChanged();
                }
            }
        }

        private EndPoint _selectedEndPoint;
        public EndPoint SelectedEndPoint
        {
            get { return _selectedEndPoint; }
            set
            {
                if (_selectedEndPoint != value)
                {
                    _selectedEndPoint = value;
                    RaisePropertyChanged();

                    if (_selectedEndPoint != null)
                    {
                        var navService = dependencyService.Get<INavigationService>();
                        if (navService != null)
                            navService.GoToPageAsync(AppPage.EndPointDetailPage, SelectedProject, dependencyService, _selectedEndPoint).IgnoreResult();
                    }
                }
            }
        }

        private Result _selectedResult;
        public Result SelectedResult
        {
            get { return _selectedResult; }
            set
            {
                _selectedResult = value;
                RaisePropertyChanged();

                if (_selectedResult != null)
                {
                    var navService = dependencyService.Get<INavigationService>();
                    if (navService != null)
                        navService.GoToPageAsync(AppPage.ResultDetailPage, SelectedResult).IgnoreResult();
                }
            }
        }

        public bool HasResults { get { return SelectedEndPoint.Results.Any(); } }

        private bool _areDetailsDirty;
        public bool AreDetailsDirty
        {
            get { return _areDetailsDirty; }
            set
            {
                _areDetailsDirty = value;
                RaisePropertyChanged("ToolbarButtonText"); // notify the view to update the toolbar button text
            }
        }

        public string ToolbarButtonText { get { return AreDetailsDirty ? "Save" : "Run"; } }
        public bool IsDisconnected { get { return !CrossConnectivity.Current.IsConnected; } }
        public Color ErrorLabelColor { get { return Color.FromHex(Constants.SpanielYellow); } }

        // Returns the formatted last tested date, "Untested" if date is null, or "Running" if test is currenly being run
        public string LastTestedValue
        {
            get
            {
                if (SelectedEndPoint.Status == TestStatus.Running)
                {
                    return TestStatus.Running.ToString();
                }
                else
                {
                    if (SelectedEndPoint.LastTested != null)
                        return string.Format(SelectedEndPoint.LastTested.ToString(), "{0:MM/dd/yyyy h:mm tt}");
                    else
                        return "Untested";
                }
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value ?? string.Empty; // handle when the search box cancel button is selected
                    RaisePropertyChanged();

                    Search.Execute(null); // this is effectively the MVVM equivalent of a TextChanged event on the search box
                }
            }
        }

        private bool _isRunningEndPoints;
        public bool IsRunningEndPoints
        {
            get { return _isRunningEndPoints; }
            set
            {
                if (_isRunningEndPoints != value)
                {
                    _isRunningEndPoints = value;
                    RaisePropertyChanged();
                }
            }
        }

        private decimal _runProgress;
        /// <summary>
        /// Progress value between 0 and 1 for progress bar indicator
        /// </summary>
        public decimal RunProgress
        {
            get { return _runProgress; }
            set
            {
                if (_runProgress != value)
                {
                    _runProgress = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion Properties


        #region Command Implementations

        /// <summary>
        /// Add or run an Endpoint
        /// </summary>
        private async Task OnActionMenu()
        {
            const string addActionText = "Add Endpoint";
            const string runAction = "Run All Endpoints";
            const string runActionFiltered = "Run Select Endpoints";
            const string deleteResultsAction = "Clear All Results";
            const string exportProjectAction = "Export This Project";
            string runActionText = runAction;

            if (!string.IsNullOrWhiteSpace(SearchText)) // change text if the end point list is filtered
                runActionText = runActionFiltered;

            // Display the Action Sheet
            var actionSheetService = dependencyService.Get<IActionSheetVisualizerService>();
            var action = await actionSheetService.ShowActionSheetAsync(null, "Cancel", deleteResultsAction, new[] { addActionText, runActionText, exportProjectAction });

            // Perform the selected action
            switch (action)
            {
                case addActionText:
                    // Create new EndPoint.  Setting it as the SelectedEndPoint will Navigate to the EndPoint detail page and show the newly created EndPoint
                    SelectedEndPoint = new EndPoint()
                    {
                        EndPointURI = "/",
                        FilterDefinition = string.Empty,
                        HttpVerb = "GET", // TODO: currently, only GET requests are supported
                        Icon = Icons.NotSelected,
                        Name = string.Empty,
                        ParameterFillIn = string.Empty,
                        Status = TestStatus.Untested,
                    };

                    // Add it to the collection of Endpoints
                    EndPoints.Add(SelectedEndPoint);
                    RaisePropertyChanged("FilteredEndPoints"); // let UI know to update list

                    break;

                case runAction:
                case runActionFiltered:
                    await OnRunAllTests();
                    break;

                case deleteResultsAction:
                    // Delete results for all Endpoints in this Project, reset statuses and test dates
                    foreach (var endPoint in SelectedProject.EndPoints)
                    {
                        endPoint.Status = TestStatus.Untested;
                        endPoint.LastTested = null;
                        endPoint.Results.Clear();
                    }
                    SelectedProject.TestStatus = TestStatus.Untested;
                    SelectedProject.LastTestRun = null;

                    // Delete from projects.xml file, reset statuses and test dates
                    var success = DataAccess.DeleteAllResultsForProject(dependencyService, SelectedProject.ID);

                    if (!success)
                        DisplayErrorMessage("Failed to save project. Error logged");


                    // Update icons
                    Icons.SetEndPointIcons(SelectedProject.EndPoints);
                    Icons.SetProjectIcon(SelectedProject);


                    // TODO: log this with App Center
                    // Track if users use this feature very often                     //Insights.Track("RemoveAllResults", new Dictionary<string, string>
                        //{
                        //    { "ProjectName", SelectedProject.Name },
                        //});

                    break;

                case exportProjectAction:

                    // get this Project node from the Project.xml file, minus any results data, and with new Guid IDs
                    var projectXmlData = DataAccess.FetchProjectXMLForExport(dependencyService, SelectedProject.ID);

                    var filename = SelectedProject.Name.Trim() + Constants.ProjectExportFileExtension;
                    var shareService = dependencyService.Get<IShareService>();
                    shareService.Share(projectXmlData, filename, SelectedProject.Name);

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Delete a selected EndPoint
        /// </summary>
        private async Task OnDeleteEndPoint(EndPoint endPoint)
        {
            var deleteEndPoint = true;

            if (!EndPointIsNewAndEmpty(endPoint)) // check to see if this EndPoint is empty and brand spanking new
            {
                // Alert user for confirmation first 
                var messageService = dependencyService.Get<IMessageVisualizerService>();
                deleteEndPoint = await messageService.ShowMessageAsync("Are you sure?", "Are you sure you want to delete endpoint: " + endPoint.Name + "?", "Yes", "No");
            }

            if (deleteEndPoint)
            {
                // remove from list
                EndPoints.Remove(endPoint);
                SelectedEndPoint = null;
                RaisePropertyChanged("FilteredEndPoints"); // let UI know to update list

                if (Device.Idiom != TargetIdiom.Phone)
                {
                    // set Detail to placeholder page
                    var navService = dependencyService.Get<INavigationService>();
                    navService.SetPlaceholderDetailPage();
                }

                // remove from file
                if (endPoint.ID != Guid.Empty) // if ID is empty, it's never been saved before; don't try to remove from file.
                {
                    var success = DataAccess.DeleteEndPoint(dependencyService, endPoint.ID);

                    if (!success)
                        DisplayErrorMessage("Failed to save project. Error logged");
                }
            }
        }

        /// <summary>
        /// Run test or Save/Update selected EndPoint
        /// </summary>
        private async Task OnRunOrSave()
        {
            if (IsDisconnected)
            {
                // Alert user if device is offline
                var messageService = dependencyService.Get<IMessageVisualizerService>();
                await messageService.ShowMessageAsync("No Network Connection", "Unable to run endpoint tests while offline", "OK");
            }
            else
            {
                if (AreDetailsDirty) // toolbar button text should be "Save"
                {
                    // Save or Update EndPoint
                    if (SelectedEndPoint.ID == Guid.Empty) // indicates a newly created EndPoint
                    {
                        // Save new EndPoint
                        SelectedEndPoint.ID = Guid.NewGuid(); // Create ID for the new EndPoint
                        var success = DataAccess.SaveNewEndPoint(dependencyService, SelectedProject.ID, SelectedEndPoint);

                        if (!success)
                            DisplayErrorMessage("Failed to save project. Error logged");
                    }
                    else
                    {
                        // Update existing EndPoint
                        var success = DataAccess.UpdateEndPoint(dependencyService, SelectedEndPoint);

                        if (!success)
                            DisplayErrorMessage("Failed to save project. Error logged");
                    }

                    AreDetailsDirty = false;

                }
                else
                {
                    // Run Test

                    // using ModernHttpClient. In testing, this improves the avg response times by 40%
                    // Note: as of iOS 9, App Transport Security (ATS) is enabled by default, requiring internet connections to be secure.
                    // Since we don't know if a Base URL entered by the user will be secure or not, we've opted to disable ATS app-wide.  See Info.plist file for setting.
                    var client = new HttpClient(new NativeMessageHandler());
                    client.BaseAddress = new Uri(SelectedProject.BaseURL);

                    // TODO: currently, only GET requests are supported
                    if (SelectedEndPoint.HttpVerb == "GET")
                    {
                        var result = await ExecuteGetRequest(client, SelectedProject, SelectedEndPoint);

                        SelectedEndPoint.Results.Insert(0, result); // add result to the front/top of the collection
                        RaisePropertyChanged("HasResults"); // let the ListView know to show

                        // Save the Result and update any changed EndPoint details
                        var success = DataAccess.SaveTestResultAndEndPoint(dependencyService, SelectedProject.ID, SelectedEndPoint, result, SelectedEndPoint.Status);

                        if (!success)
                            DisplayErrorMessage("Failed to save project. Error logged");
                    }

                    UpdateProjectTestStatus();


                    // TODO: log this with App Center
                    // Track this action
                    //Insights.Track("RunSingleEndPoint", new Dictionary<string, string>
                        //{
                        //    { "ProjectName", SelectedProject.Name },
                        //    { "EndPointName", SelectedEndPoint.Name }
                        //});


                    // TODO: try reloading the Project list so the most recently tested project appears on top (sorting is being done somewhere...)
                }
            }
        }

        /// <summary>
        /// Run all tests for EndPoints in the selected Project
        /// </summary>
        private async Task OnRunAllTests()
        {
            if (IsDisconnected)
            {
                // Alert user if device is offline
                var messageService = dependencyService.Get<IMessageVisualizerService>();
                await messageService.ShowMessageAsync("No Network Connection", "Unable to run endpoint tests while offline", "OK");
            }
            else
            {
                // TODO: log this with App Center
                // Track the time it takes to run all EndPoints
                //var trackHandle = Insights.TrackTime("TimeToRunAllEndPoints");
                //trackHandle.Start();

                var client = new HttpClient(new NativeMessageHandler());
                client.BaseAddress = new Uri(SelectedProject.BaseURL);

                var endPointCount = 0;
                RunProgress = 0; // reset progress bar
                IsRunningEndPoints = true;
                foreach (var endPoint in FilteredEndPoints) // Only run tests for the visible end points in the list
                {
                    // TODO: currently, only GET requests are supported
                    if (endPoint.HttpVerb == "GET")
                    {
                        if (endPoint.ID == Guid.Empty) // if it's a brand new EndPoint, save it first
                        {
                            var success = DataAccess.SaveNewEndPoint(dependencyService, SelectedProject.ID, endPoint);

                            if (!success)
                                DisplayErrorMessage("Failed to save project. Error logged");
                        }

                        endPointCount++;

                        var result = await ExecuteGetRequest(client, SelectedProject, endPoint);

                        // update progress bar
                        RunProgress = Convert.ToDecimal(endPointCount) / Convert.ToDecimal(FilteredEndPoints.Count);

                        endPoint.Results.Insert(0, result); // add result to the front/top of the collection
                        RaisePropertyChanged("HasResults"); // let the result ListView know it should be displayed

                        // Save the Result and update any changed EndPoint details
                        var success2 = DataAccess.SaveTestResultAndEndPoint(dependencyService, SelectedProject.ID, endPoint, result, endPoint.Status);

                        if (!success2)
                            DisplayErrorMessage("Failed to save project. Error logged");
                    }
                }

                // TODO: log this with App Center
                //trackHandle.Stop();

                UpdateProjectTestStatus();

                // TODO: log this with App Center
                // Track this action
                //Insights.Track("RunAllEndPoints", new Dictionary<string, string>
                    //{
                    //    { "ProjectName", SelectedProject.Name },
                    //    { "NumberOfEndPoints", SelectedProject.EndPoints.Count.ToString() }
                    //});


                // Hide progress bar
                IsRunningEndPoints = false;
            }
        }

        /// <summary>
        /// Raises property changed for the EndPoint list property, which searches the end point list
        /// </summary>
        private void SearchEndPointList()
        {
            // Refresh the filtered end point list, which will automatically apply the seach text
            RaisePropertyChanged("FilteredEndPoints");
        }

        /// <summary>
        /// Duplicate the selected EndPoint
        /// </summary>
        private void OnDuplicateEndPoint(EndPoint endPoint)
        {
            // duplicate the currenly selected EndPoint
            var newEndPoint = new EndPoint()
            {
                Name = endPoint.Name,
                EndPointURI = endPoint.EndPointURI,
                HttpVerb = endPoint.HttpVerb,
                Status = TestStatus.Untested,
                Icon = Icons.NotSelected,
                FilterDefinition = endPoint.FilterDefinition,
                ParameterFillIn = endPoint.ParameterFillIn
            };

            var endPointIndex = EndPoints.IndexOf(endPoint); // get list index of the currently selected EndPoint

            // Change selection and navigate to the new duplicate EndPoint
            SelectedEndPoint = newEndPoint;

            // Insert duplicate directly below the previously selected EndPoint
            EndPoints.Insert(endPointIndex + 1, SelectedEndPoint);
            RaisePropertyChanged("FilteredEndPoints"); // let UI know to update list


            // TODO: log this with App Center
            // Track if users are using this feature
            //Insights.Track("DuplicateEndPoint");
        }


        /// <summary>
        /// Cleans up memory used by response bodies of the EndPoint Results
        /// </summary>
        private void OnCleanUpResponseBodies()
        {
            foreach (var endPoint in SelectedProject.EndPoints)
            {
                foreach (var result in endPoint.Results)
                {
                    // clear out any non-empty response body
                    if (!string.IsNullOrWhiteSpace(result.ResponseBody))
                        result.ResponseBody = null;
                }
            }
        }

        #endregion Command Implementations


        /// <summary>
        /// Execute a GET request for the provided EndPoint, update appropriate icon properties
        /// </summary>
        private async Task<Result> ExecuteGetRequest(HttpClient client, Project project, EndPoint endPoint)
        {
            // set status and icon properties
            endPoint.Status = TestStatus.Running;
            RaisePropertyChanged("LastTestedValue"); // update endPoint detail icon, driven off the endPoint status
            Icons.SetEndPointIcon(endPoint);
            Icons.SetEndPointStatusIcon(SelectedEndPoint);


            var response = new HttpResponseMessage();
            var responseString = string.Empty;
            var exceptionThrown = false;
            var timer = new Stopwatch();
            var endPointUri = endPoint.EndPointURI;
            var exceptionMsg = string.Empty;

            // add in any parameter values
            var hasParameters = !string.IsNullOrWhiteSpace(endPoint.ParameterFillIn);
            if (hasParameters)
                endPointUri = ReplaceParameterPlaceholdersWithValues(endPoint);

            // append any filter definition
            var hasFilter = !string.IsNullOrWhiteSpace(endPoint.FilterDefinition);
            if (hasFilter)
                endPointUri += endPoint.FilterDefinition.Trim();

            try
            {
                timer.Start();
                // TODO: currently, only GET requests are supported
                response = await client.GetAsync(endPointUri);
                timer.Stop();

                responseString = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                exceptionMsg = ex.Message;
            }

            var lastTestTime = DateTime.Now;
            endPoint.LastTested = lastTestTime;
            project.LastTestRun = lastTestTime;

            if (!exceptionThrown && response.IsSuccessStatusCode)
                endPoint.Status = TestStatus.Successful;
            else
                endPoint.Status = TestStatus.Failed;

            // Create Result object
            var result = new Result()
            {
                HttpCode = exceptionThrown ? 0 : response.StatusCode,  // show "0" as status code when exception is thrown
                Icon = "",
                RunDate = lastTestTime,
                ResponseBody = responseString,
                ResponseTime = timer.Elapsed
            };

            if (exceptionThrown)
                result.ExceptionMessage = exceptionMsg;


            // Update icons
            RaisePropertyChanged("LastTestedValue");
            Icons.SetResultIcon(result);
            Icons.SetEndPointIcon(endPoint);
            Icons.SetEndPointStatusIcon(SelectedEndPoint);

            return result;
        }

        /// <summary>
        /// Determine the status of the SelectedProject, set the TestStatus, and save to file
        /// </summary>
        private void UpdateProjectTestStatus()
        {
            var oldStatus = SelectedProject.TestStatus;

            if (SelectedProject.EndPoints.Any(e => e.Status == TestStatus.Failed))
            {
                // determine if partial success or all failed
                if (SelectedProject.EndPoints.Any(e => e.Status == TestStatus.Successful))
                    SelectedProject.TestStatus = TestStatus.PartialSuccess;
                else
                    SelectedProject.TestStatus = TestStatus.Failed;

            }
            else
            {
                if (SelectedProject.EndPoints.Any(e => e.Status != TestStatus.Successful))
                    SelectedProject.TestStatus = TestStatus.Untested; // set to untested
                else
                    SelectedProject.TestStatus = TestStatus.Successful; // all have passed  
            }

            // Update if status has changed
            if (oldStatus != SelectedProject.TestStatus)
            {
                Icons.SetProjectIcon(SelectedProject);

                // Save update to file
                var success = DataAccess.SaveProjectStatus(dependencyService, SelectedProject.ID, SelectedProject.TestStatus);

                if (!success)
                    DisplayErrorMessage("Failed to save project. Error logged");
            }
        }

        /// <summary>
        /// Replaces the EndPoint URI parameter placeholders with values
        /// </summary>
        private string ReplaceParameterPlaceholdersWithValues(EndPoint endPoint)
        {
            var endPointUri = endPoint.EndPointURI;

            var openingBraces = endPointUri.Split('{').Length - 1;
            var closingBraces = endPointUri.Split('}').Length - 1;

            if (openingBraces == closingBraces) // make sure opening and closing braces match for parameter placeholders
            {
                var paramValues = endPoint.ParameterFillIn.Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (paramValues.Count() == openingBraces) // make sure the number of parameter values match the number parameter placeholders
                {
                    ClearErrorMessage();
                    var newEndpointUri = endPointUri;

                    for (var i = 0; i < paramValues.Count(); i++) // replace the parameter placeholders with values, one by one.
                    {
                        var openingBraceIndex = newEndpointUri.IndexOf("{", StringComparison.CurrentCulture);
                        var closingBraceIndex = newEndpointUri.IndexOf("}", StringComparison.CurrentCulture);
                        var paramDefLength = closingBraceIndex - openingBraceIndex + 1;
                        var paramDefToReplace = newEndpointUri.Substring(openingBraceIndex, paramDefLength);

                        newEndpointUri = newEndpointUri.Replace(paramDefToReplace, paramValues[i].Trim());
                    }

                    return newEndpointUri;
                }
                else
                {
                    // mismatch of parameter values and bracket pairs. this will result in a BadRequest response
                    DisplayErrorMessage("Mismatch of parameter values to brace pairs");
                }
            }
            else
            {
                // mismatch of opening and closing brackets. this will result in a BadRequest response
                DisplayErrorMessage("Mismatch of opening and closing braces");
            }

            return endPointUri;
        }




        /// <summary>
        /// Checks to see if the EndPoint is new/unsaved and completely empty
        /// </summary>
        private bool EndPointIsNewAndEmpty(EndPoint endPoint)
        {
            if (endPoint.EndPointURI != "/")
                return false;
            if (!string.IsNullOrWhiteSpace(endPoint.FilterDefinition))
                return false;
            if (endPoint.ID != Guid.Empty)
                return false;
            if (!string.IsNullOrWhiteSpace(endPoint.Name))
                return false;
            if (!string.IsNullOrWhiteSpace(endPoint.ParameterFillIn))
                return false;
            if (endPoint.Results.Count > 0)
                return false;

            return true;
        }
    }
}

