
namespace WebServiceDashboard.Shared.Services
{
    /// <summary>
    /// Defines a lightweight Dependency Service interface to loosely couple the Xamarin.Forms implementation
    /// </summary>
    public interface IDependencyService
    {
        T Get<T>() where T:class;
    }
}

