using EyeTracker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EyeTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new YouTubeAPI();

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 4000;
            aTimer.Enabled = true;
        }

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        public static string GetText(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public POINT(System.Windows.Point pt) : this((int)pt.X, (int)pt.Y) { }

            public static implicit operator System.Windows.Point(POINT p)
            {
                return new System.Windows.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Windows.Point p)
            {
                return new POINT((int)p.X, (int)p.Y);
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                IntPtr hWnd = WindowFromPoint(p);
                if (listBox.Items.Count == 0 || GetText(hWnd) != listBox.Items[listBox.Items.Count - 1].ToString())
                {
                    if (hWnd != IntPtr.Zero)
                        Dispatcher.BeginInvoke(new Action(delegate
                        {
                            listBox.Items.Add(GetText(hWnd));
                        }));
                }

            }
        }
    }


}
