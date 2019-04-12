using ParadoxIp.Managers;
using ParadoxIp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SmartHomeParadox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IpModule module = new IpModule(new Url("http://192.168.100.100:80"), 4562, "paradox");
        public IpModuleManager ipmm;
        //SqlConnection conn;
        public ObservableCollection<Device> AllDevices { get; set; }
        DispatcherTimer dt = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            //conn = new SqlConnection(@"Data Source=PC06DEVENV,1433;Initial Catalog=SmartHomeDemo2018;User ID=sa;Password=Password1");
            //conn.Open();
            AllDevices = new ObservableCollection<Device>();
            ipmm = new IpModuleManager(module);
            DataContext = this;

        }

        private void Dt_Tick(object sender, EventArgs e)
        {
            ipmm.GetStatus();
            //SqlCommand cmd = new SqlCommand("Select * from ParadoxSystem", conn);
            //using (SqlDataReader sdr = cmd.ExecuteReader())
            //{
            //    while (sdr.Read())
            //    {
            //        if (Convert.ToInt32(sdr[0]) == 1)
            //        {
            //            ipmm.AlarmAction(ParadoxIp.Enum.PartitionNumber.All, ParadoxIp.Enum.AlarmMode.RegularArm);
            //        }
            //        else if (Convert.ToInt32(sdr[0]) == 0)
            //        {
            //            ipmm.AlarmAction(ParadoxIp.Enum.PartitionNumber.All, ParadoxIp.Enum.AlarmMode.Disarm);
            //        }
            //    }
            //}
        }
        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            ipmm.Logout();
            BtnConnect.IsEnabled = false;
            if (!ipmm.IsLoggedIn)
                ipmm.Login();
            ipmm.GetAlarmInformation();
            BtnGetAlarmInfo.IsEnabled = true;
            BtnGetDeviceStatus.IsEnabled = true;
            BtnGetVersionInfo.IsEnabled = true;
            CmbArming.IsEnabled = true;
            ipmm.DeviceStatusChanged += Ipmm_DeviceStatusChanged;
            AllDevices = new ObservableCollection<Device>(ipmm.Devices);
            LstDevices.ItemsSource = AllDevices;
            dt.Interval = new TimeSpan(0, 0, 0, 1, 50);
            dt.Start();
            dt.Tick += Dt_Tick;
        }

        private void Ipmm_DeviceStatusChanged(object sender, ParadoxIp.Events.DeviceUpdateEventArgs e)
        {
            AllDevices = new ObservableCollection<Device>(ipmm.Devices);
            LstDevices.ItemsSource = AllDevices;
            string CmdStr = "";
            foreach (Device dd in AllDevices)
            {
                if (dd.Status == ParadoxIp.Enum.DeviceStatus.Bypassed)
                {
                    CmdStr = CmdStr + dd.Name + " = 1,";
                }
                else
                {
                    CmdStr = CmdStr + dd.Name + " = 0,";
                }
            }
            CmdStr = CmdStr.TrimEnd(',');

            //SqlCommand cmd = new SqlCommand("Update SensorTbl SET " + CmdStr, conn);
            //cmd.ExecuteNonQuery();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ipmm.Logout();
        }


        private void BtnChangeArm_Click(object sender, RoutedEventArgs e)
        {
            ChkArm.Fill = new SolidColorBrush(Colors.Red);
            ChkDisarm.Fill = new SolidColorBrush(Colors.Red);
            ChkSleep.Fill = new SolidColorBrush(Colors.Red);
            ChkStay.Fill = new SolidColorBrush(Colors.Red);
            if (CmbArming.Text == "Regular Arm")
            {
                ipmm.AlarmAction(ParadoxIp.Enum.PartitionNumber.All, ParadoxIp.Enum.AlarmMode.RegularArm);
                ChkArm.Fill = new SolidColorBrush(Colors.Green);
            }
            else if (CmbArming.Text == "Sleep")
            {
                ipmm.AlarmAction(ParadoxIp.Enum.PartitionNumber.All, ParadoxIp.Enum.AlarmMode.SleepArm);
                ChkSleep.Fill = new SolidColorBrush(Colors.Green);
            }
            else if (CmbArming.Text == "Stay")
            {
                ipmm.AlarmAction(ParadoxIp.Enum.PartitionNumber.All, ParadoxIp.Enum.AlarmMode.StayArm);
                ChkStay.Fill = new SolidColorBrush(Colors.Green);
            }
            else if (CmbArming.Text == "Off")
            {
                ipmm.AlarmAction(ParadoxIp.Enum.PartitionNumber.All, ParadoxIp.Enum.AlarmMode.Disarm);
                ChkDisarm.Fill = new SolidColorBrush(Colors.Green);
            }
        }

        private void CmbArming_DropDownClosed(object sender, EventArgs e)
        {
            if (CmbArming.SelectedIndex == 0)
            {
                BtnChangeArm.IsEnabled = false;
            }
            else
            {
                BtnChangeArm.IsEnabled = true;
            }
        }

        private void BtnGetAlarmInfo_Click(object sender, RoutedEventArgs e)
        {
            ipmm.GetAlarmInformation();
            AllDevices = new ObservableCollection<Device>(ipmm.Devices);
            LstDevices.ItemsSource = AllDevices;
        }

        private void BtnGetDeviceStatus_Click(object sender, RoutedEventArgs e)
        {
            ipmm.GetStatus();
            AllDevices = new ObservableCollection<Device>(ipmm.Devices);
            LstDevices.ItemsSource = AllDevices;

        }
    }
}
