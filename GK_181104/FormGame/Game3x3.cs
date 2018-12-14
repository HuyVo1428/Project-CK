using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormGame
{
    public partial class SUDOKU3x3 : Form
    {
        //System.Media.SoundPlayer PlayerStart = new System.Media.SoundPlayer(@"C:\Sudoku\media\sound01.wav");
        int [,]arr = new int[9, 9];
        string checkresult = "";
        public int Check = 0;
        public static int room;
        public SUDOKU3x3()
        {
            InitializeComponent();
            

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //PlayerStart.PlayLooping();
            button_play.Enabled = false;
            label1.Text = "ROOM:" + room.ToString();
            Thread t = new Thread(receive);
            t.IsBackground = true;
            t.Start();            
            Addevent();

        }
        private void receive()
        {
            while(true)
            {
                try
                {
                    Control1.byteReceive = new byte[82];
                    Control1.stm.Read(Control1.byteReceive, 0, 82);
                    if (Convert.ToChar(Control1.byteReceive[0]) == 'N')
                    {
                        textBox82.Text += "Người chơi ";
                        int i = 1;
                        for (; Convert.ToChar(Control1.byteReceive[i]) != '.'; i++)
                        {
                            textBox82.Text += Convert.ToChar(Control1.byteReceive[i]);
                            if (Convert.ToChar(Control1.byteReceive[i + 1]) == '.')
                            {
                                textBox82.Text += " đã vào phòng." ;
                                textBox82.Text += Environment.NewLine;
                            }
                        }
                        if(Control1.byteReceive[i+1]=='x')
                                    button_play.Enabled = true;
                    }
                    else
                    {
                        if (Convert.ToChar(Control1.byteReceive[0]) == 'c')
                        {
                            for (int i = 0; i < 9; i++)
                                for (int j = 0; j < 9; j++)
                                {
                                    //Chuyển đổi kiểu dữ liệu load mảng lên textbox (=0 bỏ qua)
                                    TextBox control = (TextBox)this.Controls.Find("textBox" + Convert.ToString(i * 9 + (j + 1)), true).SingleOrDefault();
                                    if (Convert.ToChar(Control1.byteReceive[i * 9 + j + 1]) != '0')
                                    {
                                        control.Text += Convert.ToChar(Control1.byteReceive[i * 9  + j + 1]);
                                        arr[i, j] = Convert.ToInt32(control.Text);
                                        control.Enabled = false;
                                        control.BackColor = Color.Yellow;
                                    }
                                }
                            textBox82.Text += "Chủ phòng đã bắt đầu game"+Environment.NewLine;
                        }
                        else
                        {
                            if (Convert.ToChar(Control1.byteReceive[0]) == 'r')
                            {
                                string temp="";
                                temp += Convert.ToChar(Control1.byteReceive[1]);
                                checkresult = temp;
                            }
                            else
                            {
                                if (Convert.ToChar(Control1.byteReceive[0]) == 'o')
                                {
                                    string aa = "";
                                    aa += Convert.ToChar(Control1.byteReceive[1]);
                                    string bb = "";
                                    bb += Convert.ToChar(Control1.byteReceive[2]);
                                    int x = Convert.ToInt32(aa);
                                    int y = Convert.ToInt32(bb);
                                    TextBox control = (TextBox)this.Controls.Find("textBox" + Convert.ToString((x*9)+(y+1)), true).SingleOrDefault();
                                    control.Text = "";
                                    control.Text += Convert.ToChar(Control1.byteReceive[3]);
                                    control.BackColor = Color.Blue;
                                }

                            }
                        }
                    }                    
                }
                catch
                {

                }
            }
        }
        int check3(int[,] arr, int x, int y)
        {
            for (int i = 0; i < 9; i++)
                if (arr[x, i] == arr[x, y] && i != y)
                {
                    //MessageBox.Show(x.ToString()+y.ToString()+" "+Convert.ToString(arr[x, i]) + " "+x.ToString() +i.ToString()+" "  + Convert.ToString(arr[x, y]));
                    return 0;
                }
            for (int j = 0; j < 9; j++)
                if (arr[j, y] == arr[x, y] && j != x)
                {
                    //MessageBox.Show(x.ToString() + y.ToString() + " " + Convert.ToString(arr[j,y]) + " "+j.ToString()+y.ToString()+" " + Convert.ToString(arr[x, y]));
                    return 0;
                }
            int a = x / 3;
            int b = y / 3;
            for (int i = 3 * a; i < 3 * a + 3; i++)
                for (int j = 3 * b; j < 3 * b + 3; j++)
                    if (arr[i, j] == arr[x, y] && i != x && j != y)
                    {
                        //MessageBox.Show(x.ToString() + y.ToString() + " " + Convert.ToString(arr[i, j]) + " "+i.ToString()+j.ToString()+" " + Convert.ToString(arr[x, y]));
                        return 0;
                    }
            return 1;
        }
        public void Addevent()
        {
            //Duyệt 
            for (int i = 0; i < 9; i++)
                for (int j = 0; j< 9;j++)
                {
                    #region Tìm textBox  và chuyuển đổi các thông số
                    TextBox control = (TextBox)this.Controls.Find("textBox" + (i * 9 + (j + 1)).ToString(), true).SingleOrDefault();
                    string temp1 = i.ToString();
                    string temp2 = j.ToString();
                    int temp3 = i;
                    int temp4 = j;
                    #endregion
                    #region Thêm event
                    control.KeyPress += (s, e) =>
                     {
                         #region Kiểm tra nhập vào là số từ 1-9
                         string Validchar = "123456789";
                         if (control.Text != "" || !Validchar.Contains(e.KeyChar))
                             e.Handled = true;

                         else
                         {
                             #region Xử lý khi nhập số hợp lệ
                             //Đầu vào đúng gửi dữ liệu nhập về cho server: CẤU TRÚC: ?+3+<TOADOHANG>+<TOADOCOT>+<GIATRINHAP>
                             arr[temp3, temp4] = Convert.ToInt32(Convert.ToString(e.KeyChar));
                             string test = "?" + "3" + temp1 + temp2 + e.KeyChar;
                             ASCIIEncoding encode = new ASCIIEncoding();
                             try
                             {
                                 Control1.byteSend = encode.GetBytes(test);
                                 Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                                 //Nhận kết quả kiểm tra từ server
                                 Thread.Sleep(150);                                 
                                 #region Xu ly khi ô nhập
                                 if (checkresult == "0")
                                 {
                                     control.BackColor = Color.Red;
                                 }
                                 else
                                 {
                                     if (checkresult == "1")
                                     {
                                         control.ForeColor = Color.White;
                                         control.BackColor = Color.Green;
                                     }
                                 }
                                 #endregion
                            }
                             catch (Exception)
                             {
                                 MessageBox.Show("Lỗi! server ngưng hoạt động!");
                             }
                             #endregion
                         }
                         #endregion
                         if (e.KeyChar==Convert.ToChar(8))
                         {
                             #region gửi dữ liệu update về cho server
                             e.Handled = false;
                             control.BackColor = Color.White;
                             control.ForeColor = Color.Black;
                             arr[temp3, temp4] = 0;
                             string test = "?" + "3" + temp1 + temp2 +"0";
                             ASCIIEncoding encode = new ASCIIEncoding();
                             try
                             {
                                 Control1.byteSend = encode.GetBytes(test);
                                 Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                             }
                             catch
                             {

                             }
                             #endregion
                         }
                     };
                    #endregion
                }
        }
        //Khởi tạo form lấy dữ liệu mảng từ server + Thêm event ràng buộc dữ liệu nhập vào+Gửi dữ liệu hợp lệ về server để kiểm tra
        private void SUDOKU3x3_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ASCIIEncoding encode = new ASCIIEncoding();
                Control1.byteSend = encode.GetBytes("x");
                Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                Control1.tcpclnt.Close();
                Control1.Exit();
            }
            catch
            {
            }
        }
        public Boolean CheckFinal()
        {
            Boolean result=true;
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    TextBox control = (TextBox)this.Controls.Find("textBox" + (i * 9 + (j + 1)).ToString(), true).SingleOrDefault();
                    if (control.Text == "")
                        control.BackColor = Color.Red;
                    if (control.BackColor == Color.Red)
                        result = false;
                    control.Enabled = false;                   
                }
            return result;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckFinal() == true)
                pnNotify.Visible = true;
            else
            {
                lbWin.Text = "Tiếc quá,\nbạn thua rồi!";
                pnNotify.Visible = true;
            }
            Check = 1;
            
        }
        private void btnReplay_Click(object sender, EventArgs e)
        {
            pnNotify.Visible = false;
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    TextBox control = (TextBox)this.Controls.Find("textBox" + (i * 9 + (j + 1)).ToString(), true).SingleOrDefault();
                    if(Check==1)
                    {
                        if ((control.BackColor==Color.Red)||(control.BackColor==Color.Green))
                        {
                            control.Enabled = true;
                        }
                    }
                    if (control.Enabled == true)
                    {
                        control.Clear();
                        control.BackColor = Color.White;
                        control.ForeColor = Color.Black;
                        ASCIIEncoding encode = new ASCIIEncoding();
                        Control1.byteSend = encode.GetBytes("?3" + i.ToString() + j.ToString() + "0");
                        arr[i, j] = 0;
                        Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                    }
                }

        }
        private void btnRestart_Click(object sender, EventArgs e)
        {
            Form game = new SUDOKU3x3();
            try
            {
                //if (SUDOKU3x3.active)
                //    game.Dispose();
                //else
                //    if (Game4x4.active)
                //    Game4x4.Tat();
                //Gửi yêu cầu tạo mảng 3x3
                ASCIIEncoding encode = new ASCIIEncoding();
                Control1.byteSend = encode.GetBytes("!3");
                Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                Thread.Sleep(500);
                //Load game 3x3
                game = new SUDOKU3x3();
                game.Show();
                
                this.Dispose();
            }
            catch
            {
            }
        }
        private void btnLevel_Click(object sender, EventArgs e)
        {
            //Tắt game hiện hành
            Form game = new Game4x4();
            try
            {
                //if (SUDOKU3x3.active)
                //    game.Dispose();
                //else
                //    if (Game4x4.active)
                //    Game4x4.Tat();
                //Gửi yêu cầu tạo mảng 3x3
                ASCIIEncoding encode = new ASCIIEncoding();
                Control1.byteSend = encode.GetBytes("!4");
                Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                Thread.Sleep(500);
                //Load game 3x3
                game = new Game4x4();
                game.Show();
                
                this.Dispose();
            }
            catch
            {
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ASCIIEncoding encode = new ASCIIEncoding();
                Control1.byteSend = encode.GetBytes("x");
                Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                Control1.tcpclnt.Close();
                Control1.Exit();
            }
            catch
            {
            }            
            this.Close();
        }
        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
        private void textBox23_TextChanged(object sender, EventArgs e)
        {

        }
        private void lbWin_Click(object sender, EventArgs e)
        {

        }
        private void btnSound_Click(object sender, EventArgs e)
        {
            
            if (btnSound.Text == "Tắt Âm thanh")
            {
                //PlayerStart.Stop();
                btnSound.Text = "Bật Âm thanh";
            }
            else
            {
                //PlayerStart.PlayLooping();
                btnSound.Text = "Tắt Âm thanh";
            }
        }

        private void button_play_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encode = new ASCIIEncoding();
            try
            {
                Control1.byteSend = encode.GetBytes("!");
                Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
            }
            catch
            {

            }
        }
    }
}
