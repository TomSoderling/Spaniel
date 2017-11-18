using System;
using System.Linq;
using NUnit.Framework; // as of 11/9/2015 - NUnit version >= 3 is not supported by Xamarin.UITest
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using WebServiceDashboard.Shared;
using System.Threading;
using WebServiceDashboard.Shared.Data;
using WebServiceDashboard.Shared.Services;
using WebServiceDashboard.Services;
using WebServiceDashboard.ViewModels;

namespace WebServiceDashboard.UITests
{
    // Analysis disable InconsistentNaming

    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);


            // open the REPL window before each test
            //app.Repl();
        }


        #region Project List/Detail Page

        /// <summary>
        /// Confirm the project list page appears
        /// </summary>
        [Test]
        public void t_01_ProjectListPage_IsDisplayed()
        {
            // Look for the Project List View control
            var results = app.WaitForElement("projectListView");
            app.Screenshot("Project List Page");

            Assert.IsTrue(results.Any());
        }

        /// <summary>
        /// Confirm context actions are displayed on the project list page
        /// </summary>
        [Test]
        public void t_02_ProjectListPageContextActions_IsDisplayed()
        {
            // New way of doing this

            // Create instance of helper class - used to perform actions (Methods) and test results (Properties)
            var projectListPage = new ProjectListPage(app, this.platform);

            // reveal context actions on the "iFactr Customers" list item.  This will always be there in the test data
            projectListPage.ShowContextActions("iFactr Customers");

            // look for a single details button
            // on iPad, there are 2 iems marked "Details", a UIButton and a UIButtonLabel.  This will choose the label with the text
            Assert.AreEqual(1, projectListPage.DetailsContextAction.Count(), 
                string.Format("Looking for {0} Details context action button, but found {1}", 1, projectListPage.DetailsContextAction.Count())); 

            // look for a single delete button
            Assert.AreEqual(1, projectListPage.DeleteContextAction.Count(), 
                string.Format("Looking for {0} Delete context action button, but found {1}", 1, projectListPage.DeleteContextAction.Count()));
        }

        /// <summary>
        /// Confirm the project detail page appears when using context action
        /// </summary>
        [Test]
        public void t_03_ProjectDetailPage_IsDisplayed()
        {
            // look for iFactr Customers list item.  This will always be there in the test data
            TapProjectDetailsContextAction("iFactr Customers");

            // wait for project detail screen
            //var projectDetails = app.Query(c => c.Marked("projectEditPage")); // this only works on large form factor
            var projectDetails = app.WaitForElement("projectNameTextbox"); // look for the project name textbox instead
            app.Screenshot("Project Detail Page");
            Assert.IsTrue(projectDetails.Count() == 1);

            // Tap done and make sure the details screen disappears
            app.Tap("Done");
            app.WaitForNoElement("projectNameTextbox");
        }

        /// <summary>
        /// Confirm the project detail page appears when using add project button
        /// </summary>
        [Test]
        public void t_04_ProjectDetailPage_OnAddProject_IsDisplayed()
        {
            // Tap Add button
            // the id of the ToolbarItem (addProjectButton) isn't accessible, so we have to use the class ID - different for each platform
            if (platform == Platform.Android)
                app.Tap(c => c.Class("ActionMenuView"));
            else
                app.Tap(c => c.Class("UINavigationButton"));

            // wait for project detail screen
            var projectDetails = app.WaitForElement("projectNameTextbox"); // look for the project name textbox
            app.Screenshot("Project Detail Page");
            Assert.IsTrue(projectDetails.Count() == 1);
        }

        /// <summary>
        /// Confirm a new project can be added
        /// </summary>
        [Test]
        public void t_05_ProjectListPage_AddNewProject_Succeeds()
        {
            var projectName = GetUniqueName();
            const string projectDesc = "test";
            const string user = "user";
            const string pass = "pass";
            const string testURL = "test.com";

            app.Screenshot("Project List Page");

            AddNewProject(projectName, projectDesc, user, pass, testURL);

            // wait for project list page
            var results = app.WaitForElement("projectListView");
            app.Screenshot("New Project in List Page");
            Assert.IsTrue(results.Any(), "Expected project list page to be displayed.");

            // look for new project in list
            var result = app.Query(c => c.Marked(projectName));
            Assert.AreEqual(1, result.Count(), string.Format("Expected new project named: '{0}' in list.", projectName));

            // tap the details context action button on new project list item
            TapProjectDetailsContextAction(projectName);
            var newProjectDetails = app.WaitForElement("projectNameTextbox"); // look for the project name textbox
            app.Screenshot("View Details of New Project");
            Assert.IsTrue(newProjectDetails.Count() == 1);


            // validate field values
            Assert.AreEqual(projectName, app.Query(c => c.Marked("projectNameTextbox")).First().Text);
            Assert.AreEqual(projectDesc, app.Query(c => c.Marked("projectDescTextbox")).First().Text);
            Assert.AreEqual(user, app.Query(c => c.Marked("projectUsernameTextbox")).First().Text);
            Assert.AreEqual(pass, app.Query(c => c.Marked("projectPasswordTextbox")).First().Text);
            Assert.AreEqual(Constants.BaseURLPlaceholder + testURL, app.Query(c => c.Marked("projectBaseURLTextbox")).First().Text);
        }

        /// <summary>
        /// Confirm a user alert message is displayed when deleting a project
        /// </summary>
        [Test]
        public void t_06_ProjectListPage_DeleteProject_AlertIsDisplayed()
        {
            TapProjectDeleteContextAction("iFactr Customers");

            var alert = app.WaitForElement("Are you sure?");
            app.Screenshot("User Alert Message");
            Assert.IsTrue(alert.Any(), "Expected user alert message.");

            app.Tap("No");
        }

        /// <summary>
        /// Confirm a project can be deleted
        /// </summary>
        [Test]
        public void t_07_ProjectListPage_DeleteProject_Succeeds()
        {
            var projectName = GetUniqueName();

            app.Screenshot("Project List Page");

            AddNewProject(projectName);

            // wait for project list page
            var projectListPage = app.WaitForElement("projectListView");
            app.Screenshot("New Project in List Page");
            Assert.IsTrue(projectListPage.Any(), "Expected project list page to be displayed.");

            // look for new project in list
            var newProject = app.Query(c => c.Marked(projectName));
            Assert.AreEqual(1, newProject.Count(), string.Format("Expected new project named: '{0}' in list.", projectName));

            // tap delete
            TapProjectDeleteContextAction(projectName);

            // check for alert message
            var alert = app.WaitForElement("Are you sure?");
            app.Screenshot("User alert message");
            Assert.IsTrue(alert.Any(), "Expected user alert message.");

            app.Tap("Yes");

            // look for new project in list
            app.WaitForNoElement(projectName, string.Format("Timed out waiting for project named: '{0}' to be removed from list.", projectName));
            app.Screenshot("Project Deleted from List Page");

            var noProject = app.Query(projectName);
            Assert.AreEqual(0, noProject.Count(), string.Format("Expected project named: '{0}' not to be in list.", projectName));
        }

        /// <summary>
        /// Confirm project details can be edited
        /// </summary>
        [Test]
        public void t_08_ProjectListPage_EditProjectDetails_Succeeds()
        {
            var projectName = GetUniqueName();

            const string projectDesc = "test";
            const string user = "user";
            const string pass = "pass";
            const string testURL = "test.com";

            app.Screenshot("Project List Page");

            AddNewProject(projectName, projectDesc, user, pass, testURL);

            // wait for project list page
            var results = app.WaitForElement("projectListView");
            app.Screenshot("New Project in List Page");
            Assert.IsTrue(results.Any(), "Expected project list page to be displayed.");

            // look for new project in list
            var result = app.Query(c => c.Marked(projectName));
            Assert.AreEqual(1, result.Count(), string.Format("Expected new project named: '{0}' in list.", projectName));

            // tap the details context action button on new project list item
            TapProjectDetailsContextAction(projectName);
            var projectDetails = app.WaitForElement("projectNameTextbox"); // look for the project name textbox
            app.Screenshot("Check Details of Project");
            Assert.IsTrue(projectDetails.Count() == 1);

            // change field values
            const string newPartOfProjectName = " 2";
            const string newProjectDesc = "new test";
            const string newUser = "2";
            const string newPass = "3";
            const string newTestURL = "4";

            app.EnterText("projectNameTextbox", newPartOfProjectName);
            app.ClearText("projectDescTextbox"); // clear old text
            app.EnterText("projectDescTextbox", newProjectDesc);
            app.EnterText("projectUsernameTextbox", newUser); // append to old text
            app.EnterText("projectPasswordTextbox", newPass); // append to old text

            // on iPad this field is covered by the keyboard, dismiss the keyboard so the entry field can be typed in
            app.DismissKeyboard();
            app.ClearText("projectBaseURLTextbox"); // clear old text
            app.EnterText("projectBaseURLTextbox", newTestURL);

            app.Screenshot("Edit the Project Details");
            app.Tap("Done");

            // navigate back to project details
            TapProjectDetailsContextAction(projectName + newPartOfProjectName);
            app.Screenshot("Confirm Edited Project Details");

            var newProjectDetails = app.WaitForElement("projectNameTextbox"); // look for the project name textbox
            Assert.IsTrue(newProjectDetails.Count() == 1);

            // validate updated field values
            Assert.AreEqual(projectName + newPartOfProjectName, app.Query(c => c.Marked("projectNameTextbox")).First().Text);
            Assert.AreEqual(newProjectDesc, app.Query(c => c.Marked("projectDescTextbox")).First().Text);
            Assert.AreEqual(user + newUser, app.Query(c => c.Marked("projectUsernameTextbox")).First().Text);
            Assert.AreEqual(pass + newPass, app.Query(c => c.Marked("projectPasswordTextbox")).First().Text);
            Assert.AreEqual(newTestURL, app.Query(c => c.Marked("projectBaseURLTextbox")).First().Text);


            // is there a way to confirm that the data has been saved to file?
            // this doesn't work - Xamarin.Forms.Init() has to be called before it's used.
//            var dependencyService = new DependencyServiceWrapper();
//            var projectVM = new ProjectViewModel(dependencyService);
//            projectVM.Projects = DataAccess.Load(dependencyService);
//
//            var project = projectVM.Projects.First(p => p.Name == projectName + newPartOfProjectName);
        }

        #endregion Project List/Detail Page


        #region End Point List Page

        /// <summary>
        /// Confirm the EndPoint list page appears when a project is selected
        /// </summary>
        [Test]
        public void t_10_EndPointListPage_IsDisplayed()
        {
            // look for iFactr Customers list item.  This will always be there in the test data
            var projectListItem = app.Query(c => c.Marked("iFactr Customers"));
            app.Screenshot("Project List Page");
            Assert.IsTrue(projectListItem.Any());

            // tap on the item 
            app.Tap("iFactr Customers");

            // wait for EndPoint list view to show
            var endPointListView = app.WaitForElement("endPointListView");
            app.Screenshot("End Point List Page");

            // should only be 1 list view
            Assert.AreEqual(1, endPointListView.Count());
        }

        /// <summary>
        /// Confirm context actions are displayed on the end point list page
        /// </summary>
        [Test]
        public void t_11_EndPointListPage_ContextActions_AreDisplayed()
        {
            // Go to EndPoint list page for iFactr Customers project.
            app.Tap("iFactr Customers");
            app.Screenshot("End Point List Page");

            // reveal context action for first EndPoint in list
            RevealListItemContextActions("Get Customers - json");

            // look for the delete button
            var deleteContextAction = app.WaitForElement(c => c.Marked("Delete").Text("Delete"));
            Assert.AreEqual(1, deleteContextAction.Count(), 
                string.Format("Looking for {0} Delete context action button, but found {1}", 1, deleteContextAction.Count()));

            // look for the duplicate button
            var duplicateContextAction = app.WaitForElement(c => c.Marked("Duplicate").Text("Duplicate"));
            Assert.AreEqual(1, duplicateContextAction.Count(), 
                string.Format("Looking for {0} Duplicate context action button, but found {1}", 1, duplicateContextAction.Count()));
        }

        /// <summary>
        /// Confirm that an end point can be duplicated
        /// </summary>
        [Test]
        public void t_12_EndPointListPage_DuplicateEndPoint_Succeeds()
        {
            const string expectedEndPointName = "Get Customers - json";
            var endPointListPage = new EndPointListPage(app, this.platform);
            var numberOfListItemsBefore = endPointListPage.NumberOfEndPointListItems;

            endPointListPage.DuplicateEndPoint(expectedEndPointName);
            endPointListPage.SaveEndPointDetails();

            Assert.AreEqual(expectedEndPointName, endPointListPage.EndPointNameText); // duplicated end point name should match

            app.DismissKeyboard();
            Assert.IsTrue(numberOfListItemsBefore < endPointListPage.NumberOfEndPointListItems); // should be more list items than before
        }

        /// <summary>
        /// Confirm that an end point can be deleted
        /// </summary>
        [Test]
        public void t_13_EndPointListPage_DeleteEndPoint_Succeeds()
        {
            const string existingEndPointName = "Get Customers - json";
            const string textToAppend = " 2";
            const string newEndPointName = "Get Customers - json" + textToAppend;
            var endPointListPage = new EndPointListPage(app, this.platform);

            // first, duplicate an end point that can be deleted
            endPointListPage.DuplicateEndPoint(existingEndPointName);

            // change name, save, then delete
            endPointListPage.EndPointNameText = textToAppend;
            endPointListPage.SaveEndPointDetails();
            endPointListPage.TapDeleteButton(newEndPointName);

            // ensure alert message is displayed
            Assert.IsTrue(endPointListPage.AlertMessageIsDisplayed, "Alert message on Delete action was not displayed");

            endPointListPage.ConfirmAlertMessage(); // confirm to delete
            endPointListPage.WaitForDetailPageToDissapear();

            endPointListPage.WaitForEndPointNameToDissapearFromList(newEndPointName); // need to have this wait here. without out it, it was still catching the end point name in the list.
            Assert.IsFalse(endPointListPage.IsEndPointNameInList(newEndPointName)); // new end point name shouldn't appear in the end point list
        }
            
        /// <summary>
        /// Confirm that the seach box correctly filters down the list of end points
        /// </summary>
        [Test]
        public void t_14_EndPointListPage_Search_FiltersEndPointListItems()
        {
            var endPointListPage = new EndPointListPage(app, this.platform);
            var numberOfItemsBeforeSearch = endPointListPage.NumberOfEndPointListItems;

            endPointListPage.EnterSearchText("xml");
            app.Screenshot("Text entered in search box");

            // search should result in fewer items than before
            var numberOfItemsAfterSearch = endPointListPage.NumberOfEndPointListItems;
            Assert.IsTrue(numberOfItemsAfterSearch < numberOfItemsBeforeSearch); 

            app.Tap(b => b.Button("Cancel")); // 1st tap puts cursor in the search box, and brings up keyboard
            app.Tap(b => b.Button("Cancel")); // 2nd tap clears seach box and dismisses keyboard

            app.WaitForNoElement("Cancel"); // this test can fail at the assert below if the app is slow to draw all the list item icons, this gives it a little more time.

            // after clearing search, the number of items should be the same as before
            var numberOfItemsAfterCancellingSearch = endPointListPage.NumberOfEndPointListItems;
            Assert.AreEqual(numberOfItemsBeforeSearch, numberOfItemsAfterCancellingSearch, "The number of list items before the search should match the number of items after the search box is cleared."); 
        }

        /// <summary>
        /// Confirm that a new EndPoint can be created
        /// </summary>
        [Test]
        public void t_15_EndPointListPage_AddEndPoint_Succeeds()
        {
            var endPointListPage = new EndPointListPage(app, this.platform);
            const string expectedStatus = "Untested";
            const string startingUriText = "/";
            const string newUriText = "1";
            const string newParameterText = "2";
            const string newFilterDefText = "3";

            endPointListPage.AddNewEndPoint();
            app.WaitForElement("endPointNameTextbox");

            // check for uninitialized values
            Assert.AreEqual(expectedStatus, endPointListPage.EndPointDetailLastTestStatus); // should be "Untested"
            Assert.AreEqual(string.Empty, endPointListPage.EndPointNameText); // name should be blank
            Assert.AreEqual(startingUriText, endPointListPage.EndPointUriText);
            Assert.AreEqual(string.Empty, endPointListPage.EndPointParameterText); // should be blank
            Assert.AreEqual(string.Empty, endPointListPage.EndPointFilterDefinitionText); // should be blank

            // set values
            var testEndPointName = GetUniqueName();
            endPointListPage.EndPointNameText = testEndPointName;
            endPointListPage.EndPointUriText = newUriText;
            endPointListPage.EndPointParameterText = newParameterText;
            endPointListPage.EndPointFilterDefinitionText = newFilterDefText;
            app.Screenshot("New values entered");

            app.DismissKeyboard();
            endPointListPage.SaveEndPointDetails();

            // leave end point list page and come back
            app.Tap("Projects");
            endPointListPage.GoToEndPointListPage();

            endPointListPage.ScrollDownToEndPoint(testEndPointName); // scroll to the new end point, it will most likely be off the screen at the bottom of list
            Assert.AreEqual(1, app.Query(testEndPointName).Count()); // should only be 1 in the endpoint in list with this name

            // check values
            app.Tap(testEndPointName);
            app.Screenshot("Check values");
            Assert.AreEqual(expectedStatus, endPointListPage.EndPointDetailLastTestStatus); // should be "Untested"
            Assert.AreEqual(testEndPointName, endPointListPage.EndPointNameText);
            Assert.AreEqual(startingUriText + newUriText, endPointListPage.EndPointUriText);
            Assert.AreEqual(newParameterText, endPointListPage.EndPointParameterText);
            Assert.AreEqual(newFilterDefText, endPointListPage.EndPointFilterDefinitionText);
        }

        /// <summary>
        /// Confirm all end points are run on command
        /// </summary>
        [Test]
        public void t_16_EndPointListPage_RunAllEndPoints_Succeeds()
        {
            var endPointListPage = new EndPointListPage(app, this.platform);

            if (app.Query("Status: Successful").Any() || app.Query("Status: Failed").Any())
                endPointListPage.ClearAllResults();

            endPointListPage.RunAllEndPoints();

            Thread.Sleep(1000); // wait 1 second for it to start

            if (app.Query("No Network Connection").Any()) // if we're offline, pass the test
            {
                app.Screenshot("Device is offline");
            }
            else
            {
                Thread.Sleep(5000); // give it 5 seconds to finish

                if (app.Query("Status: Untested").Any())
                    Thread.Sleep(60000); // give it 60 more seconds to finish

                // shouldn't be any end points that are untested
                Assert.IsFalse(app.Query("Status: Untested").Any());
                app.ScrollDown("endPointListView"); // scroll down to see botom of list
                Assert.IsFalse(app.Query("Status: Untested").Any());
            }
        }

        /// <summary>
        /// Confirm that clear all removes all end point result information
        /// </summary>
        [Test]
        public void t_17_EndPointListPage_ClearAllResults_RemovesAllResultsInfo()
        {
            var endPointListPage = new EndPointListPage(app, this.platform);

            if (app.Query("Status: Successful").Any() || app.Query("Status: Failed").Any())
                endPointListPage.ClearAllResults();

            const string endPointName = "Get Customers - json";
            app.Tap(endPointName);

            Assert.AreEqual("Untested", endPointListPage.EndPointDetailLastTestStatus);

            app.Tap(c => c.Button("Run"));
            app.WaitForNoElement("Untested");
            app.WaitForNoElement("Running");

            app.ScrollDown("endPointDetailScrollView"); // the center of the results list view has to been on screen in order to be found
            var numberOfResultsBefore = endPointListPage.NumberOfResultListItems;

            endPointListPage.ClearAllResults();

            Assert.IsTrue(endPointListPage.NumberOfResultListItems < numberOfResultsBefore);
            Assert.IsFalse(app.Query("Response:").Any());

            app.ScrollUp("endPointDetailScrollView"); // scroll back up to top
            app.Screenshot("Results are cleared");

            // status should be back to untested
            Assert.AreEqual("Untested", endPointListPage.EndPointDetailLastTestStatus, "EndPoint status should be reset when results are cleared.");

            // result grid shouldn't show if there are no results

            // TODO: need to fix this test: status box should show "Untested", not the last test date.
        }

        /// <summary>
        /// Confirm that only the subset of end points matching the search criteria are run
        /// </summary>
        [Test]
        public void t_18_EndPointListPage_SearchThenRunAllEndPoints_OnlyRunsSubset()
        {
            var endPointListPage = new EndPointListPage(app, this.platform);
            var numberOfItemsBeforeSearch = endPointListPage.NumberOfEndPointListItems;

            // clear any results if endpoints have been run
            if (app.Query("Status: Successful").Any() || app.Query("Status: Failed").Any())
                endPointListPage.ClearAllResults();

            // Filter down to xml endpoints only
            endPointListPage.EnterSearchText("xml");
            app.Screenshot("Text entered in search box");

            // run all, should only run visible subset in this case
            endPointListPage.RunSelectEndPoints();

        }


        #endregion End Point List Page


        #region End Point Detail Page

        /// <summary>
        /// Confirm the endpoint detail page is displayed
        /// </summary>
        [Test]
        public void t_21_EndPointDetailsPage_IsDisplayed()
        {
            // Go to EndPoint list page for iFactr Customers project.
            app.Tap("iFactr Customers");
            app.Screenshot("End Point List Page");

            // Go to first EndPoint in list
            var endPointName = "Get Customers - json";
            app.Tap(endPointName);
            app.Screenshot("End Point Detail Page");

            // look for EndPoint name textbox on detail page
            var endPointNameTextbox = app.Query("endPointNameTextbox");
            Assert.AreEqual(1, endPointNameTextbox.Count());
        }

        #endregion End Point Detail Page
       


        #region Helper methods

        private void TapProjectDetailsContextAction(string projectName)
        {
            RevealListItemContextActions(projectName);

            // now that it's visible tap the details button
            app.Tap("Details");
        }

        private void TapProjectDeleteContextAction(string projectName)
        {
            RevealListItemContextActions(projectName);

            // now that it's visible tap the delete button
            app.Tap("Delete");
        }

        private void RevealListItemContextActions(string listItemText)
        {
            // look for first item in the list
            var listItem = app.Query(c => c.Marked(listItemText)).First();

            if (listItem != null)
            {
                var startX = listItem.Rect.CenterX + 100;
                var startY = listItem.Rect.CenterY;

                if (platform == Platform.iOS)
                {
                    // iOS: simulate a right to left swipe on list item - only works on devices
                    app.DragCoordinates(startX, startY, 1, startY);
                }
                else
                {
                    // Android: simulate long touch on list item - works on device and Xamarin Android player
                    app.TouchAndHoldCoordinates(startX, startY);
                }

                app.Screenshot("Project List Context Actions");
            }
        }

        private void AddNewProject(string projectName, string projectDesc = "", string user = "", string pass = "", string testURL = "")
        {
            // Tap Add button
            // the id of the ToolbarItem (addProjectButton) isn't accessible, so we have to use the class ID - different for each platform
            if (platform == Platform.Android)
                app.Tap(c => c.Class("ActionMenuView"));
            else
                app.Tap(c => c.Class("UINavigationButton"));

            // wait for project detail screen
            app.WaitForElement("projectNameTextbox"); // look for the project name textbox
            app.Screenshot("Nav to Project Detail Page");

            app.EnterText("projectNameTextbox", projectName);

            if (!string.IsNullOrEmpty(projectDesc))
                app.EnterText("projectDescTextbox", projectDesc);

            if (!string.IsNullOrEmpty(user))
                app.EnterText("projectUsernameTextbox", user);

            if (!string.IsNullOrEmpty(pass))
                app.EnterText("projectPasswordTextbox", pass);

            if (!string.IsNullOrEmpty(testURL))
            {
                // on iPad this field is covered by the keyboard, dismiss the keyboard so the entry field can be typed in
                app.DismissKeyboard();
                app.EnterText("projectBaseURLTextbox", testURL);
            }
            
            app.Screenshot("Enter New Project Info");

            app.Tap("Done");
        }

        private string GetUniqueName()
        {
            var ticks = DateTime.Now.Ticks.ToString();
            var shortenedTickets = ticks.Substring(ticks.Length - 8);
            return "Test " + shortenedTickets;
        }



        private static void RevealListItemContextActions(IApp app, Platform platform, string listItemText)
        {
            // look for first item in the list
            var listItem = app.Query(c => c.Marked(listItemText)).First();

            if (listItem != null)
            {
                var startX = listItem.Rect.CenterX + 100;
                var startY = listItem.Rect.CenterY;

                if (platform == Platform.iOS)
                {
                    // iOS: simulate a right to left swipe on list item - only works on devices
                    app.DragCoordinates(startX, startY, 1, startY);
                }
                else
                {
                    // Android: simulate long touch on list item - works on device and Xamarin Android player
                    app.TouchAndHoldCoordinates(startX, startY);
                }

                app.Screenshot("Reveal Context Actions");
            }
        }

        private static void TapDeleteContextActionButton(IApp app, Platform platform, string listItemText)
        {
            RevealListItemContextActions(app, platform, listItemText);

            // now that it's visible tap the delete button
            app.Tap("Delete");
        }

        private static void TapDuplicateContextActionButton(IApp app, Platform platform, string listItemText)
        {
            RevealListItemContextActions(app, platform, listItemText);

            // now that it's visible tap the delete button
            app.Tap(c => c.Button("Duplicate"));

            // take screenshot
            app.Screenshot("End Point Duplicated");
        }

        #endregion Helper methods


        #region Page Helper Classes

        class ProjectListPage : BaseListPage
        {
            public ProjectListPage(IApp app, Platform platform) : base(app, platform)
            { }

            // methods for actions


            // properties for results
            public AppResult[] DetailsContextAction  
            {
                get { return app.WaitForElement(c => c.Marked("Details").Text("Details")); }
            }

            public AppResult[] DeleteContextAction
            {
                get { return app.Query(c => c.Marked("Delete").Text("Delete")); }
            }
        }


        class EndPointListPage : BaseListPage
        {
            public EndPointListPage(IApp app, Platform platform) : base(app, platform)
            { 
                GoToEndPointListPage(); // navigate to the End Point List Page
            }

            // methods for actions
            public void GoToEndPointListPage()
            {
                app.Screenshot("Project List Page");

                // Go to EndPoint list page for iFactr Customers project.
                app.Tap("iFactr Customers");

                app.Screenshot("Nav to End Point List Page");
            }

            public void DuplicateEndPoint(string endPointName)
            {
                TapDuplicateContextActionButton(app, platform, endPointName);
            }

            public void SaveEndPointDetails()
            {
                app.Tap(c => c.Button("Save"));

                app.Screenshot("End point saved");
            }

            public void WaitForDetailPageToDissapear()
            {
                app.WaitForNoElement("endPointNameTextbox");
            }

            public void WaitForEndPointNameToDissapearFromList(string endPointName)
            {
                app.WaitForNoElement(endPointName);
            }

            public bool IsEndPointNameInList(string endPointName)
            {
                return app.Query(endPointName).Any();
            }

            public void EnterSearchText(string searchText)
            {
                app.EnterText("endPointListPageSearchBox", searchText);
                app.DismissKeyboard();
            }

            public void OpenMenu()
            {
                app.Tap(b => b.Button("+"));

                app.Screenshot("Menu opened");
            }

            public void ClearAllResults()
            {
                OpenMenu();
                app.Tap("Clear All Results");

                app.Screenshot("All results cleared");
            }

            public void AddNewEndPoint()
            {
                OpenMenu();
                app.Tap("Add EndPoint");
            }

            public void RunAllEndPoints()
            {
                OpenMenu();
                app.Tap("Run All EndPoints");
            }

            public void RunSelectEndPoints()
            {
                OpenMenu();
                app.Tap("Run Select EndPoints");
            }

            public void ScrollDownToEndPoint(string endPointName)
            {
                app.ScrollDownTo(endPointName, "endPointListView");
            }

            // properties for results
            public bool AlertMessageIsDisplayed
            {
                get { return app.WaitForElement("Are you sure?").Any() ? true : false; }
            }

            public string EndPointNameText
            {
                get { return app.Query("endPointNameTextbox").Single().Text; }
                set { app.EnterText("endPointNameTextbox", value); }
            }

            public string ToolbarButtonText
            {
                get { return app.Query("endPointDetailToolbarButton").Single().Text; }
            }

            public int NumberOfEndPointListItems
            {
                get { return app.Query(c => c.Marked("endPointListView").Descendant()).Count(); }
            }

            public int NumberOfResultListItems
            {
                get { return app.Query(c => c.Marked("resultsListView").Descendant()).Count(); }
            }

            public string EndPointDetailLastTestStatus
            {
                get { return app.Query("endPointLastTestedStatus").Single().Text; }
            }

            public string EndPointUriText
            {
                get { return app.Query("endPointURITextbox").Single().Text; }
                set { app.EnterText("endPointURITextbox", value); }
            }

            public string EndPointParameterText
            {
                get { return app.Query("endPointParameterTextbox").Single().Text; }
                set { app.EnterText("endPointParameterTextbox", value); }
            }

            public string EndPointFilterDefinitionText
            {
                get { return app.Query("endPointFilterDefTextbox").Single().Text; }
                set { app.EnterText("endPointFilterDefTextbox", value); }
            }

        }

        class BaseListPage
        {
            public readonly IApp app;
            public readonly Platform platform;

            public BaseListPage(IApp app, Platform platform)
            {
                this.app = app;
                this.platform = platform;
            }

            // methods for actions
            public void ShowContextActions(string listItemText)
            {
                RevealListItemContextActions(app, platform, listItemText);
            }

            public void TapDeleteButton(string endPointName)
            {
                TapDeleteContextActionButton(app, platform, endPointName);
            }

            public void DismissAlertMessage()
            {
                app.Tap("No");
            }

            public void ConfirmAlertMessage()
            {
                app.Tap("Yes");
            }
        }


        #endregion Page Helper Classes
    }
}