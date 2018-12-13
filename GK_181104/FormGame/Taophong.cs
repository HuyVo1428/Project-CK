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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FormGame
{
    public partial class Taophong : Form
    {
        public Taophong()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pw ="c"+ textBox1.Text;
            if (checkBox1.Checked)
                pw += "|2";
            else
            {
                if (checkBox2.Checked)
                    pw += "|3";
                else
                {
                    if (checkBox3.Checked)
                        pw += "|4";
                    else
                        pw += "|2";
                }
            }       
            ASCIIEncoding encode = new ASCIIEncoding();
            Control1.byteSend = new byte[100];
            Control1.byteSend = encode.GetBytes(pw);
            Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
            this.Dispose();
         
        }
    }
}
