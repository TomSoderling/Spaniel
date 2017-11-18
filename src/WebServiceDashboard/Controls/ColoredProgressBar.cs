using Xamarin.Forms;

namespace WebServiceDashboard.Controls
{
    public class ColoredProgressBar : ProgressBar
    {
        /// <summary>
        /// Bindable property for the color of the progress bar.
        /// </summary>
        public static BindableProperty ProgressBarColor = BindableProperty.Create<ColoredProgressBar, Color>(p => p.BarColor, default(Color));

        /// <summary>
        /// Gets or sets the color of the progress bar.
        /// </summary>
        public Color BarColor
        {
            get 
            { 
                return (Color)GetValue(ProgressBarColor); 
            }
            set 
            { 
                SetValue(ProgressBarColor, value); 
            }
        }


        /// <summary>
        /// Bindable property for the color of the track behind the progress bar.
        /// </summary>
        public static BindableProperty ProgressBarTrackColor = BindableProperty.Create<ColoredProgressBar, Color>(p => p.TrackColor, default(Color));

        /// <summary>
        /// Gets or sets the color of the track behind the progress bar.
        /// </summary>
        public Color TrackColor
        {
            get 
            { 
                return (Color)GetValue(ProgressBarTrackColor); 
            }
            set 
            { 
                SetValue(ProgressBarTrackColor, value); 
            }
        }
    }
}

