using Xamarin.Forms.Platform.Android;
using Spaniel.Controls;
using Android.Graphics;
using Xamarin.Forms;
using Spaniel.Droid.Renderers;

[assembly: ExportRenderer(typeof(ColoredProgressBar), typeof(ColoredProgressBarRenderer))]

namespace Spaniel.Droid.Renderers
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
            Control.IndeterminateDrawable.SetColorFilter(element.BarColor.ToAndroid(), PorterDuff.Mode.SrcIn);
            Control.ProgressDrawable.SetColorFilter(element.BarColor.ToAndroid(), PorterDuff.Mode.SrcIn);

            // set progress bar track color
            //Control.TrackTintColor = element.TrackColor.ToUIColor();
        }
    }
}

