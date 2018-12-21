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
    public partial class Control1 : Form
    {
        #region Khai báo
        System.Media.SoundPlayer PlayerStart = new System.Media.SoundPlayer(@"C:\Sudoku\media\sound01.wav");
        public  static TcpClient tcpclnt;
        public static TcpClient tcpclnt2;
        public static NetworkStream stm,stm2;
        public static byte[] byteSend;
        public static byte[] byteReceive;
        Form game;
        public int stopthread = 0;
        Thread t;
        #endregion
        #region Load 
        public Control1()
        {
            InitializeComponent();
            Thread t = new Thread(new ThreadStart(SplashStar));
            t.Start();
            Thread.Sleep(3000);
            t.Abort();
        }
        public void SplashStar()
        {
            Application.Run(new Splash());
        }
        //Load form-(Ẩn nút NEW, EXIT)
        private void Control1_Load(object sender, EventArgs e)
        {
            game = new SUDOKU3x3();
            label3.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            usernamebox.Focus();
            PlayerStart.PlayLooping();
            //Khởi tạo kết nối
            tcpclnt = new TcpClient();
            //tcpclnt.Connect("127.0.0.1", 13000);
            tcpclnt.Connect("127.0.0.1", 13000);
            tcpclnt2 = new TcpClient();
            //tcpclnt2.Connect("127.0.0.1", 13001);
            tcpclnt2.Connect("127.0.0.1", 13001);
            stm = tcpclnt.GetStream();
            stm2 = tcpclnt2.GetStream();
        }
        /*Nút Login-(Khởi tạo kết nối + Gửi username, password + Check kết quả login từ server 
        + Chuyển màn hình: Ẩn cửa texbox nút dùng login, hiện nút NEW, EXIT)*/
        #endregion
        #region Xử lý event
        //thread receive
        private void receive()
        {
            while (stopthread==0)
            {
                try
                {
                    byte[] byteReceive2 = new byte[82];
                    Control1.stm2.Read(byteReceive2, 0, 82);
                    if (Convert.ToChar(byteReceive2[0]) == 'i')
                    {
                        for (int i = 1; byteReceive2[i] != '@'; i++)
                        {
                            textBox1.Text += Convert.ToChar(byteReceive2[i]);
                        }
                        textBox1.Text += Environment.NewLine;
                    }
                    else
                    {
                        if (Convert.ToChar(byteReceive2[0]) == '+')
                        {
                            label3.Text = "$= ";
                            for (int i = 1; byteReceive2[i] != '@'; i++)
                            {
                                label3.Text += Convert.ToChar(byteReceive2[i]);
                            }
                        }
                    }

                }
                catch
                {
                }
            }
        }        
        //Hàm gửi mess
        public static void sendmess(string mess, Stream x)
        {
            byteSend = new byte[82];
            for (; mess.Length < 82;)
                mess += '@';
            ASCIIEncoding encode = new ASCIIEncoding();
            byteSend = encode.GetBytes(mess);            
            x.Write(byteSend,0,byteSend.Length);
        }        
        private void btnSound_Click(object sender, EventArgs e)
        {
            if(btnSound.Text=="Tắt Âm thanh")
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
        //Button đăng nhập
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Check nhập texbox
                if ((usernamebox.Text == "") || (passwordbox.Text == ""))
                {
                    MessageBox.Show("Tên đăng nhập hoặc Mật khẩu chưa được nhập vào.\nVui lòng kiểm tra và nhập lại.", "Đăng nhập Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    passwordbox.Clear();
                    usernamebox.Focus();
                }
                //Gửi thông tin username, password
                else
                {
                    sendmess(usernamebox.Text, stm);
                    Thread.Sleep(150);
                    sendmess(passwordbox.Text, stm);
                    //Nhận thông tin trả về từ Server
                    byteReceive = new byte[82];
                    int k = stm.Read(byteReceive, 0, 82);
                    string result = "";
                    for (int i = 0; i < k; i++)
                    {
                        result += Convert.ToChar(byteReceive[i]);
                    }
                    //Check kết quả login và chuyển màn hình
                    if (result[0] == '1')
                    {
                        CheckForIllegalCrossThreadCalls = false;
                        usernamebox.Visible = false;
                        passwordbox.Visible = false;
                        btnExit.Visible = false;
                        button1.Visible = false;
                        button3.Visible = true;
                        button4.Visible = true;
                        button5.Visible = true;
                        textBox1.Visible = true;
                        label1.Visible = false;
                        label2.Visible = false;
                        lbLogin.Text = "Xin chào, " + usernamebox.Text;
                        string temp = "";
                        for (int i = 1; result[i] != 'x'; i++)
                            temp += result[i];
                        label3.Text = "$: " + temp;
                        label3.Visible = true;
                        t = new Thread(receive);
                        t.IsBackground = true;
                        t.Start();
                    }
                    else
                    {
                        if (result[0] == '0')
                        {
                            MessageBox.Show("Tài khoản đang được đăng nhập trên phiên làm việc khác.\nĐăng xuất khỏi các phiên và thử lại.", "Đăng nhập Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            usernamebox.Focus();
                        }
                        else
                        {
                            DialogResult Err;
                            Err = MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu.\n Vui lòng kiểm tra và nhập lại.", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            passwordbox.Clear();
                            usernamebox.Focus();
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Đã có lỗi do Server!\nVui lòng thử lại sau.", "Lỗi Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Button tạo phòng
        private void button3_Click(object sender, EventArgs e)
        {
            Taophong tp = new Taophong();
            tp.ShowDialog();
            Thread.Sleep(200);
            int room=0;

            while (true)
            {
                try
                {
                    byteReceive = new byte[82];
                    int k = stm.Read(byteReceive, 0, 82);
                    string result = "";
                    for (int i = 0; i < k; i++)
                    {
                        result += Convert.ToChar(byteReceive[i]);
                    }
                    if (result[0] == '1')
                    {
                        string showresult = "";
                        for (int i = 2; result[i] != '@'; i++)
                        {
                            showresult += result[i];
                        }
                        MessageBox.Show("Tạo phòng thành công " + showresult);
                        string numroom = "";
                        for (int i = 14; result[i] != ' '; i++)
                        {
                            numroom += result[i];
                        }
                        room = Convert.ToInt32(numroom);
                        if (result[1] == '3')
                        {
                            game = new SUDOKU3x3();
                            SUDOKU3x3.room = room;
                            game.ShowDialog();
                            break;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        MessageBox.Show("Bạn không đủ tiền để tạo phòng");
                        break;
                    }
                }
                catch
                {
                }
            }
            
        }
        //Button Vào phòng
        private void button4_Click(object sender, EventArgs e)
        {
            vaophong vp = new vaophong();
            vp.ShowDialog();
            Thread.Sleep(200);
            while (true)
            {
                try
                {
                    byteReceive = new byte[82];
                    int k = stm.Read(byteReceive, 0, 82);
                    string result = "";
                    for (int i = 0; i < k; i++)
                    {
                        result += Convert.ToChar(byteReceive[i]);
                    }
                    if (result[0] == '1')
                    {

                        //MessageBox.Show("Đã vào phòng ");
                        int room;
                        string numroom = "";
                        for (int i = 2; result[i] != '@'; i++)
                        {
                            numroom += result[i];
                        }
                        room = Convert.ToInt32(numroom);
                        if (result[1] == '3')
                        {
                            game = new SUDOKU3x3();
                            SUDOKU3x3.room = room;
                            game.ShowDialog();
                            break;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (result[0] == '0')
                        {
                            MessageBox.Show("Sai password ");
                            break;
                        }
                        else
                        {
                            if (result[0] == '-')
                            {
                                MessageBox.Show("Phòng yêu cầu hiện chưa được tạo");
                                break;
                            }
                            else
                            {
                                if (result[0] == '2')
                                {
                                    MessageBox.Show("Phòng đầy !");
                                    break;
                                }
                                else
                                    if (result[0] == '3')
                                    {
                                        MessageBox.Show("Tiền của bạn không đủ !");
                                        break;
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
        //Button Chơi ngay
        private void button5_Click(object sender, EventArgs e)
        {
            sendmess("g", stm);
            Thread.Sleep(500);
            while (true)
            {
                try
                {
                    byteReceive = new byte[82];
                    int k = stm.Read(byteReceive, 0, 82);
                    string result = "";
                    for (int i = 0; i < k; i++)
                    {
                        result += Convert.ToChar(byteReceive[i]);
                    }
                    if (result[0] == '1')
                    {
                        if (result[1] == '3')
                        {
                            int room;
                            string numroom = "";
                            for (int i = 2; result[i] != '@'; i++)
                            {
                                numroom += result[i];
                            }
                            room = Convert.ToInt32(numroom);
                            game = new SUDOKU3x3();
                            SUDOKU3x3.room = room;
                            game.ShowDialog();
                            break;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (result[0] == '0')
                        {
                            MessageBox.Show("Bạn không đủ tiền để chơi !");
                            break;
                        }
                    }
                } 
                catch
                {

                }
            }
        }
        //Các phần liên quan tới tắt
        private void Control1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ASCIIEncoding encode = new ASCIIEncoding();
                byteSend = encode.GetBytes("x");
                stm.Write(byteSend, 0, byteSend.Length);
                tcpclnt.Close();
                if (tcpclnt2.Connected)
                    tcpclnt2.Close();
            }
            catch
            {
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult exit;
            exit = MessageBox.Show("Bạn có muốn thoát?", "Thoát", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (exit == DialogResult.OK)
            {
                try
                {
                    sendmess("x", stm);
                    tcpclnt.Close();
                    if (tcpclnt2.Connected)
                        tcpclnt2.Close();
                }
                catch
                {
                }
                this.Close();
                stopthread = 1;
            }
        }
        static public void Exit()
        {
            Application.Exit();
        }
        #endregion

    }
}
