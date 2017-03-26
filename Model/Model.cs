using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace EyeTracker.Model
{
    public class Model
    {
        public string search, id, title, description, webBrowserVideo;
        int selectedIndex;
        //public DateTime publishedDate;
        public ObservableCollection<string> myListBox;
    }
}