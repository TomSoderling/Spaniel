using WebServiceDashboard.Shared.Models;
using System.Collections.ObjectModel;
using System.Net;

namespace WebServiceDashboard.Shared
{
    public static class Icons
    {
        #region Icon Paths

        // Projects
        public const string ProjectNew = "icn_ProjectNew.png";
        public const string ProjectNewDetail = "icn_ProjectNewDetail.png";
        public const string ProjectPassed = "icn_ProjectPassed.png";
        public const string ProjectPassedDetail = "icn_ProjectPassedDetail.png";
        public const string ProjectFailed = "icn_ProjectFailed.png";
        public const string ProjectFailedDetail = "icn_ProjectFailedDetail.png";
        public const string ProjectPartial = "icn_ProjectPartial.png";
        public const string ProjectPartialDetail = "icn_ProjectPartialDetail.png";

        // EndPoints
        public const string EndPointSuccessful = "icn_EndpointSuccessful.png";
        public const string EndPointSuccessfulDetail = "icn_EndpointSuccessfulDetail.png";
        public const string EndPointFailed = "icn_EndpointFailed.png";
        public const string EndPointFailedDetail = "icn_EndpointFailedDetails.png";
        public const string EndPointInProgress = "icn_EndpointInProgress.png";
        public const string EndPointInProgressDetail = "icn_EndpointInProgressDetail.png";
        public const string EndPointSelected = "icn_EndPointSelected.png";


        // General
        public const string NotSelected = "icn_NotSelected.png";
        public const string NotSelectedDetail = "icn_EndpointNewDetail.png";

        // Detail Pane Background Watermark Image
        public const string DetailPaneBackground_Landscape = "Watermark-Landscape.png";
        public const string DetailPaneBackground_Android = "img_spaniel_hdpi.png";

        #endregion Icon Paths


        #region Helper Methods

  
        public static void SetProjectIcon(Project project)
        {
            if (project.LastTestRun == null || project.TestStatus == TestStatus.Untested)
                project.Icon = ProjectNew;
            else if (project.TestStatus == TestStatus.Successful)
                project.Icon = ProjectPassed;
            else if (project.TestStatus == TestStatus.Failed)
                project.Icon = ProjectFailed;
            else if (project.TestStatus == TestStatus.PartialSuccess)
                project.Icon = ProjectPartial;
        }



        public static void SetEndPointIcons(ObservableCollection<EndPoint> endPoints)
        {
            foreach (var endPoint in endPoints)
                SetEndPointIcon(endPoint);
        }

        public static void SetEndPointIcon(EndPoint endPoint)
        {
            if (endPoint.LastTested == null || endPoint.Status == TestStatus.Untested)
                endPoint.Icon = NotSelected;
            else if (endPoint.Status == TestStatus.Successful)
                endPoint.Icon = EndPointSuccessful;
            else if (endPoint.Status == TestStatus.Failed)
                endPoint.Icon = EndPointFailed;
            else if (endPoint.Status == TestStatus.Running)
                endPoint.Icon = EndPointInProgress;
        }



        public static void SetResultIcon(Result result)
        {
            if (result.RunDate == null)
                result.Icon = NotSelected;
            else if (result.HttpCode == HttpStatusCode.Accepted || result.HttpCode == HttpStatusCode.OK || result.HttpCode == HttpStatusCode.Found)
                result.Icon = EndPointSuccessful;
            else if ((int)result.HttpCode > 300 || (int)result.HttpCode == 0) // anything over 300 is an error, except for Found (302) which is handled in the above case
                result.Icon = EndPointFailed;

            // TODO: verify that every http status code over 300 is an error (except for 302)
        }





        public static void SetEndPointStatusIcon(EndPoint endPoint)
        {
            if (endPoint != null)
            {
                if (endPoint.LastTested == null || endPoint.Status == TestStatus.Untested)
                    endPoint.StatusIcon = NotSelectedDetail;
                else if (endPoint.Status == TestStatus.Successful)
                    endPoint.StatusIcon = EndPointSuccessfulDetail;
                else if (endPoint.Status == TestStatus.Failed)
                    endPoint.StatusIcon = EndPointFailedDetail;
                else if (endPoint.Status == TestStatus.Running)
                    endPoint.StatusIcon = EndPointInProgressDetail;
            }
        }

        public static void SetProjectStatusIcon(Project project)
        {
            if (project.LastTestRun == null || project.TestStatus == TestStatus.Untested)
                project.StatusIcon = ProjectNewDetail;
            else if (project.TestStatus == TestStatus.Successful)
                project.StatusIcon = ProjectPassedDetail;
            else if (project.TestStatus == TestStatus.Failed)
                project.StatusIcon = ProjectFailedDetail;
            else if (project.TestStatus == TestStatus.PartialSuccess)
                project.StatusIcon = ProjectPartialDetail;
        }

        public static void SetResultStatusIcon(Result result)
        {
            if (result.RunDate == null)
                result.StatusIcon = NotSelectedDetail;
            else if (result.HttpCode == HttpStatusCode.Accepted || result.HttpCode == HttpStatusCode.OK || result.HttpCode == HttpStatusCode.Found)
                result.StatusIcon = EndPointSuccessfulDetail;
            else if ((int)result.HttpCode > 300 || (int)result.HttpCode == 0) // anything over 300 is an error, except for Found (302) which is handled in the above case
                result.StatusIcon = EndPointFailedDetail;
        }


        #endregion Helper Methods
    }

}

