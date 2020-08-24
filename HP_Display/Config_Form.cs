using HP_Display.OtherCS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HP_Display
{
    public partial class Config_Form : Form
    {
        public static bool config_form_exist = false;
        public Config_Form()
        {
            InitializeComponent();
        }

        private void Config_Form_Load(object sender, EventArgs e)
        {
            config_form_exist = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            if (File.Exists(Application.StartupPath + @"\Config.txt"))
            {
                using (StreamReader sr = new StreamReader(Application.StartupPath + @"\Config.txt"))
                {
                    string[] data = sr.ReadToEnd().Split('\n');
                    txt_scriptName.Text = data[0].Trim();
                    txt_ConfigName.Text = data[1].Trim();
                    txt_recognizeRate.Text = data[2].Trim();
                }
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_recognizeRate.Text))
            {
                MessageBox.Show("Recognize reate can't be null");
                return;
            }
            List<string> ParameterList = new List<string>();
            ParameterList.Add(txt_scriptName.Text.Trim());
            ParameterList.Add(txt_ConfigName.Text.Trim());
            ParameterList.Add(txt_recognizeRate.Text.Trim());

            // Tool open automatically
            if (rbtn_yes.Checked)
            {
                ParameterList.Add("yes");
                FuncClass.AddTaskToScheduler($@"{Application.StartupPath}\HP_Display.exe", "", "HP_Display");
            }
            else if (rbtn_no.Checked)
            { 
                ParameterList.Add("no");
                FuncClass.DeleteTaskFromScheduler("HP_Display");
            }

            using (StreamWriter sw = new StreamWriter(Application.StartupPath + @"\Config.txt"))
            {
                foreach (var item in ParameterList)
                {
                    sw.WriteLine(item);
                }
                sw.Close();
            }
            try
            {
                Parameters.Confidential = Convert.ToDouble(txt_recognizeRate.Text.Trim()) / 100;
            }
            catch (Exception)
            {
                Parameters.Confidential = 0.80;
            }
            this.Close();
        }

        private void Config_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            config_form_exist = false;
        }

        private void btn_script_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = dll_PublicFuntion.Folder.Check_path($@"Script\");
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txt_scriptName.Text = openFile.SafeFileName;
            }
            openFile.Dispose();
        }

        private void btn_config_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = dll_PublicFuntion.Folder.Check_path($@"ParameterData\");
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txt_ConfigName.Text = openFile.SafeFileName;
            }
            openFile.Dispose();
        }

        
    }
}