using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.Forms.MessageBox;
using PowerLineStatus = System.Windows.Forms.PowerLineStatus;

namespace HP_Display.OtherCS
{
    class ServerBehavior
    {
        /// <summary>
        /// 睡眠(純關螢幕)
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="MessageResult"></param>
        public static void PM_Sleep(ref Dictionary<string, object> ResultArray, ref string MessageResult)
        {
            HP_Display.Main main = new Main();
            try
            {
                int WM_SYSCOMMAND = 0x0112;
                int SC_MONITORPOWER = 0xF170;
                Parameters.Windows.SendMessage(main.Handle.ToInt32(), WM_SYSCOMMAND, SC_MONITORPOWER, 2);
                ResultArray[Parameters.Status] = "Pass";

                MessageResult = dll_PublicFuntion.Other.DictionaryToXml(ResultArray);
                Program.SSend(Parameters.ServerSocket.SClient, MessageResult);
            }
            catch (Exception kk)
            {
                MessageBox.Show(kk.Message);
            }
        }

        /// <summary>
        /// S3睡眠
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="MessageResult"></param>
        public static void PM_Sleep_S3(ref Dictionary<string, object> ResultArray, ref string MessageResult)
        {
            ResultArray[Parameters.Status] = "Pass";

            MessageResult = dll_PublicFuntion.Other.DictionaryToXml(ResultArray);
            Program.SSend(Parameters.ServerSocket.SClient, MessageResult);
            Application.SetSuspendState(PowerState.Suspend, true, true);
        }

        /// <summary>
        /// 休眠
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="MessageResult"></param>
        public static void PM_Hibernation(ref Dictionary<string, object> ResultArray, ref string MessageResult)
        {
            ResultArray[Parameters.Status] = "Pass";
            MessageResult = dll_PublicFuntion.Other.DictionaryToXml(ResultArray);
            Program.SSend(Parameters.ServerSocket.SClient, MessageResult);
            Application.SetSuspendState(PowerState.Hibernate, true, true);
        }

        /// <summary>
        /// 熱開機(重開機) => 搭休眠
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="MessageResult"></param>
        public static void PM_Warm(ref Dictionary<string, object> ResultArray, ref string MessageResult)
        {
            ResultArray[Parameters.Status] = "Pass";
            MessageResult = dll_PublicFuntion.Other.DictionaryToXml(ResultArray);
            Program.SSend(Parameters.ServerSocket.SClient, MessageResult);
            Process.Start("shutdown", "/r /t 0");
        }

        /// <summary>
        /// 冷開機(關機)
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="MessageResult"></param>
        public static void PM_Cold(ref Dictionary<string, object> ResultArray, ref string MessageResult)
        {
            ResultArray[Parameters.Status] = "Pass";
            MessageResult = dll_PublicFuntion.Other.DictionaryToXml(ResultArray);
            Program.SSend(Parameters.ServerSocket.SClient, MessageResult);
            Process.Start("shutdown", "/s /t 0");
        }

