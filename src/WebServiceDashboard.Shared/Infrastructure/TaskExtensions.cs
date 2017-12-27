using System.Threading.Tasks;

namespace Spaniel.Shared.Infrastructure
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Used when the NavigationService.GoToPageAsync() is called.  No need to do anything with the result.
        /// This helps clear up compiler warnings and keep things neat.
        public static void IgnoreResult(this Task t)
        {
            // do nothing
        }
    }
}

