using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WebServiceDashboard.Shared.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        // The single member of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        protected void RaiseAllPropertiesChanged()
        {
            // By convention, an empty string indicates all properties are invalid.
            PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propExpr)
        {
            var prop = (PropertyInfo)((MemberExpression)propExpr.Body).Member;
            this.RaisePropertyChanged(prop.Name);
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName= "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        // Not currently using these methods, but they could come in handy

//        protected bool SetPropertyValue<T>(ref T storageField, T newValue, Expression<Func<T>> propExpr)
//        {
//            if (Equals(storageField, newValue)) 
//                return false;
//
//            storageField = newValue;
//            var prop = (PropertyInfo)((MemberExpression)propExpr.Body).Member;
//            this.RaisePropertyChanged(prop.Name);
//
//            return true;
//        }
//
//        protected bool SetPropertyValue<T>(ref T storageField, T newValue, [CallerMemberName] string propertyName = "")
//        {
//            if (Equals(storageField, newValue))
//                return false;
//
//            storageField = newValue;
//            this.RaisePropertyChanged(propertyName);
//
//            return true;
//        }




        private bool _isError;
        public bool IsError
        {
            get { return _isError; }
            set
            {
                _isError = value;
                RaisePropertyChanged();
            }
        }

        private string _errorText;
        public string ErrorText
        { 
            get { return _errorText; }
            set
            {
                _errorText = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Displays error message to the user
        /// </summary>
        public void DisplayErrorMessage(string message)
        {
            ErrorText = message;
            IsError = true;
        }

        public void ClearErrorMessage()
        {
            ErrorText = string.Empty;
            IsError = false;
        }

    }
}

