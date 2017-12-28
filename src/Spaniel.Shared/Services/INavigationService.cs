using System.Threading.Tasks;
using Spaniel.Shared.Models;

namespace Spaniel.Shared.Services
{
    /// <summary>
    /// Defines a lightweight Navigation Service interface to loosely couple the Xamarin.Forms implementation
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to an app page async
        /// </summary>
        /// <returns>The async. operation</returns>
        /// <param name="page">The app page to navigate to</param>
        /// <param name="selectedModel">The selected model</param>
        /// <param name="dependencyService">Dependency service, used in the instantiation of the ViewModel</param>
        /// <param name="otherSelectedModel">Other selected model</param>
        Task GoToPageAsync(AppPage page, BaseModel selectedModel = null, IDependencyService dependencyService = null, BaseModel otherSelectedModel = null);

        Task GoToRootDetailPageAsync();

        void SetPlaceholderDetailPage();
    }


    /// <summary>
    /// Simple enumeration for the different types of pages/screens in our app
    /// </summary>
    public enum AppPage
    {
        ProjectListPage,
        ProjectDetailPage,
        EndPointListPage,
        EndPointDetailPage,
        ResultDetailPage
    }
}

