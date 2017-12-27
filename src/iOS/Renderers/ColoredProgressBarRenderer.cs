using Spaniel.Controls;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using Spaniel.iOS.Renderers;

[assembly: ExportRenderer(typeof(ColoredProgressBar), typeof(ColoredProgressBarRenderer))]

namespace Spaniel.iOS.Renderers
{
    public class ColoredProgressBarRenderer : ProgressBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (Control != null)
                UpdateProgressBarColor();
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ColoredProgressBar.ProgressBarColor.PropertyName)
                UpdateProgressBarColor();
        }

        void UpdateProgressBarColor()
        {
            var element = Element as ColoredProgressBar;

            // set progress bar color
            Control.ProgressTintColor = element.BarColor.ToUIColor();

            // set progress bar track color
            Control.TrackTintColor = element.TrackColor.ToUIColor();
        }
    }
}

