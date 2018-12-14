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
        public static Stream stm;
        public static byte[] byteSend;
        public static byte[] byteReceive;
        Form game=new SUDOKU3x3();
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
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            usernamebox.Focus();
            PlayerStart.PlayLooping();
            //Khởi tạo kết nối
            tcpclnt = new TcpClient();
            tcpclnt.Connect("127.0.0.1", 13000);
            stm = tcpclnt.GetStream();
        }
        /*Nút Login-(Khởi tạo kết nối + Gửi username, password + Check kết quả login từ server 
        + Chuyển màn hình: Ẩn cửa texbox nút dùng login, hiện nút NEW, EXIT)*/
        #endregion
        #region Xử lý event
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
                    ASCIIEncoding encode = new ASCIIEncoding();
                    byteSend = new byte[100];
                    byteSend = encode.GetBytes(usernamebox.Text);
                    stm.Write(byteSend, 0, byteSend.Length);
                    Thread.Sleep(150);
                    byteSend = encode.GetBytes(passwordbox.Text);
                    stm.Write(byteSend, 0, byteSend.Length);
                    //Nhận thông tin trả về từ Server
                    byteReceive = new byte[100];
                    int k = stm.Read(byteReceive, 0, 100);
                    string result = "";
                    for (int i = 0; i < k; i++)
                    {
                        result += Convert.ToChar(byteReceive[i]);
                    }
                    //Check kết quả login và chuyển màn hình
                    if (result == "1")
                    {
                        CheckForIllegalCrossThreadCalls = false;
                        usernamebox.Visible = false;
                        passwordbox.Visible = false;
                        btnExit.Visible = false;
                        button1.Visible = false;
                        button3.Visible = true;
                        button4.Visible = true;
                        button2.Visible = true;
                        button5.Visible = true;
                        label1.Visible = false;
                        label2.Visible = false;
                        lbLogin.Text = "Xin chào, " + usernamebox.Text;
                    }
                    else
                    {
                        if (result == "0")
                        {
                            MessageBox.Show("Tài khoản đang được đăng nhập trên phiên làm việc khác.\nĐăng xuất khỏi các phiên và thử lại.","Đăng nhập Thất bại",MessageBoxButtons.OK,MessageBoxIcon.Warning);
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
                MessageBox.Show("Đã có lỗi do Server!\nVui lòng thử lại sau.","Lỗi Server",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        //Nút New-(mở formGAME)
        private void button2_Click(object sender, EventArgs e)
        {
            //Tắt game hiện hành
            try
            {
                //Gửi yêu cầu tạo mảng 3x3
                ASCIIEncoding encode = new ASCIIEncoding();
                Control1.byteSend = encode.GetBytes("!3");
                Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
                Thread.Sleep(500);
           //Load game 3x3
                game = new SUDOKU3x3();
                game.Show();
                this.Visible = false;
                
            }
            catch
            {
            }
        }
        private void Control1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ASCIIEncoding encode = new ASCIIEncoding();
                byteSend = encode.GetBytes("x");
                stm.Write(byteSend, 0, byteSend.Length);
                tcpclnt.Close();
            }
            catch
            {
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult exit;
            exit = MessageBox.Show("Bạn có muốn thoát?", "Thoát", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (exit==DialogResult.OK)
            {
                try
                {

                    ASCIIEncoding encode = new ASCIIEncoding();
                    byteSend = encode.GetBytes("x");
                    stm.Write(byteSend, 0, byteSend.Length);
                    tcpclnt.Close();
                    
                }
                catch
                {
                }
                this.Close();
            }
        }
        static public void Exit()
        {
            Application.Exit();
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

        private void button3_Click(object sender, EventArgs e)
        {
            Taophong tp = new Taophong();
            tp.ShowDialog();
            Thread.Sleep(200);
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
                    string showresult = "" ;
                    for (int i = 1; result[i]!='@'; i++)
                    {
                        showresult += result[i];
                    }
                    MessageBox.Show("Tạo phòng thành công " + showresult);
                }                   
            }
            catch
            {
            }
            SUDOKU3x3 game = new SUDOKU3x3();
            game.ShowDialog();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            vaophong vp = new vaophong();
            vp.ShowDialog();
            Thread.Sleep(200);
            try
            {
                byteReceive = new byte[1];
                int k = stm.Read(byteReceive, 0, 1);
                string result = "";
                for (int i = 0; i < k; i++)
                {
                    result += Convert.ToChar(byteReceive[i]);
                }
                if (result[0] == '1')
                {
                    //MessageBox.Show("Đã vào phòng ");
                    SUDOKU3x3 game = new SUDOKU3x3();
                    game.ShowDialog();
                }
                else
                {
                    if (result[0] == '0')
                        MessageBox.Show("Sai password ");
                    else
                    {
                        if (result[0] == '-')
                            MessageBox.Show("Phòng yêu cầu hiện chưa được tạo");
                        else
                            if (result[0] == '2')
                            MessageBox.Show("Phòng đầy !");

                    }
                }
            }
            catch
            {

            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                ASCIIEncoding encode = new ASCIIEncoding();
                Control1.byteSend = new byte[1];
                Control1.byteSend = encode.GetBytes("g");
                Control1.stm.Write(Control1.byteSend, 0, Control1.byteSend.Length);
            }
            catch(SocketException)
            {

            }
            Thread.Sleep(500);
            try
            {
                byteReceive = new byte[100];
                int k = stm.Read(byteReceive, 0, 100);
                string result = "";
                for (int i = 0; i < k; i++)
                {
                    result += Convert.ToChar(byteReceive[i]);
                }
                if (result[0] == '1')
                    MessageBox.Show("Đã vào phòng "+ result);                
            }
            catch
            {

            }
        }
        #endregion

    }
}