        /// <summary>
        /// 辨識觸發power management
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="MessageResult"></param>
        /// <param name="ParameterData"></param>
        public static void PM_Special(ref Dictionary<string, object> ResultArray, ref string MessageResult, ref Dictionary<string, object> ParameterData)
        {
            Image SmallPic1 = Image.FromFile(Application.StartupPath + @"\ScreenCut\" + (string)ParameterData["PictureName1"] + ".jpg");
            FuncClass.Normal_ScreenCheck(SmallPic1, ref ResultArray);
            Thread.Sleep(500);
            FuncClass.mouseClickConstant();
            Thread.Sleep(1000);
            if ((string)ResultArray[Parameters.Remark] == "Success")
            {
                Image SmallPic2 = Image.FromFile(Application.StartupPath + @"\ScreenCut\" + (string)ParameterData["PictureName2"] + ".jpg");
                FuncClass.Normal_ScreenCheck(SmallPic2, ref ResultArray);
                Thread.Sleep(500);
                FuncClass.mouseClickConstant();
                Thread.Sleep(1000);
                if ((string)ResultArray[Parameters.Remark] == "Success")
                {
                    Image SmallPic3 = Image.FromFile(Application.StartupPath + @"\ScreenCut\" + (string)ParameterData["PictureName3"] + ".jpg");
                    FuncClass.Normal_ScreenCheck(SmallPic3, ref ResultArray);
                    MessageResult = dll_PublicFuntion.Other.DictionaryToXml(ResultArray);
                    Program.SSend(Parameters.ServerSocket.SClient, MessageResult);
                    FuncClass.mouseClickConstant();
                    Thread.Sleep(500);
                    FuncClass.mouseClickConstant();
                }
            }
        }

        /// <summary>
        /// 主螢幕檢查
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="AnswerDic"></param>
        public static void PnP_Check(ref Dictionary<string, object> ResultArray, ref Dictionary<string, object> AnswerDic)
        {
            ResultArray[Parameters.Status] = "Pass";

            if (!ResultArray.ContainsKey("ID"))
                ResultArray.Add("ID", new List<string> { });
            if (!ResultArray.ContainsKey("Type"))
                ResultArray.Add("Type", new List<string> { });
            if (!ResultArray.ContainsKey("Screen"))
                ResultArray.Add("Screen", new List<string> { });
            if (!ResultArray.ContainsKey("Resolution"))
                ResultArray.Add("Resolution", new List<string> { });

            List<string> idl = new List<string>();
            List<string> typel = new List<string>();
            List<string> screenl = new List<string>();
            List<string> resolutionl = new List<string>();

            var device = new Parameters.Windows.DISPLAY_DEVICE();
            device.cb = Marshal.SizeOf(device);
            try
            {
                foreach (var screen in Screen.AllScreens)
                {
                    for (uint id = 0; Parameters.Windows.EnumDisplayDevices(null, id, ref device, 0); id++)
                    {
                        device.cb = Marshal.SizeOf(device);
                        Parameters.Windows.EnumDisplayDevices(device.DeviceName, 0, ref device, 0);
                        device.cb = Marshal.SizeOf(device);
                        if (device.DeviceName == null || device.DeviceName == "") continue;

                        if (screen.Primary && device.DeviceName.Contains(screen.DeviceName))
                        {
                            idl.Add(FuncClass.StringCut(device.DeviceID));
                            typel.Add(device.DeviceString);
                            //resolutionl.Add($"{Screen.PrimaryScreen.Bounds.Width}x{Screen.PrimaryScreen.Bounds.Height}");
                            resolutionl.Add($"{Resolution_.DESKTOP.Width}x{Resolution_.DESKTOP.Height}");
                            screenl.Add("Primary");
                        }
                        else if (!screen.Primary && device.DeviceName.Contains(screen.DeviceName))
                        {
                            idl.Add(FuncClass.StringCut(device.DeviceID));
                            typel.Add(device.DeviceString);
                            resolutionl.Add($"{screen.Bounds.Width}x{screen.Bounds.Height}");
                            screenl.Add("Secondary");
                        }
                    }
                }
                ResultArray["ID"] = idl;
                ResultArray["Type"] = typel;
                ResultArray["Screen"] = screenl;
                ResultArray["Resolution"] = resolutionl;

                string dutSc = "";
                string dyttype = "";

                Console.WriteLine(idl.Count().ToString());
                for (int i = 0; i < idl.Count(); i++)
                {
                    if (idl[i] == (string)AnswerDic["DUTID"])
                    {
                        dutSc = (string)AnswerDic["DUTID"];
                        Console.WriteLine("1:" + dutSc);
                        dyttype = typel[i];
                    }
                }
                if (!string.IsNullOrEmpty(dutSc))
                {
                    if (dyttype == "一般 PnP 監視器" || dyttype == "Generic PnP Monitor")
                    {
                        ResultArray[Parameters.Status] = "Pass";
                    }
                    else
                    {
                        ResultArray[Parameters.Status] = "Fail";
                        ResultArray[Parameters.Remark] = "Dut monitor is not generic PnP monitor ";
                    }
                }
                else
                {
                    ResultArray[Parameters.Status] = "Fail";
                    ResultArray[Parameters.Remark] = "No Dut monitor detection.";
                }
            }
            catch (Exception)
            {
                ResultArray[Parameters.Status] = "Fail";
                ResultArray[Parameters.Remark] = "No monitor has been detective.";
            }
        }

        /// <summary>
        /// 檢查是否為一般監視器
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="AnswerDic"></param>
        public static void Display_Check(ref Dictionary<string, object> ResultArray, ref Dictionary<string, object> AnswerDic)
        {
            ResultArray[Parameters.Status] = "Pass";

            if (!ResultArray.ContainsKey("ID"))
                ResultArray.Add("ID", new List<string> { });
            if (!ResultArray.ContainsKey("Screen"))
                ResultArray.Add("Screen", new List<string> { });

            List<string> idl = new List<string>();
            List<string> screenl = new List<string>();

            var device = new Parameters.Windows.DISPLAY_DEVICE();
            device.cb = Marshal.SizeOf(device);
            try
            {
                foreach (var screen in Screen.AllScreens)
                {
                    for (uint id = 0; Parameters.Windows.EnumDisplayDevices(null, id, ref device, 0); id++)
                    {
                        device.cb = Marshal.SizeOf(device);
                        Parameters.Windows.EnumDisplayDevices(device.DeviceName, 0, ref device, 0);
                        device.cb = Marshal.SizeOf(device);

                        if (device.DeviceName == null || device.DeviceName == "") continue;
                        if (device.DeviceName.Contains(screen.DeviceName))
                        {
                            if (screen.Primary)
                            {
                                idl.Add(FuncClass.StringCut(device.DeviceID));
                                screenl.Add("Primary");
                            }
                            else
                            {
                                idl.Add(FuncClass.StringCut(device.DeviceID));
                                screenl.Add("Secondary");
                            }
                        }
                    }
                }
                ResultArray["ID"] = idl;
                ResultArray["Screen"] = screenl;

                string dutID = "";
                string dutSc = "";
                for (int i = 0; i < idl.Count(); i++)
                {
                    // PRIMARY SCREEN ID COMPARISON
                    if (idl[i] == (string)AnswerDic["DUTID"])
                    {
                        dutID = (string)AnswerDic["DUTID"];
                        dutSc = screenl[i];
                    }
                }
                // MAKE SURE IS MULTIPLE SCREENS
                if (Screen.AllScreens.Length > 1)
                {
                    if (!string.IsNullOrEmpty(dutID))
                    {

                        if (dutSc == "Primary")
                        {
                            ResultArray[Parameters.Status] = "Pass";
                        }
                        else
                        {
                            ResultArray[Parameters.Status] = "Fail";
                            ResultArray[Parameters.Remark] = "Dut monitor is not generic PnP monitor ";
                        }
                    }
                    else
                    {
                        ResultArray[Parameters.Status] = "Fail";
                        ResultArray[Parameters.Remark] = "No Dut monitor detected.";
                    }
                }
                else
                {
                    ResultArray[Parameters.Status] = "Fail";
                    ResultArray[Parameters.Remark] = "No more than two monitors.";
                }
            }
            catch (Exception)
            {
                ResultArray[Parameters.Status] = "Fail";
                ResultArray[Parameters.Remark] = "Display mode is not extend mode";
            }
        }

        /// <summary>
        /// 檢查是否充電中
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="AnswerDic"></param>
        public static void Power_Check(ref Dictionary<string, object> ResultArray)
        {
            ResultArray[Parameters.Status] = "Pass";

            string powerStatus = "";
            PowerStatus pwr = SystemInformation.PowerStatus;
            switch (pwr.PowerLineStatus)
            {
                case (PowerLineStatus.Offline):
                    powerStatus = "\r\nNot Plugged in";
                    break;

                case (PowerLineStatus.Online):
                    powerStatus = "\r\nPlugged in";
                    break;
            }
            ResultArray[Parameters.Remark] = powerStatus;
            //if (pwr.BatteryLifeRemaining > 0)
            //    powerStatus = "\r\n" +
            //        String.Format("{0} hr {1} min remaining",
            //        pwr.BatteryLifeRemaining / 3600, (pwr.BatteryLifeRemaining % 3600) / 60);
        }

        /// <summary>
        /// 解析度檢查(DUT自身主螢幕解析度)
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="AnswerDic"></param>
        public static void Resolution_Check(ref Dictionary<string, object> ResultArray, ref Dictionary<string, object> AnswerDic)
        {
            #region Windows API resolution
            List<string> idList = new List<string>();
            List<string> resolutionList = new List<string>();

            var device = new Parameters.Windows.DISPLAY_DEVICE();
            device.cb = Marshal.SizeOf(device);

            string dutSc = "";
            string dutResolution = "";

            try
            {
                foreach (var screen in Screen.AllScreens)
                {
                    for (uint id = 0; Parameters.Windows.EnumDisplayDevices(null, id, ref device, 0); id++)
                    {
                        device.cb = Marshal.SizeOf(device);
                        Parameters.Windows.EnumDisplayDevices(device.DeviceName, 0, ref device, 0);
                        device.cb = Marshal.SizeOf(device);

                        if (device.DeviceName == null || device.DeviceName == "") continue;
                        if (device.DeviceName.Contains(screen.DeviceName))
                        {
                            idList.Add(FuncClass.StringCut(device.DeviceID));
                            //resolutionList.Add($"{screen.Bounds.Width}x{screen.Bounds.Height}");
                        }
                    }
                }
                // Get primary screen resolution
                System.Drawing.Size PRI_SCREEN = Resolution_.DESKTOP;
                for (int i = 0; i < idList.Count(); i++)
                {
                    if (idList[i] == (string)AnswerDic["DUTID"])
                    {
                        dutSc = (string)AnswerDic["DUTID"];
                        //resolutionList.Add($"{Resolution_.DESKTOP.Width}x{Resolution_.DESKTOP.Height}");
                        dutResolution = PRI_SCREEN.Width + "x" + PRI_SCREEN.Height;
                    }
                }
            }
            catch (Exception)
            {
                ResultArray[Parameters.Status] = "Fail";
                ResultArray[Parameters.Remark] = "No monitor has been detective.";
            }
            #endregion

            string OriDut = dutResolution;
            // Windows api resolution
            //dutResolution = FuncClass.RexResolution(dutResolution.Trim()); 
            ResultArray[Parameters.Status] = "Pass";
            ResultArray[Parameters.Remark] = OriDut;
        }

        /// <summary>
        /// PowerDVD 播放/暫停指令
        /// </summary>
        /// <param name="ResultArray"></param>
        public static void Play_Pause(ref Dictionary<string, object> ResultArray)
        {
            ResultArray[Parameters.Status] = "Pass";
            FuncClass.UpFrontDVD();
            FuncClass.KeyBoard.keyPress(FuncClass.KeyBoard.vKeySpace);
        }

        /// <summary>
        /// 檢查 ErrorMessage
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="ParameterData"></param>
        public static void ErrorMessage(ref Dictionary<string, object> ResultArray, ref Dictionary<string, object> ParameterData) 
        {
            ResultArray[Parameters.Status] = "Pass";
            Image SmallPic = Image.FromFile(Application.StartupPath + @"\ScreenCut\" + (string)ParameterData["PictureName"] + ".jpg");
            FuncClass.ErrorMsg_Check(SmallPic, ref ResultArray);
        }

        /// <summary>
        /// 檢查螢幕截圖(小圖路徑) => 檢查errorMsg
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="ParameterData"></param>
        public static void HDCP_Check(ref Dictionary<string, object> ResultArray, ref Dictionary<string, object> ParameterData)
        {
            //Image SmallPic = Image.FromFile(Application.StartupPath + @"\ScreenCut\" + (string)ParameterData["HDCP_PictureName"] + ".jpg");

            // Client端螢幕截的小圖路徑
            string picPath = Application.StartupPath + @"\ScreenCut\" + (string)ParameterData["HDCP_PictureName"] + ".jpg";

            if (File.Exists(picPath))
            {
                Image SmallPic = FuncClass.OperateImage(picPath);
                Thread.Sleep(500);
                // Error Message檢查 (1)
                FuncClass.ErrorMsg_Check(SmallPic, ref ResultArray);

                if ((string)ResultArray[Parameters.Status] == "Fail")
                {
                    ResultArray[Parameters.Status] = "Pass";
                    ResultArray[Parameters.Remark] = "No error message.";
                }
            }
            else
            {
                ResultArray[Parameters.Status] = "Fail";
                ResultArray[Parameters.Remark] = "Picture not found.";
            }
        }

        /// <summary>
        /// 關閉PowerDVD
        /// </summary>
        /// <param name="ResultArray"></param>
        public static void PowerDVD_Off(ref Dictionary<string, object> ResultArray)
        {
            ResultArray[Parameters.Status] = "Pass";

            Process[] processes = Process.GetProcessesByName("powerDVD");
            Console.WriteLine($"processes.Length:{processes.Length}");
            if (processes.Length > 0)
            {
                foreach (Process p in processes)
                {
                    try
                    {
                        ResultArray[Parameters.Status] = "Pass";
                        p.Kill();
                        //p.WaitForExit();
                    }
                    catch (Exception)
                    {
                        ResultArray[Parameters.Status] = "Fail";
                        ResultArray[Parameters.Remark] = "PowerDVD close error.";
                    }

                }
            }
            else
            {
                ResultArray[Parameters.Status] = "Fail";
                ResultArray[Parameters.Remark] = "Can't find powerDVD.";
            }
        }

        /// <summary>
        /// Movie_Play_Check
        /// </summary>
        /// <param name="ResultArray"></param>
        public static void Movie_Play_Check(ref Dictionary<string, object> ResultArray) 
        {
            if (FuncClass.UpFrontDVD())
            {
                ResultArray[Parameters.Status] = "Pass";
            }
            else
            {
                ResultArray[Parameters.Status] = "Fail";
                ResultArray[Parameters.Remark] = "Can't find powerDVD.";
            }
        }

        /// <summary>
        /// PowerDVD 重開
        /// </summary>
        /// <param name="ResultArray"></param>
        public static void DVD_Restart(ref Dictionary<string, object> ResultArray) 
        {
            //Process[] processes = Process.GetProcessesByName("powerDVD");
            //if (processes.Length > 0)
            //{
            //    foreach (Process p in processes)
            //    {
            //        p.Kill();
            //        //p.WaitForExit();
            //    }
            //    Thread.Sleep(2000);
            //    FuncClass.DVD_On(ref ResultArray);
            //    if ((string)ResultArray[Parameters.Status] != "Fail")
            //    {
            //        ResultArray[Parameters.Status] = "Fail";
            //        ResultArray[Parameters.Remark] = "Already restart PowerDVD.";
            //    }
            //}
            //else
            //{
            //    ResultArray[Parameters.Status] = "Fail";
            //    ResultArray[Parameters.Remark] = "Can't find powerDVD.";
            //}
            FuncClass.DVD_On(ref ResultArray);
            if ((string)ResultArray[Parameters.Status] != "Fail")
            {
                ResultArray[Parameters.Status] = "Fail";
                ResultArray[Parameters.Remark] = "Already start PowerDVD.";
            }
        }

        /// <summary>
        /// 拖拉PowerDVD
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="ParameterData"></param>
        public static void Drag_DVD(ref Dictionary<string, object> ResultArray, ref Dictionary<string, object> ParameterData)
        {
            ResultArray[Parameters.Status] = "Pass";

            FuncClass.UpFrontDVD();
            Thread.Sleep(1500);
            Parameters.Windows.hw = (int)Parameters.Windows.FindWindow(Constants.vbNullString, "powerDVD");
            if (Parameters.Windows.hw >= 1000)
            {
                Parameters.Windows.SetWindowPos((IntPtr)Parameters.Windows.hw, (IntPtr)0, (Screen.PrimaryScreen.WorkingArea.Width / 2) - 640, (Screen.PrimaryScreen.WorkingArea.Height / 2) - 360, 1280, 720, 4); // 設定記事本顯示的位置和顯示的大小
                int CursorX = 0;
                int CursorY = 0;
                CursorX = Screen.PrimaryScreen.Bounds.Width / 2;
                CursorY = (Screen.PrimaryScreen.Bounds.Height / 2) - 355;
                Parameters.Windows.SetCursorPos(CursorX, CursorY);
                Thread.Sleep(300);
                Parameters.Windows.mouse_event(Parameters.Windows.MOUSEEVENTF_LEFTDOWN, CursorX, CursorY, 0, 0);
                //dll_PublicFuntion.WindowsAPI.MouseClick(true, "Left");
                Thread.Sleep(1000);
                if ((string)ParameterData["Direction"] == "LeftToRight")
                {
                    for (int i = 0; i < 200; i++)
                    {
                        CursorX = CursorX + 10;
                        Parameters.Windows.SetCursorPos(CursorX, CursorY);
                        Thread.Sleep(10);
                    }
                    Thread.Sleep(1000);
                    for (int i = 0; i < 200; i++)
                    {
                        CursorX = CursorX - 10;
                        Parameters.Windows.SetCursorPos(CursorX, CursorY);
                        Thread.Sleep(10);
                    }
                }
                else if ((string)ParameterData["Direction"] == "RightToLeft")
                {
                    for (int i = 0; i < 200; i++)
                    {
                        CursorX = CursorX - 10;
                        Parameters.Windows.SetCursorPos(CursorX, CursorY);
                        Thread.Sleep(10);
                    }
                    Thread.Sleep(1000);
                    for (int i = 0; i < 200; i++)
                    {
                        CursorX = CursorX + 10;
                        Parameters.Windows.SetCursorPos(CursorX, CursorY);
                        Thread.Sleep(10);
                    }
                }

                Thread.Sleep(100);
                Parameters.Windows.mouse_event(Parameters.Windows.MOUSEEVENTF_LEFTUP, CursorX, CursorY, 0, 0);
            }
            else
            {
                ResultArray[Parameters.Status] = "Fail";
                ResultArray[Parameters.Remark] = "No powerDVD detected.";
            }
        }

        /// <summary>
        /// Audio_Check
        /// </summary>
        /// <param name="ResultArray"></param>
        public static void Audio_Check(ref Dictionary<string, object> ResultArray, ref Dictionary<string, object> ParameterData) 
        {
            ResultArray[Parameters.Status] = "Pass";
            ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PnPEntity");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            ResultArray[Parameters.Status] = "Fail";
            ResultArray[Parameters.Remark] = "Audio is not detected.";
            foreach (ManagementObject Obj in queryCollection)
            {
                //Console.WriteLine((string)ParameterData["Audio_Name"]);
                if (Obj["Name"] != null && Obj["Name"].ToString().Contains((string)ParameterData["Audio_Name"]))
                {
                    Console.WriteLine("  Name : {0} \t Status : {1}", Obj["Name"], Obj["Status"]);
                    if (Obj["Status"].ToString().Trim() == "OK")
                    {
                        ResultArray[Parameters.Status] = "Pass";
                        ResultArray[Parameters.Remark] = "";
                    }
                    else if (Obj["Status"].ToString().Trim() != "OK")
                    {
                        ResultArray[Parameters.Status] = "Fail";
                        ResultArray[Parameters.Remark] = "Audio is not working.";
                    }
                }
            }
        }

        /// <summary>
        /// 片頭，送出Enter
        /// </summary>
        /// <param name="ResultArray"></param>
        public static void Send_Enter(ref Dictionary<string, object> ResultArray)
        {
            if (FuncClass.UpFrontDVD())
            {
                int CursorX = Screen.PrimaryScreen.Bounds.Width / 2;
                int CursorY = (Screen.PrimaryScreen.Bounds.Height / 2) - 355;
                Parameters.Windows.SetCursorPos(CursorX, CursorY);
                Thread.Sleep(300);
                Parameters.Windows.mouse_event(Parameters.Windows.MOUSEEVENTF_LEFTDOWN, CursorX, CursorY, 0, 0);
                Parameters.Windows.mouse_event(Parameters.Windows.MOUSEEVENTF_LEFTUP, CursorX, CursorY, 0, 0);
                Thread.Sleep(500);
                FuncClass.KeyBoard.keyPress(FuncClass.KeyBoard.vKeyReturn);
                // 等待三分鐘
                Thread.Sleep(180 * 1000);
                ResultArray[Parameters.Status] = "Pass";
                ResultArray[Parameters.Remark] = "Enter sent.";
            }
            else
            {
                ResultArray[Parameters.Status] = "Fail";
                ResultArray[Parameters.Remark] = "Can't find powerDVD.";
            }
        }
    }
}
