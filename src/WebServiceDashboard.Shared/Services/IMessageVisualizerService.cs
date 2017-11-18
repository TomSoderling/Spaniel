using System.Threading.Tasks;

namespace WebServiceDashboard.Shared.Services
{
    /// <summary>
    /// Interface to display an alert to the user and to loosely couple the Xamarin.Forms implementation
    /// </summary>
    public interface IMessageVisualizerService
    {
        Task<bool> ShowMessageAsync(string title, string message, string ok, string cancel=null);
    }
}

