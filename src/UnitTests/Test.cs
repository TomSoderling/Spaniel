using NUnit.Framework;
using Spaniel.Services;
using Spaniel.Shared.Services;
using Spaniel.ViewModels;

namespace UnitTests
{
    [TestFixture()]
    public class Test
    {
        DependencyServiceStub _dependencyService;

        [OneTimeSetUp]
        public void Setup()
        {
            _dependencyService = new DependencyServiceStub();

            _dependencyService.Register<INavigationService>(new NavigationService());
        }

        [Test()]
        public void TestCaseAsync()
        {
            //var navService = _dependencyService.Get<INavigationService>();

            // Application.Current is null, so we can't get past this line: if (Application.Current.MainPage != null)
            //await navService.GoToPageAsync(AppPage.ProjectListPage, null, _dependencyService);


            // CrossConnectivity.Current is null, so we can't get past the 1st line of the ctor
            var pvm = new ProjectViewModel(_dependencyService);
        }
    }
}

