using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HP_Display
{
    public partial class PicCut : Form
    {
        //Image image = null;
        Image BigShot = null;
        public PicCut()
        {
            InitializeComponent();
        }
        private static Bitmap bmpScreenshot;
        private static Graphics gfxScreenshot;
        private void FScCut_Click(object sender, EventArgs e)
        {
            if (bmpScreenshot != null) bmpScreenshot.Dispose();
            // Create a bmp for saving screen shot
            bmpScreenshot = new Bitmap(Resolution_.DESKTOP.Width, Resolution_.DESKTOP.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (gfxScreenshot != null) gfxScreenshot.Dispose();
            gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            //gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            gfxScreenshot.CopyFromScreen(0, 0, 0, 0, Resolution_.DESKTOP);
            BigShot = (Image)bmpScreenshot;
            Thread.Sleep(300);
            pictureBox1.Image = BigShot;
           
            if (BigShot != null)
            {
                int formWidth = Width - 100;
                int formHeight = Height - 100;

                for (int i = 0; i < formWidth; i++)
                {
                    int P_Width = formWidth - i;

                    double P_HeightCount = (Convert.ToDouble(P_Width) / Convert.ToDouble(BigShot.Width)) * Convert.ToDouble(BigShot.Height);
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
                }
                dll_PublicFuntion.Other.ImageWidthHeight(BigShot, pictureBox1);
                Thread.Sleep(1000);

            }/*if(image!=null) end*/

        }/*FScCut_Click end*/

        GlobalKeyboardHook gHook;
        private void PicCut_Load(object sender, EventArgs e)
        {
            gHook = new GlobalKeyboardHook(); // Create a new GlobalKeyboardHook

            // Declare a KeyDown Event
            gHook.KeyDown += new KeyEventHandler(gHook_KeyDown);

            // Add the keys you want to hook to the HookedKeys list
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                gHook.HookedKeys.Add(key);

            gHook.hook();
        }

        // Handle the KeyDown Event
        public void gHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PrintScreen)
            {
                FScCut.PerformClick();
            }
        }
        bool isDrag = false;
        Point startPoint, oldPoint;
        private Graphics ig;
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
        Graphics g;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrag)
            {
                g = pictureBox1.CreateGraphics();
                pictureBox1.Refresh();
                g.DrawRectangle(new Pen(Color.Red, 1), startPoint.X, startPoint.Y, e.X - startPoint.X, e.Y - startPoint.Y);
            }

        }

        Rectangle theRectangle = new Rectangle(new Point(0, 0), new Size(0, 0));
        Bitmap newRec;

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null && FileNameText.Text != "")
            {
                newRec.Save(string.Format(FuncClass.Check_path($@"ScreenCut\") + "{0}.jpg", FileNameText.Text, System.Drawing.Imaging.ImageFormat.Png));
                pictureBox1.Refresh();
                MessageBox.Show("Save");
            }
            else
            {
                MessageBox.Show("Please fill in the file name.");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null && FileNameText.Text != "")
            {
                BigShot.Save(string.Format(FuncClass.Check_path($@"ScreenCut\") + "{0}.jpg", FileNameText.Text, System.Drawing.Imaging.ImageFormat.Png));
                MessageBox.Show("Save");
            }
            else
            {
                MessageBox.Show("Please fill in the file name.");
            }
        }

        private void PicCut_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FileNameText_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void PicCut_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void PicCut_FormClosing(object sender, FormClosingEventArgs e)
        {
            gHook.unhook();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

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
                }
            }
            catch (Exception)
            {
            }

        }
    }
}
