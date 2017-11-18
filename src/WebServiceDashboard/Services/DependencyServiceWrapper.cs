using WebServiceDashboard.Shared.Services;
using Xamarin.Forms;

namespace WebServiceDashboard.Services
{
    /// <summary>
    /// Performs the role of a lightweight Dependency Injection container
    /// </summary>
    public class DependencyServiceWrapper : IDependencyService
    {
        public T Get<T>() where T : class
        {
            return DependencyService.Get<T>();
        }
    }
}

