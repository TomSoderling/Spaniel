using System;
using WebServiceDashboard.Shared.Models;
using System.Linq;
using System.Windows.Input;
using WebServiceDashboard.Shared.Services;
using WebServiceDashboard.Shared.Infrastructure;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using WebServiceDashboard.Shared.ViewModels;
using WebServiceDashboard.Shared;
using WebServiceDashboard.Shared.Data;
using System.Diagnostics;
using System.Collections.Generic;
using Xamarin;
using Plugin.Connectivity;

namespace WebServiceDashboard.ViewModels
{
    public class ProjectViewModel : BaseViewModel
    {
        // dependency service for DI.
        public readonly IDependencyService dependencyService;

        /// <summary>
        /// Constructor for the Project List Page
        /// </summary>
        public ProjectViewModel(IDependencyService dependencyService)
        {
            // Show or hide connection status message whenever the connectivity changes
            CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
                {
                    if (IsDisconnected)
                        DisplayErrorMessage("No Network Connection");
                    else
                        ClearErrorMessage();
                };

            // inject the passed in dependency service
            this.dependencyService = dependencyService;


            // Load project data from projects.xml file on device.  If that doesn't exist, it will load in the data from the Test Data xml file
            Debug.WriteLine("Loading Project list.");
            var timer = new Stopwatch();
            timer.Start();
            Projects = DataAccess.Load(dependencyService);
            timer.Stop();
            Debug.WriteLine(string.Format("Loaded Project list. Time: {0}", timer.ElapsedMilliseconds));


            // TODO: sort this ObservableCollection somehow
            // show recently tested projects on top as the user tests them
            //Projects = projectList.OrderByDescending(p => p.LastTestRun).ToList();



            // this allows the ViewModel to initialize the view with a known selection state. Allows us to unit test this code without the UI needing to run
            if (Projects != null)
                _selectedProject = Projects.FirstOrDefault();
            else
                DisplayErrorMessage("Unable to load projects from file");


            // Implement the Commands generically.  Use built-in Command and Command<T> to forward command to ViewModel
            // This necessitates this ViewModel being in the XamForms Project because of the Xamarin implementation of ICommand
            AddProject = new Command(async () => await OnAddProject()); // can always be executed, so use Command with 1 parameter
            EditProject = new Command<Project>(async proj => await OnEditProject(proj));
            DeleteProject = new Command<Project>(async proj => await OnDeleteProject(proj));
            CleanUpResponseBodies = new Command(OnCleanUpResponseBodies);

            // Reload the project list when a new Project is imported
            MessagingCenter.Subscribe<IProjectRepository>(this, "Reload", (sender) =>
                {
                    Projects = DataAccess.Load(dependencyService);
                });
        }

        /// <summary>
        /// Constructor for the Project Edit Page
        /// </summary>
        public ProjectViewModel(IDependencyService dependencyService, Project selectedProject)
        {
            // inject the passed in dependency service
            this.dependencyService = dependencyService;

            _selectedProject = selectedProject; // set the selected project, but don't use public property - don't want to RaisePropertyChanged

            // set the header box icon as we go to the detail page
            Icons.SetProjectStatusIcon(SelectedProject);

            Done = new Command(async () => await OnDone());
        }


        #region Properties

        // Command properties
        public ICommand AddProject { get; }
        public ICommand EditProject { get; }
        public ICommand DeleteProject { get; }
        public ICommand Done { get; }
        public ICommand CleanUpResponseBodies { get; }
        public ICommand NavigateToProjectName { get; }

