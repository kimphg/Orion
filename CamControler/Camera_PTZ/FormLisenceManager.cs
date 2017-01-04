using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Camera_PTZ
{
    public partial class FormLisenceManager : Form
    {
        public string code;
        public FormLisenceManager()
        {
            InitializeComponent();
        }
        public void  setId(string id)
        {
            string str =null;
            int n=0;
            while (true)
            {
                
                str += id.Substring(4 * n, 4) ;
                n++;
                if (4 * n >= id.Length) break;
                str += "-";
                
            }
            textBox1.Text = str;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            code = textBox2.Text;
            this.Close();
        }
    }
}
