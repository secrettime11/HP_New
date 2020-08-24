namespace HP_Display
{
    partial class Script_binding
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.treeView3 = new System.Windows.Forms.TreeView();
            this.bindbtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.s1btn = new System.Windows.Forms.Button();
            this.s2btn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fileNametext = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.saveBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(26, 65);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(279, 441);
            this.treeView1.TabIndex = 6;
            // 
            // treeView2
            // 
            this.treeView2.Location = new System.Drawing.Point(316, 65);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(279, 441);
            this.treeView2.TabIndex = 7;
            // 
            // treeView3
            // 
            this.treeView3.Location = new System.Drawing.Point(687, 65);
            this.treeView3.Name = "treeView3";
            this.treeView3.Size = new System.Drawing.Size(279, 441);
            this.treeView3.TabIndex = 8;
            // 
            // bindbtn
            // 
            this.bindbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bindbtn.Font = new System.Drawing.Font("新細明體", 25F);
            this.bindbtn.Location = new System.Drawing.Point(603, 231);
            this.bindbtn.Name = "bindbtn";
            this.bindbtn.Size = new System.Drawing.Size(75, 70);
            this.bindbtn.TabIndex = 9;
            this.bindbtn.Text = "➠";
            this.bindbtn.UseVisualStyleBackColor = true;
            this.bindbtn.Click += new System.EventHandler(this.bindbtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(24, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "Script one";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(314, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "Script two";
            // 
            // s1btn
            // 
            this.s1btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.s1btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.s1btn.Location = new System.Drawing.Point(26, 39);
            this.s1btn.Name = "s1btn";
            this.s1btn.Size = new System.Drawing.Size(60, 22);
            this.s1btn.TabIndex = 12;
            this.s1btn.Text = "Select";
            this.s1btn.UseVisualStyleBackColor = true;
            this.s1btn.Click += new System.EventHandler(this.s1btn_Click);
            // 
            // s2btn
            // 
            this.s2btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.s2btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.s2btn.Location = new System.Drawing.Point(316, 39);
            this.s2btn.Name = "s2btn";
            this.s2btn.Size = new System.Drawing.Size(60, 22);
            this.s2btn.TabIndex = 13;
            this.s2btn.Text = "Select";
            this.s2btn.UseVisualStyleBackColor = true;
            this.s2btn.Click += new System.EventHandler(this.s2btn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // fileNametext
            // 
            this.fileNametext.Location = new System.Drawing.Point(687, 39);
            this.fileNametext.Name = "fileNametext";
            this.fileNametext.Size = new System.Drawing.Size(213, 22);
            this.fileNametext.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(685, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "File name";
            // 
            // saveBtn
            // 
            this.saveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveBtn.Location = new System.Drawing.Point(906, 39);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(60, 22);
            this.saveBtn.TabIndex = 16;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // Script_binding
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(990, 530);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fileNametext);
            this.Controls.Add(this.s2btn);
            this.Controls.Add(this.s1btn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bindbtn);
            this.Controls.Add(this.treeView3);
            this.Controls.Add(this.treeView2);
            this.Controls.Add(this.treeView1);
            this.Name = "Script_binding";
            this.Text = "Script_binding";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TreeView treeView1;
        public System.Windows.Forms.TreeView treeView2;
        public System.Windows.Forms.TreeView treeView3;
        private System.Windows.Forms.Button bindbtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button s1btn;
        private System.Windows.Forms.Button s2btn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox fileNametext;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button saveBtn;
    }
}