        private ObservableCollection<Project> _projects;
        /// <summary>
        /// A collection of Projects to be displayed in the UI
        /// </summary>
        public ObservableCollection<Project> Projects
        {
            get { return _projects; }
            set
            {
                if (_projects != value)
                {
                    _projects = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Project _selectedProject;
        /// <summary>
        /// Holds the currently selected Project
        /// </summary>
        public Project SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                RaisePropertyChanged();

                if (_selectedProject != null)
                {
                    // Use the NavigationService to navigate to the endpoint list page.  
                    // This removes the dependency on Xamarin.Forms, and now this view doesn't need to know about other views.
                    var navService = dependencyService.Get<INavigationService>();
                    if (navService != null)
                    {
                        navService.GoToPageAsync(AppPage.EndPointListPage, SelectedProject, dependencyService).IgnoreResult();

                        // Keep track of which project the user is using most often, and how many EndPoints are in each Project
                        Insights.Track("ProjectSelected", new Dictionary<string, string>
                            {
                                { "ProjectName", SelectedProject.Name },
                                { "NumberOfEndPoints", SelectedProject.EndPoints.Count.ToString() }
                            });
                    }
                }
            }
        }

        public bool AreDetailsDirty { get; set; }
        public bool IsDisconnected { get { return !CrossConnectivity.Current.IsConnected; } }
        public Color ErrorLabelColor { get { return Color.FromHex(Constants.SpanielYellow); } }

        // Returns the formatted last tested date, "N/A" if date is null
        public string LastTestedValue
        {
            get
            {
                if (SelectedProject.LastTestRun != null)
                    return string.Format(SelectedProject.LastTestRun.ToString(), "{0:MM/dd/yyyy h:mm tt}");
                else
                    return "N/A";
            }
        }

        #endregion Properties


        #region Command Implementations

        /// <summary>
        /// Create a new Project 
        /// </summary>
        private async Task OnAddProject()
        {
            _selectedProject = new Project() // set the selected project, but don't use public property - don't want to RaisePropertyChanged
            {
                ID = Guid.Empty,
                BaseURL = Constants.BaseURLPlaceholder,
                TestStatus = TestStatus.Untested,
                Icon = Icons.ProjectNew,
                Name = string.Empty,
                Description = string.Empty,
                Username = string.Empty,
                Password = string.Empty
            };

            // Navigate to the Project edit page and show the newly created Project
            var navService = dependencyService.Get<INavigationService>();
            await navService.GoToPageAsync(AppPage.ProjectDetailPage, SelectedProject, dependencyService);

            Projects.Add(SelectedProject);
        }

        /// <summary>
        /// Edit a selected project
        /// </summary>
        private async Task OnEditProject(Project project)
        {
            _selectedProject = project;

            // Navigate to the Project edit page with the currently selected Project
            var navService = dependencyService.Get<INavigationService>();
            await navService.GoToPageAsync(AppPage.ProjectDetailPage, SelectedProject, dependencyService);
        }

        /// <summary>
        /// Save or Update the selected Project
        /// </summary>
        private async Task OnDone()
        {
            // show Master/Next page
            var navService = dependencyService.Get<INavigationService>();
            await navService.GoToPageAsync(AppPage.ProjectListPage);

            if (Device.Idiom != TargetIdiom.Phone)
            {
                // clear detail page
                await navService.GoToRootDetailPageAsync();
            }

            // Save or Update
            if (SelectedProject.ID == Guid.Empty)
            {
                // Save new Project
                SelectedProject.ID = Guid.NewGuid();
                var success = DataAccess.SaveNewProject(dependencyService, SelectedProject);

                if (!success)
                    DisplayErrorMessage("Failed to save project. Error logged");
            }
            else
            {
                if (AreDetailsDirty) // only save if Project details have changed
                {
                    // Update existing Project
                    var success = DataAccess.UpdateProject(dependencyService, SelectedProject);

                    if (!success)
                        DisplayErrorMessage("Failed to save project. Error logged");
                }
            }
        }

        /// <summary>
        /// Delete a Project
        /// </summary>
        private async Task OnDeleteProject(Project project)
        {
            var deleteProject = true;

            if (!ProjectIsNewAndEmpty(project)) // check to see if this Project is empty and brand spanking new
            {
                // Alert user for confirmation first
                var messageService = dependencyService.Get<IMessageVisualizerService>();
                deleteProject = await messageService.ShowMessageAsync("Are you sure?", "Are you sure you want to delete project: " + project.Name + "?", "Yes", "No");
            }

            if (deleteProject)
            {
                // remove from list
                Projects.Remove(project);
                SelectedProject = null;

                if (Device.Idiom != TargetIdiom.Phone)
                {
                    // clear Detail Page
                    var navService = dependencyService.Get<INavigationService>();
                    await navService.GoToRootDetailPageAsync();
                }

                // remove from file
                if (project.ID != Guid.Empty) // if ID is empty, it's never been saved before; don't try to remove from file.
                {
                    var success = DataAccess.DeleteProject(dependencyService, project.ID);

                    if (!success)
                        DisplayErrorMessage("Failed to save project. Error logged");
                }
            }
        }


        /// <summary>
        /// Checks to see if the project is new/unsaved and completely empty
        /// </summary>
        private bool ProjectIsNewAndEmpty(Project project)
        {
            if (project.BaseURL != Constants.BaseURLPlaceholder)
                return false;
            if (!string.IsNullOrWhiteSpace(project.Description))
                return false;
            if (project.EndPoints.Count > 0)
                return false;
            if (project.ID != Guid.Empty)
                return false;
            if (!string.IsNullOrWhiteSpace(project.Name))
                return false;
            if (!string.IsNullOrWhiteSpace(project.Password))
                return false;
            if (!string.IsNullOrWhiteSpace(project.Username))
                return false;

            return true;
        }

        /// <summary>
        /// Cleans up memory used by response bodies of the EndPoint Results
        /// </summary>
        private void OnCleanUpResponseBodies()
        {
            foreach (var project in Projects)
            {
                foreach (var endPoint in project.EndPoints)
                {
                    foreach (var result in endPoint.Results)
                    {
                        // clear out any non-empty response body
                        if (!string.IsNullOrWhiteSpace(result.ResponseBody))
                            result.ResponseBody = null;
                    }
                }
            }
        }

        #endregion Command Implementations
    }
}

