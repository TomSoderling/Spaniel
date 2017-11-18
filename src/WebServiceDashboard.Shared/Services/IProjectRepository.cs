using System.Collections.Generic;
using WebServiceDashboard.Shared.Models;
using System.Xml.Linq;

namespace WebServiceDashboard.Shared.Services
{
    public interface IProjectRepository
    {
        XDocument LoadProjectFileFromDevice();

        bool SaveProjectsDocument(XDocument doc);
        bool SaveProjectsToFile(IEnumerable<Project> projects);
    }
}

