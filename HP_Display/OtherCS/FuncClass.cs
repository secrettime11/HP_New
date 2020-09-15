using dll_PublicFuntion;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using HP_Display.OtherCS;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace HP_Display
{
    class FuncClass
    {
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;

        public static string ToolPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\";
        /// <summary>
        /// 檢查路徑資料夾(沒有則create)
        /// </summary>
        /// <param name="dirpath"></param>
        /// <returns></returns>
        public static string Check_path(string dirpath)
        {
            string dirpath_out = ToolPath + dirpath + @"\";
            if (!Directory.Exists(dirpath_out))
            {
                Directory.CreateDirectory(dirpath_out);
            }
            return dirpath_out;
        }
        /// <summary>
        /// Hardware ID 正規化
        /// </summary>
        /// <param name="oriString">整串ID</param>
        /// <returns></returns>
        public static string StringCut(string oriString)
        {
            /*MONITOR\ACR0294\{4d36e96e-e325-11ce-bfc1-08002be10318}\0002*/
            string[] xxx = oriString.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
            string sb = xxx[1];
            return sb;
        }
        /// <summary>  
        /// 修改程序在注册表中的键值  
        /// </summary>  
        /// <param name="isAuto">true:开机启动,false:不开机自启</param> 
        public static void AutoStart(bool isAuto)
        {
            try
            {
                if (isAuto == true)
                {
                    RegistryKey R_local = Registry.CurrentUser;//RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    R_run.SetValue("HD_Display.exe", System.Windows.Forms.Application.ExecutablePath);
                    R_run.Close();
                    R_local.Close();
                }
                else
                {
                    RegistryKey R_local = Registry.CurrentUser;//RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    R_run.DeleteValue("HD_Display.exe", false);
                    R_run.Close();
                    R_local.Close();
                }
                //GlobalVariant.Instance.UserConfig.AutoStart = isAuto;
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("您需要管理員權限修改", "提示");
            }
        }
        /// <summary>
        /// IO治具連接JAVA傳送方式
        /// </summary>
        /// <param name="Outport">輸出Port</param>
        /// <param name="status">開或關(HIGH/LOW)</param>
        /// <param name="receiveMsg">收到的狀態</param>
        /// <param name="Type">DO或DI</param>
        public static void DAQ(string Outport, string status, ref string receiveMsg, string Type)
        {
            try
            {

                int port = 6613;

                string host = "127.0.0.1";

                IPAddress ip = IPAddress.Parse(host);

                IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和埠轉化為IPEndPoint例項

                Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//建立一個Socket

                Console.WriteLine("Conneting...");

                c.Connect(ipe);//連線到伺服器

                string sendStr = "";
                if (Type == "DO")
                {
                    sendStr = $"{Type},{Outport},{status}" + '\n';
                }
                if (Type == "DI")
                {
                    sendStr = $"{Type},{Outport}" + '\n';
                }
                byte[] bs = Encoding.ASCII.GetBytes(sendStr);

                Console.WriteLine("Send Message");

                c.Send(bs, bs.Length, 0);//傳送測試資訊

                string recvStr = "";

                byte[] recvBytes = new byte[1024];

                int bytes;

                bytes = c.Receive(recvBytes, recvBytes.Length, 0);//從伺服器端接受返回資訊

                recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);

                receiveMsg = recvStr;

                Console.WriteLine("Client Get Message:{0}", recvStr);//顯示伺服器返回資訊

                c.Close();
            }

            catch (ArgumentNullException e1)
            {
                Console.WriteLine("ArgumentNullException: {0}", e1);
            }

            catch (SocketException e2)
            {
                Console.WriteLine("SocketException: {0}", e2);
            }
            Console.WriteLine("Press Enter to Exit");
        }
        /// <summary>
        /// 讀取圖片的顏色(依面積最大)
        /// </summary>
        /// <param name="src">Image</param>
        /// <returns>String color</returns>
        public static string getcolor(Image<Bgr, byte> src)
        {
            //設定各種顏色的HSV範圍值
            String rlt = "";
            Dictionary<string, Hsv[]> colorrange = new Dictionary<string, Hsv[]>();
            Hsv[] ListColor = null;
            ////black
            //Hsv blacklowerLimit = new Hsv(0, 0, 0);
            //Hsv blackupperLimit = new Hsv(180, 255, 50);
            //ListColor = new Hsv[] { blacklowerLimit, blackupperLimit };
            //colorrange.Add("black", ListColor);

            ////gray
            //Hsv graylowerLimit = new Hsv(0, 0, 50);
            //Hsv grayupperLimit = new Hsv(180, 43, 220);
            //ListColor = new Hsv[] { graylowerLimit, grayupperLimit };
            //colorrange.Add("gray", ListColor);

            //white
            Hsv whitelowerLimit = new Hsv(0, 0, 221);
            Hsv whiteupperLimit = new Hsv(180, 40, 255);
            ListColor = new Hsv[] { whitelowerLimit, whiteupperLimit };
            colorrange.Add("white", ListColor);


            //Orange
            Hsv OrangelowerLimit = new Hsv(8, 40, 50);
            Hsv OrangewupperLimit = new Hsv(34, 255, 255);
            ListColor = new Hsv[] { OrangelowerLimit, OrangewupperLimit };
            colorrange.Add("Orange", ListColor);

            //green
            Hsv greenlowerLimit = new Hsv(33, 40, 50);
            Hsv greenupperLimit = new Hsv(87, 255, 255);
            ListColor = new Hsv[] { greenlowerLimit, greenupperLimit };
            colorrange.Add("green", ListColor);

            //blue
            Hsv bluelowerLimit = new Hsv(90, 40, 50);
            Hsv blueupperLimit = new Hsv(135, 255, 255);
            ListColor = new Hsv[] { bluelowerLimit, blueupperLimit };
            colorrange.Add("blue", ListColor);

            //計算各顏色面積，並回傳最大面積之顏色名稱

            Image<Hsv, Byte> hsvsrc = src.Clone().Convert<Hsv, Byte>();
            double maxsumArea = 0;
            String colorD = "none";
            foreach (var item in colorrange)
            {
                Image<Gray, Byte> mask_hsv = hsvsrc.InRange(item.Value[0], item.Value[1]);
                Image<Gray, Byte> ThB = null;
                ThB = mask_hsv.ThresholdBinary(new Gray(127), new Gray(255));
                //Dilate the image
                Image<Gray, Byte> dilate = ThB.Dilate(2);
                VectorOfVectorOfPoint con = new VectorOfVectorOfPoint();
                CvInvoke.FindContours(dilate, con, src, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                double sumarea = 0;
                for (int i = 0; i < con.Size; i++)
                {
                    //獲取獨立的連通輪廓
                    VectorOfPoint contour = con[i];

                    //計算連通輪廓的面積
                    sumarea = sumarea + CvInvoke.ContourArea(contour);
                }
                if (sumarea > maxsumArea)
                {
                    colorD = item.Key;
                    maxsumArea = sumarea;
                }

            }
            //Console.WriteLine("color::"+colorD);
            rlt = colorD;
            hsvsrc.Dispose();
            return rlt;
        }
        /// <summary>
        /// 解析度正規化
        /// </summary>
        /// <param name="matchString">需要正規化的字串</param>
        /// <returns></returns>
        public static string RexResolution(string matchString)
        {
            string result = "";
            //規則字串 
            string pattern = @"\d";
            //宣告 Regex 忽略大小寫 
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(matchString);
            foreach (var item in matches)
            {
                result += item;
            }
            return result;
        }
        /// <summary>
        /// PowerDVD置頂
        /// </summary>
        public static bool UpFrontDVD()
        {
            Boolean Status = false;
            try
            {
                dll_PublicFuntion.WindowsAPI.WinformControl winformControl = new WindowsAPI.WinformControl(2, "powerDVD", ref Status);
                if (Status)
                {
                    Status = winformControl.SetForegroundWindow();
                    Status = winformControl.ShowWindow();
                    Thread.Sleep(1000);
                    ExePosition();
                    return true;
                }
            }
            catch (Exception excep)
            {
                Console.WriteLine("Upfront error:" + excep.Message);
                return false;
            }
            return false;
        }
        /// <summary>
        /// 螢幕截圖(框小圖)檢查
        /// </summary>
        /// <param name="SmallPicture"></param>
        /// <param name="ResultArray"></param>
        public static void ErrorMsg_Check(Image SmallPicture, ref Dictionary<string, object> ResultArray)
        {
            UpFrontDVD();
            // Screen shot
            //Parameters.Windows.mouse_event(MOUSEEVENTF_LEFTDOWN, Cursor.Position.X, Cursor.Position.Y, 0, 0);
            //Thread.Sleep(20);
            //Parameters.Windows.mouse_event(MOUSEEVENTF_LEFTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
            //Thread.Sleep(20);
            ResultArray["Status"] = "Pass";

            try
            {
                if (Parameters.bmpScreenshot != null) Parameters.bmpScreenshot.Dispose();
                Parameters.bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                if (Parameters.gfxScreenshot != null) Parameters.gfxScreenshot.Dispose();
                Parameters.gfxScreenshot = Graphics.FromImage(Parameters.bmpScreenshot);
                Parameters.gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                Image BigShot = (Image)Parameters.bmpScreenshot;
                Thread.Sleep(300);
                //Image SmallPic = Image.FromFile(Application.StartupPath + @"\ScreenCut\" + (string)ParameterData["PictureName"] + ".jpg");
                dll_opencv.OpenCV openCV = new dll_opencv.OpenCV();
                openCV.SourceLoadImage(BigShot);
                openCV.SourceToGray();
                openCV.MatchLoadImage(SmallPicture);
                openCV.MatchToGray();
                dll_opencv.OpenCV.MatchDataList matchDataList = openCV.GetMatchPos(Parameters.Confidential, false);

                double resultConfident = matchDataList.MaxConfidencevalue;
                // Get screen scale percent
                double ScreenScale = Resolution_.ScaleX;
                if (resultConfident > Parameters.Confidential)
                {
                    double CursorX = 0;
                    double CursorY = 0;
                    foreach (var item in matchDataList.MatchData)
                    {
                        CursorX = item.Rectangle.X + (item.Rectangle.Width / 2);
                        CursorY = item.Rectangle.Y + (item.Rectangle.Height / 2);
                    }
                    Parameters.Windows.SetCursorPos(Convert.ToInt32(Math.Round(CursorX / ScreenScale, 0)), Convert.ToInt32(Math.Round(CursorY / ScreenScale, 0)));
                    Thread.Sleep(2000);
                    ResultArray["Remark"] = "OnCancel";
                }
                else
                {
                    ResultArray["Status"] = "Fail";
                    ResultArray["Remark"] = "No match.";
                }
            }
            catch (Exception ep)
            {
                ResultArray["Status"] = "Fail";
                ResultArray["Remark"] = ep.Message;
            }
        }
        /// <summary>
        /// 一般螢幕圖片比對
        /// </summary>
        public static void Normal_ScreenCheck(Image SmallPicture, ref Dictionary<string, object> ResultArray)
        {
            ResultArray["Status"] = "Pass";

            // Screen shot
            try
            {
                if (Parameters.bmpScreenshot != null) Parameters.bmpScreenshot.Dispose();
                Parameters.bmpScreenshot = new Bitmap(Resolution_.DESKTOP.Width, Resolution_.DESKTOP.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                if (Parameters.gfxScreenshot != null) Parameters.gfxScreenshot.Dispose();
                Parameters.gfxScreenshot = Graphics.FromImage(Parameters.bmpScreenshot);
                Parameters.gfxScreenshot.CopyFromScreen(0, 0, 0, 0, Resolution_.DESKTOP);
                Image BigShot = (Image)Parameters.bmpScreenshot;
                Thread.Sleep(300);

                dll_opencv.OpenCV openCV = new dll_opencv.OpenCV();
                openCV.SourceLoadImage(BigShot);
                openCV.SourceToGray();
                openCV.MatchLoadImage(SmallPicture);
                openCV.MatchToGray();
                dll_opencv.OpenCV.MatchDataList matchDataList = openCV.GetMatchPos(Parameters.Confidential, false);
                double resultConfident = matchDataList.MaxConfidencevalue;
                double ScreenScale = Resolution_.ScaleX;
                if (resultConfident > Parameters.Confidential)
                {
                    int CursorX = 0;
                    int CursorY = 0;
                    foreach (var item in matchDataList.MatchData)
                    {
                        CursorX = item.Rectangle.X + (item.Rectangle.Width / 2);
                        CursorY = item.Rectangle.Y + (item.Rectangle.Height / 2);
                    }
                    Parameters.Windows.SetCursorPos(Convert.ToInt32(Math.Round(CursorX / ScreenScale, 0)), Convert.ToInt32(Math.Round(CursorY / ScreenScale, 0)));
                    Thread.Sleep(300);

                    ResultArray["Remark"] = "Success";
                }
                else
                {
                    ResultArray["Status"] = "Fail";
                    ResultArray["Remark"] = "No match.";
                }
            }
            catch (Exception ep)
            {
                ResultArray["Status"] = "Fail";
                ResultArray["Remark"] = ep.Message;
            }
        }
        /// <summary>
        /// 模擬滑鼠點擊
        /// </summary>
        public static void mouseClickConstant()
        {
            int CursorX = Cursor.Position.X;
            int CursorY = Cursor.Position.X;

            Parameters.Windows.mouse_event(MOUSEEVENTF_LEFTDOWN, CursorX, CursorY, 0, 0);
            Thread.Sleep(20);
            Parameters.Windows.mouse_event(MOUSEEVENTF_LEFTUP, CursorX, CursorY, 0, 0);
            Thread.Sleep(20);
            Thread.Sleep(500);
        }
        /// <summary>
        /// IDS拍照存檔
        /// </summary>
        /// <param name="UIdata">傳入字典</param>
        /// <param name="ResultArray">回傳字典</param>
        /// <param name="dvd">要不要把dvd置頂</param>
        public static void IDS_Check(Dictionary<string, object> UIdata, ref Dictionary<string, object> ResultArray, bool dvd)
        {
            //IdsStatus();
            if (dvd)
                UpFrontDVD();
            Socket socketIDS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketIDS.ReceiveTimeout = Convert.ToInt32(Parameters.timeout_socket) * 1000;
            socketIDS.SendTimeout = Convert.ToInt32(Parameters.timeout_socket) * 1000;

            try
            {
                int bytes = 0;

                socketIDS.Connect((string)UIdata["ServerIP"], 8787);
                // Sokcet send data
                Dictionary<string, object> dic = new Dictionary<string, object>
                {
                    { "Mode", "Get" },
                    { "Behavior", "NowImage" },
                };

                if (UIdata.ContainsKey("SavePath")) dic.Add("SavePath", UIdata["SavePath"]);

                byte[] bmsg = Encoding.UTF8.GetBytes(dll_PublicFuntion.Other.DictionaryToXml(dic));
                socketIDS.Send(bmsg);
                // Receive data
                byte[] getbuffer = new byte[1024 * 50000];
                bytes = socketIDS.Receive(getbuffer);
                string msg = Encoding.UTF8.GetString(getbuffer, 0, bytes).Trim('\0');
                ResultArray["Status"] = "Pass";
                socketIDS.Close();
            }
            catch (Exception)
            {
                ResultArray["Status"] = "Fail";
                ResultArray["Remark"] = "IDS connect error";
                socketIDS.Close();
            }
        }
        /// <summary>
        /// PowerDVD On
        /// </summary>
        public static void DVD_On(ref Dictionary<string, object> ResultArray)
        {
            try
            {
                string DvDPath = "";
                string ProgramFilePath = Environment.GetEnvironmentVariable("ProgramFiles");
                DvDPath = ProgramFilePath + @"\CyberLink\PowerDVD19\PowerDVD.exe";
                //DvDPath = @"C:\Program Files (x86)\CyberLink\PowerDVD18\PowerDVD.exe";
                if (!string.IsNullOrEmpty(DvDPath))
                {
                    bool DVD_Status = false;

                    if (!CheckExe("powerDVD"))
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = $@"{DvDPath}";
                        p.StartInfo.UseShellExecute = false;
                        // powerdvd程序路徑
                        //p.StartInfo.FileName = @"C:\Program Files (x86)\CyberLink\PowerDVD18\powerDVD.exe";
                        p.Start();
                        DVD_Status = true;

                        Thread.Sleep(3000);
                        KeyBoard.keyPress(KeyBoard.vKeyLeft);
                        Thread.Sleep(500);
                        KeyBoard.keyPress(KeyBoard.vKeyReturn);
                        Thread.Sleep(1000);
                    }
                    else
                        UpFrontDVD();

                    // WAIT FOR POWERDVD OPEN
                    Thread.Sleep(5000);

                    ExePosition(ref ResultArray);

                    Thread.Sleep(2000);

                    Console.WriteLine("Press1");
                    if (DVD_Status)
                    {
                        KeyBoard.keyPress(KeyBoard.vKeySpace);
                        Thread.Sleep(3000);
                    }
                    Console.WriteLine("Press2");
                    KeyBoard.keyPress(KeyBoard.vKeySpace);
                    ResultArray["Status"] = "Pass";
                }
                else
                {
                    ResultArray["Status"] = "Fail";
                    ResultArray["Remark"] = "Can't find application.";
                }
            }
            catch (Exception)
            {
                ResultArray["Status"] = "Fail";
                ResultArray["Remark"] = "Can't find powerDVD application.";
            }

        }
        /// <summary>
        /// Check Network
        /// </summary>
        /// <returns></returns>
        public static Boolean DnsTest()
        {
            Boolean NetgGet = false;
            while (!NetgGet)
            {
                try
                {
                    System.Net.IPHostEntry ipHe =
                        System.Net.Dns.GetHostByName("www.google.com");
                    NetgGet = true;
                }
                catch
                {
                    NetgGet = false;
                }
            }
            return true;
        }
        /// <summary>
        /// Programfile(x86) path
        /// </summary>
        /// <returns></returns>
        public static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }
        /// <summary>
        /// 檢查視覺是否執行中
        /// </summary>
        public static void IdsStatus()
        {
            Process[] MyProcess = Process.GetProcessesByName("VisualIdentification");

            if (MyProcess.Length <= 0)
            {
                Process MyIDS = new Process();
                MyIDS.StartInfo.FileName = Folder.Check_path(@"VisualIdentification\") + "VisualIdentification.exe";
                MyIDS.StartInfo.Arguments = $"{Parameters.ip} 8787";
                MyIDS.Start();
                Thread.Sleep(8000);
                MyProcess = Process.GetProcessesByName("VisualIdentification");
                if (MyProcess.Length > 0)
                {
                    int hwnd = MyProcess[0].MainWindowHandle.ToInt32();
                    // Minimize ids tool to deskbar
                    ShowWindow(hwnd, (int)CommandShow.SW_MINIMIZE);
                }
            }
            else
            {
                Console.WriteLine("Vision tool is ready.");
            }
        }
        /// <summary>
        /// 偵測程式執行狀態
        /// </summary>
        /// <param name="ProcessName">程式名稱</param>
        /// <returns></returns>
        public static bool CheckExe(string ProcessName)
        {
            try
            {
                Process[] ps = Process.GetProcesses();
                foreach (Process p in ps)
                {
                    if (p.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// ID偵測程式是否存在
        /// </summary>
        /// <param name="ProcessId"></param>
        /// <returns></returns>
        public static bool CheckExeById(int ProcessId)
        {
            try
            {
                Process ps = Process.GetProcessById(ProcessId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 新增排程
        /// </summary>
        /// <param name="FilePath">開啟執行檔路徑</param>
        /// <param name="Args">啟動參數</param>
        /// <param name="TaskName">排程名稱</param>
        public static void AddTaskToScheduler(string FilePath, string Args, string TaskName)
        {
            // Get the service on the remote machine
            using (TaskService ts = new TaskService())
            {
                // Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Do something";

                // Only used when the user is logged in
                td.Principal.LogonType = TaskLogonType.InteractiveToken;
                // Execute with highest authority
                td.Principal.RunLevel = TaskRunLevel.Highest;

                td.Settings.DisallowStartIfOnBatteries = false;
                td.Settings.StopIfGoingOnBatteries = false;

                // Create a trigger that will fire the task at this time every other day
                //td.Triggers.Add(new DailyTrigger { DaysInterval = 2 });
                td.Triggers.Add(new LogonTrigger { UserId = null, Enabled = true });

                td.Actions.Add(new ExecAction(FilePath, Args, null));

                // Register the task in the root folder
                ts.RootFolder.RegisterTaskDefinition(TaskName, td);
            }
        }
        /// <summary>
        /// 刪除排程
        /// </summary>
        /// <param name="TaskName"></param>
        public static void DeleteTaskFromScheduler(string TaskName)
        {
            using (TaskService ts = new TaskService())
            {
                ts.RootFolder.DeleteTask(TaskName, false);
            }
        }
        /// <summary>
        /// 取得工作排程器中是否有指定的工作
        /// </summary>
        /// <param name="TaskName"></param>
        /// <returns></returns>
        public static bool GetTaskFromScheduler(string TaskName)
        {
            bool TaskStatus = false;
            using (TaskService ts = new TaskService())
            {
                var task = ts.GetTask(TaskName);
                if (task != null) TaskStatus = true;
            }
            return TaskStatus;
        }
        /// <summary>
        /// Filestream讀取照片
        /// </summary>
        /// <param name="path">照片路徑</param>
        /// <returns></returns>
        public static Image OperateImage(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            int lenth = Convert.ToInt32(br.BaseStream.Length);

            byte[] bytes = br.ReadBytes(lenth);
            using (FileStream fs1 = new FileStream($@"{Application.StartupPath}\PicStream.bin", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fs1);
                for (int i = 0; i < bytes.Length; i++)
                {
                    bw.Write(bytes[i]);
                }
                bw.Close();
            }
            br.Close();
            fs.Close();
            MemoryStream ms = new MemoryStream(bytes);

            return Image.FromStream(ms);
        }
        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="path">當前檔的路徑</param>
        /// <returns>是否刪除成功</returns>
        public static bool FileDelete(string path)
        {
            bool ret = false;
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                ret = true;
            }
            return ret;
        }
        /// <summary>
        /// powerDVD 位置大小
        /// </summary>
        public static void ExePosition()
        {
            Dictionary<string, object> ResultArray = new Dictionary<string, object>() { { Parameters.Remark, "" } };
            ExePosition(ref ResultArray);
        }
        public static void ExePosition(ref Dictionary<string, object> ResultArray)
        {
            Application.DoEvents();
            Parameters.Windows.RECT rECT = new Parameters.Windows.RECT();
            do
            {
                IntPtr hwnd = Parameters.Windows.FindWindow(Constants.vbNullString, "powerDVD");
                if ((int)hwnd > 0)
                {
                    // Setting exe position and size
                    Parameters.Windows.SetWindowPos(hwnd, (IntPtr)0, (Screen.PrimaryScreen.WorkingArea.Width / 2) - 640, (Screen.PrimaryScreen.WorkingArea.Height / 2) - 360, 1280, 720, 4);
                    // 內坎在自己的程式內
                    //SetParent((IntPtr)hw, this.Handle); 
                    Thread.Sleep(200);
                    Parameters.Windows.GetWindowRect(hwnd, ref rECT);
                }
                else
                {
                    ResultArray[Parameters.Remark] = "PowerDVD not found.";
                    break;
                }
            } while (rECT.Left != (Screen.PrimaryScreen.WorkingArea.Width / 2) - 640 || rECT.Top != (Screen.PrimaryScreen.WorkingArea.Height / 2) - 360);
        }
        #region ShowWindow
        [DllImport("User32")]
        public static extern int ShowWindow(int hwnd, int nCmdShow);
        public enum CommandShow : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }
        #endregion
        #region Keyboard
        public class KeyBoard
        {
            public const byte vKeyLButton = 0x1;    // 鼠標左鍵
            public const byte vKeyRButton = 0x2;    // 鼠標右鍵
            public const byte vKeyCancel = 0x3;     // CANCEL 鍵
            public const byte vKeyMButton = 0x4;    // 鼠標中鍵
            public const byte vKeyBack = 0x8;       // BACKSPACE 鍵
            public const byte vKeyTab = 0x9;        // TAB 鍵
            public const byte vKeyClear = 0xC;      // CLEAR 鍵
            public const byte vKeyReturn = 0xD;     // ENTER 鍵
            public const byte vKeyShift = 0x10;     // SHIFT 鍵
            public const byte vKeyControl = 0x11;   // CTRL 鍵
            public const byte vKeyAlt = 18;         // Alt 鍵  (鍵碼18)
            public const byte vKeyMenu = 0x12;      // MENU 鍵
            public const byte vKeyPause = 0x13;     // PAUSE 鍵
            public const byte vKeyCapital = 0x14;   // CAPS LOCK 鍵
            public const byte vKeyEscape = 0x1B;    // ESC 鍵
            public const byte vKeySpace = 0x20;     // SPACEBAR 鍵
            public const byte vKeyPageUp = 0x21;    // PAGE UP 鍵
            public const byte vKeyEnd = 0x23;       // End 鍵
            public const byte vKeyHome = 0x24;      // HOME 鍵
            public const byte vKeyLeft = 0x25;      // LEFT ARROW 鍵
            public const byte vKeyUp = 0x26;        // UP ARROW 鍵
            public const byte vKeyRight = 0x27;     // RIGHT ARROW 鍵
            public const byte vKeyDown = 0x28;      // DOWN ARROW 鍵
            public const byte vKeySelect = 0x29;    // Select 鍵
            public const byte vKeyPrint = 0x2A;     // PRINT SCREEN 鍵
            public const byte vKeyExecute = 0x2B;   // EXECUTE 鍵
            public const byte vKeySnapshot = 0x2C;  // SNAPSHOT 鍵
            public const byte vKeyDelete = 0x2E;    // Delete 鍵
            public const byte vKeyHelp = 0x2F;      // HELP 鍵
            public const byte vKeyNumlock = 0x90;   // NUM LOCK 鍵

            //常用鍵 字母鍵A到Z
            public const byte vKeyA = 65;
            public const byte vKeyB = 66;
            public const byte vKeyC = 67;
            public const byte vKeyD = 68;
            public const byte vKeyE = 69;
            public const byte vKeyF = 70;
            public const byte vKeyG = 71;
            public const byte vKeyH = 72;
            public const byte vKeyI = 73;
            public const byte vKeyJ = 74;
            public const byte vKeyK = 75;
            public const byte vKeyL = 76;
            public const byte vKeyM = 77;
            public const byte vKeyN = 78;
            public const byte vKeyO = 79;
            public const byte vKeyP = 80;
            public const byte vKeyQ = 81;
            public const byte vKeyR = 82;
            public const byte vKeyS = 83;
            public const byte vKeyT = 84;
            public const byte vKeyU = 85;
            public const byte vKeyV = 86;
            public const byte vKeyW = 87;
            public const byte vKeyX = 88;
            public const byte vKeyY = 89;
            public const byte vKeyZ = 90;

            //數字鍵盤0到9
            public const byte vKey0 = 48;    // 0 鍵
            public const byte vKey1 = 49;    // 1 鍵
            public const byte vKey2 = 50;    // 2 鍵
            public const byte vKey3 = 51;    // 3 鍵
            public const byte vKey4 = 52;    // 4 鍵
            public const byte vKey5 = 53;    // 5 鍵
            public const byte vKey6 = 54;    // 6 鍵
            public const byte vKey7 = 55;    // 7 鍵
            public const byte vKey8 = 56;    // 8 鍵
            public const byte vKey9 = 57;    // 9 鍵


            public const byte vKeyNumpad0 = 0x60;    //0 鍵
            public const byte vKeyNumpad1 = 0x61;    //1 鍵
            public const byte vKeyNumpad2 = 0x62;    //2 鍵
            public const byte vKeyNumpad3 = 0x63;    //3 鍵
            public const byte vKeyNumpad4 = 0x64;    //4 鍵
            public const byte vKeyNumpad5 = 0x65;    //5 鍵
            public const byte vKeyNumpad6 = 0x66;    //6 鍵
            public const byte vKeyNumpad7 = 0x67;    //7 鍵
            public const byte vKeyNumpad8 = 0x68;    //8 鍵
            public const byte vKeyNumpad9 = 0x69;    //9 鍵
            public const byte vKeyMultiply = 0x6A;   // MULTIPLICATIONSIGN(*)鍵
            public const byte vKeyAdd = 0x6B;        // PLUS SIGN(+) 鍵
            public const byte vKeySeparator = 0x6C;  // ENTER 鍵
            public const byte vKeySubtract = 0x6D;   // MINUS SIGN(-) 鍵
            public const byte vKeyDecimal = 0x6E;    // DECIMAL POINT(.) 鍵
            public const byte vKeyDivide = 0x6F;     // DIVISION SIGN(/) 鍵


            //F1到F12按鍵
            public const byte vKeyF1 = 0x70;   //F1 鍵
            public const byte vKeyF2 = 0x71;   //F2 鍵
            public const byte vKeyF3 = 0x72;   //F3 鍵
            public const byte vKeyF4 = 0x73;   //F4 鍵
            public const byte vKeyF5 = 0x74;   //F5 鍵
            public const byte vKeyF6 = 0x75;   //F6 鍵
            public const byte vKeyF7 = 0x76;   //F7 鍵
            public const byte vKeyF8 = 0x77;   //F8 鍵
            public const byte vKeyF9 = 0x78;   //F9 鍵
            public const byte vKeyF10 = 0x79;  //F10 鍵
            public const byte vKeyF11 = 0x7A;  //F11 鍵
            public const byte vKeyF12 = 0x7B;  //F12 鍵

            [DllImport("user32.dll")]
            public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

            public static void keyPress(byte keyName)
            {
                KeyBoard.keybd_event(keyName, 0, 0, 0);
                KeyBoard.keybd_event(keyName, 0, 2, 0);
            }
        }
        #endregion
    }

}
