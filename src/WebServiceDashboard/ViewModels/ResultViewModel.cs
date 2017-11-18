using WebServiceDashboard.Shared.ViewModels;
using WebServiceDashboard.Shared.Models;
using WebServiceDashboard.Shared;

namespace WebServiceDashboard.ViewModels
{
    public class ResultViewModel : BaseViewModel
    {
        private readonly Result _selectedResult;
        public Result SelectedResult
        {
            get { return _selectedResult; }
        }

        public ResultViewModel(Result selectedResult)
        {
            _selectedResult = selectedResult;

            // set status icon
            Icons.SetResultStatusIcon(SelectedResult);
        }

        public bool HasResponseBody
        {
            get { return !string.IsNullOrWhiteSpace(_selectedResult.ResponseBody); }
        }

        public bool HasExceptionMessage
        {
            get { return !string.IsNullOrWhiteSpace(_selectedResult.ExceptionMessage); }
        }
    }
}

