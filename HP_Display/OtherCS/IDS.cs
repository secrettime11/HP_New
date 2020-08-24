using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using uEye;

namespace HP_Display
{
    class IDS
    {
        public uEye.Camera m_Camera;
        public bool m_bLive = false;
        public Rectangle imageRect;
        private IntPtr m_displayHandle = IntPtr.Zero;
        private uEye.Defines.Status nRet = 0;
        private const int m_cnNumberOfSeqBuffers = 3;
        private string _iniPath = "";
        public IDS(int DeviceID)
        {
            m_Camera = new Camera();
            //if (Form1.ARGS.Length != 0)
            //{
            //    if (string.IsNullOrEmpty(Form1.ARGS[0])) ConnectIDS(DeviceID, "IDS.ini");
            //    else ConnectIDS(DeviceID, Form1.ARGS[0]);
            //}
            //else
            //{
            //    ConnectIDS(DeviceID, "IDS.ini");
            //}
            ConnectIDS(DeviceID, "IDS.ini");
            uEye.Defines.Status statusRet;
            //取得相機Size
            statusRet = m_Camera.Size.AOI.Get(out imageRect);
        }

        private void onFrameEvent(object sender, EventArgs e)
        {
            uEye.Camera Camera = sender as uEye.Camera;

            Int32 s32MemID;
            uEye.Defines.Status statusRet = Camera.Memory.GetLast(out s32MemID);

            if ((uEye.Defines.Status.SUCCESS == statusRet) && (0 < s32MemID))
            {
                if (uEye.Defines.Status.SUCCESS == Camera.Memory.Lock(s32MemID))
                {
                    Camera.Display.Render(s32MemID, m_displayHandle, uEye.Defines.DisplayRenderMode.FitToWindow);
                    Camera.Memory.Unlock(s32MemID);
                }
            }
        }
        /// <summary>
        /// 載入ini
        /// </summary>
        public void LoadiniFile(string ini_Path)
        {
            uEye.Defines.Status statusRet = 0;

            m_Camera.Acquisition.Stop();

            Int32[] memList;
            statusRet = m_Camera.Memory.GetList(out memList);
            if (statusRet != uEye.Defines.Status.Success)
            {
                // MessageBox.Show("Get memory list failed: " + statusRet);
                Environment.Exit(-1);
            }

            statusRet = m_Camera.Memory.Free(memList);
            if (statusRet != uEye.Defines.Status.Success)
            {
                // MessageBox.Show("Free memory list failed: " + statusRet);
                Environment.Exit(-1);
            }
            if (!File.Exists(ini_Path))
            {
                using (StreamWriter sw = new StreamWriter("IDS.ini"))   //小寫TXT     
                {
                    StringBuilder strB = new StringBuilder();
                    //create html & table
                    strB.AppendLine($@"[Versions]
ueye_api_64.dll=4.92.1315
ueye_usb_64.sys=4.92.0666
ueye_boot_64.sys=4.92.0666


[Sensor]
Sensor=UI3013XC
Sensor bit depth=0
Sensor source gain=0
FPN correction mode=0
Black reference mode=0
Sensor digital gain=0


[Image size]
Start X=0
Start Y=0
Start X absolute=0
Start Y absolute=0
Width=1280
Height=720
Binning=0
Subsampling=0


[Scaler]
Mode=0
Factor=0.000000


[Multi AOI]
Enabled=0
Mode=0
x1=0
x2=0
x3=0
x4=0
y1=0
y2=0
y3=0
y4=0


[Shutter]
Mode=0
Linescan number=0


[Log Mode]
Mode=3
Manual value=0
Manual gain=0


[Timing]
Pixelclock=81
Extended pixelclock range=0
Framerate=30.000000
Exposure=33.400000
Long exposure=0
Dual exposure ratio=0


[Selected Converter]
IS_SET_CM_RGB32=1
IS_SET_CM_RGB24=1
IS_SET_CM_RGB16=8
IS_SET_CM_RGB15=8
IS_SET_CM_Y8=8
IS_SET_CM_RGB8=0
IS_SET_CM_BAYER=8
IS_SET_CM_UYVY=8
IS_SET_CM_UYVY_MONO=0
IS_SET_CM_UYVY_BAYER=0
IS_CM_CBYCRY_PACKED=8
IS_SET_CM_RGBY=0
IS_SET_CM_RGB30=0
IS_SET_CM_Y12=0
IS_SET_CM_BAYER12=0
IS_SET_CM_Y16=0
IS_SET_CM_BAYER16=0
IS_CM_BGR12_UNPACKED=0
IS_CM_BGRA12_UNPACKED=0
IS_CM_JPEG=0
IS_CM_SENSOR_RAW10=8
IS_CM_MONO10=0
IS_CM_BGR10_UNPACKED=0
IS_CM_RGBA8_PACKED=0
IS_CM_RGB8_PACKED=0
IS_CM_RGBY8_PACKED=0
IS_CM_RGB10V2_PACKED=0
IS_CM_RGB12_UNPACKED=0
IS_CM_RGBA12_UNPACKED=0
IS_CM_RGB10_UNPACKED=0
IS_CM_RGB8_PLANAR=0


[Parameters]
Colormode=1
Gamma=1.000000
Hardware Gamma=0
Blacklevel Mode=0
Blacklevel Offset=0
Hotpixel Mode=0
Hotpixel Threshold=0
Sensor Hotpixel=0
Adaptive hotpixel correction enable=0
Adaptive hotpixel correction mode=0
Adaptive hotpixel correction sensitivity=3
GlobalShutter=0
AllowRawWithLut=0


[Gain]
Master=0
Red=0
Green=0
Blue=0
GainBoost=0


[Processing]
EdgeEnhancementFactor=1
RopEffect=0
Whitebalance=0
Whitebalance Red=1.000000
Whitebalance Green=1.000000
Whitebalance Blue=1.000000
Whitebalance Sensor Mode=1
Color correction=0
Color_correction_factor=1.000000
Color_correction_satU=130
Color_correction_satV=130
Bayer Conversion=1
JpegCompression=0
NoiseMode=0
ImageEffect=0
LscModel=0
WideDynamicRange=1
Saturation=0
Sharpness=0


[Auto features]
Auto Framerate control=0
Brightness exposure control=0
Brightness gain control=0
Auto Framerate Sensor control=0
Brightness exposure Sensor control=1
Brightness gain Sensor control=0
Brightness exposure Sensor control photometry=1
Brightness gain Sensor control photometry=1
Brightness control once=0
Brightness reference=128
Brightness speed=50
Brightness max gain=100
Brightness max exposure=33.333333
Brightness Aoi Left=0
Brightness Aoi Top=0
Brightness Aoi Width=1280
Brightness Aoi Height=720
Brightness Hysteresis=2
AutoImageControlMode=2
AutoImageControlPeakWhiteChannel=0
AutoImageControlExposureMinimum=0.000000
AutoImageControlPeakWhiteChannelMode=0
AutoImageControlPeakWhiteGranularity=0
Brightness Sensor Correction=0.000000
Brightness Sensor Backlight Compensation=0
Anti Flicker Mode=1
Auto WB control=0
Auto WB type=2
Auto WB RGB color model=1
Auto WB RGB color temperature=0
Auto WB offsetR=0
Auto WB offsetB=0
Auto WB Sensor offsetR=0
Auto WB Sensor offsetB=0
Auto WB gainMin=0
Auto WB gainMax=100
Auto WB speed=50
Auto WB Aoi Left=0
Auto WB Aoi Top=0
Auto WB Aoi Width=1280
Auto WB Aoi Height=720
Auto WB Once=0
Auto WB Hysteresis=2
Brightness Skip Frames Trigger Mode=4
Brightness Skip Frames Freerun Mode=4
Auto WB Skip Frames Trigger Mode=4
Auto WB Skip Frames Freerun Mode=4


[Trigger and Flash]
Trigger mode=0
Trigger timeout=200
Trigger delay=0
Trigger debounce mode=0
Trigger debounce delay time=1
Trigger burst size=1
Trigger prescaler frame=1
Trigger prescaler line=1
Trigger input=1
Flash strobe=0
Flash delay=0
Flash duration=0
Flash auto freerun=0
PWM mode=0
PWM frequency=20000000
PWM dutycycle=20000000
GPIO state=0
GPIO direction=0
GPIO1 Config=0
GPIO2 Config=0


[Lens Focus]
Focus manual value=35
Focus AF enable=0
Focus AF use FDT AOI=0
Focus AF zone preset=0


[Image Stabilization]
Image Stabilization enable=0


[Face Detection]
Search angle=0
Search angle enable=1
Maximum number overlays=8
Overlay line width=2
Enable=0


[Zoom]
Digital zoom factor=1.000000


[Memory]
Camera memory mode=0

                                        ");
                    sw.Write(strB);
                }
                _iniPath = "IDS.ini";
                Thread.Sleep(500);
                statusRet = m_Camera.Parameter.Load("IDS.ini");
                //MessageBox.Show("Please confirm the existence of IDS.ini");
                //return;
            }
            else
            {
                string iniContent = "";
                //讀取.ini檔案
                using (StreamReader SR = new StreamReader(ini_Path))
                {
                    iniContent = SR.ReadToEnd();
                }
                //將.ini中的Colormode改為1,否則會無法存取Bitmap
                iniContent = Regex.Replace(iniContent, "Colormode=.", "Colormode=1");
                //重新寫入.ini
                using (StreamWriter SW = new StreamWriter(ini_Path))
                {
                    SW.WriteLine(iniContent);
                }
                _iniPath = ini_Path;
                Thread.Sleep(500);
                statusRet = m_Camera.Parameter.Load(ini_Path);
            }
            if (statusRet != uEye.Defines.Status.Success)
            {
                //MessageBox.Show("Loading parameter failed: " + statusRet);
            }

            statusRet = m_Camera.Memory.Allocate();
            if (statusRet != uEye.Defines.Status.SUCCESS)
            {
                //MessageBox.Show("Allocate Memory failed");
                Environment.Exit(-1);
            }

            if (m_bLive == true)
            {
                m_Camera.Acquisition.Capture();
            }
        }
        /// <summary>
        /// 將目前畫面儲存到指定路徑,預設儲存png
        /// </summary>
        /// <param name="FileName">圖片完整儲存路徑包含副檔名</param>
        public void CaptureToFile(string FileName)
        {
            if (m_bLive != true)
            {
                StartLive(IntPtr.Zero);
                Thread.Sleep(500);
            }
            Bitmap saveBitmap = new Bitmap(GetBitmap());
            saveBitmap.Save(FileName, ImageFormat.Png);
            saveBitmap.Dispose();
            //Console.WriteLine(m_Camera.Image.Save(FileName, ImageFormat.Png));
        }
        /// <summary>
        /// 開始更新畫面到指定的視窗
        /// </summary>
        /// <param name="PictureboxHandle">畫面的視窗控制代碼</param>
        public void StartLive(IntPtr PictureboxHandle)
        {
            // Open Camera and Start Live Video
            if (m_Camera.Acquisition.Capture() == uEye.Defines.Status.Success)
            {
                m_displayHandle = PictureboxHandle;
                m_bLive = true;
            }
        }
        /// <summary>
        /// 停止畫面更新,將視窗控制碼改為Zero
        /// </summary>
        public void StopLive()
        {
            // Stop Live Video
            if (m_Camera.Acquisition.Stop() == uEye.Defines.Status.Success)
            {
                m_displayHandle = IntPtr.Zero;
                m_bLive = false;
            }
        }
        /// <summary>
        /// 觸發一次自動對焦
        /// </summary>
        public void AutoFocus()
        {
            if (m_Camera.IsOpened)
            {
                m_Camera.Focus.Trigger();
                //自動將對焦後的數值寫回ini
                string iniContent = "";
                Focus focus = m_Camera.Focus;
                uint num = 0;
                focus.Manual.Get(out num);
                //讀取.ini檔案
                using (StreamReader SR = new StreamReader(_iniPath))
                {
                    iniContent = SR.ReadToEnd();
                }
                //將.ini中的Focus maual value改為目前設定值
                iniContent = Regex.Replace(iniContent, @"Focus manual value=\d*", $"Focus manual value={num}");
                //重新寫入.ini
                using (StreamWriter SW = new StreamWriter(_iniPath))
                {
                    SW.WriteLine(iniContent);
                }
            }
        }
        /// <summary>
        /// 設定畫面鏡像模式
        /// </summary>
        /// <param name="Mode">使用uEye.Defines.RopEffectMode選擇模式,UpDown：沿水平軸鏡像圖像。LeftRight：沿垂直軸鏡像圖像。MirrorNone：無鏡像。</param>
        /// <param name="ModeEnable">是否禁用該模式</param>
        public void CameraRopEffect(uEye.Defines.RopEffectMode Mode, bool ModeEnable)
        {
            //•uEye.Defines.RopEffectMode.UpDown：沿水平軸鏡像圖像。
            //•uEye.Defines.RopEffectMode.LeftRight：沿垂直軸鏡像圖像。
            //•uEye.Defines.RopEffectMode。MirrorNone：無鏡像。
            m_Camera.RopEffect.Set(Mode, ModeEnable);
        }

        public Bitmap GetBitmap()
        {
            uEye.Defines.Status statusRet = 0;

            // Get last image memory
            Int32 s32LastMemId;
            Int32 s32Width;
            Int32 s32Height;
            statusRet = m_Camera.Memory.GetLast(out s32LastMemId);
            statusRet = m_Camera.Memory.Lock(s32LastMemId);
            statusRet = m_Camera.Memory.GetSize(s32LastMemId, out s32Width, out s32Height);

            Bitmap MyBitmap;
            statusRet = m_Camera.Memory.ToBitmap(s32LastMemId, out MyBitmap);

            // clone bitmap
            Rectangle cloneRect = new Rectangle(0, 0, s32Width, s32Height);
            System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            Bitmap cloneBitmap = MyBitmap.Clone(cloneRect, format);

            // unlock image buffer
            statusRet = m_Camera.Memory.Unlock(s32LastMemId);
            MyBitmap.Dispose();
            return cloneBitmap;
        }

        public void CloseIDS()
        {
            if (m_Camera.IsOpened)
            {
                m_Camera.EventFrame -= onFrameEvent;
                m_Camera.Exit();
            }
        }

        public void ConnectIDS(int DeviceID, string iniPath)
        {
            //初始化相機
            nRet = m_Camera.Init(DeviceID);
            Thread.Sleep(200);
            LoadiniFile(iniPath);
            Thread.Sleep(200);
            //分配相機記憶體
            nRet = m_Camera.Memory.Allocate();
            //相機Frame更新時觸發
            m_Camera.EventFrame += onFrameEvent;
        }
    }
}
