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
    public partial class vaophong : Form
    {
        public vaophong()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                MessageBox.Show("Vui lòng nhập số phòng");
            else
            {
                string send ="j"+textBox1.Text+"|"+textBox2.Text+"|";
                ASCIIEncoding encode = new ASCIIEncoding();
                Control1.byteSend = new byte[100];
                Control1.byteSend = encode.GetBytes(send);
                Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                this.Dispose();
            }
        }
    }
}
