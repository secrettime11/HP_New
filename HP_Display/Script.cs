using HP_Display.OtherCS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static HP_Display.OtherCS.Behavior;

namespace HP_Display
{
    public partial class Script : Form
    {
        Dictionary<string, Dictionary<string, string>> scriptDic = new Dictionary<string, Dictionary<string, string>>(); // 腳本字典
        Dictionary<string, string> funcDic = new Dictionary<string, string>();
        List<string> controlTextValue = new List<string>();
        public GroupForm groupForm;
        Main main = new Main();
        int rowindex;
        #region FunctionDic
        public static Dictionary<string, string> TypeDic = new Dictionary<string, string>
        {
            {"IOcbDic","Check I/O Status" },
            {"IOcontrolDic", "I/O Control" },
            {"PWMDic", "Power Management"},
            {"LEDDic", "LED Check" },
            {"ACDic", "AC On / Off"},
            {"VoltageDic", "Voltage / Power Charge"},
            {"WinDic", "Windows Check"},
            {"DvdDic", "PowerDVD"},
            {"WaitDic", "Wait Time"},
        };
        public static Dictionary<string, string> IOcbDic = new Dictionary<string, string>
        {
            {Check_PD_Plug,"Check (PD Jig)_Plug" },
            {Check_PD_Unplug, "Check (PD Jig)_Unplug" },
            {Check_PD_Original, "Check (PD Jig)_Original"},
            {Check_PD_Reserve, "Check (PD Jig)_Reserve" },
            {Check_DC_Push, "Check Monitor DC_On/Off_Push"},
            {Check_DC_Back, "Check Monitor DC_On/Off_Back"},
            {Check_PB_Press, "Check Power Button Press" },
            {Check_PB_Release, "Check Power Button Release" },
            {Check_Mouse_Click, "Check Mouse Click" },
            {Check_Mouse_Release, "Check Mouse Release" }
        };
        public static Dictionary<string, string> IOcontrolDic = new Dictionary<string, string>
        {
            {Monitor_OnOff,"Monitor DC On/Off" },
            {Hot_Plug, "Hot Plug (PD Jig)" },
            {Hot_Unplug, "Hot Unplug (PD Jig)" },
            {Hot_Plug_Reverse, "Hot Plug (PD Jig)_Reverse"},
            {Hot_Plug_UnReverse, "Hot Plug (PD Jig)_UnReverse"},
            {Press_PW_Button, "Press Power Button" },
            {Mouse_Click, "Mouse Click"},
            {Monitor_OSD, "Monitor OSD"},
            {Monitor_OSD_1, "OSD Button 1"},
            {Monitor_OSD_2, "OSD Button 2"},
            {Monitor_OSD_3, "OSD Button 3"},
            {Monitor_OSD_4, "OSD Button 4"},
            {Monitor_OSD_5, "OSD Button 5"},
        };
        public static Dictionary<string, string> PWMDic = new Dictionary<string, string>
        {
            {PM_Sleep,"PM Mode - Sleep" },
            {PM_Sleep_S3,"PM Mode - Sleep(S3)" },
            {PM_Hibernation, "PM Mode - Hibernation" },
            {PM_Warm, "PM Mode - Warm Boot"},
            {PM_Cold, "PM Mode - Cold Boot" },
            {Behavior.PM_Special_Hibernation, "PM Mode - Special Hibernation" },
            {Behavior.PM_Special_WarmBoot, "PM Mode - Special Warm Boot" },
            {Behavior.PM_Special_ColdBoot, "PM Mode - Special Cold Boot" },
        };

        public static Dictionary<string, string> LEDDic = new Dictionary<string, string>
        {
            {Behavior.LED_Check,"LED color check" },
        };
        public static Dictionary<string, string> ACDic = new Dictionary<string, string>
        {
            {Behavior.AC_OnOff,"AC On/Off" },
        };
        public static Dictionary<string, string> VoltageDic = new Dictionary<string, string>
        {
            {Behavior.VA_Check,"Voltage Check" },
            {Power_Check,"Power charge check" }
        };
        public static Dictionary<string, string> WinDic = new Dictionary<string, string>
        {
            {PnP_Check,"PnP Check" },
            {Behavior.Resolution_Check,"Resolution Check" },
            {Display_Check,"Display mode Check" }
        };
        public static Dictionary<string, string> DvdDic = new Dictionary<string, string>
        {
            {PowerDVD_On,"PowerDVD On" },
            {PowerDVD_Off,"PowerDVD Off" },
            {Play_Pause,"Play / Pause" },
            {Behavior.Screen_Check,"Screen Check" },
            {Behavior.ErrorMessage,"ErrorMessage Check" },
            {Behavior.HDCP_Check,"HDCP Check" },
            {Behavior.Movie_Play_Check,"Movie Play Check" },
            {Behavior.Drag_DVD,"Drag DVD" },
            {Behavior.Audio_Check,"Audio Check" },
            {Behavior.Opening_Check,"Opening Check" },
        };
        public static Dictionary<string, string> WaitDic = new Dictionary<string, string>
        {
            {Behavior.WaitTime,"Wait" }
        };
        Dictionary<string, TabPage> publicDic = new Dictionary<string, TabPage>
        {
        };
        #endregion

