using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HP_Display
{
    public partial class OSDcutForm : Form
    {
        dll_IDS.IDS CaptureIDS = null;
        private Dictionary<int, string> ListCamerasData = null;
        public static string NowCamera = "";

        Rectangle theRectangle = new Rectangle(new Point(0, 0), new Size(0, 0));
        Bitmap newRec;

        bool isDrag = false;
        Point startPoint, oldPoint;
        private Graphics ig;
        Graphics g;

        public OSDcutForm()
        {
            InitializeComponent();
        }


        private void OSDcutForm_Load(object sender, EventArgs e)
        {
            ListCamerasData = new Dictionary<int, string>();


            //-> Find systems IDS cameras with uEye.Types.CameraInformation[]
            uEye.Types.CameraInformation[] cameraList;
            uEye.Info.Camera.GetCameraList(out cameraList);
            int _DeviceIndex = 0;
            foreach (uEye.Types.CameraInformation _Camera in cameraList)
            {
                ListCamerasData.Add(_Camera.DeviceID, _Camera.Model);
                CBox_CameraList.Items.Add(new ComboboxItem(_Camera.DeviceID.ToString(), _Camera.Model));
                _DeviceIndex++;
            }

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            //自動連上相機
            this.Cursor = Cursors.WaitCursor;
            CBox_CameraList.SelectedIndex = 0;
            NowCamera = CBox_CameraList.Text;
            CaptureIDS = new dll_IDS.IDS(0);
            CaptureIDS.LoadiniFile();
            CaptureIDS.StartLive(pictureBox1.Handle,pictureBox1,null,null);

            Thread.Sleep(500);

            this.Cursor = Cursors.Default;
        }
        public class ComboboxItem
        {
            public ComboboxItem(string value, string text) { Value = value; Text = text; }
            public string Value { get; set; }
            public string Text { get; set; }
            public override string ToString() { return Text; }
        }
        Bitmap NowFrame;
        private void FScCut_Click(object sender, EventArgs e)
        {
            //CameraConfig.Capture.CaptureToFile("temp.png");
            NowFrame = new Bitmap(CaptureIDS.GetImage());
            if (NowFrame != null)
            {
                int formWidth = Width - 100;
                int formHeight = Height - 100;

                for (int i = 0; i < formWidth; i++)
                {
                    int P_Width = formWidth - i;

                    double P_HeightCount = (Convert.ToDouble(P_Width) / Convert.ToDouble(NowFrame.Width)) * Convert.ToDouble(NowFrame.Height);
                    if (!int.TryParse(P_HeightCount.ToString(), out int n))
                    {
                        continue;
                    }
                    int P_Height = int.Parse(P_HeightCount.ToString());
                    if (P_Width < formWidth && P_Height < formHeight)
                    {

                        pictureBox1.Width = P_Width;
                        pictureBox1.Height = P_Height;
                        break;
                    }
                }/*for (int i = 0; i < formWidth; i++) end*/
                dll_PublicFuntion.Other.ImageWidthHeight(NowFrame, pictureBox1);
                Thread.Sleep(1000);
            }/*if(image!=null) end*/

            //截完圖釋放掉CutImaageForm
            CaptureIDS.StopLive();
            //pictureBox1.Dispose();
            //pictureBox1 = null;
            //開啟截圖畫面
            //pictureBox1.Image = NowFrame;
            // if (NowFrame != null) NowFrame.Dispose();
        }

        List<double> CutRange;
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDrag = false;

            //創Graphic在picturebox
            ig = pictureBox1.CreateGraphics();
            //繪製矩形
            ig.DrawRectangle(new Pen(Color.Red, 1), startPoint.X, startPoint.Y, e.X - startPoint.X, e.Y - startPoint.Y);
            theRectangle = new Rectangle(startPoint.X, startPoint.Y, e.X - startPoint.X, e.Y - startPoint.Y);
            g.DrawRectangle(new Pen(Color.Red, 1), theRectangle);

            try
            {
                /*框選範圍轉為解析度尺寸*/
                double New_PX = (Convert.ToDouble(pictureBox1.Image.Width) / Convert.ToDouble(pictureBox1.Width)) * Convert.ToDouble(theRectangle.X);
                double New_PY = (Convert.ToDouble(pictureBox1.Image.Height) / Convert.ToDouble(pictureBox1.Height)) * Convert.ToDouble(theRectangle.Y);
                if (New_PX >= 0 && New_PY >= 0)
                {
                    double New_Width = (Convert.ToDouble(pictureBox1.Image.Width) / Convert.ToDouble(pictureBox1.Width)) * Convert.ToDouble(theRectangle.Width);
                    double New_Height = (Convert.ToDouble(pictureBox1.Image.Height) / Convert.ToDouble(pictureBox1.Height)) * Convert.ToDouble(theRectangle.Height);
                    newRec = new Bitmap(Convert.ToInt32(New_Width), Convert.ToInt32(New_Height));
                    Graphics grPhoto = Graphics.FromImage(newRec);
                    grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
                    grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    grPhoto.CompositingQuality = CompositingQuality.HighQuality;
                    grPhoto.DrawImage(pictureBox1.Image, 0, 0, new Rectangle(Convert.ToInt32(New_PX), Convert.ToInt32(New_PY), Convert.ToInt32(New_Width), Convert.ToInt32(New_Height)), GraphicsUnit.Pixel);
                    dll_PublicFuntion.Other.ImageWidthHeight(newRec, pictureBox2);

                    CutRange = new List<double> { New_PX, New_PY, New_Width, New_Height };
                }
            }
            catch (Exception)
            {
            }
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrag)
            {
                g = pictureBox1.CreateGraphics();
                pictureBox1.Refresh();
                g.DrawRectangle(new Pen(Color.Red, 1), startPoint.X, startPoint.Y, e.X - startPoint.X, e.Y - startPoint.Y);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null && FileNameText.Text != "")
            {
                NowFrame.Save(string.Format(FuncClass.Check_path($@"IDSCut\{DirText.Text}\") + "{0}.jpg", FileNameText.Text, System.Drawing.Imaging.ImageFormat.Png));
                MessageBox.Show("Save");
                FileNameText.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null && FileNameText.Text != "")
            {
                newRec.Save(string.Format(FuncClass.Check_path($@"IDSCut\{DirText.Text}\") + "{0}.jpg", FileNameText.Text, System.Drawing.Imaging.ImageFormat.Png));
                if (!Directory.Exists($@"IDSCut\{DirText.Text}"))
                {
                    Directory.CreateDirectory($@"IDSCut\{DirText.Text}");
                }
                using (StreamWriter sw = new StreamWriter(FuncClass.Check_path($@"IDSCut\{DirText.Text}") + $"{FileNameText.Text}.txt"))
                {
                    foreach (var item in CutRange)
                    {
                        sw.WriteLine(item);
                    }
                    sw.Close();
                }
                pictureBox1.Refresh();
                MessageBox.Show("Save");
                FileNameText.Clear();
            }
        }

        private void OSDcutForm_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //如果开始绘制，则开始记录鼠标位置
                if ((isDrag = !isDrag) == true)
                {
                    startPoint = new Point(e.X, e.Y);
                    oldPoint = new Point(e.X, e.Y);
                }
            }
            pictureBox1.Refresh();
        }
    }
}
