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
    public partial class Script_binding : Form
    {
        public Script_binding()
        {
            InitializeComponent();
        }
        string path = "";
        string[] datapath = new string[] { "", "" };

        Dictionary<string, int> GroupListLoop = new Dictionary<string, int>();
        private void s1btn_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "(*.xml)|*.xml|(*.json)|*.json";
            openFileDialog1.InitialDirectory = dll_PublicFuntion.Folder.Check_path($@"Script\");
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName;
                datapath[0] = path;
                Console.WriteLine("path1:" + datapath[0]);
                ScriptInit(false, treeView1);
            }
        }
        private void s2btn_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "(*.xml)|*.xml|(*.json)|*.json";
            openFileDialog1.InitialDirectory = dll_PublicFuntion.Folder.Check_path($@"Script\");
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName;
                datapath[1] = path;
                Console.WriteLine("path2:" + datapath[1]);
                ScriptInit(false, treeView2);
            }
        }
        private void ScriptInit(Boolean Show, TreeView treeView)
        {
            if (!Show)
            {
                treeView.Nodes.Clear();
                treeView.ShowLines = false;
            }
            if (File.Exists(path))
            {
                Script script = new Script();
                script.dataGridView1.Rows.Clear();
                dll_PublicFuntion.DGV.XmlLoadDGV(path, script.dataGridView1, "Script");  //讀取XML到DATAVIEW
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
                    string groupList = $"{v.Cells["C1"].Value}"; // 新增group到groupCb
                    if (!vs.Contains(groupList))
                    {
                        script.groupCb.Items.Add(groupList);
                        //LogAdd("ScriptInit", $"groupList {groupList}", false);//Console.WriteLine($"groupList {groupList}");
                        vs.Add(groupList);
                    }
                }
                if (Show)
                {
                    script.JsonName.Text = (Path.GetFileName($@"{path}")).Replace(".xml", ""); //檔名
                    script.ShowDialog();
                    string FilePath = dll_PublicFuntion.Folder.Check_path(@"Script\") + script.JsonName.Text + ".xml";
                    if (File.Exists(FilePath))
                    {
                        path = FilePath;
                        ScriptInit(false, treeView);
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
                        treeView.Nodes.Add(GroupNode);
                    }

                    treeView.ExpandAll();
                }
            }
            else
            {
                MessageBox.Show("No script.");
            }
        }

        Script script = new Script();
        private void bindbtn_Click(object sender, EventArgs e)
        {
            script.dataGridView1.Rows.Clear();
            
            foreach (string file in datapath)
            {
                Script _script = new Script();
                _script.dataGridView1.Rows.Clear();
                dll_PublicFuntion.DGV.XmlLoadDGV(file, _script.dataGridView1, "Script");  //讀取XML到DATAVIEW
                foreach (DataGridViewRow v in _script.dataGridView1.Rows)
                {
                    int RowIndex = script.dataGridView1.Rows.Add();
                    for (int x = 0; x < script.dataGridView1.Columns.Count; x++)
                    {
                        script.dataGridView1[script.dataGridView1.Columns[x].Name, RowIndex].Value = v.Cells[script.dataGridView1.Columns[x].Name].Value;
                    }
                }
            }
            dll_PublicFuntion.DGV.DGVSaveXml(Application.StartupPath + @"\BindLog.xml", script.dataGridView1, "Script");
            path = Application.StartupPath + @"\BindLog.xml";
            ScriptInit(false, treeView3);
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            string FileNameName = fileNametext.Text;
            string FilePath = dll_PublicFuntion.Folder.Check_path(@"Script\") + FileNameName + ".xml";

            if (!string.IsNullOrEmpty(FileNameName))
            {
                if (File.Exists(FilePath))
                {
                    if (MessageBox.Show("This file name has been used!" + Environment.NewLine + "Do you want to cover it?", "Reminds", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (!dll_PublicFuntion.DGV.DGVXmlCheck(FilePath, script.dataGridView1, "Script"))
                        {
                            if (MessageBox.Show("Are you sure?", "Caveat", MessageBoxButtons.YesNo, MessageBoxIcon.None) != DialogResult.Yes)
                            {
                                MessageBox.Show("Save fail");
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
                dll_PublicFuntion.DGV.DGVSaveXml(FilePath, script.dataGridView1, "Script"); //DATAVIEW寫入XML
                MessageBox.Show("Save success");
            }
            else
                MessageBox.Show("File name is empty!");   
        }
    }
}
