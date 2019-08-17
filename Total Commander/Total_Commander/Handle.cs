using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Total_Commander
{
    public partial class Handle : Form
    {
        public int result { get; set; }
        public Handle()
        {
            InitializeComponent();
            result = -1;
        }

        private void Button_Skip_Click(object sender, EventArgs e)
        {
            result = 1;
            this.Close();
        }

        private void Button_SkipAll_Click(object sender, EventArgs e)
        {
            result = 2;
            this.Close();
        }

        private void Button_Overwrite_Click(object sender, EventArgs e)
        {
            result = 3;
            this.Close();
        }

        private void Button_OverwriteAll_Click(object sender, EventArgs e)
        {
            result = 4;
            this.Close();
        }

        private void Button_Cancel_Click(object sender, EventArgs e)
        {
            result = 0;
            this.Close();
        }
    }
}
