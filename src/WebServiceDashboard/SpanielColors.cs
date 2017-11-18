using Xamarin.Forms;
using WebServiceDashboard.Shared;

namespace WebServiceDashboard
{
    public static class SpanielColors
    {
        public static Color SpanielDarkGrey
        {
            get { return Color.FromHex(Constants.SpanielDarkGrey); }
        }

        public static Color SpanielYellow 
        {
            get { return Color.FromHex(Constants.SpanielYellow); }
        }

        public static Color SpanielLightGrey
        {
            get { return Color.FromHex(Constants.SpanielLightGrey); }
        }



        public static Color NavBarBackground 
        {
            get { return SpanielDarkGrey; }
        }

        public static Color NavBarText
        {
            get { return SpanielYellow; }
        }

    }
}

