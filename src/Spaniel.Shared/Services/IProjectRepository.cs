using System.Collections.Generic;
using System.Xml.Linq;
using Spaniel.Shared.Models;

namespace Spaniel.Shared.Services
{
    public interface IProjectRepository
    {
        XDocument LoadProjectFileFromDevice();

        bool SaveProjectsDocument(XDocument doc);
        bool SaveProjectsToFile(IEnumerable<Project> projects);
    }
}

