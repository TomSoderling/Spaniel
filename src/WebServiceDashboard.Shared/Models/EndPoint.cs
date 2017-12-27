using System;
using System.Collections.ObjectModel;

namespace Spaniel.Shared.Models
{
    /// <summary>
    /// Represents a EndPoint entity
    /// </summary>
    public class EndPoint : BaseModel
    {
        public EndPoint()
        {
            Results = new ObservableCollection<Result>();
        }

        public Guid ID
        {
            get;
            set;
        }

        private TestStatus _status;
        public TestStatus Status
        { 
            get { return _status; } 
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisePropertyChanged();
                }
            }
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

        private string _endPointURI;
        public string EndPointURI 
        { 
            get { return _endPointURI; } 
            set
            {
                if (_endPointURI != value)
                {
                    _endPointURI = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _parameterFillIn;
        public string ParameterFillIn
        { 
            get { return _parameterFillIn; }
            set
            {
                if (_parameterFillIn != value)
                {
                    _parameterFillIn = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _filterDefinition;
        public string FilterDefinition
        { 
            get { return _filterDefinition; }
            set
            {
                if (_filterDefinition != value)
                {
                    _filterDefinition = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _httpVerb;
        public string HttpVerb
        { 
            get { return _httpVerb; }
            set
            {
                if (_httpVerb != value)
                {
                    _httpVerb = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime? _lastTested;
        public DateTime? LastTested
        { 
            get { return _lastTested; }
            set
            {
                if (_lastTested != value)
                {
                    _lastTested = value;
                    RaisePropertyChanged();
                }
            }
        }

//        private bool _isFailureAllowed;
//        public bool IsFailureAllowed 
//        { 
//            get { return _isFailureAllowed; }
//            set
//            {
//                if (_isFailureAllowed != value)
//                {
//                    _isFailureAllowed = value;
//                    RaisePropertyChanged();
//                }
//            }
//        }

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

        private ObservableCollection<Result> _results;
        public ObservableCollection<Result> Results 
        { 
            get { return _results; }
            private set 
            { 
                if (_results != value)
                {
                    _results = value;
                    RaisePropertyChanged(); // not sure we need this. this set will only be called in the constructor of this model.
                }
            }
        }

    }
}