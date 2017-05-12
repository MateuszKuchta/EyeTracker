using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
            search = ""
        };

        #region Properties
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
            set
            {
                model.description = value;
                OnPropertyChanged("description");
            }
        }

        public string id
        {
            get
            {
                return model.id;
            }
        }

        public string title
        {
            get
            {
                return model.title;
            }
            set
            {
                model.title = value;
                OnPropertyChanged("title");
            }
        }

        private ObservableCollection<Model.Lista> listBox;
        public ObservableCollection<Model.Lista> myListBox
        {
            get
            {
                if (listBox == null)
                {
                    listBox = new ObservableCollection<Model.Lista>();
                }
                return listBox;
            }
            set
            {
                listBox = value;
                OnPropertyChanged("myListBox");
            }
        }

        private Model.Lista selectedItem;
        public Model.Lista SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        #endregion

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
            if (model.search != "")
            {
                var videoRequest = ytService.Search.List("snippet");
                videoRequest.Q = model.search;
                videoRequest.MaxResults = 10;

                var response = videoRequest.Execute();

                ListView listView = UIHelper.FindChild<ListView>(Application.Current.MainWindow, "playList");

                if ((response.Items.Count > 0))
                {
                    myListBox = new ObservableCollection<Model.Lista>();
                    foreach (var item in response.Items)
                    {
                        Model.Lista lista = new Model.Lista();
                        lista.Img = "http://img.youtube.com/vi/" + item.Id.VideoId + "/0.jpg";
                        lista.Title = item.Snippet.Title;
                        myListBox.Add(lista);
                    }
                    listView.ItemsSource = myListBox;
                }
                else { }

            }
        }
        public string ReturnId(string myTitle, int start)
        {
            var videoRequest = ytService.Search.List("snippet");
            videoRequest.Q = myTitle;

            var response = videoRequest.Execute();
            if ((response.Items.Count > 0))
            {
                int i = 0;
                foreach (var item in response.Items)
                {
                    if (response.Items[i++].Snippet.Title == myTitle)
                    {
                        model.title = response.Items[i].Snippet.Title;
                        model.description = response.Items[i].Snippet.Description;

                        return ("http://www.youtube.com/v/" + item.Id.VideoId + "?controls=1&start=" + start + "&autoplay=1");
                    }
                }
            }
            else { }

            return "https://www.youtube.com/";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #region ICommands
        private ICommand letsSearchCommand;
        private ICommand playButtonCommand;
        private ICommand rewindCommand;
        private ICommand forwardCommand;
        private ICommand volumeDownCommand;
        private ICommand volumeUpCommand;

        public ICommand LetsSearch
        {
            get
            {
                if (letsSearchCommand == null)
                    letsSearchCommand = new MvvmCommand(parameter => { GetVideoInfo(); });
                return letsSearchCommand;
            }
        }

        public ICommand PlayButton
        {
            get
            {
                if (playButtonCommand == null)
                    playButtonCommand = new MvvmCommand(parameter => { DoPlayAndPause(); });
                return playButtonCommand;
            }
        }

        public ICommand Rewind
        {
            get
            {
                if (rewindCommand == null)
                    rewindCommand = new MvvmCommand(parameter => { DoRewindVideo(); });
                return rewindCommand;
            }
        }

        public ICommand Forward
        {
            get
            {
                if (forwardCommand == null)
                    forwardCommand = new MvvmCommand(parameter => { DoForwardVideo(); });
                return forwardCommand;
            }
        }

        public ICommand VolumeDown
        {
            get
            {
                if (volumeDownCommand == null)
                    volumeDownCommand = new MvvmCommand(parameter => { DoVolumeDown(); });
                return volumeDownCommand;
            }
        }

        public ICommand VolumeUp
        {
            get
            {
                if (volumeUpCommand == null)
                    volumeUpCommand = new MvvmCommand(parameter => { DoVolumeUp(); });
                return volumeUpCommand;
            }
        }
        #endregion

        public static int pointX;
        public static int pointY;
        public static long actualDate;
        public static bool play = false;
        public static bool rewind = false;
        public static bool forward = false;
        public static bool volUp = false;
        public static bool volDown = false;

        public bool TimeBeforeClick()
        {
            Thread.Sleep(3000);
            return true;
        }

        public void DoPlayAndPause()
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                pointX = p.X;
                pointY = p.Y;
            }
            Task.Run(() => play = TimeBeforeClick());
            if (play)
            {
                LeftMouseClick(pointX, pointY - 170);
                play = false;
            }
        }

        public void DoRewindVideo()
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                pointX = p.X;
                pointY = p.Y;
            }
            Task.Run(() => rewind = TimeBeforeClick());
            if (rewind)
            {
                LeftMouseClick(pointX + 170, pointY);
                LeftMouseClick(pointX + 170, pointY);
                System.Windows.Forms.SendKeys.SendWait("J");
                rewind = false;
            }

        }

        public void DoForwardVideo()
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                pointX = p.X;
                pointY = p.Y;
            }
            Task.Run(() => forward = TimeBeforeClick());
            if (forward)
            {
                LeftMouseClick(pointX - 170, pointY);
                LeftMouseClick(pointX - 170, pointY);
                System.Windows.Forms.SendKeys.SendWait("L");
                forward = false;
            }
        }

        public void DoVolumeDown()
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                pointX = p.X;
                pointY = p.Y;
            }
            Task.Run(() => volDown = TimeBeforeClick());
            if (volDown)
            {
                LeftMouseClick(pointX, pointY - 170);
                LeftMouseClick(pointX, pointY - 170);
                System.Windows.Forms.SendKeys.SendWait("{DOWN}");
                volDown = false;
            }
        }

        public void DoVolumeUp()
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                pointX = p.X;
                pointY = p.Y;
            }
            Task.Run(() => volUp = TimeBeforeClick());
            if (volUp)
            {
                LeftMouseClick(pointX, pointY + 170);
                LeftMouseClick(pointX, pointY + 170);
                System.Windows.Forms.SendKeys.SendWait("{UP}");
                volUp = false;
            }
        }

        #region DllImport
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        #endregion
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
            SetCursorPos(pointX, pointY);
        }
    }
}
