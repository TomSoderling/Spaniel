using Spaniel.Shared;
using Xamarin.Forms;

namespace Spaniel.ViewModels
{
    public class PlaceHolderPageViewModel
    {
        private ImageSource _backgroundImage;
        public ImageSource BackgroundImage
        {
            get 
            { 
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        {
                            _backgroundImage = ImageSource.FromFile(Icons.DetailPaneBackground_Landscape);
                            break;
                        }
                    case Device.Android:
                        {
                            _backgroundImage = ImageSource.FromFile(Icons.DetailPaneBackground_Android);
                            break;
                        }
                }

                return _backgroundImage;
            }
        }
    }
}

