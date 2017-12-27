using Xamarin.Forms;

namespace Spaniel.Controls
{
    public partial class PaddedLabel : ContentView
    {
        public PaddedLabel()
        {
            InitializeComponent();

            root.BindingContext = this;
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(PaddedLabel), null);

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }
    }
}

