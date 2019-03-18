using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace shecan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool ismove = false;
        bool isconnect = false;
        Point mp = new Point();
        Thread th;
        Style st;

        public MainWindow()
        {
            InitializeComponent();
            th = new Thread(() => {
                
            });
        }

        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
            if (!isconnect)
            {
                th = new Thread(() => {
                    for (int i = 270; i < 360; i++)
                    {
                        this.Dispatcher.Invoke(new Action(delegate ()
                        {
                            if (i > 295)
                            {
                                img_status.Source = new BitmapImage(new Uri(@"/pics/taeed.png", UriKind.Relative));
                            }
                            img_status.LayoutTransform = new RotateTransform(i, 50, 50);
                        }));
                        Thread.Sleep(1);
                    }
                });
                th.Start();

                SetDNS("178.22.122.100" , "94.232.174.194");
                lbl_status.Text = "متصل شدید";
                btn_connect.Background = new SolidColorBrush(Color.FromRgb(204,34,34));
                btn_connect.Content = "نشکن!";
                st = FindResource("RoundedButtonStyle_red") as Style;
                btn_connect.Style = st;
                isconnect = true;
            }
            else
            {
                th = new Thread(() => {
                    for (int i = 0; i < 90; i++)
                    {
                        this.Dispatcher.Invoke(new Action(delegate ()
                        {
                            if (i > 45)
                            {
                                img_status.Source = new BitmapImage(new Uri(@"/pics/zarbdar.png", UriKind.Relative));
                            }
                            img_status.LayoutTransform = new RotateTransform(i, 50, 50);
                        }));
                        Thread.Sleep(1);
                    }
                });
                th.Start();

                UnsetDNS();
                lbl_status.Text = "متصل نیستید";
                btn_connect.Background = new SolidColorBrush(Color.FromRgb(65, 168, 124));
                btn_connect.Content = "بشکن!";
                st = FindResource("RoundedButtonStyle") as Style;
                btn_connect.Style = st;
                isconnect = false;
            }
        }

        private void btn_exit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UnsetDNS();
            th.Abort();
            Application.Current.Shutdown();
        }

        private void Border_title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ismove = true;
            this.mp = e.GetPosition(this);
            this.mp.Y = Convert.ToInt16(this.Top) + this.mp.Y;
            this.mp.X = Convert.ToInt16(this.Left) + this.mp.X;
        }

        private void Border_title_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ismove = false;
        }

        private void Border_title_MouseMove(object sender, MouseEventArgs e)
        {
            if (ismove)
            {
                Point MousePosition = e.GetPosition(this);
                Point MousePositionAbs = new Point();
                MousePositionAbs.X = Convert.ToInt16(this.Left) + MousePosition.X;
                MousePositionAbs.Y = Convert.ToInt16(this.Top) + MousePosition.Y;
                this.Left = this.Left + (MousePositionAbs.X - this.mp.X);
                this.Top = this.Top + (MousePositionAbs.Y - this.mp.Y);
                this.mp = MousePositionAbs;
            }
        }

        public static NetworkInterface GetActiveEthernetOrWifiNetworkInterface()
        {
            var Nic = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(
                a => a.OperationalStatus == OperationalStatus.Up &&
                (a.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || a.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
                a.GetIPProperties().GatewayAddresses.Any(g => g.Address.AddressFamily.ToString() == "InterNetwork"));

            return Nic;
        }

        public static void SetDNS(string DnsString1, string DnsString2)
        {
            string[] Dns = { DnsString1,DnsString2 };
            var CurrentInterface = GetActiveEthernetOrWifiNetworkInterface();
            if (CurrentInterface == null) return;

            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    if (objMO["Caption"].ToString().Contains(CurrentInterface.Description))
                    {
                        ManagementBaseObject objdns = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                        if (objdns != null)
                        {
                            objdns["DNSServerSearchOrder"] = Dns;
                            objMO.InvokeMethod("SetDNSServerSearchOrder", objdns, null);
                        }
                    }
                }
            }
        }

        public static void UnsetDNS()
        {
            var CurrentInterface = GetActiveEthernetOrWifiNetworkInterface();
            if (CurrentInterface == null) return;

            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    if (objMO["Caption"].ToString().Contains(CurrentInterface.Description))
                    {
                        ManagementBaseObject objdns = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                        if (objdns != null)
                        {
                            objdns["DNSServerSearchOrder"] = null;
                            objMO.InvokeMethod("SetDNSServerSearchOrder", objdns, null);
                        }
                    }
                }
            }
        }

        private void btn_minimize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btn_abr_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://abrlab.ir");
        }
    }
}
