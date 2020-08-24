using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HP_Display
{
    public partial class WebCam : Form
    {
        public bool cap_Live = false;
        public VideoCapture cap = null; // WebCam物件
        public WebCam()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConnectCamera(0);
        }
        public void ConnectCamera(int CameraIndex)
        {
            try
            {
                cap = new VideoCapture(CameraIndex); //連結到相機  
                
                if (cap.IsOpened)
                {
                    cap.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, 1080);
                    cap.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, 1920);
                    richTextBox1.Text += $"[{DateTime.Now}] Webcam started successfully.\n" + Environment.NewLine;
                    richTextBox1.Text += $"{cap}" + Environment.NewLine;
                    //OpenScript.Enabled = true;
                    //_mainForm.Btn_SetCamera.Enabled = true;
                    cap_Live = true;
                    //_mainForm.metroButton2.Text = "Disconnect";
                }
                else
                {
                    button2.Text = "Connect";
                    richTextBox1.Text += $"[{DateTime.Now}] Webcam failed to start.\n" + Environment.NewLine;
                    //_mainForm.OpenScript.Enabled = false;
                    //_mainForm.Btn_SetCamera.Enabled = false;
                    cap_Live = false;
                }
            }
            catch
            {
                button2.Text = "Connect";
                richTextBox1.Text += $"[{DateTime.Now}] Webcam failed to start.\n" + Environment.NewLine;
                //_mainForm.OpenScript.Enabled = false;
                //_mainForm.Btn_SetCamera.Enabled = false;
                cap_Live = false;
            }
        }
        
        public void DisconnectCamera()
        {
            try
            {
                cap.Dispose();
                cap = null;
                richTextBox1.Text += $"[{DateTime.Now}] Webcam stoped successfully.\n" + Environment.NewLine;
                //_mainForm.OpenScript.Enabled = false;
                button3.Enabled = false;
                button2.Text = "Connect";
                cap_Live = false;
            }
            catch (Exception ex)
            {
                richTextBox1.Text += $"[{DateTime.Now}] {ex.Message}\n" + Environment.NewLine;
                //_mainForm.OpenScript.Enabled = false;
                button3.Enabled = false;
                cap_Live = false;
            }
        }

        public void CapImg(string ImgPath)
        {
            cap.QueryFrame();//儲存圖片前先刷新一次相機畫面
            Bitmap frame = cap.QueryFrame().Bitmap; //Query攝影機的畫面
            frame.Save(ImgPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            frame.Dispose();
        }

        public double Comparison(string FilePath)
        {
            string path = Path.GetDirectoryName(FilePath);
            cap.QueryFrame();
            Thread.Sleep(100);
            Image<Bgr, byte> source = new Image<Bgr, byte>(cap.QueryFrame().Bitmap);
            Thread.Sleep(100);
            source.Save($@"{path}\source.jpg");
            Thread.Sleep(100);
            Image<Bgr, byte> match = new Image<Bgr, byte>(FilePath);
            Image<Gray, float> result = new Image<Gray, float>(source.Width, source.Height);
            result = source.MatchTemplate(match, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
            double min = 0, max = 0;
            Point maxp = new Point(0, 0);
            Point minp = new Point(0, 0);

            CvInvoke.MinMaxLoc(result, ref min, ref max, ref minp, ref maxp);
            //將比對率轉為百分比並且四捨五入到小數第二位
            max = max * 100;

            return Math.Round(max, 2);
        }
        public bool GetCameraVertical()
        {
            return cap.FlipVertical;
        }

        public bool GetCameraHorizontal()
        {
            return cap.FlipHorizontal;
        }
        public void SetCameraVertical(bool Vertical)
        {
            cap.FlipVertical = Vertical;
        }
        public void SetCameraHorizontal(bool Horizontal)
        {
            cap.FlipHorizontal = Horizontal;
        }
    }
}
