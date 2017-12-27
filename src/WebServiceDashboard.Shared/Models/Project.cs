using System;
using System.Collections.ObjectModel;

namespace Spaniel.Shared.Models
{
    /// <summary>
    /// Represents a Project entity
    /// </summary>
    public class Project : BaseModel
    {
        public Project()
        {
            EndPoints = new ObservableCollection<EndPoint>();
        }

        public Guid ID
        {
            get;
            set;
        }

        private string _name;
        public string Name
        { 
            get { return _name; } 
            set 
            { 
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _description;
        public string Description
        { 
            get { return _description; } 
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _username;
        public string Username
        { 
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _password;
        public string Password
        { 
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _baseURL;
        public string BaseURL
        { 
            get { return _baseURL; }
            set
            {
                if (_baseURL != value)
                {
                    _baseURL = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _icon;
        public string Icon
        { 
            get { return _icon; }
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _statusIcon;
        public string StatusIcon
        { 
            get { return _statusIcon; }
            set
            {
                if (_statusIcon != value)
                {
                    _statusIcon = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime? _lastTestRun;
        public DateTime? LastTestRun
        { 
            get { return _lastTestRun; }
            set
            {
                if (_lastTestRun != value)
                {
                    _lastTestRun = value;
                    RaisePropertyChanged();
                }
            }
        }

        private TestStatus _testStatus;
        public TestStatus TestStatus
        { 
            get { return _testStatus; }
            set
            {
                if (_testStatus != value)
                {
                    _testStatus = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ObservableCollection<EndPoint> _endPoints;
        public ObservableCollection<EndPoint> EndPoints 
        { 
            get { return _endPoints; }
            private set 
            { 
                if (_endPoints != value)
                {
                    _endPoints = value;
                    RaisePropertyChanged(); // not sure we need this. this set will only be called in the constructor of this model.
                }
            }
        }

    }
}