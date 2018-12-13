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
    public partial class Game4x4 : Form
    {
        //Biến cơ bản
        System.Media.SoundPlayer PlayerStart = new System.Media.SoundPlayer(@"C:\Sudoku\media\sound01.wav");
        int[,] arr = new int[16, 16];
        public Game4x4()
        {
            InitializeComponent();
        }
        int check4(int[,] arr, int x, int y)
        {
            for (int i = 0; i < 16; i++)
                if (arr[x, i] == arr[x, y] && i != y)
                    return 0;
            for (int j = 0; j < 16; j++)
                if (arr[j, y] == arr[x, y] && j != x)
                    return 0;
            int a = x / 4;
            int b = y / 4;
            for (int i = 4 * a; i < 4 * a + 4; i++)
                for (int j = 4 * b; j < 4 * b + 4; j++)
                    if (arr[i, j] == arr[x, y] && i != x && j != y)
                        return 0;
            return 1;
        }

        //Hàm phát sinh event
        public void Addevent()
        {
            //Duyệt
            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 16; j++)
                {
                    //Tìm textBox
                    TextBox control = (TextBox)this.Controls.Find("textBox" + (i * 16 + (j + 1)).ToString(), true).SingleOrDefault();
                    string temp1 = i.ToString();
                    string temp2 = j.ToString();
                    int temp3 = i;
                    int temp4 = j;
                    //Thêm event
                    control.KeyPress += (s, e) =>
                    {
                        string Validchar = "0123456789";
                        //Kiểm tra nhập vào là số từ 1-16
                        if (!Validchar.Contains(e.KeyChar)||(control.Text=="1"&&(e.KeyChar=='7'||e.KeyChar=='8'||e.KeyChar=='9'))
                            ||(control.Text!="1"&&control.Text!="")||(control.Text==""&&e.KeyChar.ToString()=="0"))
                            e.Handled = true;
                        else
                        {
                            #region Xử lý khi nhập số hợp lệ
                            /*Gửi dữ liệu nhập về cho server-CẤU TRÚC :?+4+<TOADOHANG>+|+<TOADOCOT>+|+<GIATRI>+X (thêm | để phân biệt giữa các vị trí
                             vì Game4x4 có tọa độ lúc là 1 chữ số lúc là 2 chữ số thay vì chỉ có 1 chữ số như 3x3 nên phải có đặc điểm để xác định kết thúc*/
                            string test = "?" +"4"+ temp1+"|"+temp2+"|"+control.Text+e.KeyChar+"x";
                            try
                            {
                                arr[temp3, temp4] = Convert.ToInt32(control.Text + e.KeyChar);
                                ASCIIEncoding encode = new ASCIIEncoding();
                                Control1.byteSend = encode.GetBytes(test);
                                Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                                //Nhận dữ liệu kiểm tra từ server
                                Control1.byteReceive = new byte[100];
                                int k = Control1.stm.Read(Control1.byteReceive, 0, 100);
                                test = "";
                                for (int h = 0; h < k; h++)
                                {
                                    test += Convert.ToChar(Control1.byteReceive[h]);
                                }
                                if (test == "0")
                                {
                                    control.BackColor = Color.Red;
                                    control.ForeColor = Color.White;
                                    #region doi mau o lien quan
                                    for (int kt = 0; kt < 16; kt++)
                                    {
                                        TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (temp3 * 16 + (kt + 1)).ToString(), true).SingleOrDefault();
                                        if (arr[temp3, kt] == arr[temp3, temp4] && kt != temp4 && kiemtra.Enabled)
                                        {
                                            kiemtra.BackColor = Color.Red;
                                            kiemtra.ForeColor = Color.White;
                                        }
                                    }
                                    for (int kt = 0; kt < 16; kt++)
                                    {
                                        TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (kt * 16 + (temp4 + 1)).ToString(), true).SingleOrDefault();
                                        if (arr[kt, temp4] == arr[temp3, temp4] && kt != temp3 && kiemtra.Enabled)
                                        {
                                            kiemtra.BackColor = Color.Red;
                                            kiemtra.ForeColor = Color.White;
                                        }
                                    }

                                    for (int hangocon = 4 * (temp3 / 4); hangocon < 4 * (temp3 / 4) + 4; hangocon++)
                                        for (int cotocon = 4 * (temp4 / 4); cotocon < 4 * (temp4 / 4) + 4; cotocon++)
                                        {
                                            TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (hangocon * 16 + (cotocon + 1)).ToString(), true).SingleOrDefault();
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
                                    control.BackColor = Color.Green;
                                    control.ForeColor = Color.White;
                                }
                                #region đổi màu lien quan
                                for (int kt = 0; kt < 16; kt++)
                                {
                                    TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (temp3 * 16 + (kt + 1)).ToString(), true).SingleOrDefault();
                                    if (kiemtra.BackColor == Color.Red)
                                    {
                                        if (check4(arr, temp3, kt) == 1)
                                        {
                                            kiemtra.ForeColor = Color.White;
                                            kiemtra.BackColor = Color.Green;
                                        }
                                    }
                                }
                                for (int kt = 0; kt < 16; kt++)
                                {
                                    TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (kt * 16 + (temp4 + 1)).ToString(), true).SingleOrDefault();
                                    if (kiemtra.BackColor == Color.Red)
                                    {
                                        if (check4(arr, kt, temp4) == 1)
                                        {
                                            kiemtra.ForeColor = Color.White;
                                            kiemtra.BackColor = Color.Green;
                                        }
                                    }
                                }
                                for (int hangocon = 4 * (temp3 / 4); hangocon < 4 * (temp3 / 4) + 4; hangocon++)
                                    for (int cotocon = 4 * (temp4 / 4); cotocon < 4 * (temp4 / 4) + 4; cotocon++)
                                    {
                                        TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (hangocon * 16 + (cotocon + 1)).ToString(), true).SingleOrDefault();
                                        if (kiemtra.BackColor == Color.Red)
                                            if (check4(arr, hangocon, cotocon) == 1)
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
                        if (e.KeyChar == Convert.ToChar(8))
                        {
                            #region xử lý nút xóa
                            e.Handled = false;
                            if(control.Text.Length==1)
                            {
                                #region khi số đang là 1 chữ số
                                control.BackColor = Color.White;
                                control.ForeColor = Color.Black;
                                arr[temp3, temp4] = 0;
                                string test = "?" + "4" + temp1 + "|" + temp2 + "|" + "0" + "x";
                                try
                                {
                                    ASCIIEncoding encode = new ASCIIEncoding();
                                    Control1.byteSend = encode.GetBytes(test);
                                    Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                                }
                                catch
                                {
                                }
                                for (int kt = 0; kt < 16; kt++)
                                {
                                    TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (temp3 * 16 + (kt + 1)).ToString(), true).SingleOrDefault();
                                    if (kiemtra.BackColor == Color.Red)
                                    {
                                        if (check4(arr, temp3, kt) == 1)
                                        {
                                            kiemtra.ForeColor = Color.White;
                                            kiemtra.BackColor = Color.Green;
                                        }
                                    }
                                }
                                for (int kt = 0; kt < 16; kt++)
                                {
                                    TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (kt * 16 + (temp4 + 1)).ToString(), true).SingleOrDefault();
                                    if (kiemtra.BackColor == Color.Red)
                                    {
                                        if (check4(arr, kt, temp4) == 1)
                                        {
                                            kiemtra.ForeColor = Color.White;
                                            kiemtra.BackColor = Color.Green;
                                        }
                                    }
                                }
                                for (int hangocon = 4 * (temp3 / 4); hangocon < 4 * (temp3 / 4) + 4; hangocon++)
                                    for (int cotocon = 4 * (temp4 / 4); cotocon < 4 * (temp4 / 4) + 4; cotocon++)
                                    {
                                        TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (hangocon * 16 + (cotocon + 1)).ToString(), true).SingleOrDefault();
                                        if (kiemtra.BackColor == Color.Red)
                                            if (check4(arr, hangocon, cotocon) == 1)
                                            {
                                                kiemtra.ForeColor = Color.White;
                                                kiemtra.BackColor = Color.Green;
                                            }
                                    }

                            }
                            #endregion
                            else
                            {
                                #region khi số đang là 2 chữ số
                                arr[temp3, temp4] = arr[temp3, temp4] / 10;
                                string test = "?" + "4" + temp1 + "|" + temp2 + "|" + "1" + "x";
                                try
                                {
                                    ASCIIEncoding encode = new ASCIIEncoding();
                                    Control1.byteSend = encode.GetBytes(test);
                                    //MessageBox.Show(test);
                                    Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                                    //Nhận dữ liệu kiểm tra từ server
                                    Control1.byteReceive = new byte[100];
                                    int k = Control1.stm.Read(Control1.byteReceive, 0, 100);
                                    test = "";
                                    for (int h = 0; h < k; h++)
                                    {
                                        test += Convert.ToChar(Control1.byteReceive[h]);
                                    }
                                    //MessageBox.Show(test);
                                    if (test == "0")
                                    {
                                        control.BackColor = Color.Red;
                                        control.ForeColor = Color.White;
                                        #region doi mau ô sai lien quan
                                        for (int kt = 0; kt < 16; kt++)
                                        {
                                            TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (temp3 * 16 + (kt + 1)).ToString(), true).SingleOrDefault();
                                            if (arr[temp3, kt] == arr[temp3, temp4] && kt != temp4 && kiemtra.Enabled)
                                            {
                                                kiemtra.BackColor = Color.Red;
                                                kiemtra.ForeColor = Color.White;
                                            }
                                        }
                                        for (int kt = 0; kt < 16; kt++)
                                        {
                                            TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (kt * 16 + (temp4 + 1)).ToString(), true).SingleOrDefault();
                                            if (arr[kt, temp4] == arr[temp3, temp4] && kt != temp3 && kiemtra.Enabled)
                                            {
                                                kiemtra.BackColor = Color.Red;
                                                kiemtra.ForeColor = Color.White;
                                            }
                                        }

                                        for (int hangocon = 4 * (temp3 / 4); hangocon < 4 * (temp3 / 4) + 4; hangocon++)
                                            for (int cotocon = 4 * (temp4 / 4); cotocon < 4 * (temp4 / 4) + 4; cotocon++)
                                            {
                                                TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (hangocon * 16 + (cotocon + 1)).ToString(), true).SingleOrDefault();
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
                                        control.BackColor = Color.Green;
                                        control.ForeColor = Color.White;
                                    }
                                    #region đổi màu ô sai thành đúng
                                    for (int kt = 0; kt < 16; kt++)
                                    {
                                        TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (temp3 * 16 + (kt + 1)).ToString(), true).SingleOrDefault();
                                        if (kiemtra.BackColor == Color.Red)
                                        {
                                            if (check4(arr, temp3, kt) == 1)
                                            {
                                                kiemtra.ForeColor = Color.White;
                                                kiemtra.BackColor = Color.Green;
                                            }
                                        }
                                    }
                                    for (int kt = 0; kt < 16; kt++)
                                    {
                                        TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (kt * 16 + (temp4 + 1)).ToString(), true).SingleOrDefault();
                                        if (kiemtra.BackColor == Color.Red)
                                        {
                                            if (check4(arr, kt, temp4) == 1)
                                            {
                                                kiemtra.ForeColor = Color.White;
                                                kiemtra.BackColor = Color.Green;
                                            }
                                        }
                                    }
                                    for (int hangocon = 4 * (temp3 / 4); hangocon < 4 * (temp3 / 4) + 4; hangocon++)
                                        for (int cotocon = 4 * (temp4 / 4); cotocon < 4 * (temp4 / 4) + 4; cotocon++)
                                        {
                                            TextBox kiemtra = (TextBox)this.Controls.Find("textBox" + (hangocon * 16 + (cotocon + 1)).ToString(), true).SingleOrDefault();
                                            if (kiemtra.BackColor == Color.Red)
                                                if (check4(arr, hangocon, cotocon) == 1)
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
                        }
                    };
                }
        }
        //Load form 
        private void Game4x4_Load(object sender, EventArgs e)
        {
            Control1.byteReceive = new byte[900];
            Control1.stm.Read(Control1.byteReceive, 0, 900);
            int chiso = 0;
            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 16; j++)
                {
                    /*Chuyển đổi kiểu dữ liệu load mảng lên textbox (=0 bỏ qua)
                     Cấu trúc:đọc được ký tự 'o'(one) tức giá trị có 1 chữ số=>đọc 1 bytes ngược lại đọc 2 bytes*/
                    TextBox control = (TextBox)this.Controls.Find("textBox" + Convert.ToString(i * 16 + (j + 1)), true).SingleOrDefault();
                    if (Convert.ToChar(Control1.byteReceive[chiso]) =='o')
                    {
                        #region Khi phần tử có 1 chữ số
                        if (Convert.ToChar(Control1.byteReceive[chiso + 1]) != '0')
                        {
                            control.Text += Convert.ToChar(Control1.byteReceive[chiso + 1]);
                            arr[i, j] = Convert.ToInt32(control.Text);
                            chiso = chiso + 2;
                            control.BackColor = Color.Yellow;
                            control.Enabled = false;
                        }
                        else
                        {
                            chiso = chiso + 2;
                        }
                        #endregion
                    }
                    else
                    {
                        #region Khi phần tử có 2 chữ số
                        if (Convert.ToChar(Control1.byteReceive[chiso + 1]) != '0')
                        {
                            control.Text += Convert.ToChar(Control1.byteReceive[chiso + 1]);
                            control.Text += Convert.ToChar(Control1.byteReceive[chiso + 2]);
                            arr[i, j] = Convert.ToInt32(control.Text);
                            chiso = chiso + 3;
                            control.BackColor = Color.Yellow;
                            control.Enabled = false;
                        }
                        else
                        {
                            chiso = chiso + 3;
                        }
                        #endregion
                    }
                }
            //Thêm event
            Addevent();

        }
        private void Game4x4_FormClosing(object sender, FormClosingEventArgs e)
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
            Boolean result = true;
            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 16; j++)
                {
                    TextBox control = (TextBox)this.Controls.Find("textBox" + Convert.ToString(i * 16 + (j + 1)), true).SingleOrDefault();
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
            {
                pnNotify.Visible = true;
                lbWin.Visible = true;
            }
            else
            {
                lbWin.Text = "Tiếc quá,\nbạn thua rồi!";
                lbWin.Visible = true;
                pnNotify.Visible = true;
            }
            Check = 1;
        }
        private void btnReplay_Click(object sender, EventArgs e)
        {
            pnNotify.Visible = false;
            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 16; j++)
                {
                    TextBox control = (TextBox)this.Controls.Find("textBox" + Convert.ToString(i * 16 + (j + 1)), true).SingleOrDefault();
                    if (Check == 1)
                    {
                        if ((control.BackColor == Color.Red) || (control.BackColor == Color.Green))
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
                        Control1.byteSend = encode.GetBytes("?4" + i.ToString() + j.ToString() + "0");
                        arr[i, j] = 0;
                        Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                    }
                }
        }
        private void Exit_Click(object sender, EventArgs e)
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

        private void textBox122_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
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

        private void btnSound_Click(object sender, EventArgs e)
        {
            if (btnSound.Text == "Tắt Âm thanh")
            {
                PlayerStart.Stop();
                btnSound.Text = "Bật Âm thanh";
            }
            else
            {
                PlayerStart.PlayLooping();
                btnSound.Text = "Tắt Âm thanh";
            }
        }
    }
}
