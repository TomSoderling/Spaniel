using System;
using Xamarin.Forms;

namespace WebServiceDashboard.Pages
{
    public partial class ProjectListPage : ContentPage
    {
        public ProjectListPage()
        {
            InitializeComponent();
        }
            
        public void OnItemTapped(object sender, EventArgs e)
        {
            // Don't show the item as selected (highlighted) in the list. Without this line, the same list item can't be re-selected once it's already selected.
            projectListView.SelectedItem = null;
        }
    }
}

