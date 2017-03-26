using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EyeTracker.ViewModel
{
    public class YouTubeAPI : INotifyPropertyChanged
    {
        public Model.Model model = new Model.Model()
        {
            id = "",
            search = "",
            title = ""
        };
        
        public string search
        {
            get
            {
                return model.search;
            }
            set
            {
                model.search = value;
                OnPropertyChanged("search");
            }
        }

        public string description
        {
            get
            {
                return model.description;
            }
        }

        public string id
        {
            get
            {
                return model.id;
            }
        }

        //public string webBrowserVideo
        //{
        //    //get
        //    //{
        //    //    return (string)GetValue(WebBrowserVideoProperty);
        //    //}
        //    //set
        //    //{
        //    //    SetValueDp(WebBrowserVideoProperty, value);
        //    //}
        //}

        public string title
        {
            get
            {
                return model.title;
            }
        }

        private string selectedItem;
        public string SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
               
            }
        }
      

        //public static readonly DependencyProperty WebBrowserVideoProperty =
        //    DependencyProperty.Register("webBrowserVideo", typeof(string), typeof(Model.Model), null);

        //void SetValueDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        //{

        //    //SetValue
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, new PropertyChangedEventArgs(p));
        //}


        private ObservableCollection<string> listBox;

        public ObservableCollection<string> myListBox
        {
            get
            {
                if (listBox == null)
                {
                    listBox = new ObservableCollection<string>();
                }
                return listBox;
            }
            set
            {
                listBox = value;
                OnPropertyChanged("myListBox");
            }
        }

        private YouTubeService ytService;

        public YouTubeAPI()
        {
            ytService = Auth();
            GetVideoInfo();
        }
           
           
        private YouTubeService Auth()
        {

            UserCredential creds;
            using (var stream = new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                creds = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.YoutubeReadonly },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("EyeTracker")
                    ).Result;
            }

            var service = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = creds,
                ApplicationName = "EyeTracker"
            });

            return service;
        }
        
        public void GetVideoInfo()
        {
            //var video = model;
            var videoRequest = ytService.Search.List("snippet");
            videoRequest.Q = model.search;
            var response = videoRequest.Execute();

            if ((response.Items.Count > 0))
            {
                myListBox = new ObservableCollection<string>();
                foreach (var item in response.Items)
                {
                    myListBox.Add("https://www.youtube.com/watch?v=" + item.Id.VideoId);
                }
                
            }
            else { }

        }

        public void ShowVideo(int index)
        {
            var videoRequest = ytService.Search.List("snippet");
            var videoRequestId = ytService.Videos.List("snippet");

            videoRequest.Q = model.search;

            var response = videoRequest.Execute();
            if (response.Items.Count > 0 && index != -1)
            {
                model.title = response.Items[index].Snippet.Title;
                model.description = response.Items[index].Snippet.Description;
                
                OnPropertyChanged("title");
                OnPropertyChanged("description");
            }
            else { }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private ICommand letsSearchCommand;

        public ICommand LetsSearch
        {
            get
            {
                if (letsSearchCommand == null)
                    letsSearchCommand = new MvvmCommand(parameter => { GetVideoInfo(); });
                return letsSearchCommand;
            }
        }
    }

    public static class WebBrowserHelper
    {
        public static readonly DependencyProperty LinkSourceProperty =
            DependencyProperty.RegisterAttached("LinkSource", typeof(string), typeof(WebBrowserHelper), new UIPropertyMetadata(null, LinkSourcePropertyChanged));

        public static string GetLinkSource(DependencyObject obj)
        {
            return (string)obj.GetValue(LinkSourceProperty);
        }

        public static void SetLinkSource(DependencyObject obj, string value)
        {
            obj.SetValue(LinkSourceProperty, value);
        }

        // When link changed navigate to site.
        public static void LinkSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var browser = o as WebBrowser;
            if (browser != null)
            {
                string uri = e.NewValue as string;
                browser.Source = uri != null ? new Uri(uri) : null;
            }
        }

    }

}
