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
                Device.OnPlatform(
                    iOS: () => 
                    {
                        _backgroundImage = ImageSource.FromFile(Icons.DetailPaneBackground_Landscape);
                    },
                    Android: () =>
                    {
                        _backgroundImage = ImageSource.FromFile(Icons.DetailPaneBackground_Android);
                    }
                );

                return _backgroundImage;
            }
        }
    }
}

