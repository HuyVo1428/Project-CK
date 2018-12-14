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
using FormServer.Properties;

namespace FormServer
{
    public partial class Nguoichoi
    {
        public Socket sk;
        public int room;
        public string username;
        public int[,] arr;
        #region Các Hàm check
        public int check3(int[,] arr, int x, int y)
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
        public int check4(int[,] arr, int x, int y)
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
        #endregion
        public  Nguoichoi()
        {
            room = -1;
            username ="";
            arr = new int [16, 16];
        }
        public void check(object x)
        {
            int y = (Int32)x;
            while (ServerGame.stop == 0)
            {
                try
                {
                    //Nhận thông tin từ Client                    
                    byte[] bytereceive = new byte[100];
                    int k = sk.Receive(bytereceive);
                    string checkrequest = "";
                    for (int i = 0; i < k; i++)
                    {
                        checkrequest += Convert.ToChar(bytereceive[i]);
                    }
                    #region Kiểm tra yêu cầu của Client (!: khởi tạo mảng ?:Check kiểm tra x:Ngắt kết nối)
                    if (checkrequest[0] == '!')
                    {
                        #region Truòng hợp yêu cầu là !(phát sinh mảng)
                        //Check size của mảng
                        if (ServerGame.phongchoi[room].loaiphong==3)
                        {
                            arr = new int[9, 9];
                            //Phat sinh mảng
                            Phatsinh9 phatsinh = new Phatsinh9();
                            arr = phatsinh.phatsinh();
                            //Gửi mảng về cho Client
                            ASCIIEncoding encode = new ASCIIEncoding();
                            string send = "c";
                            for (int i = 0; i < 9; i++)
                                for (int j = 0; j < 9; j++)
                                    send += arr[i, j].ToString();
                            sk.Send(encode.GetBytes(send));
                            ServerGame.Ngchoi[ServerGame.phongchoi[room].index2].sk.Send(encode.GetBytes(send));
                        }                        
                    }

                    #endregion
                    else
                    {
                        if (checkrequest[0] == '?')
                        {
                            #region Trường hợp yêu cầu là ?(Kiểm tra)
                            if (ServerGame.phongchoi[room].loaiphong==3)
                            {
                                #region Xử lý yêu cầu kiểm tra của Game3x3
                                if (Convert.ToChar(bytereceive[4]) != '0')
                                {
                                    //Xac định vị trí phần tử kiểm tra
                                    int toadox, toadoy;
                                    string temp = "";
                                    temp += Convert.ToChar(bytereceive[2]);
                                    toadox = Convert.ToInt32(temp);
                                    temp = "";
                                    temp += Convert.ToChar(bytereceive[3]);
                                    toadoy = Convert.ToInt32(temp);
                                    temp = "";
                                    temp += Convert.ToChar(bytereceive[4]);
                                    arr[toadox, toadoy] = Convert.ToInt32(temp);
                                    //Check
                                    ASCIIEncoding encode = new ASCIIEncoding();
                                    string send = check3(arr, toadox, toadoy).ToString();
                                    for (; send.Length < 82;)
                                        send += '@';
                                    sk.Send(encode.GetBytes(Convert.ToString(send)));
                                }
                                //Trường hợp là nút xóa, cập nhật giá trị 0
                                else
                                {
                                    int toadox, toadoy;
                                    string temp = "";
                                    temp += Convert.ToChar(bytereceive[2]);
                                    toadox = Convert.ToInt32(temp);
                                    temp = "";
                                    temp += Convert.ToChar(bytereceive[3]);
                                    toadoy = Convert.ToInt32(temp);
                                    arr[toadox, toadoy] = 0;
                                }
                                #endregion
                            }
                        }
                        #endregion
                        else
                        {
                            if (checkrequest[0] == 'x')
                            {
                                #region Th yêu cầu là x(ngắt kết nối)

                                DateTime now = DateTime.Now;
                                sk.Close();
                                //inforBox.Text += now + "User " + ServerGame.userlogin[y] + " has logout" + Environment.NewLine;
                                ServerGame.userlogin[y] = "";
                                break;
                                #endregion
                            }
                            else
                            {
                                if (checkrequest[0] == 'c')
                                #region Th yêu cầu là tạo phòng
                                {
                                    #region Tạo phòng
                                    Random rd = new Random();
                                    while (room == -1 || ServerGame.phongchoi[room].active == 1)
                                    {
                                        room = rd.Next(0, 16);
                                        ServerGame.phongchoi[room] = new Room();
                                    }
                                    ServerGame.phongchoi[room].index1 = y;
                                    ServerGame.phongchoi[room].active = 1;
                                    ServerGame.phongchoi[room].count++;
                                    #endregion
                                    #region tạo pass và loại phòng
                                    int i = 1;
                                    for (; checkrequest[i] != '|'; i++)
                                        ServerGame.phongchoi[room].pw += checkrequest[i];
                                    i++;
                                    if (checkrequest[i] == '2')
                                    {
                                        #region Xử lý tạo phòng 2x2
                                        ServerGame.phongchoi[room].loaiphong = 2;
                                        #endregion
                                    }
                                    else
                                    {
                                        if (checkrequest[i] == '3')
                                        {
                                            #region Xử lý tạo phòng 3x3
                                            ServerGame.phongchoi[room].loaiphong = 3;
                                            MessageBox.Show(ServerGame.phongchoi[room].loaiphong.ToString());
                                            #endregion
                                        }

                                    }
                                    ASCIIEncoding encode = new ASCIIEncoding();
                                    string send = "1room index: " + room.ToString() + " password: " + ServerGame.phongchoi[room].pw;
                                    for (; send.Length < 82;)
                                        send += '@';
                                    sk.Send(encode.GetBytes(send));
                                    #endregion
                                }
                                #endregion
                                else
                                {
                                    if (checkrequest[0] == 'j')
                                    #region Th yêu cầu là vào phòng
                                    {

                                        int i = 1;
                                        string tempr = "";
                                        string tempmk = "";
                                        for (; checkrequest[i] != '|'; i++)
                                        {
                                            tempr += checkrequest[i];
                                        }
                                        i++;
                                        for (; checkrequest[i] != '|'; i++)
                                        {
                                            tempmk += checkrequest[i];
                                        }
                                        int result = -1;
                                        if (ServerGame.phongchoi[Convert.ToInt32(tempr)].active == 1
                                            && ServerGame.phongchoi[Convert.ToInt32(tempr)].pw == tempmk
                                            && ServerGame.phongchoi[Convert.ToInt32(tempr)].count < 2)
                                        {
                                            result = 1;
                                            room = Convert.ToInt32(tempr);
                                            ServerGame.phongchoi[room].index2 = y;
                                            ServerGame.phongchoi[room].count++;
                                        }
                                        else
                                        {
                                            if (ServerGame.phongchoi[Convert.ToInt32(tempr)].pw != tempmk)
                                                result = 0;
                                            else
                                            {
                                                if (ServerGame.phongchoi[Convert.ToInt32(tempr)].count == 2)
                                                    result = 2;
                                            }
                                        }
                                        ASCIIEncoding encode = new ASCIIEncoding();
                                        sk.Send(encode.GetBytes(result.ToString()));
                                        if (result == 1)
                                        {
                                            string send = "N" + ServerGame.userlogin[y] + ".x";
                                            for (; send.Length < 82;)
                                                send += '@';
                                            ServerGame.Ngchoi[ServerGame.phongchoi[room].index1].sk.Send(encode.GetBytes(send));
                                            send = "N" + ServerGame.userlogin[y] + ".";
                                            for (; send.Length < 82;)
                                                send += '@';
                                            ServerGame.Ngchoi[ServerGame.phongchoi[room].index2].sk.Send(encode.GetBytes(send));
                                        }
                                    }

                                    #endregion
                                    else
                                    {
                                        if (checkrequest[0] == 'g')
                                        #region TH là vào ngay
                                        {
                                            int check = 0;
                                            #region Check có room chờ hay không
                                            for (int i = 0; i < 15; i++)
                                                if (ServerGame.phongchoi[i].active == 1 && ServerGame.phongchoi[i].count < 2
                                                    && ServerGame.phongchoi[i].pw == "")
                                                {
                                                    check = 1;
                                                    room = i;
                                                    ServerGame.phongchoi[room].index2 = y;
                                                    ServerGame.phongchoi[room].count++;
                                                    ASCIIEncoding encode = new ASCIIEncoding();
                                                    sk.Send(encode.GetBytes(room.ToString()));
                                                    break;
                                                }
                                            #endregion
                                            #region Th k có room nào đang chờ
                                            if (check == 0)
                                            {
                                                Random rd = new Random();
                                                while (room == -1 || ServerGame.phongchoi[room].active == 1)
                                                {
                                                    room = rd.Next(0, 16);
                                                    ServerGame.phongchoi[room] = new Room();
                                                }
                                                ServerGame.phongchoi[room].index1 = y;
                                                ServerGame.phongchoi[room].active = 1;
                                                ServerGame.phongchoi[room].count++;
                                                //Thêm xử lý tạo màn 2x2
                                                ASCIIEncoding encode = new ASCIIEncoding();
                                                sk.Send(encode.GetBytes(room.ToString()));
                                            }
                                            #endregion
                                        }
                                        #endregion
                                    }
                                }
                            }
                        }
                        #endregion
                    }

                }
                catch
                {
                }
            }
        }
    }
}
