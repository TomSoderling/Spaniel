using Xamarin.Forms;
using WebServiceDashboard.ViewModels;
using System;
using Xamarin.Forms.Xaml;

namespace WebServiceDashboard.Pages
{
    public partial class EndPointDetailPage : ContentPage
    {
        public EndPointDetailPage()
        {
            InitializeComponent();
        }

        public void OnItemTapped(object sender, EventArgs e)
        {
            // Don't show the item as selected. Without this line, the list item can't be re-selected once it's selected.
            resultsListView.SelectedItem = null;
        }

        public void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: Could this be done using a Command instead and elimiate this from the page code-behind?


            // Set AreDetailsDirty property when user changes any EndPoint details
            if (e.OldTextValue != null) // OldTextValue will be null when the view is first displayed
            {
                if (((EndPointViewModel)BindingContext).AreDetailsDirty == false) // set the property if it's not already set
                    ((EndPointViewModel)BindingContext).AreDetailsDirty = true;
            }
        }

    }
}

