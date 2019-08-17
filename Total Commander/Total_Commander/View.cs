using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Total_Commander
{
    public partial class View : Form
    {
        public View()
        {
            InitializeComponent();
        }
        public void SHOW(TYPE t, FileSystemInfo fi)
        {
        /*    string text = "";

            if (t == TYPE.FILE)
                text = File.ReadAllText(fi.FullName);
            viewBox.AppendText(text);
            */
            if(t == TYPE.FILE)
            {
                FileStream theFile = File.Open(fi.FullName, FileMode.Open, FileAccess.Read);

                StreamReader rdr = new StreamReader(theFile);
                viewBox.Text = String.Empty;
                while (!rdr.EndOfStream)
                {
                    string line = rdr.ReadLine();
                    viewBox.Text += line + "\r\n";
                }
                this.Text = fi.FullName;
                rdr.Close();
                theFile.Close();
            }
            ShowDialog();
            return;
        }
    }
}
