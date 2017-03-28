using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EyeTracker.ViewModel
{
    public static class WebBrowserHelper
    {
        public static readonly DependencyProperty LinkSourceProperty =
            DependencyProperty.RegisterAttached("LinkSource", typeof(Model.Lista), typeof(WebBrowserHelper), new UIPropertyMetadata(null, LinkSourcePropertyChanged));

        public static string GetLinkSource(DependencyObject obj)
        {
            return (string)obj.GetValue(LinkSourceProperty);
        }

        public static void SetLinkSource(DependencyObject obj, string value)
        {
            obj.SetValue(LinkSourceProperty, value);
        }

        public static void LinkSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            YouTubeAPI yt = new YouTubeAPI();
            var browser = o as System.Windows.Controls.WebBrowser;
            Model.Lista list = e.NewValue as Model.Lista;
            if ((browser != null) && (list != null))
            {

                string title = list.Title;
                string uri = yt.ReturnId(title, 0);
                browser.Source = uri != null ? new Uri(uri) : null;
            }
        }
    }
}
