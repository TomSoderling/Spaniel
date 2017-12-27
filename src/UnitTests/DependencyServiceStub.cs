using System;
using System.Collections.Generic;
using Spaniel.Shared.Services;

namespace UnitTests
{
    /// <summary>
    /// Dependency service stub. Using the stub means that we never use the DependencyService class, and therefore never have to call Xamarin.Forms.Init() during a unit test.
    /// http://arteksoftware.com/unit-testing-with-xamarin-forms-dependencyservice/
    /// </summary>
    public class DependencyServiceStub : IDependencyService
    {
        private readonly Dictionary<Type, object> registeredServices = new Dictionary<Type, object>();

        public void Register<T>(object impl)
        {
            this.registeredServices[typeof(T)] = impl;
        }

        public T Get<T>() where T : class
        {
            return (T)registeredServices[typeof(T)];
        }
    }
}
