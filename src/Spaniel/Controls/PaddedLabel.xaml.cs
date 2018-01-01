using Xamarin.Forms;

namespace Spaniel.Controls
{
    /// <summary>
    /// Label with standard padding and other options set that can be use all over
    /// </summary>
    public partial class PaddedLabel : ContentView
    {
        public PaddedLabel()
        {
            InitializeComponent();

            root.BindingContext = this;
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(PaddedLabel), null);

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

