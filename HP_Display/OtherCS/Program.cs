using HP_Display.OtherCS;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HP_Display.FuncClass;
using static HP_Display.OtherCS.ServerBehavior;

namespace HP_Display
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // read host name
            String strHostName = Dns.GetHostName();
            //string ip = "";

            if (DnsTest())
            {
                // get IpHostEntry class object
                IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);
                foreach (IPAddress ipaddress in iphostentry.AddressList)
                {
                    Parameters.ip = ipaddress.ToString();
                }
            }
            // Socket start listening
            if (!string.IsNullOrEmpty(Parameters.ip))
            {
                // build ip
                IPEndPoint ipE = new IPEndPoint(IPAddress.Parse(Parameters.ip), Parameters.Socket_port);
                // set socket type
                Parameters.ServerSocket.SListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Parameters.ServerSocket.SListen.Bind(ipE);
                Parameters.ServerSocket.SListen.Listen(100);
                SAccept(Parameters.ServerSocket.SListen);
            }

            // use command line call this tool
            if (args.Length > 0)
            {
                Parameters.CommandLine.configStr = args[0];
                Parameters.CommandLine.scrPathStr = args[1];
            }

            Application.Run(new Main());
        }
        public static void SReceive(Socket socket)
        {
            Task.Factory.StartNew(() =>
            {
                // 接收資料
                string Text = "";
                byte[] clientData = new byte[1024 * 50000];

                do
                {
                    int receivedBytesLen = socket.Receive(clientData);
                    Console.WriteLine($"receivedBytesLen:{receivedBytesLen}");
                    Text += Encoding.UTF8.GetString(clientData, 0, receivedBytesLen);
                } while (socket.Available != 0); // 如果還沒接收完則繼續接收

                Console.WriteLine("--------------------START--------------------");
                // 最後送出的XML字串
                string MessageResult = "";
                // 收到的XML資料
                string MessageText = "";
                // 客戶端的網路節點
                string RemoteEndPoint = Parameters.ServerSocket.SClient.RemoteEndPoint.ToString();
                MessageText = Text;

                if (Text != "")
                    Console.WriteLine($"當前客戶端節點 : {RemoteEndPoint}");
                // 要傳送的資料
                Dictionary<string, object> ResultArray = new Dictionary<string, object>
                {
                    { "Status", "" },
                    { "Behavior", "" },
                    { "Remark", "" },
                    { "Necessary", "" },
                };

                try
                {
                    Dictionary<string, object> UIdata = new Dictionary<string, object>();
                    Boolean GetStr = true;
                    try
                    {
                        Console.WriteLine("Server receive message text: " + MessageText);
                        UIdata = dll_PublicFuntion.Other.XmlToDictionary(MessageText);
                    }
                    catch
                    {
                        GetStr = false;
                    }

                    if (GetStr)
                    {
                        // 參數
                        Dictionary<string, object> ParameterData = new Dictionary<string, object>();
                        // 標準答案字典
                        Dictionary<string, object> AnswerDic = new Dictionary<string, object>();
                        // 參數Parameter有可能是空的
                        try { ParameterData = dll_PublicFuntion.Other.XmlToDictionary((string)UIdata["Parameter"]); } catch { }
                        if (UIdata.ContainsKey("Answer"))
                        {
                            try
                            {
                                AnswerDic = dll_PublicFuntion.Other.XmlToDictionary((string)UIdata["Answer"]);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        string group = "";
                        string behavior = "";

                        if (UIdata.ContainsKey("Group"))
                            group = $"{UIdata["Group"]}";

                        if (UIdata.ContainsKey("Behavior"))
                        {
                            behavior = $"{UIdata["Behavior"]}";
                            ResultArray["Behavior"] = behavior;
                        }
                        if (UIdata.ContainsKey("Necessary"))
                            ResultArray["Necessary"] = $"{UIdata["Necessary"]}";

                        /*power magement*/
                        if (behavior == Behavior.PM_Sleep)
                        {
                            PM_Sleep(ref ResultArray, ref MessageResult);
                        }
                        else if (behavior == Behavior.PM_Sleep_S3)
                        {
                            PM_Sleep_S3(ref ResultArray, ref MessageResult);
                        }
                        else if (behavior == Behavior.PM_Hibernation)
                        {
                            PM_Hibernation(ref ResultArray, ref MessageResult);
                        }
                        else if (behavior == Behavior.PM_Warm)
                        {
                            PM_Warm(ref ResultArray, ref MessageResult);
                        }
                        else if (behavior == Behavior.PM_Cold)
                        {
                            PM_Cold(ref ResultArray, ref MessageResult);
                        }
                        else if (behavior == Behavior.PM_Special_ColdBoot || behavior == Behavior.PM_Special_WarmBoot || behavior == Behavior.PM_Special_Hibernation)
                        {
                            PM_Special(ref ResultArray, ref MessageResult,ref ParameterData);
                        }
                        /*windows check*/
                        else if (behavior == Behavior.PnP_Check)
                        {
                            PnP_Check(ref ResultArray, ref AnswerDic);
                        }
                        else if (behavior == Behavior.Display_Check)
                        {
                            Display_Check(ref ResultArray, ref AnswerDic);
                        }
                        else if (behavior == Behavior.Power_Check)
                        {
                            Power_Check(ref ResultArray);
                        }
                        else if (behavior == Behavior.Resolution_Check)
                        {
                            Resolution_Check(ref ResultArray, ref AnswerDic);
                        }
                        /*Power DVD*/
                        else if (behavior == Behavior.Play_Pause)
                        {
                            Play_Pause(ref ResultArray);
                        }
                        else if (behavior == Behavior.Screen_Check)
                        {
                            IDS_Check(UIdata, ref ResultArray, true);
                        }
                        else if (behavior == Behavior.ErrorMessage)
                        {
                            ErrorMessage(ref ResultArray, ref ParameterData);
                        }
                        else if (behavior == Behavior.HDCP_Check)
                        {
                            HDCP_Check(ref ResultArray, ref ParameterData);
                        }
                        else if (behavior == Behavior.HDCP_CheckPass)
                        {
                            Play_Pause(ref ResultArray);
                        }
                        else if (behavior == Behavior.PowerDVD_On)
                        {
                            ResultArray["Status"] = "Pass";
                            DVD_On(ref ResultArray);
                        }
                        else if (behavior == Behavior.PowerDVD_Off)
                        {
                            PowerDVD_Off(ref ResultArray);
                        }
                        else if (behavior == Behavior.Movie_Play_Check)
                        {
                            Movie_Play_Check(ref ResultArray);
                        }
                        else if (behavior == Behavior.DVD_Restart)
                        {
                            DVD_Restart(ref ResultArray);
                        }
                        else if (behavior == Behavior.Drag_DVD)
                        {
                            Drag_DVD(ref ResultArray, ref ParameterData);
                        }
                        else if (behavior == Behavior.Audio_Check)
                        {
                            Audio_Check(ref ResultArray,ref ParameterData);
                        }
                        else if (behavior == Behavior.Send_Enter)
                        {
                            Send_Enter(ref ResultArray);
                        }
                        else
                        {
                        }
                    }
                }
                catch (Exception eqq)
                {
                    ResultArray["Status"] = "Fail";
                    ResultArray["Remark"] = eqq.Message;
                    Console.WriteLine(eqq.Message);
                }

                // 將字典轉換成XML回傳
                MessageResult = dll_PublicFuntion.Other.DictionaryToXml(ResultArray);
                // 回傳給Client
                SSend(Parameters.ServerSocket.SClient, MessageResult);

                // 重複回傳確保Socket斷開
                Thread.Sleep(8000);
                int OverTime = 500;
                while (SSend(Parameters.ServerSocket.SClient, ""))
                {
                    Thread.Sleep(500);
                    Console.WriteLine("Should bot exist:" + OverTime);
                    OverTime += 500;
                }
            });
        }
        public static bool SSend(Socket socket, string message)
        {
            if (socket == null || message == string.Empty)
            {
                return false;
            }
            byte[] data = Encoding.UTF8.GetBytes(message);
            try
            {
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, Result =>
                {
                    int length = socket.EndSend(Result);
                }, null);
                return true;
            }
            catch (Exception excep)
            {
                Console.WriteLine($"SSend exp: {excep.Message}");
                return false;
            }
        }
        public static void SAccept(Socket socket)
        {
            Parameters.ServerSocket.SListen.BeginAccept(SResult =>
            {
                Parameters.ServerSocket.SClient = socket.EndAccept(SResult);
                // get client ip
                Parameters.ServerSocket.SEndPoint = Parameters.ServerSocket.SClient.RemoteEndPoint.ToString();
                SReceive(Parameters.ServerSocket.SClient);
                // add another client
                SAccept(socket);
            }, null
            );
        }
    }
}

