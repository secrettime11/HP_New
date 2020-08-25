using Emgu.CV;
using Emgu.CV.Structure;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HP_Display.OtherCS
{
    class ClientBehavior
    {
        public static Main main;
        /// <summary>
        /// 開關AC
        /// </summary>
        /// <param name="SendMessage"></param>
        /// <param name="MainResultArray"></param>
        /// <param name="AnswerDic"></param>
        public static void AC_OnOff(ref Dictionary<string, object> SendMessage, ref Dictionary<string, object> MainResultArray, Dictionary<string, object> AnswerDic)
        {
            Dictionary<string, object> FDic = new Dictionary<string, object>();

            FDic = dll_PublicFuntion.Other.XmlToDictionary(SendMessage[Parameters.Parameter].ToString());

            string p1 = "";
            string p2 = "";
            string ip = "";
            if (FDic.ContainsKey("On"))
            {
                string on = FDic["On"].ToString();
                if (on == "")
                {
                    p1 = "#";
                }
                else
                {
                    for (int i = 0; i < on.Length; i++)
                    {
                        p1 += on.Substring(i, 1) + ",";
                    }
                    p1 = p1.Substring(0, p1.Length - 1);
                }

            }
            if (FDic.ContainsKey("Off"))
            {
                string off = FDic["Off"].ToString();
                if (off == "")
                {
                    p2 = "#";
                }
                else
                {
                    for (int i = 0; i < off.Length; i++)
                    {
                        p2 += off.Substring(i, 1) + ",";
                    }
                    p2 = p2.Substring(0, p2.Length - 1);
                }
            }
            if (FDic.ContainsKey("Ip"))
            {
                ip = $"{FDic["Ip"]}";

                if (ip == AnswerDic["ACIP"].ToString())
                {
                    try
                    {
                        Process p = new Process();
                        String str = null;

                        p.StartInfo.FileName = "cmd.exe";
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardInput = true;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.RedirectStandardError = true;
                        p.StartInfo.CreateNoWindow = false;
                        p.Start();
                        p.StandardInput.WriteLine($"PDU_ICGW08.exe {ip} {p1} {p2}");
                        p.StandardInput.WriteLine("exit");
                        str = p.StandardOutput.ReadToEnd();
                        p.WaitForExit();
                        p.Close();
                        Console.WriteLine(str);
                        if (str.Contains("請輸入正確"))
                        {
                            MainResultArray[Parameters.Status] = "Fail";
                            MainResultArray[Parameters.Remark] = "Wrong parameters.";
                        }
                        /*
                        else if (!str.Contains("超級用戶"))
                        {
                            MainResultArray[Parameters.Status] = "Fail";
                            MainResultArray[Parameters.Remark] = "PDU connect error.";
                        }
                        */
                    }
                    catch
                    {
                        MainResultArray[Parameters.Status] = "Fail";
                        MainResultArray[Parameters.Remark] = "PDU excuted fail.";
                    }
                }
                else
                {
                    MainResultArray[Parameters.Status] = "Fail";
                    MainResultArray[Parameters.Remark] = "Wrong IP.";
                }
            }
        }
        /// <summary>
        /// 電壓電流檢查
        /// </summary>
        /// <param name="SendMessage"></param>
        /// <param name="MainResultArray"></param>
        /// <param name="Combobox_text"></param>
        public static void VA_Check(ref Dictionary<string, object> SendMessage, ref Dictionary<string, object> MainResultArray)
        {
            Dictionary<string, object> FDic = new Dictionary<string, object>();

            FDic = dll_PublicFuntion.Other.XmlToDictionary(SendMessage[Parameters.Parameter].ToString());

            // SETTING RESULT
            double[] formulaResult = { Convert.ToDouble((string)FDic["V_Max"]), Convert.ToDouble((string)FDic["V_Min"]), Convert.ToDouble((string)FDic["A_Pass"]), Convert.ToDouble((string)FDic["A_Fail"]) };

            DataVa dATAVA = new DataVa();
            main.Invoke((MethodInvoker)delegate ()
            {
                dATAVA.OpenSerialPort(main.comboBox1.Text);
            });

            Thread.Sleep(1500);

            string resultStr = "";
            string V = "voltage";
            string Vlast = "V;";
            string A = "current";
            string Alast = "A;";

            bool GetRightResult = false;
            int aStart = 0, aEnd = 0, vStart = 0, vEnd = 0;

            DateTime dateTime = DateTime.Now;
            while (!GetRightResult)
            {
                TimeSpan timeSpan = DateTime.Now - dateTime;
                try
                {
                    if (timeSpan.TotalSeconds > 5)
                    {
                        MainResultArray[Parameters.Status] = "Fail";
                        MainResultArray[Parameters.Remark] = $"Data received error";
                        break;
                    }
                    resultStr = dATAVA.getDATA();
                    if (resultStr.Trim() != "" && resultStr != "error")
                    {
                        try
                        {
                            aStart = resultStr.IndexOf(A);

                            aEnd = resultStr.IndexOf(Alast);

                            vStart = resultStr.IndexOf(V);

                            vEnd = resultStr.IndexOf(Vlast);

                            if (aStart >= 0 && aEnd >= 0 && vStart >= 0 && vEnd >= 0)
                            {
                                Console.WriteLine("resultStr:" + resultStr + $"[{resultStr.Length}]");
                                Console.WriteLine($"aStart:{aStart} / aEnd:{aEnd} / vStart:{vStart} / vEnd:{vEnd}");

                                V = resultStr.Substring(vStart + 8, vEnd - vStart - 8);
                                A = resultStr.Substring(aStart + 8, aEnd - aStart - 8);

                                Console.WriteLine("V:" + V);
                                Console.WriteLine("A:" + A);
                                double digV = Convert.ToDouble(V);
                                double digA = Math.Abs(Convert.ToDouble(A));

                                // 0 = vmax | 1 = vmin | 2 = apass | 3 = afail
                                if (digV == 0)
                                    MainResultArray[Parameters.Status] = "Fail";
                                if (digV > formulaResult[0] || digV < formulaResult[1])
                                    MainResultArray[Parameters.Status] = "Verify";
                                if (digA < formulaResult[3])
                                    MainResultArray[Parameters.Status] = "Fail";
                                if (digA > formulaResult[3] && digA < formulaResult[2])
                                    MainResultArray[Parameters.Status] = "Verify";

                                MainResultArray[Parameters.Remark] = $"Voltage = {V}; Current = {Math.Abs(Convert.ToDouble(A)).ToString("f3")}";

                                GetRightResult = true;
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            GetRightResult = false;
                        }
                    }
                    Thread.Sleep(300);
                }
                catch (Exception)
                {
                }
            }
            dATAVA.CloseSerialPort();
        }
        /// <summary>
        /// 等待秒數
        /// </summary>
        /// <param name="SendMessage"></param>
        public static void WaitTime(ref Dictionary<string, object> SendMessage)
        {
            Dictionary<string, object> FDic = new Dictionary<string, object>();

            FDic = dll_PublicFuntion.Other.XmlToDictionary(SendMessage[Parameters.Parameter].ToString());

            if (FDic.ContainsKey("Wait"))
            {
                main.Invoke((MethodInvoker)delegate ()
                {
                    main.Timelb.Text = (string)FDic["Wait"];
                });

                double waitTime = Convert.ToDouble((string)FDic["Wait"]) * 1000;

                bool timeMinusFlag = true;
                double timeAll = Convert.ToDouble((string)FDic["Wait"]);
                while (timeMinusFlag)
                {
                    main.Invoke((MethodInvoker)delegate ()
                    {
                        main.Timelb.Text = timeAll.ToString();
                    });
                    Thread.Sleep(1000);
                    timeAll--;
                    if (timeAll == 0)
                    {
                        timeMinusFlag = false;
                    }
                }
                main.Invoke((MethodInvoker)delegate ()
                {
                    main.Timelb.Text = "0";
                });
            }
        }
        /// <summary>
        /// 解析度
        /// </summary>
        /// <param name="SendMessage"></param>
        /// <param name="MainResultArray"></param>
        /// <param name="SendString"></param>
        public static void Resolution_Check(ref Dictionary<string, object> SendMessage, ref Dictionary<string, object> MainResultArray, ref string SendString)
        {
            // 檢查IDS是否啟動
            FuncClass.IdsStatus();

            SendMessage.Add("SavePath", dll_PublicFuntion.Folder.ToolPath + "IDSNowImage.png");
            try { SendString = dll_PublicFuntion.Other.DictionaryToXml(SendMessage); } catch { }
            FuncClass.IDS_Check(SendMessage, ref MainResultArray, false);
            Thread.Sleep(200);

            if (!File.Exists((string)SendMessage["SavePath"]))
            {
                MainResultArray[Parameters.Status] = "Fail";
                MainResultArray[Parameters.Remark] = "IDS connect error.";
            }
            if ((string)MainResultArray[Parameters.Status] == "Pass")
            {
                Dictionary<string, object> FDic = new Dictionary<string, object>();
                try { FDic = dll_PublicFuntion.Other.XmlToDictionary(SendMessage[Parameters.Parameter].ToString()); } catch { }

                try
                {
                    Bitmap newRec = new Bitmap((int.Parse((string)FDic["ImgWidth"])), int.Parse((string)FDic["ImgHeight"]));
                    Image NowImg = dll_PublicFuntion.Other.LoadImage((string)SendMessage["SavePath"]);
                    Graphics grPhoto = Graphics.FromImage(newRec);

                    grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
                    grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    grPhoto.CompositingQuality = CompositingQuality.HighQuality;

                    Rectangle rectangleHAHA = new Rectangle();
                    rectangleHAHA.X = Convert.ToInt32((string)FDic["startX"]);
                    rectangleHAHA.Y = Convert.ToInt32((string)FDic["startY"]);
                    rectangleHAHA.Width = Convert.ToInt32((string)FDic["ImgWidth"]);
                    rectangleHAHA.Height = Convert.ToInt32((string)FDic["ImgHeight"]);

                    grPhoto.DrawImage(NowImg, 0, 0, rectangleHAHA, GraphicsUnit.Pixel);

                    grPhoto = Graphics.FromImage(NowImg);
                    grPhoto.DrawRectangle(new Pen(System.Drawing.Color.Red, 3), rectangleHAHA);
                    string F1 = string.Format(FuncClass.Check_path($@"IDSData\{(string)FDic["DataFile"]}") + @"{0}.png", "FullScreen");
                    string F2 = string.Format(FuncClass.Check_path($@"IDSData\{(string)FDic["DataFile"]}") + @"{0}.png", "OcrRegion");

                    NowImg.Save(F1, System.Drawing.Imaging.ImageFormat.Png);
                    newRec.Save(F2, System.Drawing.Imaging.ImageFormat.Png);
                    Thread.Sleep(1000);
                    dll_GoogleOCR.GoogleOCR googleOCR = new dll_GoogleOCR.GoogleOCR();
                    MainResultArray["DataFile"] = (string)FDic["DataFile"];

                    if (NowImg != null)
                    {
                        MainResultArray["IDSImageFull"] = NowImg;
                    }
                    if (newRec != null)
                    {
                        MainResultArray["IDSImagePortion"] = newRec;
                    }

                    string OCRresult = googleOCR.UploadFileToTxt(Application.StartupPath + $@"\IDSData\{(string)FDic["DataFile"]}\" + "OcrRegion.png", null, true);
                    string OriOCR = OCRresult.Replace("_", " ").Trim();

                    // Delete NowImage in case of wrong time
                    FuncClass.FileDelete(dll_PublicFuntion.Folder.ToolPath + "IDSNowImage.png");

                    // OSD display value
                    OCRresult = FuncClass.RexResolution(OCRresult.Replace("_", " ").Trim());

                    // Receive
                    string receMsg = "";
                    Dictionary<string, object> receivedData = new Dictionary<string, object>();

                    // Send to server
                    main.Resolution_ConncetNet(SendString, ref receMsg);
                    receivedData = dll_PublicFuntion.Other.XmlToDictionary(receMsg);

                    // Dut resolution
                    string OriDut = (string)receivedData["Remark"];
                    string dutResolution = FuncClass.RexResolution(((string)receivedData["Remark"]).Trim());
                    if (OriDut != "")
                    {
                        if (!OCRresult.Equals(dutResolution))
                        {
                            MainResultArray[Parameters.Status] = "Fail";
                            MainResultArray[Parameters.Remark] = $"OSD : {OriOCR} ; Windows : {(string)receivedData["Remark"]}";
                        }
                    }
                    else
                    {
                        MainResultArray[Parameters.Status] = "Fail";
                        MainResultArray[Parameters.Remark] = "No monitor has been detective.";
                    }

                    MainResultArray[Parameters.Remark] = $"OSD : {OriOCR} ; Windows : {OriDut}";

                }
                catch (Exception)
                {
                    MainResultArray[Parameters.Status] = "Fail";
                    MainResultArray[Parameters.Remark] = "Data received error.";
                }
            }

        }
        /// <summary>
        /// LED 燈色檢查
        /// </summary>
        /// <param name="SendMessage"></param>
        /// <param name="MainResultArray"></param>
        /// <param name="SendString"></param>
        public static void LED_Check(ref Dictionary<string, object> SendMessage, ref Dictionary<string, object> MainResultArray, ref string SendString)
        {
            string str = Application.StartupPath + @"\IDSData\LedData";
            DateTime timeNow = DateTime.Now;

            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);

            FuncClass.IdsStatus();
            SendMessage.Add("SavePath", dll_PublicFuntion.Folder.ToolPath + "IDSNowImage.png");
            try { SendString = dll_PublicFuntion.Other.DictionaryToXml(SendMessage); } catch { }
            FuncClass.IDS_Check(SendMessage, ref MainResultArray, false);

            if (!File.Exists((string)SendMessage["SavePath"]))
            {
                MainResultArray[Parameters.Status] = "Fail";
                MainResultArray[Parameters.Remark] = "IDS connect error.";
            }

            if ((string)MainResultArray[Parameters.Status] == "Pass")
            {
                Dictionary<string, object> FDic = new Dictionary<string, object>();
                try { FDic = dll_PublicFuntion.Other.XmlToDictionary(SendMessage[Parameters.Parameter].ToString()); } catch { }

                try
                {
                    // 創一個取樣大小的bitmap
                    Bitmap newRec = new Bitmap((int.Parse((string)FDic["ImgWidth"])), int.Parse((string)FDic["ImgHeight"]));
                    Image FullScreen = dll_PublicFuntion.Other.LoadImage((string)SendMessage["SavePath"]);

                    Graphics grPhoto = Graphics.FromImage(newRec);

                    Rectangle resultRec = new Rectangle();
                    resultRec.X = Convert.ToInt32((string)FDic["startX"]);
                    resultRec.Y = Convert.ToInt32((string)FDic["startY"]);
                    resultRec.Width = Convert.ToInt32((string)FDic["ImgWidth"]);
                    resultRec.Height = Convert.ToInt32((string)FDic["ImgHeight"]);
                    grPhoto.DrawImage(FullScreen, 0, 0, resultRec, GraphicsUnit.Pixel);

                    grPhoto = Graphics.FromImage(FullScreen);
                    grPhoto.DrawRectangle(new Pen(System.Drawing.Color.Red, 3), resultRec);
                    newRec.Save(string.Format(FuncClass.Check_path($@"IDSData\LedData\{(string)FDic["DataFile"]}") + "{0}.png", $"{timeNow:HH-mm-ss}", System.Drawing.Imaging.ImageFormat.Png));
                    Thread.Sleep(1000);
                    MainResultArray["DataFile"] = (string)FDic["DataFile"];

                    if (newRec != null)
                    {
                        MainResultArray["IDSImagePortion"] = newRec;
                        string fileplace = str + $@"\{(string)FDic["DataFile"]}\{timeNow:HH-mm-ss}.png";

                        Image GetBitmap = Image.FromFile(fileplace);
                        Image<Bgr, byte> inputImage = new Image<Bgr, byte>(fileplace);

                        string returnColor = FuncClass.getcolor(inputImage);
                        MainResultArray[Parameters.Remark] = returnColor;
                    }

                    // Delete NowImage in case of wrong time
                    FuncClass.FileDelete(dll_PublicFuntion.Folder.ToolPath + "IDSNowImage.png");
                }
                catch (Exception)
                {
                    // Delete NowImage in case of wrong time
                    FuncClass.FileDelete(dll_PublicFuntion.Folder.ToolPath + "IDSNowImage.png");

                    MainResultArray[Parameters.Status] = "Fail";
                    MainResultArray[Parameters.Remark] = "Image catch fail.";
                }
            }

        }
        /// <summary>
        /// PowerDVD 是否播放中
        /// </summary>
        /// <param name="SendMessage"></param>
        /// <param name="MainResultArray"></param>
        /// <param name="SendString"></param>
        /// <param name="GroupName"></param>
        /// <param name="ExcelStepCounter"></param>
        /// <param name="TotalStepCount"></param>
        /// <param name="sheet"></param>
        /// <param name="SocketDataCheck"></param>
        /// <param name="halfSocketRemark"></param>
        /// <param name="AnswerDic"></param>
        public static void Movie_Play_Check(ref Dictionary<string, object> SendMessage, ref Dictionary<string, object> MainResultArray, ref string SendString, string GroupName, ref int ExcelStepCounter, ref int TotalStepCount, ref ISheet sheet, ref int SocketDataCheck, ref string halfSocketRemark, ref Dictionary<string, object> AnswerDic)
        {
            FuncClass.IdsStatus();
            bool socketResult = false;
            // Socket send data
            Dictionary<string, object> SendMessageMovie = new Dictionary<string, object>
            {
                { Parameters.Group, GroupName},
                { Parameters.Behavior, Behavior.Movie_Play_Check},
                { Parameters.Necessary, "" },
                { Parameters.Parameter, ""},
                { Parameters.ServerIP, Parameters.ip},
                { Parameters.Answer, dll_PublicFuntion.Other.DictionaryToXml(AnswerDic) },
            };

            // Parameter => IconName / PicturePath / Wait
            Dictionary<string, object> FDic = new Dictionary<string, object>();
            try { FDic = dll_PublicFuntion.Other.XmlToDictionary(SendMessage[Parameters.Parameter].ToString()); } catch { }

            // Loop five times if not success, break if success
            for (int i = 0; i < 5; i++)
            {
                MainResultArray[Parameters.Remark] = "";
                // Add save path to dictionary at first time
                if (i == 0)
                {
                    SendMessageMovie.Add("SavePath", dll_PublicFuntion.Folder.ToolPath + "IDSNowImage.png");
                    try { SendString = dll_PublicFuntion.Other.DictionaryToXml(SendMessageMovie); } catch { }
                }

                do
                {
                    // Send to server
                    main.ConnectNet(dll_PublicFuntion.Other.DictionaryToXml(SendMessageMovie), TotalStepCount, sheet, GroupName, ExcelStepCounter, ref socketResult, Behavior.Movie_Play_Check);
                    dll_PublicFuntion.Other.Wait(1.5);
                } while (SocketDataCheck == 1);

                if (!socketResult)
                {
                    MainResultArray[Parameters.Status] = "Fail";
                    MainResultArray[Parameters.Remark] = "Can't find powerDVD.";
                    break;
                }

                // take a picture
                FuncClass.IDS_Check(SendMessageMovie, ref MainResultArray, true);

                Thread.Sleep(200);

                if (!File.Exists((string)SendMessageMovie["SavePath"]))
                {
                    MainResultArray[Parameters.Status] = "Fail";
                    MainResultArray[Parameters.Remark] = "IDS connect error.";
                    break;
                }


                // Picture comparison
                if (File.Exists((string)SendMessageMovie["SavePath"]) && socketResult == true)
                {
                    Image FullScreen = FuncClass.OperateImage((string)SendMessageMovie["SavePath"]);

                    Image SmallPic = FuncClass.OperateImage((string)FDic["PicturePath"]);

                    dll_opencv.OpenCV openCV = new dll_opencv.OpenCV();
                    openCV.SourceLoadImage(FullScreen);
                    openCV.SourceToGray();
                    openCV.MatchLoadImage(SmallPic);
                    openCV.MatchToGray();
                    dll_opencv.OpenCV.MatchDataList matchDataList = openCV.GetMatchPos(Parameters.Confidential, false);

                    double resultConfident = matchDataList.MaxConfidencevalue;
                    Thread.Sleep(50);

                    // Delete NowImage in case of wrong time
                    FuncClass.FileDelete(dll_PublicFuntion.Folder.ToolPath + "IDSNowImage.png");

                    if (resultConfident < Parameters.Confidential)
                    {
                        MainResultArray[Parameters.Status] = "Fail";
                        MainResultArray[Parameters.Remark] = "No match.";
                    }
                    else
                    {
                        MainResultArray[Parameters.Status] = "Pass";
                        break;
                    }
                }
            }
            if ((string)MainResultArray[Parameters.Status] == "Fail")
            {
                // Socket send data
                Dictionary<string, object> SendMessageRestart = new Dictionary<string, object>
                {
                    { Parameters.Group, GroupName},
                    { Parameters.Behavior, Behavior.DVD_Restart},
                    { Parameters.Necessary, "" },
                    { Parameters.Parameter, ""},
                    { Parameters.ServerIP, Parameters.ip},
                    { Parameters.Answer, "" },
                };
                do
                {
                    // Send to server
                    main.ConnectNet(dll_PublicFuntion.Other.DictionaryToXml(SendMessageRestart), TotalStepCount, sheet, GroupName, ExcelStepCounter, ref socketResult, Behavior.DVD_Restart);
                    dll_PublicFuntion.Other.Wait(1.5);
                } while (SocketDataCheck == 1);
                if (socketResult == false)
                {
                    MainResultArray[Parameters.Status] = "Fail";
                    MainResultArray[Parameters.Remark] = halfSocketRemark;
                }
                WaitTime(ref SendMessage);
            }
        }
        public static void Opening_Check(ref Dictionary<string, object> SendMessage, ref Dictionary<string, object> MainResultArray, ref string SendString, string GroupName, ref int ExcelStepCounter, ref int TotalStepCount, ref ISheet sheet, ref int SocketDataCheck, ref string halfSocketRemark)
        {
            FuncClass.IdsStatus();
            SendMessage.Add("SavePath", dll_PublicFuntion.Folder.ToolPath + "IDSNowImage.png");
            try { SendString = dll_PublicFuntion.Other.DictionaryToXml(SendMessage); } catch { }
            FuncClass.IDS_Check(SendMessage, ref MainResultArray, false);

            if (!File.Exists((string)SendMessage["SavePath"]))
            {
                MainResultArray[Parameters.Status] = "Fail";
                MainResultArray[Parameters.Remark] = "IDS connect error.";
            }
            // Parameter => IconName / PicturePath
            Dictionary<string, object> FDic = new Dictionary<string, object>();
            try { FDic = dll_PublicFuntion.Other.XmlToDictionary(SendMessage[Parameters.Parameter].ToString()); } catch { }

            // Picture comparison
            if (File.Exists((string)SendMessage["SavePath"]))
            {
                Image FullScreen = FuncClass.OperateImage((string)SendMessage["SavePath"]);

                Image SmallPic = FuncClass.OperateImage((string)FDic["PicturePath"]);

                dll_opencv.OpenCV openCV = new dll_opencv.OpenCV();
                openCV.SourceLoadImage(FullScreen);
                openCV.SourceToGray();
                openCV.MatchLoadImage(SmallPic);
                openCV.MatchToGray();
                dll_opencv.OpenCV.MatchDataList matchDataList = openCV.GetMatchPos(Parameters.Confidential, false);

                double resultConfident = matchDataList.MaxConfidencevalue;
                Thread.Sleep(50);
                // Delete NowImage in case of wrong time
                FuncClass.FileDelete(dll_PublicFuntion.Folder.ToolPath + "IDSNowImage.png");
                if (resultConfident < Parameters.Confidential)
                {
                    MainResultArray[Parameters.Status] = "Pass";
                    MainResultArray[Parameters.Remark] = "Didn't find the title icon.";
                }
                else
                {
                    bool socketResult = false;
                    MainResultArray[Parameters.Status] = "Pass";
                    do
                    {
                        SendMessage[Parameters.Behavior] = Behavior.Send_Enter;
                        // Send to server
                        main.ConnectNet(dll_PublicFuntion.Other.DictionaryToXml(SendMessage), TotalStepCount, sheet, GroupName, ExcelStepCounter, ref socketResult, Behavior.Send_Enter);
                        dll_PublicFuntion.Other.Wait(1.5);
                    } while (SocketDataCheck == 1);

                    if (socketResult == false)
                    {
                        MainResultArray[Parameters.Status] = "Fail";
                        MainResultArray[Parameters.Remark] = halfSocketRemark;
                    }
                }
            }

        }
        /*---- DAQ Function↓----*/
        public static void Monitor_OnOff(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("8", "HIGH", ref DaqReceive, "DO");

            if (DaqReceive.Trim() == "Success")
            {
                Thread.Sleep(300);
                FuncClass.DAQ("8", "LOW", ref DaqReceive, "DO");
                if (DaqReceive.Trim() != "Success")
                    MainResultArray[Parameters.Status] = "Fail";
            }
            else
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Hot_Plug(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("9", "HIGH", ref DaqReceive, "DO");
            if (DaqReceive.Trim() != "Success")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Hot_Unplug(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("9", "LOW", ref DaqReceive, "DO");
            if (DaqReceive.Trim() != "Success")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Hot_Plug_Reverse(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("10", "HIGH", ref DaqReceive, "DO");
            if (DaqReceive.Trim() != "Success")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Hot_Plug_UnReverse(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("10", "LOW", ref DaqReceive, "DO");
            if (DaqReceive.Trim() != "Success")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Press_PW_Button(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("13", "HIGH", ref DaqReceive, "DO");
            if (DaqReceive.Trim() == "Success")
            {
                Thread.Sleep(300);
                FuncClass.DAQ("13", "LOW", ref DaqReceive, "DO");
                if (DaqReceive.Trim() != "Success")
                    MainResultArray[Parameters.Status] = "Fail";
            }
            else
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Mouse_Click(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("14", "HIGH", ref DaqReceive, "DO");
            if (DaqReceive.Trim() == "Success")
            {
                Thread.Sleep(300);
                FuncClass.DAQ("14", "LOW", ref DaqReceive, "DO");
                if (DaqReceive.Trim() != "Success")
                    MainResultArray[Parameters.Status] = "Fail";
            }
            else
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Monitor_OSD(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("11", "HIGH", ref DaqReceive, "DO");
            if (DaqReceive.Trim() == "Success")
            {
                Thread.Sleep(300);
                FuncClass.DAQ("11", "LOW", ref DaqReceive, "DO");
                if (DaqReceive.Trim() != "Success")
                    MainResultArray[Parameters.Status] = "Fail";
            }
            else
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Monitor_OSD_1(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("24", "HIGH", ref DaqReceive, "DO");
            if (DaqReceive.Trim() == "Success")
            {
                Thread.Sleep(500);
                FuncClass.DAQ("24", "LOW", ref DaqReceive, "DO");
                if (DaqReceive.Trim() != "Success")
                    MainResultArray[Parameters.Status] = "Fail";
            }
            else
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Monitor_OSD_2(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("25", "HIGH", ref DaqReceive, "DO");
            if (DaqReceive.Trim() == "Success")
            {
                Thread.Sleep(500);
                FuncClass.DAQ("25", "LOW", ref DaqReceive, "DO");
                if (DaqReceive.Trim() != "Success")
                    MainResultArray[Parameters.Status] = "Fail";
            }
            else
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Monitor_OSD_3(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("26", "HIGH", ref DaqReceive, "DO");
            if (DaqReceive.Trim() == "Success")
            {
                Thread.Sleep(500);
                FuncClass.DAQ("26", "LOW", ref DaqReceive, "DO");
                if (DaqReceive.Trim() != "Success")
                    MainResultArray[Parameters.Status] = "Fail";
            }
            else
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Monitor_OSD_4(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("27", "HIGH", ref DaqReceive, "DO");
            if (DaqReceive.Trim() == "Success")
            {
                Thread.Sleep(500);
                FuncClass.DAQ("27", "LOW", ref DaqReceive, "DO");
                if (DaqReceive.Trim() != "Success")
                    MainResultArray[Parameters.Status] = "Fail";
            }
            else
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Monitor_OSD_5(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("28", "HIGH", ref DaqReceive, "DO");
            if (DaqReceive.Trim() == "Success")
            {
                Thread.Sleep(500);
                FuncClass.DAQ("28", "LOW", ref DaqReceive, "DO");
                if (DaqReceive.Trim() != "Success")
                    MainResultArray[Parameters.Status] = "Fail";
            }
            else
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Check_PD_Plug(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("2", "X", ref DaqReceive, "DI");
            if (DaqReceive.Trim() != "YES")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Check_PD_Unplug(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("3", "X", ref DaqReceive, "DI");
            if (DaqReceive.Trim() != "YES")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Check_PD_Original(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("4", "X", ref DaqReceive, "DI");
            if (DaqReceive.Trim() != "YES")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Check_PD_Reserve(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("5", "X", ref DaqReceive, "DI");
            if (DaqReceive.Trim() != "YES")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Check_DC_Push(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("6", "X", ref DaqReceive, "DI");
            if (DaqReceive.Trim() != "YES")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Check_DC_Back(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("7", "X", ref DaqReceive, "DI");
            if (DaqReceive.Trim() != "YES")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Check_PB_Press(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("18", "X", ref DaqReceive, "DI");
            if (DaqReceive.Trim() != "YES")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Check_PB_Release(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("19", "X", ref DaqReceive, "DI");
            if (DaqReceive.Trim() != "YES")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Check_Mouse_Click(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("20", "X", ref DaqReceive, "DI");
            if (DaqReceive.Trim() != "YES")
                MainResultArray[Parameters.Status] = "Fail";
        }
        public static void Check_Mouse_Release(ref Dictionary<string, object> MainResultArray, ref string DaqReceive)
        {
            FuncClass.DAQ("21", "X", ref DaqReceive, "DI");
            if (DaqReceive.Trim() != "YES")
                MainResultArray[Parameters.Status] = "Fail";
        }
    }
}
