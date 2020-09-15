using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HP_Display.FuncClass;
using Image = System.Drawing.Image;
using HP_Display.OtherCS;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;
using Application = System.Windows.Forms.Application;
using Castle.Components.DictionaryAdapter;

namespace HP_Display
{

    public partial class Main : Form
    {
        Dictionary<string, int> GroupListLoop = new Dictionary<string, int>();
        /// <summary>
        /// Log object
        /// </summary>
        dll_PublicFuntion.Logger logger = new dll_PublicFuntion.Logger();
        /// <summary>
        /// Socket send twice => first time remark info
        /// </summary>
        string halfSocketRemark = "";
        /// <summary>
        /// Socket send not done yet => 1 => continue sending
        /// </summary>
        int SocketDataCheck = 0;
        /// <summary>
        /// False => stop script running until this one done
        /// </summary>
        Boolean runFlag = true;
        /// <summary>
        /// False => whole script come a conclusion
        /// </summary>
        Boolean RunStatus = false;
        /// <summary>
        /// True => continue executive
        /// </summary>
        Boolean StopStatus = false;
        /// <summary>
        /// true = green light ; false = red light
        /// </summary>
        Boolean NodeStatus = false;
        private void LogAdd(string ServiceName, string ExeText, Boolean Show)
        {
            try
            {
                ExeText = ExeText.Replace(Environment.NewLine, " ");
                dll_PublicFuntion.Logger.LoggerBody LoggerBody = new dll_PublicFuntion.Logger.LoggerBody();
                LoggerBody.LogTime = DateTime.Now;
                LoggerBody.LogType = $"{ServiceName}";
                LoggerBody.LogText = $"{ExeText}";
                logger.AddLog(Show, LoggerBody);
            }
            catch { }
        }
        public Main()
        {
            InitializeComponent();
            // Read comport => add to combobox
            string[] myPorts = SerialPort.GetPortNames();
            try
            {
                if (myPorts[0] != null)
                {
                    comboBox1.Items.AddRange(myPorts);
                    comboBox1.SelectedIndex = 0;
                }
            }
            catch { }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("x:" + System.Windows.Forms.Cursor.Position.X);
            Console.WriteLine("y:" + System.Windows.Forms.Cursor.Position.Y);
            //this.skinEngine1.SkinFile = "MacOs.ssk";
            txt_loop.ImeMode = ImeMode.Off;
            Stopbtn.Enabled = false;
            this.Text = $"Main[{Parameters.ip}]";

            ClientBehavior.main = this;

            if (!Directory.Exists(ToolPath + @"ExcelFile"))
                Directory.CreateDirectory(ToolPath + @"ExcelFile");

            // If tool open by command line => read initial file and do the script
            if (!string.IsNullOrEmpty(Parameters.CommandLine.scrPathStr) && !string.IsNullOrEmpty(Parameters.CommandLine.configStr))
            {
                try
                {
                    Parameters.path = Parameters.CommandLine.scrPathStr;
                    LogAdd("ScriptLoad", $"path: {Parameters.path}", false);
                    ScriptInit(false);

                    string Openpath = Parameters.CommandLine.configStr;
                    StreamReader sr = new StreamReader(Openpath);
                    string[] ParameterList = sr.ReadToEnd().Split('\n');
                    // server ip
                    if (!string.IsNullOrEmpty(ParameterList[0]))
                        ip1.Text = ParameterList[0].Trim();
                    if (!string.IsNullOrEmpty(ParameterList[1]))
                        DUTIDText.Text = ParameterList[1].Trim();
                    if (!string.IsNullOrEmpty(ParameterList[2]))
                        ACIPText.Text = ParameterList[2].Trim();
                    sr.Close();

                    Thread.Sleep(1000);

                    runBtn.PerformClick();
                }
                catch (Exception ee)
                {
                    Console.WriteLine("AA:" + ee.Message);
                }
            }
            // Read config file
            Config_Load();


        }
        private void btn_load_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "(*.xml)|*.xml|(*.json)|*.json";
            openFileDialog1.InitialDirectory = dll_PublicFuntion.Folder.Check_path($@"Script\");
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Parameters.path = openFileDialog1.FileName;
                LogAdd("ScriptLoad", $"path: {Parameters.path}", false);//Console.WriteLine(path);
                ScriptInit(false);
            }
        }
        private void runBtn_Click(object sender, EventArgs e)
        {
            runFlag = true;
            if (string.IsNullOrEmpty(ip1.Text))
            {
                MessageBox.Show("Please fill in the server ip.", "Remind", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // button setting
            runBtn.Enabled = false;
            Stopbtn.Enabled = true;
            Thread.Sleep(100);

            LogBox.Clear();
            LogBox.Text = "";
            RunStatus = true;
            StopStatus = true;

            Timer_Status.Stop();

            // Dictionary to XML ( Send to socket ) => Save as a string
            string SendString = "";
            // Total step
            int TotalStepCount = 1;

            DateTime StartRunTime = DateTime.Now;


            Dictionary<string, object> AnswerDic = new Dictionary<string, object>
                            {
                                { "ACIP",$"{ACIPText.Text}"},
                                { "DUTID",$"{DUTIDText.Text}"},
                            };

            Task.Factory.StartNew(() =>
            {
                #region Excel header
                IWorkbook wkBook = new HSSFWorkbook();
                ISheet sheet = wkBook.CreateSheet("Result");
                IRow rowHeadtext = sheet.CreateRow(0);
                rowHeadtext.CreateCell(0).SetCellValue(Parameters.Group);
                rowHeadtext.CreateCell(1).SetCellValue(Parameters.Step);
                rowHeadtext.CreateCell(2).SetCellValue(Parameters.Behavior);
                rowHeadtext.CreateCell(3).SetCellValue(Parameters.Status);
                rowHeadtext.CreateCell(4).SetCellValue(Parameters.Remark);
                rowHeadtext.CreateCell(5).SetCellValue(Parameters.Time);
                #endregion

                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        try
                        {
                            string Log_Text = "";
                            this.Invoke((MethodInvoker)delegate ()
                            {
                                Log_Text = LogBox.Text;
                            });
                            string Log_Text_Now = logger.SelectLogText(StartRunTime, null, null, null);
                            if (Log_Text_Now != Log_Text)
                            {
                                this.Invoke((MethodInvoker)delegate ()
                                {
                                    LogBox.Text = Log_Text_Now;
                                    LogBox.ScrollBars = ScrollBars.Both;
                                    LogBox.SelectionStart = LogBox.Text.Length;
                                    LogBox.ScrollToCaret();
                                });
                            }
                            if (!RunStatus) break;
                        }
                        catch { }
                    }
                });

                // Group (whole loop)
                for (int j = 0; j < Convert.ToInt32(txt_loop.Text); j++)
                {
                    // whole script loop header line in excel
                    IRow row_ = sheet.CreateRow(TotalStepCount);
                    row_.CreateCell(0).SetCellValue($"---- Loop:{j + 1} ----");
                    TotalStepCount++;

                    this.Invoke((MethodInvoker)delegate ()
                    {
                        txt_nowloop.Text = (j + 1).ToString();
                        ScriptInit(false);
                    });
                    Thread.Sleep(100);
                    foreach (TreeNode Group in treeView1.Nodes)
                    {
                        if (!runFlag) break;
                        // Daq response data record
                        string DaqReceive = "";
                        if (!RunStatus) break;
                        for (int K = 0; K < GroupListLoop[Group.Tag.ToString()]; K++)
                        {
                            // Excel record step
                            int ExcelStepCounter = 1;
                            if (Group.PrevNode != null)
                                this.Invoke((MethodInvoker)delegate ()
                                {
                                    Group.PrevNode.Collapse();
                                });
                            // Step
                            foreach (TreeNode Step in Group.Nodes)
                            {
                                this.Invoke((MethodInvoker)delegate ()
                                {
                                    Step.Collapse();
                                    Step.Expand();
                                });
                                if (StopStatus)
                                {
                                    Step.BackColor = System.Drawing.Color.Yellow;
                                    try
                                    {
                                        // Socket send data
                                        Dictionary<string, object> SendMessage = new Dictionary<string, object>
                                    {
                                        { Parameters.Group,$"{Group.Text}"},
                                        { Parameters.Behavior,$"{Step.Tag}"},
                                        { Parameters.Necessary,"" },
                                        { Parameters.Parameter,""},
                                        { Parameters.ServerIP,Parameters.ip},
                                        { Parameters.Answer,dll_PublicFuntion.Other.DictionaryToXml(AnswerDic) },
                                    };
                                        // Main behavior result
                                        Dictionary<string, object> MainResultArray = new Dictionary<string, object>
                                    {
                                        { Parameters.Status, "Pass" },
                                        { Parameters.Behavior, "" },
                                        { Parameters.Remark, "" },
                                        { Parameters.Necessary, "" },
                                    };

                                        foreach (TreeNode Data in Step.Nodes)
                                        {
                                            if (SendMessage.ContainsKey((string)Data.Tag))
                                            {
                                                SendMessage[(string)Data.Tag] = Data.Text.Replace($"{(string)Data.Tag}:", "");
                                            }
                                            if ((string)Data.Tag == Parameters.Parameter)
                                            {
                                                SendMessage[Parameters.Parameter] = Data.Name;
                                            }
                                        }
                                        dll_PublicFuntion.Other.Wait(2);

                                        MainResultArray[Parameters.Behavior] = Step.Tag.ToString();
                                        MainResultArray[Parameters.Necessary] = SendMessage[Parameters.Necessary].ToString();

                                        // If IsSocket is null => not record excel in connect function
                                        string IsSocket = "";

                                        #region DAQ Behavior
                                        if (Step.Tag.ToString() == Behavior.Monitor_OnOff)
                                        {
                                            ClientBehavior.Monitor_OnOff(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Hot_Plug)
                                        {
                                            ClientBehavior.Hot_Plug(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Hot_Unplug)
                                        {
                                            ClientBehavior.Hot_Unplug(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Hot_Plug_Reverse)
                                        {
                                            ClientBehavior.Hot_Plug_Reverse(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Hot_Plug_UnReverse)
                                        {
                                            ClientBehavior.Hot_Plug_UnReverse(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Press_PW_Button)
                                        {
                                            ClientBehavior.Press_PW_Button(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Mouse_Click)
                                        {
                                            ClientBehavior.Mouse_Click(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Monitor_OSD)
                                        {
                                            ClientBehavior.Monitor_OSD(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Monitor_OSD_1)
                                        {
                                            ClientBehavior.Monitor_OSD_1(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Monitor_OSD_2)
                                        {
                                            ClientBehavior.Monitor_OSD_2(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Monitor_OSD_3)
                                        {
                                            ClientBehavior.Monitor_OSD_3(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Monitor_OSD_4)
                                        {
                                            ClientBehavior.Monitor_OSD_4(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Monitor_OSD_5)
                                        {
                                            ClientBehavior.Monitor_OSD_5(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Check_PD_Plug)
                                        {
                                            ClientBehavior.Check_PD_Plug(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Check_PD_Unplug)
                                        {
                                            ClientBehavior.Check_PD_Unplug(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Check_PD_Original)
                                        {
                                            ClientBehavior.Check_PD_Original(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Check_PD_Reserve)
                                        {
                                            ClientBehavior.Check_PD_Reserve(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Check_DC_Push)
                                        {
                                            ClientBehavior.Check_DC_Push(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Check_DC_Back)
                                        {
                                            ClientBehavior.Check_DC_Back(ref MainResultArray, ref DaqReceive);

                                        }
                                        else if (Step.Tag.ToString() == Behavior.Check_PB_Press)
                                        {
                                            ClientBehavior.Check_PB_Press(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Check_PB_Release)
                                        {
                                            ClientBehavior.Check_PB_Release(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Check_Mouse_Click)
                                        {
                                            ClientBehavior.Check_Mouse_Click(ref MainResultArray, ref DaqReceive);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Check_Mouse_Release)
                                        {
                                            ClientBehavior.Check_Mouse_Release(ref MainResultArray, ref DaqReceive);
                                        }
                                        #endregion
                                        else if (Step.Tag.ToString() == Behavior.AC_OnOff)
                                        {
                                            ClientBehavior.AC_OnOff(ref SendMessage, ref MainResultArray, AnswerDic);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.VA_Check)
                                        {
                                            ClientBehavior.VA_Check(ref SendMessage, ref MainResultArray);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.WaitTime)
                                        {
                                            ClientBehavior.WaitTime(ref SendMessage);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Resolution_Check)
                                        {
                                            ClientBehavior.Resolution_Check(ref SendMessage, ref MainResultArray, ref SendString);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.LED_Check)
                                        {
                                            ClientBehavior.LED_Check(ref SendMessage, ref MainResultArray, ref SendString);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Movie_Play_Check)
                                        {
                                            ClientBehavior.Movie_Play_Check(ref SendMessage, ref MainResultArray, ref SendString, Group.Text, ref ExcelStepCounter, ref TotalStepCount, ref sheet, ref SocketDataCheck, ref halfSocketRemark, ref AnswerDic);
                                        }
                                        else if (Step.Tag.ToString() == Behavior.Opening_Check)
                                        {
                                            ClientBehavior.Opening_Check(ref SendMessage,ref MainResultArray,ref SendString, Group.Text, ref ExcelStepCounter, ref TotalStepCount, ref sheet, ref SocketDataCheck, ref halfSocketRemark);
                                        }
                                        else
                                        {
                                            // Trans send data from dictionary into xml
                                            SendString = dll_PublicFuntion.Other.DictionaryToXml(SendMessage);
                                            // Add to this if function needs Ids
                                            if (Step.Tag.ToString() == Behavior.Screen_Check || Step.Tag.ToString() == Behavior.HDCP_Check)
                                            {
                                                // Check Ids status
                                                IdsStatus();
                                                SendMessage.Add("SavePath", dll_PublicFuntion.Folder.ToolPath + "IDSNowImage.png");
                                                try { SendString = dll_PublicFuntion.Other.DictionaryToXml(SendMessage); }
                                                catch { Console.WriteLine("SendString trans fail"); }
                                            }

                                            // Write log
                                            //LogAdd("ScriptRun", $"SendString: {dll_PublicFuntion.Other.DictionaryToString(SendMessage)}", false);

                                            // Fail => false ; Pass => true
                                            bool socketResult = false;
                                            do
                                            {
                                                // Send to server
                                                ConnectNet(SendString, TotalStepCount, sheet, Group.Text, ExcelStepCounter, ref socketResult, Step.Tag.ToString());
                                                dll_PublicFuntion.Other.Wait(1.5);
                                            } while (SocketDataCheck == 1);

                                            IsSocket = "Record excel in connect function";

                                            if (Step.Tag.ToString() == Behavior.Screen_Check || Step.Tag.ToString() == Behavior.HDCP_Check)
                                            {
                                                // halfSocketRemark == Receive remark
                                                if (halfSocketRemark == "OnCancel")
                                                {
                                                    DAQ("14", "HIGH", ref DaqReceive, "DO");
                                                    if (DaqReceive.Trim() == "Success")
                                                    {
                                                        Thread.Sleep(300);
                                                        DAQ("14", "LOW", ref DaqReceive, "DO");
                                                    }
                                                    Thread.Sleep(50);
                                                }

                                                if (Step.Tag.ToString() == Behavior.HDCP_Check)
                                                {
                                                    IDS_Check(SendMessage, ref MainResultArray, true);
                                                    if (!File.Exists((string)SendMessage["SavePath"]))
                                                    {
                                                        socketResult = false;
                                                        MainResultArray[Parameters.Status] = "Fail";
                                                        MainResultArray[Parameters.Remark] = "IDS connect error.";
                                                    }
                                                    if ((string)MainResultArray[Parameters.Status] == "Pass")
                                                    {
                                                        socketResult = true;
                                                    }
                                                }

                                                if (Step.Tag.ToString() == Behavior.Screen_Check)
                                                {
                                                    if (!File.Exists((string)SendMessage["SavePath"]))
                                                    {
                                                        socketResult = false;
                                                        MainResultArray[Parameters.Status] = "Fail";
                                                        MainResultArray[Parameters.Remark] = "IDS connect error.";
                                                    }
                                                }

                                                if (socketResult == true)
                                                {

                                                    Dictionary<string, object> FDic = new Dictionary<string, object>();
                                                    try { FDic = dll_PublicFuntion.Other.XmlToDictionary(SendMessage["Parameter"].ToString()); } catch{ }

                                                    if (File.Exists((string)SendMessage["SavePath"]) && socketResult == true)
                                                    {
                                                        Image FullScreen = OperateImage((string)SendMessage["SavePath"]);
                                                        Image SmallPic = OperateImage((string)FDic["PicturePath"]);
                                                        dll_opencv.OpenCV openCV = new dll_opencv.OpenCV();
                                                        openCV.SourceLoadImage(FullScreen);
                                                        openCV.SourceToGray();
                                                        openCV.MatchLoadImage(SmallPic);
                                                        openCV.MatchToGray();
                                                        dll_opencv.OpenCV.MatchDataList matchDataList = openCV.GetMatchPos(Parameters.Confidential, false);

                                                        double resultConfident = matchDataList.MaxConfidencevalue;

                                                        if (resultConfident < Parameters.Confidential)
                                                        {
                                                            if (Step.Tag.ToString() == "HDCP_Check")
                                                            {
                                                                MainResultArray[Parameters.Status] = "Pass";
                                                                MainResultArray[Parameters.Remark] = "No match.";
                                                            }
                                                            else
                                                            {
                                                                MainResultArray[Parameters.Status] = "Fail";
                                                                MainResultArray[Parameters.Remark] = "No match.";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            MainResultArray[Parameters.Status] = "Pass";

                                                            if (Step.Tag.ToString() == Behavior.HDCP_Check)
                                                            {

                                                                Dictionary<string, object> SendMessageHDCP = new Dictionary<string, object> //Socket傳送資料
                                                            {
                                                                { Parameters.Group,$"{Group.Text}"},
                                                                { Parameters.Behavior,$"{Behavior.HDCP_Check}Pass"},
                                                                { Parameters.Necessary,"" },
                                                                { Parameters.Parameter,""},
                                                                { Parameters.ServerIP,Parameters.ip},
                                                                { Parameters.Answer,dll_PublicFuntion.Other.DictionaryToXml(AnswerDic) },
                                                            };

                                                                do
                                                                {
                                                                    ConnectNet(dll_PublicFuntion.Other.DictionaryToXml(SendMessageHDCP), TotalStepCount, sheet, Group.Text, ExcelStepCounter, ref socketResult, $"{Step.Tag}Pass");
                                                                    // send to server
                                                                    dll_PublicFuntion.Other.Wait(1.5);
                                                                } while (SocketDataCheck == 1);

                                                                MainResultArray[Parameters.Remark] = "DVD is playing.";
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    MainResultArray[Parameters.Status] = "Fail";
                                                    MainResultArray[Parameters.Remark] = "IDS connect error.";
                                                }

                                                IsSocket = "";
                                            }
                                        }

                                        this.Invoke((MethodInvoker)delegate ()
                                        {
                                            if (IsSocket == "")
                                            {
                                                LogBox.Text += MainResultArray["Status"] + Environment.NewLine;
                                                LogAdd("ConnectNet", $"Status: {MainResultArray[Parameters.Status]}", false);
                                                LogBox.Text += MainResultArray["Behavior"] + Environment.NewLine;
                                                LogAdd("ConnectNet", $"Behavior: {MainResultArray[Parameters.Behavior]}", false);
                                                LogBox.Text += MainResultArray["Necessary"] + Environment.NewLine;
                                                LogAdd("ConnectNet", $"Necessary: {MainResultArray[Parameters.Necessary]}", false);
                                                LogBox.Text += MainResultArray["Remark"] + Environment.NewLine;
                                                LogAdd("ConnectNet", $"Remark: {MainResultArray[Parameters.Remark]}", false);
                                                IRow row = sheet.CreateRow(TotalStepCount);
                                                row.CreateCell(0).SetCellValue(Group.Text);
                                                row.CreateCell(1).SetCellType(CellType.Numeric);
                                                row.GetCell(1).SetCellValue(ExcelStepCounter);
                                                row.CreateCell(2).SetCellValue((string)MainResultArray[Parameters.Behavior]);
                                                row.CreateCell(3).SetCellValue((string)MainResultArray[Parameters.Status]);
                                                row.CreateCell(4).SetCellValue((string)MainResultArray[Parameters.Remark]);
                                                row.CreateCell(5).SetCellValue(DateTime.Now.ToLongTimeString());
                                                for (int i = 0; i < row.Count(); i++)
                                                {
                                                    sheet.AutoSizeColumn(i);
                                                }

                                                if ((string)MainResultArray[Parameters.Status] == "Fail")
                                                {
                                                    NodeStatus = false;
                                                    if ((string)MainResultArray[Parameters.Necessary] == "Yes")
                                                        StopStatus = false;
                                                }
                                                else
                                                    NodeStatus = true;
                                            }
                                        });
                                        if (NodeStatus == false)
                                            Step.BackColor = System.Drawing.Color.Red;
                                        else if (NodeStatus == true && SocketDataCheck == 1)
                                        {
                                            Step.BackColor = System.Drawing.Color.Brown;
                                            SocketDataCheck = 0;
                                        }
                                        else
                                            Step.BackColor = System.Drawing.Color.Green;
                                    }
                                    catch (Exception exp) { Console.WriteLine(exp.StackTrace); }
                                    ExcelStepCounter++;
                                }
                                TotalStepCount++;
                                if (!runFlag)
                                {
                                    break;
                                }
                                halfSocketRemark = "";
                            }
                        }
                    }
                    
                }/* for group end */

                RunStatus = false;
                // Log
                try
                {
                    string Log_Text = "";
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        Log_Text = LogBox.Text;
                    });
                    string Log_Text_Now = logger.SelectLogText(StartRunTime, null, null, null);
                    if (Log_Text_Now != Log_Text)
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            LogBox.Text = Log_Text_Now;
                            LogBox.ScrollBars = ScrollBars.Both;
                            LogBox.SelectionStart = LogBox.Text.Length;
                            LogBox.ScrollToCaret();
                        });
                    }
                }
                catch { }

                string excelName = "";
                this.Invoke((MethodInvoker)delegate
                {
                    string fileName = DateTime.Now.ToString("HH-mm-ss");
                    string dirName = DateTime.Now.ToString("yyyy-MM-dd");
                    excelName = dll_PublicFuntion.Folder.Check_path($@"ExcelFile\{dirName}\") + fileName + ".xls";
                    FileStream fsWrite = new FileStream(dll_PublicFuntion.Folder.Check_path($@"ExcelFile\{dirName}\") + fileName + ".xls", FileMode.Create);
                    wkBook.Write(fsWrite);
                    // Close file stream
                    fsWrite.Close();
                    // Close work book
                    wkBook.Close();
                    // Dispose file stream
                    fsWrite.Dispose();
                    Timer_Status.Start();
                });
                this.Invoke((MethodInvoker)delegate ()
                {
                    runBtn.Enabled = true;
                    Stopbtn.Enabled = false;
                });


                if (!string.IsNullOrEmpty(Parameters.CommandLine.configStr) && !string.IsNullOrEmpty(Parameters.CommandLine.scrPathStr))
                {
                    Environment.Exit(Environment.ExitCode);
                }
            });
        }
        private void Excelbtn_Click(object sender, EventArgs e)
        {
            Process excelOpener = new Process();
            excelOpener.StartInfo.FileName = dll_PublicFuntion.Folder.Check_path("ExcelFile");
            excelOpener.Start();
        }
        private void Stopbtn_Click(object sender, EventArgs e)
        {
            runFlag = false;
            runBtn.Enabled = true;
        }
        private void DAQbtn_Click(object sender, EventArgs e)
        {
            Process daqProcess = new Process();
            daqProcess.StartInfo.FileName = "USB5858_Server.exe";
            daqProcess.Start();
        }
        private void editBtn_Click(object sender, EventArgs e)
        {
            ScriptInit(true);
        }
        private void PaSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FileText.Text))
            {
                string str = System.IO.Directory.GetCurrentDirectory();
                if (!Directory.Exists(str + @"\ParameterData"))
                {
                    Directory.CreateDirectory(str + @"\ParameterData");
                }
                List<string> ParameterList = new List<string>();
                ParameterList.Add(ip1.Text.Trim());
                ParameterList.Add(DUTIDText.Text.Trim());
                ParameterList.Add(ACIPText.Text.Trim());
                using (StreamWriter sw = new StreamWriter(str + @"\ParameterData" + $@"\{FileText.Text}.txt"))
                {
                    foreach (var item in ParameterList)
                    {
                        sw.WriteLine(item);
                    }
                    sw.Close();
                }
                MessageBox.Show("Save success.");
            }
            else
            {
                MessageBox.Show("Please fill in the file name.");
            }
        }
        private void PaLoad_Click(object sender, EventArgs e)
        {
            TxtOPen.Filter = "txt files (*.txt)|*.txt";
            TxtOPen.InitialDirectory = dll_PublicFuntion.Folder.Check_path($@"ParameterData\");
            if (TxtOPen.ShowDialog() == DialogResult.OK)
            {
                string Openpath = TxtOPen.FileName;
                StreamReader sr = new StreamReader(Openpath);
                string[] ParameterList = sr.ReadToEnd().Split('\n');
                if (!string.IsNullOrEmpty(ParameterList[0]))//server ip
                    ip1.Text = ParameterList[0].Trim();
                if (!string.IsNullOrEmpty(ParameterList[1]))
                    DUTIDText.Text = ParameterList[1].Trim();
                if (!string.IsNullOrEmpty(ParameterList[2]))
                    ACIPText.Text = ParameterList[2].Trim();

                sr.Close();
            }
        }
        private void IDSbtn_Click(object sender, EventArgs e)
        {
            /*
            IdsCb.Items.Clear();
            ListCamerasData = new Dictionary<int, string>();

            uEye.Types.CameraInformation[] cameraList;
            uEye.Info.Camera.GetCameraList(out cameraList);
            int _DeviceIndex = 0;
            foreach (uEye.Types.CameraInformation _Camera in cameraList)
            {
                ListCamerasData.Add(_Camera.DeviceID, _Camera.Model);
                IdsCb.Items.Add(new ComboboxItem(_Camera.DeviceID.ToString(), _Camera.Model));
                _DeviceIndex++;
            }

            this.Cursor = Cursors.WaitCursor;
            IdsCb.SelectedIndex = 0;
            NowCamera = IdsCb.Text;
            if (CaptureIDS == null)
            {
                CaptureIDS = new dll_IDS.IDS(0);
                CaptureIDS.LoadiniFile();
            }
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
            CaptureIDS.StartLive(pictureBox1.Handle);

            Thread.Sleep(500);

            this.Cursor = Cursors.Default;
            */
            dll_PublicFuntion.Cmd.StartExe(dll_PublicFuntion.Folder.Check_path(@"VisualIdentification\") + "VisualIdentification.exe", $"{Parameters.ip} 8787");
        }
        private void RLoadBtn_Click(object sender, EventArgs e)
        {
            ScriptInit(false);
        }
        private void scriptBindingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Script_binding script_Binding = new Script_binding();
            script_Binding.Show();
        }
        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Config_Form.config_form_exist)
            {
                Config_Form config_Form = new Config_Form();
                config_Form.Show();
            }
        }
        private void scriptEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Script script = new Script();
            script.Show();
        }
        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
        private void Timer_Status_Tick(object sender, EventArgs e)
        {
            string Log_Text = "";
            Log_Text = TBoxAllLog.Text;
            string Log_Text_Now = logger.SelectLogText(null, null, null, null);
            if (Log_Text_Now != Log_Text)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    TBoxAllLog.Text = Log_Text_Now;
                    TBoxAllLog.ScrollBars = ScrollBars.Both;
                    TBoxAllLog.SelectionStart = TBoxAllLog.Text.Length;
                    TBoxAllLog.ScrollToCaret();
                });
            }
        }

        /// <summary>
        /// Script load initial
        /// </summary>
        /// <param name="Show">false => clear</param>
        private void ScriptInit(Boolean Show)
        {
            if (!Show)
            {
                treeView1.Nodes.Clear();
                treeView1.ShowLines = false;
            }
            if (File.Exists(Parameters.path))
            {
                Script script = new Script();
                script.dataGridView1.Rows.Clear();
                dll_PublicFuntion.DGV.XmlLoadDGV(Parameters.path, script.dataGridView1, "Script");  //讀取XML到DATAVIEW
                List<string> vs = new List<string> { };
                foreach (DataGridViewRow v in script.dataGridView1.Rows)
                {
                    string Behavior = $"{v.Cells["C4"].Value}"; //C4 = 真實的行為名稱
                    string BehaviorText = "";
                    List<Dictionary<string, string>> Loops = new List<Dictionary<string, string>> {
                        Script.IOcbDic,
                        Script.IOcontrolDic,
                        Script.PWMDic,
                        Script.LEDDic,
                        Script.ACDic,
                        Script.VoltageDic,
                        Script.WinDic,
                        Script.DvdDic,
                        Script.WaitDic,
                    };
                    foreach (Dictionary<string, string> Loop in Loops)
                    {
                        if (BehaviorText != "") break;
                        if (Loop.ContainsKey(Behavior))
                        {
                            BehaviorText = Loop[Behavior];
                        }
                    }
                    v.Cells["C2"].Value = BehaviorText;
                    // 新增group到groupCb
                    string groupList = $"{v.Cells["C1"].Value}";
                    if (!vs.Contains(groupList))
                    {
                        script.groupCb.Items.Add(groupList);
                        //LogAdd("ScriptInit", $"groupList {groupList}", false);//Console.WriteLine($"groupList {groupList}");
                        vs.Add(groupList);
                    }
                }
                if (Show)
                {
                    script.JsonName.Text = (Path.GetFileName($@"{Parameters.path}")).Replace(".xml", ""); //檔名
                    script.ShowDialog();
                    string FilePath = dll_PublicFuntion.Folder.Check_path(@"Script\") + script.JsonName.Text + ".xml";
                    if (File.Exists(FilePath))
                    {
                        Parameters.path = FilePath;
                        ScriptInit(false);
                    }
                }
                else
                {
                    foreach (string Group in vs)
                    {
                        TreeNode GroupNode = new TreeNode();
                        GroupNode.Text = $"{Group}";
                        GroupNode.Tag = $"{Group}";

                        var obj = (from DataGridViewRow drv in script.dataGridView1.Rows
                                   where drv.Cells["C1"].Value != null
                                   && $"{drv.Cells["C1"].Value}" == $"{Group}"
                                   select drv);
                        foreach (DataGridViewRow v in obj)
                        {
                            TreeNode StepNode = new TreeNode();
                            StepNode.Text = $"{v.Cells["C2"].Value}";
                            StepNode.Tag = $"{v.Cells["C4"].Value}";
                            for (int x = 0; x < script.dataGridView1.Columns.Count; x++)
                            {
                                TreeNode DataNode = new TreeNode();
                                DataNode.Tag = $"{script.dataGridView1.Columns[x].HeaderText}";
                                DataNode.Text = $"{script.dataGridView1.Columns[x].HeaderText}:{$"{v.Cells[script.dataGridView1.Columns[x].Name].Value}"}";
                                //display
                                string[] array_Function = new string[]  {
                                    "Step","Group","Behavior","ParameterME","GroupLoop","Type"
                                };
                                if ((string)DataNode.Tag == "Parameter")
                                {
                                    DataNode.Name = $"{v.Cells[script.dataGridView1.Columns["C5"].Name].Value}";
                                }
                                if ((string)DataNode.Tag == "GroupLoop")
                                {
                                    int GroupLoop = 1;
                                    try
                                    {
                                        GroupLoop = Convert.ToInt32($"{v.Cells[script.dataGridView1.Columns[x].Name].Value}");
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    if (!GroupListLoop.ContainsKey(Group))
                                    {
                                        GroupListLoop.Add(Group, GroupLoop);
                                        GroupListLoop[Group] = GroupLoop;
                                    }
                                    else
                                    {
                                        /*if (GroupLoop > GroupListLoop[Group])*/
                                        GroupListLoop[Group] = GroupLoop;
                                    }
                                    DataNode.Name = $"{v.Cells[script.dataGridView1.Columns["C5"].Name].Value}";
                                }

                                if (Array.FindIndex(array_Function, t => t == (string)DataNode.Tag) == (-1)) // -1 = null 
                                {
                                    StepNode.Nodes.Add(DataNode);
                                }
                            }
                            GroupNode.Nodes.Add(StepNode);
                        }
                        GroupNode.Text = $"{Group}({GroupListLoop[Group]})";
                        treeView1.Nodes.Add(GroupNode);
                    }

                    treeView1.ExpandAll();
                }
            }
            else
            {
                MessageBox.Show("No script.");
            }
        }
        /// <summary>
        /// Resolution socket connect
        /// </summary>
        /// <param name="send">send data</param>
        /// <param name="msg">receive data</param>
        public void Resolution_ConncetNet(string send, ref string msg)
        {
            DateTime dateTime = DateTime.Now;
            bool ConnectStatus = false;
            int timePile = 0;
            while (!ConnectStatus && timePile <= Parameters.timeout_socket)
            {
                ConnectStatus = dll_PublicFuntion.Other.SocketStatus(ip1.Text.Trim(), Parameters.Socket_port);
                timePile = Convert.ToInt32(new TimeSpan(dateTime.Ticks).Subtract(new TimeSpan(DateTime.Now.Ticks)).Duration().TotalSeconds);

                dll_PublicFuntion.Other.Wait(1);
            }
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Dictionary<string, object> UIdata = new Dictionary<string, object>();
            if (ConnectStatus)
            {

                try
                {
                    socket.Connect(ip1.Text.Trim(), Parameters.Socket_port);

                    try //send receive
                    {
                        int bytes = 0;
                        Console.Write(send + "\n");
                        byte[] bmsg = Encoding.UTF8.GetBytes(send);
                        Console.WriteLine("send length:" + bmsg.Length);
                        socket.Send(bmsg);
                        Thread.Sleep(10);
                        // Receive
                        byte[] getbuffer = new byte[1024 * 50000];
                        bytes = socket.Receive(getbuffer);
                        msg = System.Text.Encoding.UTF8.GetString(getbuffer, 0, bytes).Trim('\0');
                        //Console.Write("msg: " + msg + "\n");
                        UIdata = dll_PublicFuntion.Other.XmlToDictionary(msg);
                        socket.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("socket exception: " + e.Message);
                        socket.Close();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Internet Error");
                }
            }
        }
        /// <summary>
        /// Load initial profile
        /// </summary>
        private void Config_Load()
        {
            if (File.Exists(Application.StartupPath + @"\Config.txt"))
            {
                using (StreamReader sr = new StreamReader(Application.StartupPath + @"\Config.txt"))
                {
                    string[] data = sr.ReadToEnd().Split('\n');

                    if (File.Exists(Application.StartupPath + $@"\Script\{data[0].Trim()}"))
                    {
                        Parameters.path = $@"{Application.StartupPath}\Script\{data[0].Trim()}";
                        LogAdd("ScriptLoad", $"path: {Parameters.path}", false);
                        ScriptInit(false);
                    }
                    if (File.Exists(Application.StartupPath + $@"\ParameterData\{data[1].Trim()}"))
                    {
                        string OpenpathQ = Application.StartupPath + $@"\ParameterData\{data[1].Trim()}";
                        StreamReader srQ = new StreamReader(OpenpathQ);
                        string[] ParameterListQ = srQ.ReadToEnd().Split('\n');
                        if (!string.IsNullOrEmpty(ParameterListQ[0]))//server ip
                            ip1.Text = ParameterListQ[0].Trim();
                        if (!string.IsNullOrEmpty(ParameterListQ[1]))
                            DUTIDText.Text = ParameterListQ[1].Trim();
                        if (!string.IsNullOrEmpty(ParameterListQ[2]))
                            ACIPText.Text = ParameterListQ[2].Trim();
                        srQ.Close();
                    }
                    if (!string.IsNullOrEmpty(data[2].Trim()))
                    {
                        try
                        {
                            Parameters.Confidential = Convert.ToDouble(data[2].Trim()) / 100;
                        }
                        catch (Exception)
                        {
                            Parameters.Confidential = 0.80;
                        }
                    }
                }
            }
            else
                AddTaskToScheduler($@"{Application.StartupPath}\HP_Display.exe", "", "HP_Display");
        }
        /// <param name="e"></param>
        /// /// <summary>
        /// Socket connect
        /// </summary>
        /// <param name="send">send data</param>
        /// <param name="stepCount">which step</param>
        /// <param name="sheet">excel sheet</param>
        /// <param name="group">group name</param>
        /// <param name="stepCounter">excel step count</param>
        /// <param name="result">true => pass ; false => fail</param>
        /// <param name="stepName">step name</param>
        public void ConnectNet(string send, int stepCount, ISheet sheet, string group, int stepCounter, ref bool result, string stepName)
        {
            string msg = "";
            ConnectNet(send, stepCount, sheet, group, stepCounter, ref result, stepName, ref msg);
        }
        public void ConnectNet(string send, int stepCount, ISheet sheet, string group, int stepCounter, ref bool result, string stepName, ref string msg)
        {
            string Status = "Fail";
            string Behavior_ = stepName;
            string Remark = "Internet error.";
            string Necessary = "";

            DateTime dateTime = DateTime.Now;
            bool ConnectStatus = false;
            int timePile = 0;

            // Check socket connect status
            while (!ConnectStatus && timePile <= Parameters.timeout_socket)
            {
                ConnectStatus = dll_PublicFuntion.Other.SocketStatus(ip1.Text.Trim(), Parameters.Socket_port);
                timePile = Convert.ToInt32(new TimeSpan(dateTime.Ticks).Subtract(new TimeSpan(DateTime.Now.Ticks)).Duration().TotalSeconds);
                dll_PublicFuntion.Other.Wait(2);
            }

            Dictionary<string, object> UIdata = new Dictionary<string, object>();
            // Socket send/receive after connect status equals true
            if (ConnectStatus)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    socket.Connect(ip1.Text.Trim(), Parameters.Socket_port);
                    // send && received
                    try
                    {
                        int bytes = 0;
                        Console.Write("send data:" + send + "\n");
                        byte[] bmsg = Encoding.UTF8.GetBytes(send);
                        Console.WriteLine("send length:" + bmsg.Length);
                        socket.Send(bmsg);
                        Thread.Sleep(10);
                        // Receive data
                        byte[] getbuffer = new byte[1024 * 50000];
                        bytes = socket.Receive(getbuffer);
                        msg = Encoding.UTF8.GetString(getbuffer, 0, bytes).Trim('\0');
                        Console.Write("msg: " + msg + "\n");
                        UIdata = dll_PublicFuntion.Other.XmlToDictionary(msg);
                        socket.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("socket exception: " + e.Message);
                    }
                }
                catch (Exception)
                {
                    Status = "Fail";
                    Remark = "Socket connect error.";
                }
            }

            // Get UIdata => write into excel => green / red light
            try
            {
                // Dont record the functions as below
                if ((string)UIdata[Parameters.Behavior] != Behavior.Screen_Check && (string)UIdata[Parameters.Behavior] != Behavior.HDCP_Check && (string)UIdata[Parameters.Behavior] != Behavior.HDCP_CheckPass && (string)UIdata[Parameters.Behavior] != Behavior.Movie_Play_Check && (string)UIdata[Parameters.Behavior] != Behavior.DVD_Restart && (string)UIdata[Parameters.Behavior] != Behavior.Resolution_Check && (string)UIdata[Parameters.Behavior] != Behavior.Opening_Check && (string)UIdata[Parameters.Behavior] != Behavior.Send_Enter)
                {
                    LogAdd("ConnectNet", $"UIdata: {dll_PublicFuntion.Other.DictionaryToString(UIdata)}", false);
                }

                Image IDSImageFull;
                Image IDSImagePortion;

                if (UIdata.ContainsKey(Parameters.Status))
                    Status = $"{UIdata[Parameters.Status]}";

                if (UIdata.ContainsKey(Parameters.Behavior))
                    Behavior_ = $"{UIdata[Parameters.Behavior]}";

                if (UIdata.ContainsKey(Parameters.Remark))
                    Remark = $"{UIdata[Parameters.Remark]}";

                if (UIdata.ContainsKey(Parameters.Necessary))
                    Necessary = $"{UIdata[Parameters.Necessary]}";

                // IDSImage ( Full / Portion )
                if (UIdata.ContainsKey("IDSImageFull") && UIdata["IDSImageFull"] is Image)
                {
                    IDSImageFull = (Image)UIdata["IDSImageFull"];
                    IDSImageFull.Save(string.Format(FuncClass.Check_path($@"IDSData\{(string)UIdata["DataFile"]}") + "{0}.jpg", "FullScreen", System.Drawing.Imaging.ImageFormat.Png));
                }
                if (UIdata.ContainsKey("IDSImagePortion") && UIdata["IDSImagePortion"] is Image)
                {
                    IDSImagePortion = (Image)UIdata["IDSImagePortion"];
                    IDSImagePortion.Save(string.Format(FuncClass.Check_path($@"IDSData\{(string)UIdata["DataFile"]}") + "{0}.jpg", "OcrRegion", System.Drawing.Imaging.ImageFormat.Png));
                }
                halfSocketRemark = Remark;
            }
            catch (Exception) { }

            if (Status != "")
            {
                SocketDataCheck = 0;

                if (Behavior_ != Behavior.Screen_Check && Behavior_ != Behavior.HDCP_Check && Behavior_ != Behavior.HDCP_CheckPass && Behavior_ != Behavior.Movie_Play_Check && Behavior_ != Behavior.DVD_Restart && Behavior_ != Behavior.Opening_Check && Behavior_ != Behavior.Resolution_Check && Behavior_ != Behavior.Send_Enter)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        LogAdd("ConnectNet", $"{Parameters.Status}: {Status}", false);
                        LogBox.Text += Behavior_ + Environment.NewLine;
                        LogAdd("ConnectNet", $"{Parameters.Behavior}: {Behavior_}", false);
                        LogBox.Text += Remark + Environment.NewLine;
                        LogAdd("ConnectNet", $"{Parameters.Necessary}: {Necessary}", false);
                        LogAdd("ConnectNet", $"{Parameters.Remark}: {Remark}", false);
                        IRow row = sheet.CreateRow(stepCount);
                        row.CreateCell(0).SetCellValue(group);
                        row.CreateCell(1).SetCellType(CellType.Numeric);
                        row.GetCell(1).SetCellValue(stepCounter);
                        row.CreateCell(2).SetCellValue(Behavior_);
                        row.CreateCell(3).SetCellValue(Status);
                        row.CreateCell(4).SetCellValue(Remark);
                        row.CreateCell(5).SetCellValue(DateTime.Now.ToLongTimeString());
                        for (int i = 0; i < row.Count(); i++)
                            sheet.AutoSizeColumn(i);
                    });
                }
                if (Status == "Fail")
                {
                    result = false;
                    NodeStatus = false;
                    if (Necessary == "Yes")
                    {
                        StopStatus = false;
                    }
                }
                else if (Status == "Pass")
                {
                    result = true;
                    NodeStatus = true;
                }
            }

            else
                SocketDataCheck = 1;
        }

        #region Equal scaling
        private System.Drawing.Size m_szInit;//初始窗體大小
        private Dictionary<Control, Rectangle> m_dicSize = new Dictionary<Control, Rectangle>();
        protected override void OnLoad(EventArgs e)
        {
            m_szInit = this.Size;//get initial size
            this.GetInitSize(this);
            base.OnLoad(e);
        }
        private void GetInitSize(Control ctrl)
        {
            foreach (Control c in ctrl.Controls)
            {
                m_dicSize.Add(c, new Rectangle(c.Location, c.Size));
                this.GetInitSize(c);
            }
        }
        protected override void OnResize(EventArgs e)
        {
            //Calculate size propotion between present and initial
            float fx = (float)this.Width / m_szInit.Width;
            float fy = (float)this.Height / m_szInit.Height;
            foreach (var v in m_dicSize)
            {
                v.Key.Left = (int)(v.Value.Left * fx);
                v.Key.Top = (int)(v.Value.Top * fy);
                v.Key.Width = (int)(v.Value.Width * fx);
                v.Key.Height = (int)(v.Value.Height * fy);
            }
            base.OnResize(e);
        }

        #endregion

    }
}
