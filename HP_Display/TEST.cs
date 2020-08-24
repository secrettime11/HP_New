using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HP_Display
{
    public partial class TEST : Form
    {
        int startPort;
        int portCount;
        string Rdata;
        public TEST()
        {
            InitializeComponent();
            startPort = 0;
            portCount = 1;
        }

        private void TEST_Load(object sender, EventArgs e)
        {
            //Console.WriteLine(Screen.PrimaryScreen);

            string kfcs = @"MONITOR\ACR0294\{4d36e96e-e325-11ce-bfc1-08002be10318}\0002";
            string fuckyou = FuncClass.StringCut(kfcs);
            //MessageBox.Show(fuckyou);
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                Console.WriteLine("Device Name: " + screen.DeviceName);//裝置名稱
                Console.WriteLine("Bounds: " + screen.Bounds.ToString());//螢幕解析度
                Console.WriteLine("Working Area: " + screen.WorkingArea.ToString());//實際工作區域
                
            }
            
            /*
            try
            {
                ManagementObjectSearcher USB = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE service =\"monitor\"");
                ManagementObjectCollection USBinfo = USB.Get();
                string serialNumber = "";
                List<string> name = new List<string>();
                foreach (ManagementObject MO in USBinfo)
                {
                    serialNumber = (string)MO["HardwareID"];
                    name.Add((string)MO["Name"]);
                    Console.WriteLine(serialNumber);
                }
            }
            catch (ManagementException ex)
            {
                MessageBox.Show("An error occurred while querying for WMI data: " + ex.Message);
            }
            */
            /*
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher("Select * From Win32_PnPEntity WHERE service =\"monitor\""))
                collection = searcher.Get();
            foreach (ManagementBaseObject device in collection)
            {
                string[] vs = (string[])device.GetPropertyValue("HardwareID");
                foreach (var item in vs)
                {
                    Console.WriteLine("56456"+item);
                }
                Console.WriteLine((string)device.GetPropertyValue("Availability"));
                Console.WriteLine((string)device.GetPropertyValue("PNPDeviceID"));
                Console.WriteLine((string)device.GetPropertyValue("Description"));
                //Console.WriteLine((string)device.GetPropertyValue("HardwareID[0]"));
            }
            collection.Dispose();
            */
            /*
            var device = new DISPLAY_DEVICE();
            device.cb = Marshal.SizeOf(device);
            foreach (var screen in Screen.AllScreens)
            {
                for (uint id = 0; EnumDisplayDevices(null, id, ref device, 0); id++)
                {
                    device.cb = Marshal.SizeOf(device);
                    EnumDisplayDevices(device.DeviceName, 0, ref device, 0);
                    device.cb = Marshal.SizeOf(device);

                    Console.WriteLine("id={0}, name={1}, devicestring={2}", device.DeviceID, device.DeviceName, device.DeviceString);
                    if (device.DeviceName == null || device.DeviceName == "") continue;

                    //if (screen.DeviceName.Contains("DISPLAY1") && device.DeviceName.Contains("DISPLAY1"))
                    //{
                    //    //idl.Add();
                    //}
                }
            }
            */
        }
        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            ///<summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            ///<summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            ///<summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            ///<summary>The device is VGA compatible.</summary>
            VGACompatible = 0x16,
            ///<summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            ///<summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
            try
            {
                Process p = new Process();
                String str = null;

                p.StartInfo.FileName = "cmd.exe";
                //p.StartInfo.Arguments = "";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = false;
                p.Start();
                p.StandardInput.WriteLine("PDU_ICGW08.exe 192.168.0.254 1 #");
                p.StandardInput.WriteLine("exit");
                str = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();


                Console.WriteLine(str);
                Console.ReadKey();
            }
            catch
            {
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            FuncClass.Output("0x08", "USB-5856,BID#0", portCount,startPort);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FuncClass.Output("0x09", "USB-5856,BID#0", portCount, startPort);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FuncClass.Output("0x10", "USB-5856,BID#0", portCount, startPort);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FuncClass.Output("0x13", "USB-5856,BID#0", portCount, startPort);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FuncClass.Output("0x14", "USB-5856,BID#0", portCount, startPort);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FuncClass.Input("USB-5856,BID#0", startPort, Rdata);
            textBox1.Text += Rdata + Environment.NewLine;
        }
    }
}



