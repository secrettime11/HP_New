using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HP_Display.OtherCS
{
    class Parameters
    {
        /// <summary>
        /// picture recognize rate
        /// </summary>
        public static double Confidential = 0.80;
        /// <summary>
        /// socket timeout included => main control / ids
        /// </summary>
        public static double timeout_socket = 300;
        /// <summary>
        /// socket port (client/server)
        /// </summary>
        public static int Socket_port = 8795;
        /// <summary>
        /// Script name
        /// </summary>
        public static string path = "";
        /// <summary>
        /// ServerIP
        /// </summary>
        public static string ip = "";
        /// <summary>
        /// IDS object
        /// </summary>
        public static dll_IDS.IDS CaptureIDS = null;
        public static Bitmap bmpScreenshot;
        public static Graphics gfxScreenshot;

        public const string Group = "Group";
        public const string Step = "Step";
        public const string ServerIP = "ServerIP";
        public const string Answer = "Answer";
        public const string Status = "Status";
        public const string Remark = "Remark";
        public const string Parameter = "Parameter";
        public const string Behavior = "Behavior";
        public const string Necessary = "Necessary";

        public class CommandLine
        {
            /// <summary>
            /// config file initial
            /// </summary>
            public static string configStr = "";
            /// <summary>
            /// initial script file
            /// </summary>
            public static string scrPathStr = "";
        }
        public class ServerSocket
        {
            /// <summary>
            /// for listening
            /// </summary>
            public static Socket SListen;
            /// <summary>
            /// which SListen accepted client
            /// </summary>
            public static Socket SClient;
            /// <summary>
            /// record each client ip n port
            /// </summary>
            public static string SEndPoint;
        }
        public class Windows
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [DllImport("user32.dll")]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

            [DllImport("user32.dll")]
            public static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

            [DllImport("user32.dll")]
            public static extern int SetCursorPos(int x, int y);
            [DllImport("user32.dll")]
            public static extern void mouse_event(
                int dwFlags,
                int dx,
                int dy,
                int dwData,
                int dwExtraInfo
            );
            [DllImport("user32.dll")]
            public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);
            
            /// <summary>
            /// global variables, record API FindWindow found hw
            /// </summary>
            public static int hw = 0;
            public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
            public const int MOUSEEVENTF_LEFTUP = 0x0004;
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

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
        }
    }
    public class Behavior 
    {
        /*IOcbDic*/
        public const string Check_PD_Plug = "Check_PD_Plug";
        public const string Check_PD_Unplug = "Check_PD_Unplug";
        public const string Check_PD_Original = "Check_PD_Original";
        public const string Check_PD_Reserve = "Check_PD_Reserve";
        public const string Check_DC_Push = "Check_DC_Push";
        public const string Check_DC_Back = "Check_DC_Back";
        public const string Check_PB_Press = "Check_PB_Press";
        public const string Check_PB_Release = "Check_PB_Release";
        public const string Check_Mouse_Click = "Check_Mouse_Click";
        public const string Check_Mouse_Release = "Check_Mouse_Release";

        /*IOcontrolDic*/
        public const string Monitor_OnOff = "Monitor_OnOff";
        public const string Hot_Plug = "Hot_Plug";
        public const string Hot_Unplug = "Hot_Unplug";
        public const string Hot_Plug_Reverse = "Hot_Plug_Reverse";
        public const string Hot_Plug_UnReverse = "Hot_Plug_UnReverse";
        public const string Press_PW_Button = "Press_PW_Button";
        public const string Mouse_Click = "Mouse_Click";
        public const string Monitor_OSD = "Monitor_OSD";
        public const string Monitor_OSD_1 = "Monitor_OSD_1";
        public const string Monitor_OSD_2 = "Monitor_OSD_2";
        public const string Monitor_OSD_3 = "Monitor_OSD_3";
        public const string Monitor_OSD_4 = "Monitor_OSD_4";
        public const string Monitor_OSD_5 = "Monitor_OSD_5";

        /*PWMDic*/
        public const string PM_Sleep = "PM_Sleep";
        public const string PM_Sleep_S3 = "PM_Sleep_S3";
        public const string PM_Hibernation = "PM_Hibernation";
        public const string PM_Warm = "PM_Warm";
        public const string PM_Cold = "PM_Cold";
        public const string PM_Special_Hibernation = "PM_Special_Hibernation";
        public const string PM_Special_WarmBoot = "PM_Special_WarmBoot";
        public const string PM_Special_ColdBoot = "PM_Special_ColdBoot";

        /*LEDDic*/
        public const string LED_Check = "LED_Check";

        /*ACDic*/
        public const string AC_OnOff = "AC_OnOff";

        /*VoltageDic*/
        public const string VA_Check = "VA_Check";
        public const string Power_Check = "Power_Check";

        /*WinDic*/
        public const string PnP_Check = "PnP_Check";
        public const string Resolution_Check = "Resolution_Check";
        public const string Display_Check = "Display_Check";

        /*DvdDic*/
        public const string PowerDVD_On = "PowerDVD_On";
        public const string PowerDVD_Off = "PowerDVD_Off";
        public const string Play_Pause = "Play_Pause";
        public const string Screen_Check = "Screen_Check";
        public const string ErrorMessage = "ErrorMessage";
        public const string HDCP_Check = "HDCP_Check";
        public const string Movie_Play_Check = "Movie_Play_Check";
        public const string Drag_DVD = "Drag_DVD";
        public const string Audio_Check = "Audio_Check";
        /*Dvd extra*/
        public const string HDCP_CheckPass = "HDCP_CheckPass";
        public const string DVD_Restart = "DVD_Restart";

        /*WaitDic*/
        public const string WaitTime = "WaitTime";
    }
    public class ComboboxItem
    {
        public ComboboxItem(string value, string text) { Value = value; Text = text; }
        public string Value { get; set; }
        public string Text { get; set; }
        public override string ToString() { return Text; }
    }
    public class DataVa
    {
        public DataVa()
        {

        }
        private SerialPort serialPort = new SerialPort();
        PacketCounter packetCounter = new PacketCounter();
        public bool OpenSerialPort(string portName)
        {
            try
            {
                serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
                serialPort.Handshake = Handshake.RequestToSend;
                // set timeout else writes to port without RTS will freeze application
                serialPort.WriteTimeout = 100;
                serialPort.DtrEnable = true;
                serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                serialPort.Open();
                //UpdateCaption();
                packetCounter.Reset();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        TextBoxBuffer textBoxBuffer = new TextBoxBuffer(4096);

        public string getDATA()
        {
            // Print textBoxBuffer to terminal
            if (!textBoxBuffer.IsEmpty())
            {
                try
                {
                    string getString = textBoxBuffer.Get().Substring(0, 78);
                    return getString;
                }
                catch (Exception)
                {
                    return "error";
                }
            }
            else
            {
                textBoxBuffer.Clear();
            }
            return "error";
        }
        private SerialDecoder serialDecoder = new SerialDecoder();
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Get bytes from serial port

            int bytesToRead = serialPort.BytesToRead;
            byte[] readBuffer = new byte[bytesToRead];
            serialPort.Read(readBuffer, 0, bytesToRead);
            // Process bytes one at a time
            foreach (byte b in readBuffer)
            {
                // Decode channel value if XStick communication active

                // Parse bytes to textBoxBuffer and serialDecoder
                if ((b < 0x20 || b > 0x7F) && b != '\r')    // replace non-printable characters with '.'
                {
                    textBoxBuffer.Put('.');
                }
                else if (b == '\r')     // replace carriage return with '↵' and valid new line
                {
                    textBoxBuffer.Put("<" + Environment.NewLine);
                }
                else    // parse all other characters to textBoxBuffer
                {
                    textBoxBuffer.Put((char)b);
                }
                serialDecoder.ProcessNewByte(b);    // process every byte through serialDecoder
            }
        }
        public void CloseSerialPort()
        {
            try
            {
                serialPort.Close();
            }
            catch { }
        }

    }
}
