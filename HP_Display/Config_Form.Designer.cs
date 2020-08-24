namespace HP_Display
{
    partial class Config_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Config_Form));
            this.txt_scriptName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_ConfigName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_recognizeRate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_script = new System.Windows.Forms.Button();
            this.btn_config = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.rbtn_yes = new System.Windows.Forms.RadioButton();
            this.rbtn_no = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // txt_scriptName
            // 
            this.txt_scriptName.Location = new System.Drawing.Point(161, 45);
            this.txt_scriptName.Name = "txt_scriptName";
            this.txt_scriptName.Size = new System.Drawing.Size(168, 22);
            this.txt_scriptName.TabIndex = 71;
            this.txt_scriptName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 20);
            this.label1.TabIndex = 72;
            this.label1.Text = "Script name";
            // 
            // txt_ConfigName
            // 
            this.txt_ConfigName.Location = new System.Drawing.Point(161, 83);
            this.txt_ConfigName.Name = "txt_ConfigName";
            this.txt_ConfigName.Size = new System.Drawing.Size(168, 22);
            this.txt_ConfigName.TabIndex = 73;
            this.txt_ConfigName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 20);
            this.label2.TabIndex = 74;
            this.label2.Text = "Config name";
            // 
            // txt_recognizeRate
            // 
            this.txt_recognizeRate.Location = new System.Drawing.Point(161, 120);
            this.txt_recognizeRate.Name = "txt_recognizeRate";
            this.txt_recognizeRate.Size = new System.Drawing.Size(93, 22);
            this.txt_recognizeRate.TabIndex = 75;
            this.txt_recognizeRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 20);
            this.label3.TabIndex = 76;
            this.label3.Text = "Recognize rate";
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(260, 120);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(100, 22);
            this.btn_save.TabIndex = 77;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // btn_script
            // 
            this.btn_script.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_script.BackgroundImage")));
            this.btn_script.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_script.Location = new System.Drawing.Point(332, 45);
            this.btn_script.Name = "btn_script";
            this.btn_script.Size = new System.Drawing.Size(28, 22);
            this.btn_script.TabIndex = 78;
            this.btn_script.UseVisualStyleBackColor = true;
            this.btn_script.Click += new System.EventHandler(this.btn_script_Click);
            // 
            // btn_config
            // 
            this.btn_config.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_config.BackgroundImage")));
            this.btn_config.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_config.Location = new System.Drawing.Point(332, 83);
            this.btn_config.Name = "btn_config";
            this.btn_config.Size = new System.Drawing.Size(28, 22);
            this.btn_config.TabIndex = 79;
            this.btn_config.UseVisualStyleBackColor = true;
            this.btn_config.Click += new System.EventHandler(this.btn_config_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 20);
            this.label4.TabIndex = 81;
            this.label4.Text = "Tool auto";
            // 
            // rbtn_yes
            // 
            this.rbtn_yes.AutoSize = true;
            this.rbtn_yes.Checked = true;
            this.rbtn_yes.Location = new System.Drawing.Point(161, 12);
            this.rbtn_yes.Name = "rbtn_yes";
            this.rbtn_yes.Size = new System.Drawing.Size(40, 16);
            this.rbtn_yes.TabIndex = 82;
            this.rbtn_yes.TabStop = true;
            this.rbtn_yes.Text = "Yes";
            this.rbtn_yes.UseVisualStyleBackColor = true;
            // 
            // rbtn_no
            // 
            this.rbtn_no.AutoSize = true;
            this.rbtn_no.Location = new System.Drawing.Point(217, 12);
            this.rbtn_no.Name = "rbtn_no";
            this.rbtn_no.Size = new System.Drawing.Size(37, 16);
            this.rbtn_no.TabIndex = 83;
            this.rbtn_no.TabStop = true;
            this.rbtn_no.Text = "No";
            this.rbtn_no.UseVisualStyleBackColor = true;
            // 
            // Config_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 165);
            this.Controls.Add(this.rbtn_no);
            this.Controls.Add(this.rbtn_yes);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btn_config);
            this.Controls.Add(this.btn_script);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.txt_recognizeRate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt_ConfigName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_scriptName);
            this.Controls.Add(this.label1);
            this.Name = "Config_Form";
            this.Text = "Config_Form";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Config_Form_FormClosed);
            this.Load += new System.EventHandler(this.Config_Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txt_scriptName;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txt_ConfigName;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txt_recognizeRate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_script;
        private System.Windows.Forms.Button btn_config;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rbtn_yes;
        private System.Windows.Forms.RadioButton rbtn_no;
    }
}