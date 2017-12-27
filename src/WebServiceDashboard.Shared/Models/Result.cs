using System;
using System.Net;

namespace Spaniel.Shared.Models
{
    /// <summary>
    /// Represents a Result entity
    /// </summary>
    public class Result : BaseModel
    {
        private DateTime? _runDate;
        public DateTime? RunDate
        { 
            get { return _runDate; } 
            set
            {
                if (_runDate != value)
                {
                    _runDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        private HttpStatusCode? _httpCode;
        public HttpStatusCode? HttpCode
        { 
            get { return _httpCode; } 
            set
            {
                if (_httpCode != value)
                {
                    _httpCode = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _responseBody;
        public string ResponseBody
        { 
            get { return _responseBody; } 
            set
            {
                if (_responseBody != value)
                {
                    _responseBody = value;
                    RaisePropertyChanged();
                }
            }
        }

        // not sure if this is needed for anything
//        private HttpResponseHeaders _responseHeaders;
//        public HttpResponseHeaders ResponseHeaders
//        { 
//            get { return _responseHeaders; } 
//            set
//            {
//                if (_responseHeaders != value)
//                {
//                    _responseHeaders = value;
//                    RaisePropertyChanged();
//                }
//            }
//        }

        private TimeSpan? _responseTime;
        public TimeSpan? ResponseTime
        { 
            get { return _responseTime; } 
            set
            {
                if (_responseTime != value)
                {
                    _responseTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int ResponseTimeMilliseconds
        { 
            get 
            { 
                if (_responseTime != null)
                    return Convert.ToInt32(_responseTime.Value.TotalMilliseconds);
                else
                    return 0;
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

        private string _exceptionMessage;
        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set
            {
                if (_exceptionMessage != value)
                {
                    _exceptionMessage = value;
                    RaisePropertyChanged();
                }
            }
        }

    }
}