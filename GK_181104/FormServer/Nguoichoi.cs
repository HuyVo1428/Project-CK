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
                        if (checkrequest[1] == '4')
                        {
                            arr = new int[16, 16];
                            //Phat sinh mảng
                            phatsinh16 phatsinh = new phatsinh16();
                            arr = phatsinh.phatsinh();
                            //Gửi mảng về cho Client
                            ASCIIEncoding encode = new ASCIIEncoding();
                            for (int i = 0; i < 16; i++)
                                for (int j = 0; j < 16; j++)
                                {
                                    if (arr[i, j] < 10)
                                        sk.Send(encode.GetBytes("o" + Convert.ToString(arr[i, j])));
                                    else
                                        sk.Send(encode.GetBytes("t" + Convert.ToString(arr[i, j])));
                                }
                        }
                        else
                        {
                            if (checkrequest[1] == '3')
                            {
                                arr = new int[9, 9];
                                //Phat sinh mảng
                                Phatsinh9 phatsinh = new Phatsinh9();
                                arr = phatsinh.phatsinh();
                                //Gửi mảng về cho Client
                                ASCIIEncoding encode = new ASCIIEncoding();
                                for (int i = 0; i < 9; i++)
                                    for (int j = 0; j < 9; j++)
                                        sk.Send(encode.GetBytes(Convert.ToString(arr[i, j])));
                            }
                        }
                    }

                    #endregion
                    else
                    {
                        if (checkrequest[0] == '?')
                        {
                            #region Trường hợp yêu cầu là ?(Kiểm tra)
                            if (checkrequest[1] == '3')
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
                                    sk.Send(encode.GetBytes(Convert.ToString(check3(arr, toadox, toadoy))));

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

                            else
                            {
                                #region Xử lý yêu cầu kiểm tra của Game4x4
                                //Xac định vị trí phần tử kiểm tra, cấu trúc request ?4<Toadohang>|<toadocot>|<giatri>x
                                int toadox, toadoy;
                                string temp = "";
                                int i = 2;
                                //Xác định tọa độ x
                                for (;;)
                                {
                                    if (bytereceive[i] != '|')
                                    {
                                        temp += Convert.ToChar(bytereceive[i]);
                                        i++;
                                    }
                                    else
                                    {
                                        toadox = Convert.ToInt32(temp);
                                        i++;
                                        break;
                                    }
                                }
                                //Xác định tọa độ y
                                temp = "";
                                for (;;)
                                {
                                    if (bytereceive[i] != '|')
                                    {
                                        temp += Convert.ToChar(bytereceive[i]);
                                        i++;
                                    }
                                    else
                                    {
                                        toadoy = Convert.ToInt32(temp);
                                        i++;
                                        break;
                                    }
                                }
                                //Xác định giá trị
                                temp = "";
                                for (;;)
                                {
                                    if (bytereceive[i] != 'x')
                                    {
                                        temp += Convert.ToChar(bytereceive[i]);
                                        i++;
                                    }
                                    else
                                    {
                                        arr[toadox, toadoy] = Convert.ToInt32(temp);
                                        break;
                                    }
                                }
                                //Check và gửi kết quả cho Client
                                if (arr[toadox, toadoy] != 0)
                                {
                                    ASCIIEncoding encode = new ASCIIEncoding();
                                    sk.Send(encode.GetBytes(Convert.ToString(check4(arr, toadox, toadoy))));
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
                                for (; checkrequest[i] != '|';i++)
                                    ServerGame.phongchoi[room].pw += checkrequest[i];
                                if (checkrequest[i] == '2')
                                {
                                    #region Xử lý tạo phòng 2x2
                                    #endregion
                                }
                                else
                                {
                                    if (checkrequest[i] == '3')
                                    {
                                        #region Xử lý tạo phòng 3x3
                                        #endregion
                                    }
                                    else
                                        if (checkrequest[i] == '4')
                                    {
                                        #region Xử lý tạo phòng 4x4
                                        #endregion
                                    }
                                }
                                ASCIIEncoding encode = new ASCIIEncoding();
                                sk.Send(encode.GetBytes("1 room index= " + room.ToString() + " pw= " +ServerGame.phongchoi[room].pw));
                                //    Thread.Sleep(500);
                                //for(int j=0;i<20;j++)
                                //    {
                                //        if (ServerGame.Ngchoi[j].sk.Connected)
                                //            sk.Send(encode.GetBytes("i"+"Phòng "+room.ToString()+ " đã được tạo. Số người hiện tại: "+
                                //                ServerGame.phongchoi[room].count.ToString()));
                                //    }
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
                                        for(;checkrequest[i]!='|';i++)
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
                                            && ServerGame.phongchoi[Convert.ToInt32(tempr)].count<2)
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
                                    }
                                        
                                    #endregion
                                    else
                                    {
                                        if (checkrequest[0] == 'g')
                                        #region TH là vào ngay
                                        {
                                            int check = 0;
                                            #region Check có room chờ hay không
                                            for (int i=0;i<15;i++)
                                                if(ServerGame.phongchoi[i].active==1&&ServerGame.phongchoi[i].count<2
                                                    &&ServerGame.phongchoi[i].pw=="")
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
                                            if (check==0)
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
