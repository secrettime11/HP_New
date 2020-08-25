using Sunisoft.IrisSkin;

namespace HP_Display
{
    partial class Main
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btn_load = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.scriptEditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptBindingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runBtn = new System.Windows.Forms.Button();
            this.editBtn = new System.Windows.Forms.Button();
            this.ip1 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RLoadBtn = new System.Windows.Forms.Button();
            this.Timer_Status = new System.Windows.Forms.Timer(this.components);
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.WebCamCb = new System.Windows.Forms.ComboBox();
            this.TBoxAllLog = new System.Windows.Forms.TextBox();
            this.LogBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.IdsCb = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.FileText = new System.Windows.Forms.TextBox();
            this.PaLoad = new System.Windows.Forms.Button();
            this.PaSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.DUTIDText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ACIPText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.IDSbtn = new System.Windows.Forms.Button();
            this.DAQbtn = new System.Windows.Forms.Button();
            this.TxtOPen = new System.Windows.Forms.OpenFileDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Excelbtn = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.Timelb = new System.Windows.Forms.Label();
            this.Stopbtn = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine();
            this.txt_loop = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_nowloop = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(93, 124);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(279, 254);
            this.treeView1.TabIndex = 5;
            // 
            // btn_load
            // 
            this.btn_load.Location = new System.Drawing.Point(8, 155);
            this.btn_load.Name = "btn_load";
            this.btn_load.Size = new System.Drawing.Size(75, 28);
            this.btn_load.TabIndex = 6;
            this.btn_load.Text = "Load";
            this.btn_load.UseVisualStyleBackColor = true;
            this.btn_load.Click += new System.EventHandler(this.btn_load_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scriptEditToolStripMenuItem,
            this.scriptBindingToolStripMenuItem,
            this.configToolStripMenuItem,
            this.restartToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1135, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // scriptEditToolStripMenuItem
            // 
            this.scriptEditToolStripMenuItem.Name = "scriptEditToolStripMenuItem";
            this.scriptEditToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.scriptEditToolStripMenuItem.Text = "New Script";
            this.scriptEditToolStripMenuItem.Click += new System.EventHandler(this.scriptEditToolStripMenuItem_Click);
            // 
            // scriptBindingToolStripMenuItem
            // 
            this.scriptBindingToolStripMenuItem.Name = "scriptBindingToolStripMenuItem";
            this.scriptBindingToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.scriptBindingToolStripMenuItem.Text = "Script binding";
            this.scriptBindingToolStripMenuItem.Click += new System.EventHandler(this.scriptBindingToolStripMenuItem_Click);
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.configToolStripMenuItem.Text = "Config";
            this.configToolStripMenuItem.Click += new System.EventHandler(this.configToolStripMenuItem_Click);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.restartToolStripMenuItem.Text = "Restart";
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.restartToolStripMenuItem_Click);
            // 
            // runBtn
            // 
            this.runBtn.Location = new System.Drawing.Point(8, 33);
            this.runBtn.Name = "runBtn";
            this.runBtn.Size = new System.Drawing.Size(75, 59);
            this.runBtn.TabIndex = 8;
            this.runBtn.Text = "Run";
            this.runBtn.UseVisualStyleBackColor = true;
            this.runBtn.Click += new System.EventHandler(this.runBtn_Click);
            // 
            // editBtn
            // 
            this.editBtn.Location = new System.Drawing.Point(8, 225);
            this.editBtn.Name = "editBtn";
            this.editBtn.Size = new System.Drawing.Size(75, 28);
            this.editBtn.TabIndex = 9;
            this.editBtn.Text = "Edit";
            this.editBtn.UseVisualStyleBackColor = true;
            this.editBtn.Click += new System.EventHandler(this.editBtn_Click);
            // 
            // ip1
            // 
            this.ip1.Font = new System.Drawing.Font("新細明體", 15F);
            this.ip1.Location = new System.Drawing.Point(49, 17);
            this.ip1.Name = "ip1";
            this.ip1.Size = new System.Drawing.Size(177, 31);
            this.ip1.TabIndex = 0;
            this.ip1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ip1);
            this.groupBox1.Location = new System.Drawing.Point(93, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(279, 59);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IP";
            // 
            // RLoadBtn
            // 
            this.RLoadBtn.Location = new System.Drawing.Point(8, 190);
            this.RLoadBtn.Name = "RLoadBtn";
            this.RLoadBtn.Size = new System.Drawing.Size(75, 28);
            this.RLoadBtn.TabIndex = 18;
            this.RLoadBtn.Text = "Reload";
            this.RLoadBtn.UseVisualStyleBackColor = true;
            this.RLoadBtn.Click += new System.EventHandler(this.RLoadBtn_Click);
            // 
            // Timer_Status
            // 
            this.Timer_Status.Interval = 1000;
            this.Timer_Status.Tick += new System.EventHandler(this.Timer_Status_Tick);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.WebCamCb);
            this.groupBox6.Controls.Add(this.TBoxAllLog);
            this.groupBox6.Location = new System.Drawing.Point(0, 393);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(1121, 297);
            this.groupBox6.TabIndex = 68;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "ServiceLog";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(896, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 20);
            this.label6.TabIndex = 83;
            this.label6.Text = "WebCam";
            this.label6.Visible = false;
            // 
            // WebCamCb
            // 
            this.WebCamCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WebCamCb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.WebCamCb.FormattingEnabled = true;
            this.WebCamCb.Location = new System.Drawing.Point(984, 27);
            this.WebCamCb.Name = "WebCamCb";
            this.WebCamCb.Size = new System.Drawing.Size(124, 20);
            this.WebCamCb.TabIndex = 82;
            this.WebCamCb.Visible = false;
            // 
            // TBoxAllLog
            // 
            this.TBoxAllLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.TBoxAllLog.Location = new System.Drawing.Point(13, 19);
            this.TBoxAllLog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TBoxAllLog.Multiline = true;
            this.TBoxAllLog.Name = "TBoxAllLog";
            this.TBoxAllLog.Size = new System.Drawing.Size(1101, 270);
            this.TBoxAllLog.TabIndex = 20;
            // 
            // LogBox
            // 
            this.LogBox.Font = new System.Drawing.Font("Consolas", 9F);
            this.LogBox.Location = new System.Drawing.Point(662, 33);
            this.LogBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LogBox.Multiline = true;
            this.LogBox.Name = "LogBox";
            this.LogBox.Size = new System.Drawing.Size(459, 345);
            this.LogBox.TabIndex = 21;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.IdsCb);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.FileText);
            this.groupBox2.Controls.Add(this.PaLoad);
            this.groupBox2.Controls.Add(this.PaSave);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.comboBox1);
            this.groupBox2.Controls.Add(this.DUTIDText);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.ACIPText);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(380, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(275, 275);
            this.groupBox2.TabIndex = 69;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Information";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(54, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 20);
            this.label5.TabIndex = 81;
            this.label5.Text = "IDS";
            // 
            // IdsCb
            // 
            this.IdsCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IdsCb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IdsCb.FormattingEnabled = true;
            this.IdsCb.Location = new System.Drawing.Point(107, 144);
            this.IdsCb.Name = "IdsCb";
            this.IdsCb.Size = new System.Drawing.Size(124, 20);
            this.IdsCb.TabIndex = 80;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(56, 182);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 20);
            this.label4.TabIndex = 79;
            this.label4.Text = "File";
            // 
            // FileText
            // 
            this.FileText.Location = new System.Drawing.Point(107, 180);
            this.FileText.Name = "FileText";
            this.FileText.Size = new System.Drawing.Size(124, 22);
            this.FileText.TabIndex = 78;
            this.FileText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // PaLoad
            // 
            this.PaLoad.Location = new System.Drawing.Point(153, 215);
            this.PaLoad.Name = "PaLoad";
            this.PaLoad.Size = new System.Drawing.Size(78, 23);
            this.PaLoad.TabIndex = 77;
            this.PaLoad.Text = "Load";
            this.PaLoad.UseVisualStyleBackColor = true;
            this.PaLoad.Click += new System.EventHandler(this.PaLoad_Click);
            // 
            // PaSave
            // 
            this.PaSave.Location = new System.Drawing.Point(55, 215);
            this.PaSave.Name = "PaSave";
            this.PaSave.Size = new System.Drawing.Size(75, 23);
            this.PaSave.TabIndex = 70;
            this.PaSave.Text = "Save";
            this.PaSave.UseVisualStyleBackColor = true;
            this.PaSave.Click += new System.EventHandler(this.PaSave_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(48, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 20);
            this.label3.TabIndex = 76;
            this.label3.Text = "COM";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(107, 112);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(124, 20);
            this.comboBox1.TabIndex = 75;
            // 
            // DUTIDText
            // 
            this.DUTIDText.Location = new System.Drawing.Point(107, 75);
            this.DUTIDText.Name = "DUTIDText";
            this.DUTIDText.Size = new System.Drawing.Size(124, 22);
            this.DUTIDText.TabIndex = 71;
            this.DUTIDText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(31, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 20);
            this.label2.TabIndex = 72;
            this.label2.Text = "DUT ID";
            // 
            // ACIPText
            // 
            this.ACIPText.Location = new System.Drawing.Point(107, 39);
            this.ACIPText.Name = "ACIPText";
            this.ACIPText.Size = new System.Drawing.Size(124, 22);
            this.ACIPText.TabIndex = 2;
            this.ACIPText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(43, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 70;
            this.label1.Text = "AC IP";
            // 
            // IDSbtn
            // 
            this.IDSbtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IDSbtn.Location = new System.Drawing.Point(17, 20);
            this.IDSbtn.Name = "IDSbtn";
            this.IDSbtn.Size = new System.Drawing.Size(113, 28);
            this.IDSbtn.TabIndex = 70;
            this.IDSbtn.Text = "IDS";
            this.IDSbtn.UseVisualStyleBackColor = true;
            this.IDSbtn.Click += new System.EventHandler(this.IDSbtn_Click);
            // 
            // DAQbtn
            // 
            this.DAQbtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DAQbtn.Location = new System.Drawing.Point(153, 20);
            this.DAQbtn.Name = "DAQbtn";
            this.DAQbtn.Size = new System.Drawing.Size(99, 28);
            this.DAQbtn.TabIndex = 73;
            this.DAQbtn.Text = "DAQ Jig";
            this.DAQbtn.UseVisualStyleBackColor = true;
            this.DAQbtn.Click += new System.EventHandler(this.DAQbtn_Click);
            // 
            // TxtOPen
            // 
            this.TxtOPen.FileName = "openFileDialog2";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.IDSbtn);
            this.groupBox3.Controls.Add(this.DAQbtn);
            this.groupBox3.Location = new System.Drawing.Point(380, 33);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(275, 59);
            this.groupBox3.TabIndex = 84;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "External devices";
            // 
            // Excelbtn
            // 
            this.Excelbtn.Location = new System.Drawing.Point(8, 333);
            this.Excelbtn.Name = "Excelbtn";
            this.Excelbtn.Size = new System.Drawing.Size(75, 45);
            this.Excelbtn.TabIndex = 85;
            this.Excelbtn.Text = "Open Excel";
            this.Excelbtn.UseVisualStyleBackColor = true;
            this.Excelbtn.Click += new System.EventHandler(this.Excelbtn_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.Timelb);
            this.groupBox5.Location = new System.Drawing.Point(8, 261);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(75, 55);
            this.groupBox5.TabIndex = 86;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Timer";
            // 
            // Timelb
            // 
            this.Timelb.AutoSize = true;
            this.Timelb.Font = new System.Drawing.Font("新細明體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Timelb.Location = new System.Drawing.Point(26, 20);
            this.Timelb.Name = "Timelb";
            this.Timelb.Size = new System.Drawing.Size(22, 24);
            this.Timelb.TabIndex = 0;
            this.Timelb.Text = "0";
            // 
            // Stopbtn
            // 
            this.Stopbtn.Location = new System.Drawing.Point(8, 103);
            this.Stopbtn.Name = "Stopbtn";
            this.Stopbtn.Size = new System.Drawing.Size(75, 46);
            this.Stopbtn.TabIndex = 87;
            this.Stopbtn.Text = "Stop";
            this.Stopbtn.UseVisualStyleBackColor = true;
            this.Stopbtn.Click += new System.EventHandler(this.Stopbtn_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // skinEngine1
            // 
            this.skinEngine1.@__DrawButtonFocusRectangle = true;
            this.skinEngine1.DisabledButtonTextColor = System.Drawing.Color.Gray;
            this.skinEngine1.DisabledMenuFontColor = System.Drawing.SystemColors.GrayText;
            this.skinEngine1.InactiveCaptionColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.skinEngine1.SerialNumber = "";
            this.skinEngine1.SkinFile = null;
            // 
            // txt_loop
            // 
            this.txt_loop.Location = new System.Drawing.Point(173, 98);
            this.txt_loop.Name = "txt_loop";
            this.txt_loop.Size = new System.Drawing.Size(49, 22);
            this.txt_loop.TabIndex = 84;
            this.txt_loop.Text = "1";
            this.txt_loop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(114, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 20);
            this.label7.TabIndex = 85;
            this.label7.Text = "Loop";
            // 
            // txt_nowloop
            // 
            this.txt_nowloop.Location = new System.Drawing.Point(285, 98);
            this.txt_nowloop.Name = "txt_nowloop";
            this.txt_nowloop.ReadOnly = true;
            this.txt_nowloop.Size = new System.Drawing.Size(40, 22);
            this.txt_nowloop.TabIndex = 88;
            this.txt_nowloop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(247, 98);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 89;
            this.label8.Text = "Now";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1135, 702);
            this.Controls.Add(this.txt_nowloop);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txt_loop);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.Stopbtn);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.Excelbtn);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.LogBox);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.RLoadBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.editBtn);
            this.Controls.Add(this.runBtn);
            this.Controls.Add(this.btn_load);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.helpProvider1.SetShowHelp(this, true);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btn_load;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem scriptEditToolStripMenuItem;
        private System.Windows.Forms.Button runBtn;
        private System.Windows.Forms.Button editBtn;
        private System.Windows.Forms.TextBox ip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button RLoadBtn;
        public System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Timer Timer_Status;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox TBoxAllLog;
        private System.Windows.Forms.TextBox LogBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox DUTIDText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox ACIPText;
        private System.Windows.Forms.Button DAQbtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button PaLoad;
        private System.Windows.Forms.Button PaSave;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox FileText;
        private System.Windows.Forms.OpenFileDialog TxtOPen;
        private System.Windows.Forms.Button IDSbtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox IdsCb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox WebCamCb;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button Excelbtn;
        private SkinEngine skinEngine1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button Stopbtn;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.ToolStripMenuItem scriptBindingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        public System.Windows.Forms.TextBox txt_loop;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox txt_nowloop;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        public System.Windows.Forms.ComboBox comboBox1;
        public System.Windows.Forms.Label Timelb;
    }
}

