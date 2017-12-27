using System.Threading.Tasks;

namespace Spaniel.Shared.Services
{
    public interface IActionSheetVisualizerService
    {
        Task<string> ShowActionSheetAsync(string title, string cancelButtonText, string destroyButtonText, params string[] buttons);
    }
}

