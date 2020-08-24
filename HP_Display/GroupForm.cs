using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HP_Display
{
    public partial class GroupForm : Form
    {
        public GroupForm()
        {
            InitializeComponent();
        }
        public static string gName = ""; 

        private void button1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                gName = textBox1.Text;
                Script script = (Script)this.Owner;
                script.groupCb.Items.Add(gName);
                script.groupCb.SelectedIndex = script.groupCb.Items.Count - 1;
                this.Close();
            }
            else
            {
                MessageBox.Show("Fill in the group name please.");
            }
        }

        private void GroupForm_Load(object sender, EventArgs e)
        {

        }
    }
}
