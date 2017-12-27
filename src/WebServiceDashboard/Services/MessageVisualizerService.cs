using System.Threading.Tasks;
using Spaniel.Shared.Services;
using Xamarin.Forms;

namespace Spaniel.Services
{
    public class MessageVisualizerService : IMessageVisualizerService
    {
        public async Task<bool> ShowMessageAsync(string title, string message, string ok, string cancel=null)
        {
            if (cancel == null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, ok);
                return true;
            }

            return await Application.Current.MainPage.DisplayAlert(title, message, ok, cancel);
        }
    }
}

