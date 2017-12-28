using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Spaniel.Pages
{
    public partial class EndPointListPage : ContentPage
    {
        public EndPointListPage()
        {
            InitializeComponent();

            // large navigation page titles - iOS 11 only.
            if (Device.Idiom == TargetIdiom.Phone)
            {
                // off for this page and any sub pages.
                On<Xamarin.Forms.PlatformConfiguration.iOS>().SetLargeTitleDisplay(LargeTitleDisplayMode.Never);
            }
            else
            {
                // on for this page. This mimics what the iOS Mail app does on iPad
                On<Xamarin.Forms.PlatformConfiguration.iOS>().SetLargeTitleDisplay(LargeTitleDisplayMode.Automatic);
            }
        }

        public void OnItemTapped(object sender, EventArgs e)
        {
            // Don't show the item as selected.  without this line, the list item can't be re-selected once it's selected.
            if (Device.Idiom == TargetIdiom.Phone)
                endPointListView.SelectedItem = null;
        }
    }
}

