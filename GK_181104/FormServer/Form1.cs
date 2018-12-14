using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace FormServer
{
    public partial class ServerGame : Form
    {
        public ServerGame()
        {
            InitializeComponent();
        }
        #region Khai báo các biến cơ bản
        public static int stop = 0;
        public static IPAddress ipAd;
        public static TcpListener server;
        public static TcpListener server2;
        public static int count = 0;
        public static string[] userlogin;
        public static string[] user= { "admin", "hung", "huy" };
        public static string[] password= { "admin", "hung", "huy" };
        public static Nguoichoi[] Ngchoi;
        public static Room[] phongchoi;
        #endregion
        //Lắng nghe và chấp nhận kết nối
        private void listen()
        {
            while (stop == 0)
            {
                try
                {
                    if (count == 11)
                        break;
                    //Chấp nhận kết nối và bật thread kiểm tra login(Nhanuser)
                    Ngchoi[count].sk = server.AcceptSocket();
                    Ngchoi[count].sk2 = server2.AcceptSocket();
                    count++;
                    CheckForIllegalCrossThreadCalls = false;
                    Thread t = new Thread(Nhanuser);
                    t.IsBackground = true;
                    t.Start(count - 1);
                }
                catch(SocketException)
                {
                }
            }
        }
        //Kiểm tra thông tin login
        private void Nhanuser(object x)
        {
            int resultcheck = -1;
            while (resultcheck != 1)
            {
                try
                {
                    //Nhận thông tin username
                    int y = (Int32)x;
                    byte[] bytereceive = new byte[100];
                    int k = Ngchoi[y].sk.Receive(bytereceive);
                    string checkuser = "";
                    for (int i = 0; i < k; i++)
                    {
                        checkuser += Convert.ToChar(bytereceive[i]);
                    }
                    //Nhận thông tin password
                    bytereceive = new byte[100];
                    k = Ngchoi[y].sk.Receive(bytereceive);
                    string checkpass = "";
                    for (int i = 0; i < k; i++)
                    {
                        checkpass += Convert.ToChar(bytereceive[i]);
                    }
                    //Kiểm tra khớp username, password
                    resultcheck = -1;
                    for (int i = 0; i < 3; i++)
                    {
                        if (user[i] == checkuser && password[i] == checkpass)
                        {
                            for (int j = 0; j < 10; j++)
                            {
                                if (checkuser == userlogin[j])
                                {
                                    resultcheck = 0;
                                    break;
                                }
                                resultcheck = 1;
                            }
                            break;
                        }
                        else
                        {
                            resultcheck = -1;
                        }
                    }
                    //Thông báo cho Client kết quả kiểm tra
                    ASCIIEncoding encode = new ASCIIEncoding();
                    Ngchoi[y].sk.Send(encode.GetBytes(Convert.ToString(resultcheck)));
                    //Nếu username, password khớp thì bật thread Communicate, ngược lại ngắt kết nối
                    if (resultcheck == 1)
                    {
                        DateTime now = DateTime.Now;
                        inforBox.Text += now + " User " + checkuser + " has logon" + Environment.NewLine;
                        Ngchoi[y].username = checkuser;
                        userlogin[y] = checkuser;
                        Thread t2 = new Thread(Ngchoi[y].check);
                        t2.IsBackground = true;
                        t2.Start(y);
                    }
                }
                catch
                {
                }
            }         
        }       
        //Khởi tạo server, và chạy listen
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Start Server")
            {
                #region Nút listen
                //Các khởi tạo cơ bản
                stop = 0;
                //socket = new Socket[10];
                userlogin = new string[10];
                Ngchoi = new Nguoichoi[20];
                for (int i = 0; i < 20; i++)
                    Ngchoi[i] = new Nguoichoi();
                phongchoi = new Room[15];
                for (int i = 0; i < 15; i++)
                    phongchoi[i] = new Room();
                ipAd = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(ipAd, 13000);
                server2 = new TcpListener(ipAd, 13001);
                server.Start();
                server2.Start();
                //Chạy thread listen và nhảy thông báo textbox
                Thread t = new Thread(listen);
                t.Start();
                button1.Text = "Shutdown Server";
                button1.BackColor = Color.Red;
                DateTime now = DateTime.Now;
                inforBox.Text += now + " Server has Started!"+Environment.NewLine;
                #endregion
            }
            else
            {
                #region Nút Stop
                stop = 1;
                for (int i=0; i<count;i++)
                {
                    if (Ngchoi[i].sk.Connected)
                        Ngchoi[i].sk.Close();
                }
                server.Stop();
                count = 0;
                button1.Text = "Start Server";
                button1.BackColor = Color.Green;
                DateTime now = DateTime.Now;
                inforBox.Text += now + "Server has Stoped!" + Environment.NewLine;
                #endregion
            }
        }
        //Tắt Server
        private void ServerGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int i = 0; i < count; i++)
                {
                    if (Ngchoi[i].sk.Connected)
                        Ngchoi[i].sk.Close();
                }
                server.Stop();
                stop = 1;
            }
            catch(Exception)
            {

            }
        }
    }
}
