using System.Threading.Tasks;

namespace Spaniel.Shared.Services
{
    public interface IShareService
    {
        /// <summary>
        /// Shares the project file, allowing the user to select how: Email, AirDrop, DropBox, Bluetooth
        /// </summary>
        /// <param name="xmlData">Xml project file data.</param>
        /// <param name="filename">Filename.</param>
        /// <param name="projectName">The name of the Project.</param>
        void Share(string xmlData, string filename, string projectName);
    }
}

