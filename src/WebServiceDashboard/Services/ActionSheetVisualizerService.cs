using System.Threading.Tasks;
using Spaniel.Shared.Services;
using Xamarin.Forms;

namespace Spaniel.Services
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

