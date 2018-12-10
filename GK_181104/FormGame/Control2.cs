using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace FormGame
{
    public partial class Control2 : Form
    {
        public Control2()
        {
            InitializeComponent();
        }
        static public void Tat()
        {
            Application.Exit();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encode = new ASCIIEncoding();
            Control1.byteSend = encode.GetBytes("2");
            Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
            Thread.Sleep(300);
            Form game = new SUDOKU3x3();
            game.Show();
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encode = new ASCIIEncoding();
            Control1.byteSend = encode.GetBytes("3");
            Control1.stm.Write(Control1.byteSend,0,Control1.byteSend.Length);
            Thread.Sleep(300);
            Form game = new SUDOKU3x3();
            game.Show();
            this.Close();
        }
        
        private void Control2_Load(object sender, EventArgs e)
        {

        }
    }
}