        public Script()
        {
            InitializeComponent();

        }
        private void Script_Load(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, string> keyValuePair in TypeDic)
            {
                CBox_Type.Items.Add(new dll_PublicFuntion.Other.ComboboxItem(keyValuePair.Key, keyValuePair.Value));
            }
            tabControl1.Visible = false;
            foreach (TabPage item in tabControl1.TabPages)
            {
                if (!publicDic.ContainsKey(item.Name))
                {
                    publicDic.Add(item.Name, item);
                    tabControl1.TabPages.Remove(item);
                }
            }
            searchLabel.Text = "";
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Please Add Script.");
                return;
            }
            string jsonN = JsonName.Text;
            if (JsonName.Text == "")
            {
                MessageBox.Show("Please fill in the file name.");
                return;
            }
            string FilePath = dll_PublicFuntion.Folder.Check_path(@"Script\") + jsonN + ".xml";
            if (File.Exists(FilePath))
            {
                if (MessageBox.Show("This file name has been used!" + Environment.NewLine + "Do you want to cover it?", "Reminds", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (!dll_PublicFuntion.DGV.DGVXmlCheck(FilePath, dataGridView1, "Script"))
                    {
                        if (MessageBox.Show("Are you sure?", "Caveat", MessageBoxButtons.YesNo, MessageBoxIcon.None) != DialogResult.Yes)
                        {
                            MessageBox.Show("Save Not success");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Script Not Change");
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            dll_PublicFuntion.DGV.DGVSaveXml(FilePath, dataGridView1, "Script"); //DATAVIEW寫入XML
            MessageBox.Show("Save success");
        }
        private void button5_Click(object sender, EventArgs e)
        {
            dll_PublicFuntion.DGV.DataGridSelectRowIndexList(dataGridView1, "Delete");
            //int i = dataGridView1.CurrentRow.Index;//獲取當前行
            //dataGridView1.Rows.Remove(dataGridView1.Rows[i]);
        }
        private void groupBtn_Click(object sender, EventArgs e)
        {
            groupForm = new GroupForm();
            groupForm.Owner = this;
            groupForm.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewSelectedRowCollection dgvsrc = dataGridView1.SelectedRows;//獲取選中行的集合
                Console.WriteLine($"dgvsrc {dgvsrc.Count}");
                if (dgvsrc.Count > 0)
                {
                    int index = dataGridView1.SelectedRows[0].Index;//獲取當前選中行的索引
                    if (index > 0)//如果該行不是第一行
                    {
                        DataGridViewRow dgvr = dataGridView1.Rows[index - dgvsrc.Count];//獲取當前選中行的索引
                        dataGridView1.Rows.RemoveAt(index - dgvsrc.Count);//删除原選中行的上一行
                        dataGridView1.Rows.Insert((index), dgvr);//將選中行的上一行插入到選中行的後面
                        for (int i = 0; i < dgvsrc.Count; i++)//選中移動後的行
                        {
                            dataGridView1.Rows[index - i - 1].Selected = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewSelectedRowCollection dgvsrc = dataGridView1.SelectedRows;//獲取選中行的集合
                if (dgvsrc.Count > 0)
                {
                    int index = dataGridView1.SelectedRows[0].Index;//獲取當前選中行的索引
                    if (index >= 0 & (dataGridView1.RowCount - 1) != index)//如果該行不是最後一行
                    {
                        DataGridViewRow dgvr = dataGridView1.Rows[index + 1];//獲取選中行的下一行
                        dataGridView1.Rows.RemoveAt(index + 1);//刪除原選中行的上一行
                        dataGridView1.Rows.Insert((index + 1 - dgvsrc.Count), dgvr);//將選中行的上一行插入到選中行的後面
                        for (int i = 0; i < dgvsrc.Count; i++)//選中移動後的行
                        {
                            dataGridView1.Rows[index + 1 - i].Selected = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void CBox_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                dll_PublicFuntion.Other.ComboboxItem Type_item = CBox_Type.Items[CBox_Type.SelectedIndex] as dll_PublicFuntion.Other.ComboboxItem;
                Dictionary<string, string> keys = null;
                switch (Type_item.Value)
                {
                    case "IOcbDic":
                        keys = IOcbDic;
                        break;
                    case "IOcontrolDic":
                        keys = IOcontrolDic;
                        break;
                    case "PWMDic":
                        keys = PWMDic;
                        break;
                    case "LEDDic":
                        keys = LEDDic;
                        break;
                    case "ACDic":
                        keys = ACDic;
                        break;
                    case "VoltageDic":
                        keys = VoltageDic;
                        break;
                    case "WinDic":
                        keys = WinDic;
                        break;
                    case "DvdDic":
                        keys = DvdDic;
                        break;
                    case "WaitDic":
                        keys = WaitDic;
                        break;
                }
                if (keys != null)
                {
                    CBox_Behavior.Items.Clear();
                    foreach (KeyValuePair<string, string> keyValuePair in keys)
                    {
                        CBox_Behavior.Items.Add(new dll_PublicFuntion.Other.ComboboxItem(keyValuePair.Key, keyValuePair.Value));
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            //dll_PublicFuntion.DGV.SelectDataGridCol(dataGridView1, dll_PublicFuntion.DGV.DataGridSelectRowIndextMin(dataGridView1), 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            Process.Start("devmgmt.msc");//裝置管理員
            Bitmap screenshot = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            Graphics graph = Graphics.FromImage(screenshot);
            graph.CopyFromScreen(Location.X, Location.Y, 0, 0, Size, CopyPixelOperation.SourceCopy);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Image File (*.png) | *.png";
            if (sfd.ShowDialog() == DialogResult.OK) 
            {
                string filename = sfd.FileName;
                screenshot.Save(filename);
            }
            */
            PicCut picCut = new PicCut();
            picCut.Show();
        }

        private void CBox_Behavior_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl1.Visible = false;
            dll_PublicFuntion.Other.ComboboxItem Behavior_item = CBox_Behavior.Items[CBox_Behavior.SelectedIndex] as dll_PublicFuntion.Other.ComboboxItem;
            foreach (TabPage item in tabControl1.TabPages)
            {
                tabControl1.TabPages.Remove(item);
            }
            if (publicDic.ContainsKey(Behavior_item.Value))
            {
                tabControl1.TabPages.Add(publicDic[Behavior_item.Value]);
            }
            if (tabControl1.TabPages.Count > 0)
            {
                tabControl1.Visible = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.InitialDirectory = dll_PublicFuntion.Folder.Check_path(@"IDSData\IDSInfo");

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string Openpath = openFileDialog1.FileName;
                StreamReader sr = new StreamReader(Openpath);
                string[] ParameterList = sr.ReadToEnd().Split('\n');
                if (!string.IsNullOrEmpty(ParameterList[0]))//server ip
                    sX.Text = ParameterList[0].Trim();
                if (!string.IsNullOrEmpty(ParameterList[1]))
                    sY.Text = ParameterList[1].Trim();
                if (!string.IsNullOrEmpty(ParameterList[2]))
                    eX.Text = ParameterList[2].Trim();
                if (!string.IsNullOrEmpty(ParameterList[2]))
                    eY.Text = ParameterList[3].Trim();
                string[] txtName = openFileDialog1.FileName.Split(new[] { "\\" }, StringSplitOptions.None);
                string Nametxt = txtName[txtName.Length - 1].Replace(".txt", "");
                filenameText.Text = Nametxt.Trim();
                sr.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = dll_PublicFuntion.Folder.Check_path(@"VisualIdentification\Data\ImageData");
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string path = openFileDialog1.FileName;
                    DvdPicPath.Text = path;
                    string[] txtName = openFileDialog1.FileName.Split(new[] { "\\" }, StringSplitOptions.None);
                    string Nametxt = txtName[txtName.Length - 2];
                    IconName.Text = Nametxt.Trim();
                }
            }

        }

        private void LR_CheckedChanged(object sender, EventArgs e)
        {
            if (LR.Checked)
            {
                RL.Checked = false;
            }
        }

        private void RL_CheckedChanged(object sender, EventArgs e)
        {
            if (RL.Checked)
            {
                LR.Checked = false;
            }
        }

        #region 等比縮放
        private Size m_szInit;//初始窗體大小
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

        #region 拖曳datagridview
        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (dataGridView1.HitTest(e.X, e.Y).Type != DataGridViewHitTestType.Cell) return;

            if (e.Button == MouseButtons.Left)
            {
                if (dataGridView1.Rows[dataGridView1.HitTest(e.X, e.Y).RowIndex].IsNewRow) return;
            }
        }

        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dataGridView1.HitTest(e.X, e.Y).Type != DataGridViewHitTestType.Cell) return;

            if (e.Button == MouseButtons.Left)
            {
                dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Selected = true;
                dataGridView1.DoDragDrop(dataGridView1.SelectedRows[0], DragDropEffects.Move);
            }
        }

        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            if (((DataObject)e.Data).GetData(typeof(DataGridViewRow)) == null) return;

            Point TargetPoint = dataGridView1.PointToClient(new Point(e.X, e.Y));
            if ((dataGridView1.HitTest(TargetPoint.X, TargetPoint.Y).Type != DataGridViewHitTestType.Cell)) return;

            int TargetRowIndex = dataGridView1.HitTest(TargetPoint.X, TargetPoint.Y).RowIndex;
            if ((dataGridView1.Rows[TargetRowIndex].IsNewRow) || (dataGridView1.SelectedRows.Contains(dataGridView1.Rows[TargetRowIndex]))) return;

            if (dataGridView1.DataSource == null)
            {
                DataGridViewRow SourceRow = dataGridView1.SelectedRows[0];
                dataGridView1.Rows.Remove(SourceRow);
                dataGridView1.Rows.Insert(TargetRowIndex, SourceRow);
            }
            else
            {
                DataTable SourceData = dataGridView1.DataSource.GetType() == typeof(BindingSource) ? ((DataSet)((BindingSource)dataGridView1.DataSource).DataSource).Tables[0] : (DataTable)dataGridView1.DataSource;
                SourceData.PrimaryKey = new DataColumn[] { SourceData.Columns[0] };

                DataRow OriginRow = SourceData.Rows.Find(dataGridView1.SelectedRows[0].Cells[0].Value);
                if (OriginRow == null) return;

                DataRow SourceRow = SourceData.NewRow();
                int InsertIndex = SourceData.Rows.IndexOf(SourceData.Rows.Find(dataGridView1.Rows[TargetRowIndex].Cells[0].Value));
                SourceRow.ItemArray = OriginRow.ItemArray;
                SourceData.Rows.Remove(OriginRow);
                SourceData.Rows.InsertAt(SourceRow, InsertIndex);

                SourceData.AcceptChanges();
            }
            dataGridView1.CurrentCell = dataGridView1.Rows[TargetRowIndex].Cells[dataGridView1.CurrentCell.ColumnIndex];
        }
        #endregion
        int cellPosition = -1;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            cellPosition = e.RowIndex; // 選取位置
            Console.WriteLine("e.RowIndex:" + cellPosition.ToString());
            try
            {
                groupCb.Text = dataGridView1["C1", cellPosition].Value.ToString();
                CBox_Type.Text = dataGridView1["C8", cellPosition].Value.ToString();
                CBox_Behavior.Text = dataGridView1["C2", cellPosition].Value.ToString();
                LoopText.Text = dataGridView1["C7", cellPosition].Value.ToString();
            }
            catch (Exception)
            {
            }

        }

        List<string> functionProtectedList = new List<string> { Behavior.AC_OnOff, Behavior.WaitTime, Behavior.Resolution_Check, Behavior.Screen_Check, Behavior.PM_Special_ColdBoot, Behavior.PM_Special_WarmBoot, Behavior.PM_Special_Hibernation, Behavior.ErrorMessage, Behavior.HDCP_Check, Behavior.Movie_Play_Check, Behavior.Audio_Check, Behavior.LED_Check,Behavior.Opening_Check };
        private void button2_Click(object sender, EventArgs e)
        {
            string Behavior = "";
            string BehaviorText = "";
            try
            {
                dll_PublicFuntion.Other.ComboboxItem Behavior_item = CBox_Behavior.Items[CBox_Behavior.SelectedIndex] as dll_PublicFuntion.Other.ComboboxItem;
                Behavior = Behavior_item.Value;
                BehaviorText = Behavior_item.Text;
            }
            catch (Exception)
            {
                Behavior = "";
            }
            if (groupCb.Text != "")
            {
                if (Behavior != "")
                {
                    if (ParameterPretended(Behavior))
                    {
                        rowindex = dataGridView1.Rows.Add();
                        editFunction(Behavior, rowindex, BehaviorText, false);
                    }
                }
                else
                {
                    MessageBox.Show("Select a behavior please.");
                }
            }
            else
            {
                MessageBox.Show("Create a group please.");
            }
        }
        private void Editbtn_Click(object sender, EventArgs e)
        {
            int rowindex1 = cellPosition;
            if (groupCb.Text != "")
            {
                string Behavior = "";
                string BehaviorText = "";
                try
                {
                    dll_PublicFuntion.Other.ComboboxItem Behavior_item = CBox_Behavior.Items[CBox_Behavior.SelectedIndex] as dll_PublicFuntion.Other.ComboboxItem;
                    Behavior = Behavior_item.Value;
                    BehaviorText = Behavior_item.Text;
                }
                catch (Exception)
                {
                    Behavior = "";
                }

                if (Behavior != "")
                {
                    //rowindex1 = dataGridView1.Rows.Add();
                    if (ParameterPretended(Behavior))
                    {
                        // 清空所有值
                        for (int i = 1; i <= 8; i++)
                            dataGridView1[$"C{i}", rowindex1].Value = "";
                        editFunction(Behavior, rowindex1, BehaviorText, false);
                    }
                }
                else
                {
                    MessageBox.Show("Select Behavior please.");
                }
            }
        }
        private void button7_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = dll_PublicFuntion.Folder.Check_path(@"VisualIdentification\Data\ImageData");
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string path = openFileDialog1.FileName;
                    HDCP_PathText.Text = path;
                    string[] txtName = openFileDialog1.FileName.Split(new[] { "\\" }, StringSplitOptions.None);
                    string Nametxt = txtName[txtName.Length - 2];
                    HDCP_NameText.Text = Nametxt.Trim();
                }
            }
        }
        private void button8_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = dll_PublicFuntion.Folder.Check_path(@"VisualIdentification\Data\ImageData");
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string path = openFileDialog1.FileName;
                    MoviePathText.Text = path;
                    string[] txtName = openFileDialog1.FileName.Split(new[] { "\\" }, StringSplitOptions.None);
                    string Nametxt = txtName[txtName.Length - 2];
                    MovieNameText.Text = Nametxt.Trim();
                }
            }
        }
        private void button10_Click_1(object sender, EventArgs e)
        {
            int rowIndex = cellPosition;
            Console.WriteLine($"rowIndex:{rowIndex}");
            string Behavior = "";
            string BehaviorText = "";
            try
            {
                dll_PublicFuntion.Other.ComboboxItem Behavior_item = CBox_Behavior.Items[CBox_Behavior.SelectedIndex] as dll_PublicFuntion.Other.ComboboxItem;
                Behavior = Behavior_item.Value;
                BehaviorText = Behavior_item.Text;
            }
            catch (Exception)
            {
                Behavior = "";
            }
            if (groupCb.Text != "")
            {
                if (Behavior != "")
                {
                    if (ParameterPretended(Behavior))
                    {
                        dataGridView1.Rows.Insert(rowIndex, new DataGridViewRow());
                        editFunction(Behavior, rowIndex, BehaviorText, false);
                    }
                }
                else
                {
                    MessageBox.Show("Select a behavior please.");
                }
            }
            else
            {
                MessageBox.Show("Create a group please.");
            }
        }
        private void editFunction(string Behavior_, int rowindex1, string BehaviorText, bool replaceFlag)
        {
            if (replaceFlag != true)
            {
                dataGridView1["C1", rowindex1].Value = groupCb.Text;
                if (Yes.Checked)
                    dataGridView1["C3", rowindex1].Value = "Yes";
                else if (No.Checked)
                    dataGridView1["C3", rowindex1].Value = "No";
            }
            dataGridView1["C2", rowindex1].Value = BehaviorText;
            dataGridView1["C4", rowindex1].Value = Behavior_;
            string C5 = "";
            if (Behavior_ == Behavior.AC_OnOff)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                    new Dictionary<string, object> {
                         { "Ip",$"{textBox4.Text}"},
                         { "On",$"{textBox1.Text}" },
                         { "Off",$"{textBox2.Text}" }
                    });
            }
            else if (Behavior_ == Behavior.VA_Check)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                    new Dictionary<string, object> {
                          { "V_Max",$"{numericVMax.Value}"},
                          { "V_Min",$"{numericVMin.Value}"},
                          { "A_Pass",$"{numericAPass.Value}"},
                          { "A_Fail",$"{numericAFail.Value}"},
                    });
            }
            else if (Behavior_ == Behavior.WaitTime)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                new Dictionary<string, object> {
                                { "Wait",$"{WaitBox.Text}"},
                });
            }
            else if (Behavior_ == Behavior.Resolution_Check)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                    new Dictionary<string, object> {
                                { "startX",$"{sX.Text}"},
                                { "startY",$"{sY.Text}"},
                                { "ImgWidth",$"{eX.Text}"},
                                { "ImgHeight",$"{eY.Text}"},
                                { "DataFile",$"{filenameText.Text}"},
                    });
            }
            else if (Behavior_ == Behavior.LED_Check)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                    new Dictionary<string, object> {
                                { "startX",$"{startX.Text}"},
                                { "startY",$"{startY.Text}"},
                                { "ImgWidth",$"{endX.Text}"},
                                { "ImgHeight",$"{endY.Text}"},
                                { "DataFile",$"{LEDName.Text}"},
                    });
            }
            else if (Behavior_ == Behavior.Screen_Check)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                     new Dictionary<string, object> {
                                { "IconName",IconName.Text},
                                { "PicturePath",DvdPicPath.Text},
                         });
            }
            else if (Behavior_ == Behavior.PM_Special_ColdBoot)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                     new Dictionary<string, object> {
                                { "PictureName1",CPic1Text.Text},
                                { "PictureName2",CPic2Text.Text},
                                { "PictureName3",CPic3Text.Text},
                         });
            }
            else if (Behavior_ == Behavior.PM_Special_WarmBoot)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                     new Dictionary<string, object> {
                                { "PictureName1",WPic1Text.Text},
                                { "PictureName2",WPic2Text.Text},
                                { "PictureName3",WPic3Text.Text},
                         });
            }
            else if (Behavior_ == Behavior.PM_Special_Hibernation)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                     new Dictionary<string, object> {
                                { "PictureName1",HPic1Text.Text},
                                { "PictureName2",HPic2Text.Text},
                                { "PictureName3",HPic3Text.Text},
                         });
            }
            else if (Behavior_ == Behavior.ErrorMessage)
            {

                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                     new Dictionary<string, object> {
                                { "PictureName",pictureName.Text},
                         });
            }
            else if (Behavior_ == Behavior.HDCP_Check)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                     new Dictionary<string, object> {
                                { "HDCP_PictureName",HDCP_PicNameText.Text},
                                { "HDCP_IconName",HDCP_NameText.Text},
                                { "PicturePath",HDCP_PathText.Text}
                         });
            }
            else if (Behavior_ == Behavior.Movie_Play_Check)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                     new Dictionary<string, object> {
                                { "IconName",MovieNameText.Text},
                                { "PicturePath",MoviePathText.Text},
                                { "Wait",txt_mpcWait.Text}
                         });
            }
            else if (Behavior_ == Behavior.Drag_DVD)
            {
                if (LR.Checked)
                {
                    C5 = dll_PublicFuntion.Other.DictionaryToXml(
                     new Dictionary<string, object> {
                                { "Direction", "LeftToRight"},
                         });
                }
                if (RL.Checked)
                {
                    C5 = dll_PublicFuntion.Other.DictionaryToXml(
                     new Dictionary<string, object> {
                                { "Direction", "RightToLeft"},
                         });
                }
            }
            else if (Behavior_ == Behavior.Audio_Check)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                     new Dictionary<string, object> {
                                { "Audio_Name", audioText.Text},
                         });
            }
            else if (Behavior_ == Behavior.Opening_Check)
            {
                C5 = dll_PublicFuntion.Other.DictionaryToXml(
                     new Dictionary<string, object> {
                                { "IconName",txt_openIcon.Text},
                                { "PicturePath",txt_openPath.Text},
                         });
            }
            dataGridView1["C5", rowindex1].Value = C5;
            try
            {
                dataGridView1["C6", rowindex1].Value = dll_PublicFuntion.Other.DictionaryToString(dll_PublicFuntion.Other.XmlToDictionary(C5));
            }
            catch (Exception)
            {
            }
            string GroupName = dataGridView1["C1", rowindex1].Value.ToString();
            Console.WriteLine("GroupName:" + GroupName);

            if (replaceFlag != true)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (GroupName == dataGridView1["C1", i].Value.ToString())
                    {
                        dataGridView1["C7", i].Value = LoopText.Text;
                    }
                }
            }

            dataGridView1["C8", rowindex1].Value = CBox_Type.Text;
        }

        /// <summary>
        /// 參數防呆
        /// </summary>
        /// <param name="Behavior">功能</param>
        private Boolean ParameterPretended(string Behavior)
        {

            if (Behavior == functionProtectedList[0])
            {
                if (string.IsNullOrEmpty(textBox4.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }

            }
            else if (Behavior == functionProtectedList[1])
            {
                if (string.IsNullOrEmpty(WaitBox.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }
            }
            else if (Behavior == functionProtectedList[2])
            {
                if (string.IsNullOrEmpty(sX.Text) || string.IsNullOrEmpty(sY.Text) || string.IsNullOrEmpty(eX.Text) || string.IsNullOrEmpty(eY.Text) || string.IsNullOrEmpty(filenameText.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }
            }
            else if (Behavior == functionProtectedList[3])
            {
                if (string.IsNullOrEmpty(IconName.Text) || string.IsNullOrEmpty(DvdPicPath.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }
            }
            else if (Behavior == functionProtectedList[4])
            {
                if (string.IsNullOrEmpty(CPic1Text.Text) || string.IsNullOrEmpty(CPic2Text.Text) || string.IsNullOrEmpty(CPic3Text.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }
            }
            else if (Behavior == functionProtectedList[5])
            {
                if (string.IsNullOrEmpty(WPic1Text.Text) || string.IsNullOrEmpty(WPic2Text.Text) || string.IsNullOrEmpty(WPic3Text.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }
            }
            else if (Behavior == functionProtectedList[6])
            {
                if (string.IsNullOrEmpty(HPic1Text.Text) || string.IsNullOrEmpty(HPic2Text.Text) || string.IsNullOrEmpty(HPic3Text.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }
            }
            else if (Behavior == functionProtectedList[7])
            {
                if (string.IsNullOrEmpty(pictureName.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }
            }
            else if (Behavior == functionProtectedList[8])
            {
                if (string.IsNullOrEmpty(HDCP_PicNameText.Text) || string.IsNullOrEmpty(HDCP_NameText.Text) || string.IsNullOrEmpty(HDCP_PathText.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }
            }
            else if (Behavior == functionProtectedList[9])
            {
                if (string.IsNullOrEmpty(MovieNameText.Text) || string.IsNullOrEmpty(MoviePathText.Text) || string.IsNullOrEmpty(txt_mpcWait.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }
            }
            else if (Behavior == functionProtectedList[10])
            {
                if (string.IsNullOrEmpty(audioText.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }
            }
            else if (Behavior == functionProtectedList[11])
            {
                if (string.IsNullOrEmpty(startX.Text) || string.IsNullOrEmpty(startY.Text) || string.IsNullOrEmpty(endX.Text) || string.IsNullOrEmpty(endY.Text) || string.IsNullOrEmpty(LEDName.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }
            }
            else if (Behavior == functionProtectedList[12])
            {
                if (string.IsNullOrEmpty(txt_openPath.Text) || string.IsNullOrEmpty(txt_openIcon.Text))
                {
                    MessageBox.Show("Parameters error.");
                    return false;
                }
            }
            return true;
        }

        List<int> searchList = new List<int>();
        private void searchBtn_Click(object sender, EventArgs e)
        {
            searchList.Clear();
            bool firstFlag = true;
            if (!string.IsNullOrEmpty(searchText.Text))
            {
                try
                {
                    // 取得總列數
                    int row = dataGridView1.Rows.Count;
                    Console.WriteLine($"Im row:{row}");
                    // 欄位
                    int cell = dataGridView1.Rows[0].Cells.Count;
                    Console.WriteLine($"Im cell:{cell}");
                    // 搜尋內容
                    string strTxt = searchText.Text;
                    //Regex r = new Regex(strTxt);
                    bool searchFlag = false;
                    for (int i = 0; i < row; i++)
                    {
                        for (int j = 0; j < cell; j++)
                        {
                            //Match m = Regex.IsMatch(dataGridView1.Rows[i].Cells[j].Value.ToString(), $@"*{strTxt}*");
                            if (Regex.IsMatch($"{dataGridView1.Rows[i].Cells[j].Value}", $@"/*{strTxt}/*"))
                            {
                                if (dataGridView1.Rows[i].Cells[j].Visible == false)
                                {
                                    continue;
                                }
                                dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                                // 上下搜尋需要用到的清單
                                if (!searchList.Contains(i))
                                {
                                    searchList.Add(i);
                                }
                                if (firstFlag)
                                {
                                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                                    firstFlag = false;
                                }
                                searchFlag = true;
                            }
                        }
                    }
                    if (searchFlag)
                    {
                        searchLabel.Text = "查詢成功!";
                        searchLabel.ForeColor = Color.Green;
                    }
                    else
                    {
                        searchLabel.Text = "查無結果...";
                        searchLabel.ForeColor = Color.Red;
                    }
                }
                catch (Exception)
                {
                }

            }
        }

        private void searchText_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
            searchLabel.Text = "";
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            if (searchList.Count > 0)
            {
                //// 外循環是循環的次數
                //for (int i = 0; i < searchList.Count; i++)
                //{
                //    // 内循環是 外循環一次比較的次數
                //    for (int j = searchList.Count - 1; j > i; j--)
                //    {
                //        if (searchList[i] == searchList[j])
                //        {
                //            searchList.RemoveAt(j);
                //        }
                //    }
                //}

                Console.WriteLine("-----SearchList-----");
                foreach (var item in searchList)
                {
                    Console.WriteLine(item.ToString());
                }
                Console.WriteLine("-----SearchList end-----");

                int firstPosition = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                int ListPosition = searchList.IndexOf(firstPosition);
                Console.WriteLine($"firstPosition:{firstPosition} ListPosition:{ListPosition}");
                if (ListPosition == searchList.Count - 1)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[searchList[0]].Cells[0];
                }
                else
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[searchList[ListPosition + 1]].Cells[0];
                }
            }
        }

        private void LastBtn_Click(object sender, EventArgs e)
        {
            if (searchList.Count > 0)
            {
                int firstPosition = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                int ListPosition = searchList.IndexOf(firstPosition);
                Console.WriteLine($"firstPosition:{firstPosition} ListPosition:{ListPosition}");
                if (ListPosition == 0)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[searchList[searchList.Count - 1]].Cells[0];
                }
                else
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[searchList[ListPosition - 1]].Cells[0];
                }
            }
        }
        private void ReplaceBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
            List<int> allLine = new List<int>();
            try
            {
                // 取得總列數
                int row = dataGridView1.Rows.Count;
                // 欄位
                int cell = dataGridView1.Rows[0].Cells.Count;
                Console.WriteLine($"Im row:{row} / Im cell:{cell}");
                // 搜尋內容
                string strTxt = CBox_Behavior.Text;
                //Regex r = new Regex(strTxt);

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < cell; j++)
                    {
                        //Match m = Regex.IsMatch(dataGridView1.Rows[i].Cells[j].Value.ToString(), $@"*{strTxt}*");
                        if ($"{dataGridView1.Rows[i].Cells[1].Value}" == strTxt)
                        {
                            if (!allLine.Contains(i))
                            {
                                allLine.Add(i);
                            }
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightCyan;
                        }
                    }
                }

                if (groupCb.Text != "")
                {
                    string Behavior = "";
                    string BehaviorText = "";
                    try
                    {
                        dll_PublicFuntion.Other.ComboboxItem Behavior_item = CBox_Behavior.Items[CBox_Behavior.SelectedIndex] as dll_PublicFuntion.Other.ComboboxItem;
                        Behavior = Behavior_item.Value;
                        BehaviorText = Behavior_item.Text;
                    }
                    catch (Exception)
                    {
                        Behavior = "";
                    }

                    if (Behavior != "")
                    {
                        //rowindex1 = dataGridView1.Rows.Add();
                        if (ParameterPretended(Behavior))
                        {
                            // 清空所有值
                            for (int i = 1; i <= 8; i++)
                            {
                                if (i != 1 && i != 3 && i != 7)
                                {
                                    foreach (var item in allLine)
                                    {
                                        dataGridView1[$"C{i}", item].Value = "";
                                        editFunction(Behavior, item, BehaviorText, true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Select Behavior please.");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        List<int> replaceLine = new List<int>();
        private void resetBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }

            if (resetBtn.Text == "Set")
            {
                if (!string.IsNullOrEmpty(CBox_Behavior.Text))
                {
                    Editbtn.Enabled = false; button2.Enabled = false; saveBtn.Enabled = false;
                    resetBtn.Text = "Replace";
                    try
                    {
                        // 取得總列數
                        int row = dataGridView1.Rows.Count;
                        // 欄位
                        int cell = dataGridView1.Rows[0].Cells.Count;
                        Console.WriteLine($"Im row:{row} / Im cell:{cell}");
                        // 搜尋內容
                        string strTxt = CBox_Behavior.Text;
                        //Regex r = new Regex(strTxt);

                        for (int i = 0; i < row; i++)
                        {
                            for (int j = 0; j < cell; j++)
                            {
                                //Match m = Regex.IsMatch(dataGridView1.Rows[i].Cells[j].Value.ToString(), $@"*{strTxt}*");
                                if ($"{dataGridView1.Rows[i].Cells[1].Value}" == strTxt)
                                {
                                    if (!replaceLine.Contains(i))
                                    {
                                        replaceLine.Add(i);
                                    }
                                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightYellow;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    MessageBox.Show("Please choose a step.");
                }
            }
            else if (resetBtn.Text == "Replace")
            {
                try
                {
                    string Behavior = "";
                    string BehaviorText = "";
                    if (string.IsNullOrEmpty(Behavior))
                    {
                        try
                        {
                            dll_PublicFuntion.Other.ComboboxItem Behavior_item = CBox_Behavior.Items[CBox_Behavior.SelectedIndex] as dll_PublicFuntion.Other.ComboboxItem;
                            Behavior = Behavior_item.Value;
                            BehaviorText = Behavior_item.Text;
                        }
                        catch (Exception)
                        {
                            Behavior = "";
                        }
                        if (ParameterPretended(Behavior))
                        {
                            // 清空所有值
                            for (int i = 1; i <= 8; i++)
                            {
                                if (i != 1 && i != 3 && i != 7)
                                {
                                    foreach (var item in replaceLine)
                                    {
                                        dataGridView1[$"C{i}", item].Value = "";
                                        editFunction(Behavior, item, BehaviorText, true);
                                        dataGridView1.Rows[item].DefaultCellStyle.BackColor = Color.LightGreen;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }


                Editbtn.Enabled = true; button2.Enabled = true; saveBtn.Enabled = true;

                resetBtn.Text = "Set";
                replaceLine.Clear();

            }
        }

        private void LedBtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.InitialDirectory = dll_PublicFuntion.Folder.Check_path(@"IDSData\LedData");

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string Openpath = openFileDialog1.FileName;
                StreamReader sr = new StreamReader(Openpath);
                string[] ParameterList = sr.ReadToEnd().Split('\n');
                if (!string.IsNullOrEmpty(ParameterList[0]))//server ip
                    startX.Text = ParameterList[0].Trim();
                if (!string.IsNullOrEmpty(ParameterList[1]))
                    startY.Text = ParameterList[1].Trim();
                if (!string.IsNullOrEmpty(ParameterList[2]))
                    endX.Text = ParameterList[2].Trim();
                if (!string.IsNullOrEmpty(ParameterList[2]))
                    endY.Text = ParameterList[3].Trim();
                string[] txtName = openFileDialog1.FileName.Split(new[] { "\\" }, StringSplitOptions.None);
                string Nametxt = txtName[txtName.Length - 1].Replace(".txt", "");
                LEDName.Text = Nametxt.Trim();
                sr.Close();
            }
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = dll_PublicFuntion.Folder.Check_path(@"VisualIdentification\Data\ImageData");
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string path = openFileDialog1.FileName;
                    txt_openPath.Text = path;
                    string[] txtName = openFileDialog1.FileName.Split(new[] { "\\" }, StringSplitOptions.None);
                    string Nametxt = txtName[txtName.Length - 2];
                    txt_openIcon.Text = Nametxt.Trim();
                }
            }
        }
    }
}
