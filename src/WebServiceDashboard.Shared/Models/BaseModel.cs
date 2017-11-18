using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WebServiceDashboard.Shared.Models
{
    /// <summary>
    /// The Base Model simply implements INotifyPropertyChanged.
    /// It's done in a this base class so each model doesn't need to implement it (and duplicate the code)
    /// </summary>
    public class BaseModel : INotifyPropertyChanged
    {
        // The single member of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

