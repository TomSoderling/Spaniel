using Spaniel.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Spaniel.Pages
{
    public partial class ProjectDetailPage : ContentPage
    {
        public ProjectDetailPage()
        {
            InitializeComponent();

            // turn large navigation page titles off for this page. iOS 11 only.
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetLargeTitleDisplay(LargeTitleDisplayMode.Never);         }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: Could this be done using a Command instead and elimiate this from the page code-behind?


            // Set AreDetailsDirty property when user changes any Project details
            if (e.OldTextValue != null) // OldTextValue will be null when the view is first displayed
            {
                if (((ProjectViewModel)BindingContext).AreDetailsDirty == false) // set the property if it's not already set
                    ((ProjectViewModel)BindingContext).AreDetailsDirty = true; 
            }
        }
    }
}

