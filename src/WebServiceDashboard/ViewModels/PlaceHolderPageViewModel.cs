using Xamarin.Forms;
using WebServiceDashboard.Shared;

namespace WebServiceDashboard.ViewModels
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

