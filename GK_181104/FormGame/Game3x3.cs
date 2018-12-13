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
        //Biến cơ bản
        //System.Media.SoundPlayer PlayerStart = new System.Media.SoundPlayer(@"C:\Sudoku\media\sound01.wav");
        int [,]arr = new int[9, 9];
        public SUDOKU3x3()
        {
            InitializeComponent();
            

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
                                 Control1.byteReceive = new byte[100];
                                 int k = Control1.stm.Read(Control1.byteReceive, 0, 100);
                                 test = "";
                                 for (int h = 0; h < k; h++)
                                 {
                                     test += Convert.ToChar(Control1.byteReceive[h]);
                                 }
                                 #region Xu ly khi ô nhập bị sai

                                 if (test == "0")
                                 {
                                     control.BackColor = Color.Red;
                                     control.ForeColor = Color.White;
                                    #region doi mau các ô sai liên quan
                                    for (int kt = 0; kt < 9; kt++)
                                     {
                                         TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (temp3 * 9 + (kt + 1)).ToString(), true).SingleOrDefault();
                                         if (arr[temp3, kt] == arr[temp3, temp4] && kt != temp4 && kiemtra.Enabled)
                                         {
                                             kiemtra.BackColor = Color.Red;
                                             kiemtra.ForeColor = Color.White;
                                         }
                                     }
                                     for (int kt = 0; kt < 9; kt++)
                                     {
                                         TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (kt * 9 + (temp4 + 1)).ToString(), true).SingleOrDefault();
                                         if (arr[kt, temp4] == arr[temp3, temp4] && kt != temp3&& kiemtra.Enabled)
                                         {
                                             kiemtra.BackColor = Color.Red;
                                             kiemtra.ForeColor = Color.White;
                                         }
                                     }

                                     for (int hangocon = 3 * (temp3 / 3); hangocon < 3 * (temp3 / 3) + 3; hangocon++)
                                         for (int cotocon = 3 * (temp4 / 3); cotocon < 3 * (temp4 / 3) + 3; cotocon++)
                                         {
                                             TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (hangocon * 9 + (cotocon + 1)).ToString(), true).SingleOrDefault();
                                             if (arr[hangocon, cotocon] == arr[temp3, temp4] && hangocon != temp3 && cotocon != temp4 && kiemtra.Enabled)
                                             {
                                                 kiemtra.BackColor = Color.Red;
                                                 kiemtra.ForeColor = Color.White;
                                             }

                                         }
                                    #endregion
                                    //string Pressed = Convert.ToString(e.KeyChar);  
                                    //CheckTrungTrenHang(Pressed, temp3);
                                    //CheckTrungTrenCot(Pressed, temp4);
                                    //CheckTrungArea(Pressed, temp3, temp4);
                                    control.BackColor = Color.Red;
                                     control.ForeColor = Color.White;
                                 }
                                 else
                                 {
                                     if (control.Enabled == false)
                                     {
                                         control.ForeColor = Color.Yellow;
                                         control.BackColor = Color.Black;
                                     }
                                     else
                                     {
                                         control.ForeColor = Color.White;
                                         control.BackColor = Color.Green;
                                     }
                                 }
                                 #endregion
                                 #region đổi màu các ô liên quan nếu nó đã đúng
                                 for (int kt = 0; kt < 9; kt++)
                                 {
                                     TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (temp3 * 9 + (kt + 1)).ToString(), true).SingleOrDefault();
                                     if (kiemtra.BackColor == Color.Red)
                                     {
                                         if (check3(arr, temp3, kt) == 1)
                                         {
                                             if (kiemtra.Enabled == false)
                                             {
                                                 kiemtra.ForeColor = Color.Yellow;
                                                 kiemtra.BackColor = Color.Black;
                                             }
                                             else
                                             {
                                                 kiemtra.ForeColor = Color.White;
                                                 kiemtra.BackColor = Color.Green;
                                             }
                                         
                                         }
                                     }
                                 }
                                 for (int kt = 0; kt < 9; kt++)
                                 {
                                     TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (kt * 9 + (temp4 + 1)).ToString(), true).SingleOrDefault();
                                     if (kiemtra.BackColor == Color.Red)
                                     {
                                         if (check3(arr, kt, temp4) == 1)
                                         {
                                             kiemtra.ForeColor = Color.White;
                                             kiemtra.BackColor = Color.Green;
                                         }
                                     }
                                 }
                                 for (int hangocon = 3 * (temp3 / 3); hangocon < 3 * (temp3 / 3) + 3; hangocon++)
                                     for (int cotocon = 3 * (temp4 / 3); cotocon < 3 * (temp4 / 3) + 3; cotocon++)
                                     {
                                         TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (hangocon * 9 + (cotocon + 1)).ToString(), true).SingleOrDefault();
                                         if (kiemtra.BackColor == Color.Red)
                                             if (check3(arr, hangocon, cotocon) == 1)
                                             {
                                                 kiemtra.ForeColor = Color.White;
                                                 kiemtra.BackColor = Color.Green;
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
                             #region xử lý khi nhập nút xóa
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
                             #region Đổi màu các ô sai liên quan nếu nó đã đúng
                             for (int kt = 0; kt < 9; kt++)
                             {
                                 TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (temp3 * 9 + (kt + 1)).ToString(), true).SingleOrDefault();
                                 if (kiemtra.BackColor == Color.Red)
                                 {
                                     if (check3(arr, temp3, kt) == 1)
                                     {
                                         kiemtra.ForeColor = Color.White;
                                         kiemtra.BackColor = Color.Green;
                                     }
                                 }
                             }
                             for (int kt = 0; kt < 9; kt++)
                             {
                                 TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (kt * 9 + (temp4 + 1)).ToString(), true).SingleOrDefault();
                                 if (kiemtra.BackColor == Color.Red)
                                 {
                                     if (check3(arr, kt, temp4) == 1)
                                     {
                                         kiemtra.ForeColor = Color.White;
                                         kiemtra.BackColor = Color.Green;
                                     }
                                 }
                             }
                             for (int hangocon = 3 * (temp3 / 3); hangocon < 3 * (temp3 / 3) + 3; hangocon++)
                                 for (int cotocon = 3 * (temp4 / 3); cotocon < 3 * (temp4 / 3) + 3; cotocon++)
                                 {
                                     TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (hangocon * 9 + (cotocon + 1)).ToString(), true).SingleOrDefault();
                                     if (kiemtra.BackColor == Color.Red)
                                         if (check3(arr, hangocon, cotocon) == 1)
                                         {
                                             kiemtra.ForeColor = Color.White;
                                             kiemtra.BackColor = Color.Green;
                                         }
                                 }
                             #endregion

                             #endregion
                         }
                     };
                    #endregion
                }
        }
        //Khởi tạo form lấy dữ liệu mảng từ server + Thêm event ràng buộc dữ liệu nhập vào+Gửi dữ liệu hợp lệ về server để kiểm tra
        private void Form1_Load(object sender, EventArgs e)
        {
            //PlayerStart.PlayLooping();
            Control1.byteReceive = new byte[100];
            Control1.stm.Read(Control1.byteReceive, 0, 100);
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    //Chuyển đổi kiểu dữ liệu load mảng lên textbox (=0 bỏ qua)
                    TextBox control = (TextBox)this.Controls.Find("textBox" + Convert.ToString(i * 9 + (j + 1)), true).SingleOrDefault();
                    if (Convert.ToChar(Control1.byteReceive[i * 9 + j]) != '0')
                    {
                        control.Text += Convert.ToChar(Control1.byteReceive[i * 9 + j]);
                        arr[i, j] = Convert.ToInt32(control.Text);
                        control.Enabled = false;
                        control.BackColor = Color.Yellow;  
                    }
                     
                }
            Addevent();
            
        }
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
        public int Check = 0;
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
    }
}
