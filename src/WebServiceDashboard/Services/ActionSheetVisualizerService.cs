using WebServiceDashboard.Shared.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WebServiceDashboard.Services
{
    public class ActionSheetVisualizerService : IActionSheetVisualizerService
    {
        public async Task<string> ShowActionSheetAsync(string title, string cancelButtonText, string destroyButtonText, params string[] buttons)
        {
            var result = await Application.Current.MainPage.DisplayActionSheet(title, cancelButtonText, destroyButtonText, buttons);
            return result;
        }
    }
}

