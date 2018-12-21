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
        public Socket sk,sk2;
        public int room;
        public string username;
        public int tien;
        public int diem;
        public int stop = 0;
        int time = 30;
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
            tien = -1;
            diem = -1;
            stop = 0;
            time = 30;
        }
        public void sendmess(string infor, Socket sk)
        {
            for (; infor.Length < 82;)
                infor += '@';
            ASCIIEncoding encode = new ASCIIEncoding();
            sk.Send(encode.GetBytes(infor));
        }
        public void outroom(int y)
        {
            if (y != ServerGame.phongchoi[room].index2)
            {
                ServerGame.phongchoi[room].index1 = -1;
                ServerGame.phongchoi[room].count--;                               
            }
            else
            {
                ServerGame.phongchoi[room].index2 = -1;
                ServerGame.phongchoi[room].count--;
            }
            if (ServerGame.phongchoi[room].count == 0)
            {
                ServerGame.phongchoi[room].active = 0;
                ServerGame.phongchoi[room].pw = "";
                ServerGame.phongchoi[room].loaiphong = 0;
            }
            else
            {
                for (int j = 0; j < 20; j++)
                {
                    if (ServerGame.Ngchoi[j].username != "")
                        sendmess("iRom "+room.ToString() +" host in room: 1", ServerGame.Ngchoi[j].sk2);
                }
            }
            room = -1;
            diem = -1;

        }
        public void Timeout(object x)
        {
            int y = (Int32)x;
            time = 30;
            int indexdoithu=-1;
            int indexroom = room;
            if (y != ServerGame.phongchoi[room].index2)
                indexdoithu = ServerGame.phongchoi[room].index2;
            else
                indexdoithu = ServerGame.phongchoi[room].index1;
            #region gửi thông báo thời gian 
            while (time!=0&&ServerGame.Ngchoi[indexdoithu].time!=0)
            {
                ASCIIEncoding encode = new ASCIIEncoding();
                string send = "t";
                sendmess(send, sk);
                sendmess(send, ServerGame.Ngchoi[indexdoithu].sk);
                time--;
                Thread.Sleep(1000);
            }
            #endregion
            #region Thông báo hết giờ
            if (ServerGame.phongchoi[indexroom].count==2)
            #region Th còn 2 người trong phòng
            {
                if (diem > ServerGame.Ngchoi[indexdoithu].diem)
                {
                    tien = tien + 500;
                    ServerGame.Ngchoi[indexdoithu].tien -= 500;
                    string send = "f1" + diem.ToString() + "|" + ServerGame.Ngchoi[indexdoithu].diem.ToString() + "|" + tien.ToString();
                    sendmess(send, sk);
                    send = "f0" + ServerGame.Ngchoi[indexdoithu].diem.ToString() + "|" + diem.ToString()
                        + "|" + ServerGame.Ngchoi[indexdoithu].tien.ToString() + "@c";
                    sendmess(send, ServerGame.Ngchoi[indexdoithu].sk);
                    send = "+" + tien.ToString();
                    sendmess(send, sk2);
                    send = "+" + ServerGame.Ngchoi[indexdoithu].tien.ToString();
                    sendmess(send, ServerGame.Ngchoi[indexdoithu].sk2);
                    if (ServerGame.Ngchoi[indexdoithu].tien < 500)
                        ServerGame.Ngchoi[indexdoithu].outroom(indexdoithu);
                }
                else
                {
                    if (diem < ServerGame.Ngchoi[indexdoithu].diem)
                    {
                        tien = tien - 500;
                        ServerGame.Ngchoi[indexdoithu].tien += 500;
                        string send = "f0" + diem.ToString() + "|" + ServerGame.Ngchoi[indexdoithu].diem.ToString() + "|"
                            + tien.ToString();
                        sendmess(send, sk);
                        send = "f1" + ServerGame.Ngchoi[indexdoithu].diem.ToString() + "|" + diem.ToString()
                            + "|" + ServerGame.Ngchoi[indexdoithu].tien.ToString() + "@c";
                        sendmess(send, ServerGame.Ngchoi[indexdoithu].sk);
                        send = "+" + tien.ToString();
                        sendmess(send, sk2);
                        send = "+" + ServerGame.Ngchoi[indexdoithu].tien.ToString();
                        sendmess(send, ServerGame.Ngchoi[indexdoithu].sk2);
                        if (tien < 500)
                            outroom(y);
                    }
                    else
                    {
                        string send = "f2" + diem.ToString() + "|" + ServerGame.Ngchoi[indexdoithu].diem.ToString();
                        sendmess(send, sk);
                        send = "f2" + ServerGame.Ngchoi[indexdoithu].diem.ToString() + "|" + diem.ToString() + "@c";
                        sendmess(send, ServerGame.Ngchoi[indexdoithu].sk);
                    }
                }
            }
            #endregion
            else
            #region Th 1 người đã out
            {
                string send = "";
                tien = tien - 500;
                ServerGame.Ngchoi[indexdoithu].tien += 500;
                send = "f1" + ServerGame.Ngchoi[indexdoithu].diem.ToString() + "|" + diem.ToString()
                    + "|" + ServerGame.Ngchoi[indexdoithu].tien.ToString() + "@c";
                sendmess(send, ServerGame.Ngchoi[indexdoithu].sk);
                send = "+" + tien.ToString();
                sendmess(send, sk2);
                send = "+" + ServerGame.Ngchoi[indexdoithu].tien.ToString();
                sendmess(send, ServerGame.Ngchoi[indexdoithu].sk2);
            }
            #endregion
            #endregion
        }
        public void check(object x)
        {
            int y = (Int32)x;
            while (true)
            {
                try
                {
                    #region Nhận thông tin từ Client 
                    string checkrequest = "";
                    byte[] bytereceive = new byte[82];
                    if (stop == 0)
                    {
                        int k = sk.Receive(bytereceive);
                        for (int i = 0; i < k; i++)
                        {
                            checkrequest += Convert.ToChar(bytereceive[i]);
                        }
                    }
                    #endregion
                    #region Kiểm tra yêu cầu của Client (!: khởi tạo mảng ?:Check kiểm tra x:Ngắt kết nối)
                    if (checkrequest[0] == '!')
                    {
                        #region Truòng hợp yêu cầu là !(phát sinh mảng)
                        //Check size của mảng
                        if (ServerGame.phongchoi[room].loaiphong==3)
                        {
                            ServerGame.phongchoi[room].arr = new int[9, 9];
                            //Phat sinh mảng
                            Phatsinh9 phatsinh = new Phatsinh9();
                            ServerGame.phongchoi[room].arr = phatsinh.phatsinh();
                            //Gửi mảng về cho Client
                            ASCIIEncoding encode = new ASCIIEncoding();
                            string send = "c";
                            for (int i = 0; i < 9; i++)
                                for (int j = 0; j < 9; j++)
                                    send += ServerGame.phongchoi[room].arr[i, j].ToString();
                            sk.Send(encode.GetBytes(send));
                            if(y!=ServerGame.phongchoi[room].index2)
                                ServerGame.Ngchoi[ServerGame.phongchoi[room].index2].sk.Send(encode.GetBytes(send));
                            else
                                ServerGame.Ngchoi[ServerGame.phongchoi[room].index1].sk.Send(encode.GetBytes(send));
                            //Set điểm
                            ServerGame.Ngchoi[ServerGame.phongchoi[room].index1].diem = 500;
                            ServerGame.Ngchoi[ServerGame.phongchoi[room].index2].diem = 500;
                            //Bật thread tính giờ
                            ServerGame.Ngchoi[ServerGame.phongchoi[room].index1].time = 30;
                            ServerGame.Ngchoi[ServerGame.phongchoi[room].index2].time = 30;
                            Thread t = new Thread(Timeout);
                            t.Start(y);
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
                                    #region Th là giá trị bình thường
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
                                    if (ServerGame.phongchoi[room].arr[toadox, toadoy] == 0)
                                    {
                                        ServerGame.phongchoi[room].arr[toadox, toadoy] = Convert.ToInt32(temp);
                                        //Check và gửi về
                                        string send = "o" + toadox.ToString() + toadoy.ToString() + ServerGame.phongchoi[room].arr[toadox, toadoy].ToString();
                                        if (y != ServerGame.phongchoi[room].index2)
                                        {
                                            sendmess(send, ServerGame.Ngchoi[ServerGame.phongchoi[room].index2].sk);
                                        }
                                        else
                                        {
                                            sendmess(send, ServerGame.Ngchoi[ServerGame.phongchoi[room].index1].sk);
                                        }
                                        send = "r" + check3(ServerGame.phongchoi[room].arr, toadox, toadoy).ToString();
                                        sendmess(send, sk);
                                        //kiểm tra tính điểm
                                        if (check3(ServerGame.phongchoi[room].arr, toadox, toadoy) == 1)
                                            diem = diem + 100;
                                        else
                                        {
                                            diem = diem - 100;
                                            if (diem < 0)
                                                time = 0;
                                        }
                                    }
                                    #endregion
                                }
                                #region Trường hợp là nút xóa, cập nhật giá trị 0
                                else
                                {
                                    int toadox, toadoy;
                                    string temp = "";
                                    temp += Convert.ToChar(bytereceive[2]);
                                    toadox = Convert.ToInt32(temp);
                                    temp = "";
                                    temp += Convert.ToChar(bytereceive[3]);
                                    toadoy = Convert.ToInt32(temp);
                                    ServerGame.phongchoi[room].arr[toadox, toadoy] = 0;
                                    //tính điểm vàa gửi về
                                    diem = diem - 200;
                                    if (diem < 0)
                                        time = 0;
                                    else
                                    {
                                        string send = "o" + toadox.ToString() + toadoy.ToString() + "x";
                                        if (y != ServerGame.phongchoi[room].index2)
                                        {
                                            sendmess(send, ServerGame.Ngchoi[ServerGame.phongchoi[room].index2].sk);
                                        }
                                        else
                                        {
                                            sendmess(send, ServerGame.Ngchoi[ServerGame.phongchoi[room].index1].sk);
                                        }
                                    }
                                }
                                #endregion
                                #endregion
                            }
                        }
                        #endregion
                        else
                        {
                            if (checkrequest[0] == 'x')
                            {
                                #region Th yêu cầu là x(thoat game)

                                DateTime now = DateTime.Now;
                                sk.Close();
                                sk2.Close();
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
                                    #region check pass và loại phòng
                                    int i = 1;
                                    string temppw = "";
                                    string temploaiphong = "";
                                    for (; checkrequest[i] != '|'; i++)
                                        temppw += checkrequest[i];
                                    i++;
                                    int checktien = -1;
                                    if (checkrequest[i] == '2')
                                    {
                                        #region Xử lý tạo phòng 2x2
                                        if (tien >= 200)
                                            checktien = 1;
                                        temploaiphong += checkrequest[i];                                   
                                        #endregion
                                    }
                                    else
                                    {
                                        if (checkrequest[i] == '3')
                                        {
                                            #region Xử lý tạo phòng 3x3
                                            if (tien >= 500)
                                                checktien = 1;
                                            temploaiphong += checkrequest[i];
                                            //MessageBox.Show(ServerGame.phongchoi[room].loaiphong.ToString());
                                            #endregion
                                        }
                                    }
                                    #endregion
                                    #region kiểm tra tiền và Tạo phòng
                                    if (checktien == 1)
                                    {
                                        Random rd = new Random();
                                        while (room == -1 || ServerGame.phongchoi[room].active == 1)
                                        {
                                            room = rd.Next(0, 14);
                                            ServerGame.phongchoi[room] = new Room();
                                        }
                                        ServerGame.phongchoi[room].index1 = y;
                                        ServerGame.phongchoi[room].active = 1;
                                        ServerGame.phongchoi[room].count++;
                                        ServerGame.phongchoi[room].pw = temppw;
                                        ServerGame.phongchoi[room].loaiphong = Convert.ToInt32(temploaiphong);
                                        //MessageBox.Show(ServerGame.phongchoi[room].loaiphong.ToString());
                                        ASCIIEncoding encode = new ASCIIEncoding();
                                        string send = "1"+ ServerGame.phongchoi[room].loaiphong.ToString()+ "room index: " + room.ToString() + " password: " + ServerGame.phongchoi[room].pw;
                                        sendmess(send, sk);
                                        send = "iRoom " + room.ToString()+" " +ServerGame.phongchoi[room].loaiphong.ToString()+"x"+ ServerGame.phongchoi[room].loaiphong.ToString() + " has created. " + "Host in room: " + ServerGame.phongchoi[room].count.ToString() + ".";
                                        for (int j = 0; j < 20; j++)
                                        {
                                            if (ServerGame.Ngchoi[j].username != "")
                                                sendmess(send, ServerGame.Ngchoi[j].sk2);
                                        }
                                    }
                                    else
                                    {
                                        ASCIIEncoding encode = new ASCIIEncoding();
                                        sendmess("0", sk);
                                    }
                                    #endregion                                                             
                                }
                                #endregion
                                else
                                {
                                    if (checkrequest[0] == 'j')
                                    #region Th yêu cầu là vào phòng
                                    {
                                        //Kiểm tra phòng
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
                                            if (ServerGame.phongchoi[Convert.ToInt32(tempr)].loaiphong == 2)
                                            {
                                                if (tien >= 200)
                                                {
                                                    result = 1;
                                                    room = Convert.ToInt32(tempr);
                                                    if (ServerGame.phongchoi[room].index2 == -1)
                                                    {
                                                        ServerGame.phongchoi[room].index2 = y;
                                                    }
                                                    else
                                                    {
                                                        ServerGame.phongchoi[room].index1 = y;
                                                    }
                                                    ServerGame.phongchoi[room].count++;
                                                }
                                                else
                                                    result = 3;
                                            }
                                            else
                                            {
                                                if (ServerGame.phongchoi[Convert.ToInt32(tempr)].loaiphong == 3)
                                                {
                                                    if (tien >= 500)
                                                    {
                                                        result = 1;
                                                        room = Convert.ToInt32(tempr);
                                                        if (ServerGame.phongchoi[room].index2 == -1)
                                                        {
                                                            ServerGame.phongchoi[room].index2 = y;
                                                        }
                                                        else
                                                        {
                                                            ServerGame.phongchoi[room].index1 = y;
                                                        }
                                                        ServerGame.phongchoi[room].count++;
                                                    }
                                                    else
                                                        result = 3;
                                                }
                                            }

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
                                        //Gửi về kết quả                                       
                                        if (result == 1)
                                        {
                                            ASCIIEncoding encode = new ASCIIEncoding();
                                            string send = result.ToString()+ServerGame.phongchoi[room].loaiphong.ToString() + room.ToString();
                                            sendmess(send, sk);
                                            if (y == ServerGame.phongchoi[room].index2)
                                            {
                                                send = "N" + ServerGame.userlogin[y] + ".x";
                                                sendmess(send, ServerGame.Ngchoi[ServerGame.phongchoi[room].index1].sk);
                                                send = "N" + ServerGame.userlogin[y] + ".";
                                                sendmess(send, ServerGame.Ngchoi[ServerGame.phongchoi[room].index2].sk);
                                                send = "iRoom " + room.ToString() + " has Full.";
                                            }
                                            else
                                            {
                                                send = "N" + ServerGame.userlogin[y] + ".x";
                                                sendmess(send, ServerGame.Ngchoi[ServerGame.phongchoi[room].index2].sk);
                                                send = "N" + ServerGame.userlogin[y] + ".";
                                                sendmess(send, ServerGame.Ngchoi[ServerGame.phongchoi[room].index1].sk);
                                                send = "iRoom " + room.ToString() + " has Full.";
                                            }
                                            for (int j = 0; j < 20; j++)
                                            {
                                                if (ServerGame.Ngchoi[j].username != "")
                                                    sendmess(send, ServerGame.Ngchoi[j].sk2);
                                            }
                                        }
                                        else
                                        {
                                            ASCIIEncoding encode = new ASCIIEncoding();
                                            string send = result.ToString();
                                            sendmess(send, sk);
                                        }
                                    }

                                    #endregion
                                    else
                                    {
                                        if (checkrequest[0] == 'g')
                                        #region TH là vào ngay
                                        {
                                            int check = 0;
                                            
                                            if (tien >= 200)
                                            {
                                                #region Check có room chờ hay không
                                                for (int i = 0; i < 15; i++)
                                                    if (ServerGame.phongchoi[i].active == 1 && ServerGame.phongchoi[i].count < 2
                                                        && ServerGame.phongchoi[i].pw == "")
                                                    {
                                                        if (ServerGame.phongchoi[i].loaiphong == 2)
                                                        {
                                                            check = 1;
                                                            room = i;
                                                            int indexdoithu;
                                                            if (ServerGame.phongchoi[room].index2 == -1)
                                                            {
                                                                ServerGame.phongchoi[room].index2 = y;
                                                                indexdoithu = ServerGame.phongchoi[room].index1;
                                                            }
                                                            else
                                                            {
                                                                ServerGame.phongchoi[room].index1 = y;
                                                                indexdoithu = ServerGame.phongchoi[room].index2;
                                                            }
                                                            ServerGame.phongchoi[room].count++;
                                                            ASCIIEncoding encode = new ASCIIEncoding();
                                                            string send = "12" + room.ToString();
                                                            sendmess(send, sk);
                                                            send = "N" + ServerGame.userlogin[y] + ".x";
                                                            sendmess(send, ServerGame.Ngchoi[indexdoithu].sk);
                                                            send = "N" + ServerGame.userlogin[y] + ".";
                                                            sendmess(send, sk);
                                                            send = "iRoom " + room.ToString() + " has Full.";
                                                            for (int j = 0; j < 20; j++)
                                                            {
                                                                if (ServerGame.Ngchoi[j].username != "")
                                                                    sendmess(send, ServerGame.Ngchoi[j].sk2);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            if (ServerGame.phongchoi[i].loaiphong == 3 && tien >= 500)
                                                            {
                                                                check = 1;
                                                                room = i;
                                                                int indexdoithu;
                                                                if (ServerGame.phongchoi[room].index2 == -1)
                                                                {
                                                                    ServerGame.phongchoi[room].index2 = y;
                                                                    indexdoithu = ServerGame.phongchoi[room].index1;
                                                                }
                                                                else
                                                                {
                                                                    ServerGame.phongchoi[room].index1 = y;
                                                                    indexdoithu = ServerGame.phongchoi[room].index2;
                                                                }
                                                                ServerGame.phongchoi[room].count++;
                                                                string send = "13" + room.ToString();
                                                                sendmess(send, sk);
                                                                send = "N" + ServerGame.userlogin[y] + ".x";
                                                                sendmess(send, ServerGame.Ngchoi[indexdoithu].sk);
                                                                send = "N" + ServerGame.userlogin[y] + ".";
                                                                sendmess(send, sk);
                                                                send = "iRoom " + room.ToString() + " has Full.";
                                                                for (int j = 0; j < 20; j++)
                                                                {
                                                                    if (ServerGame.Ngchoi[j].username != "")
                                                                        sendmess(send, ServerGame.Ngchoi[j].sk2);
                                                                }
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                ASCIIEncoding encode = new ASCIIEncoding();
                                                                sendmess("0", sk);
                                                            }
                                                        }
                                                    }
                                                #endregion
                                                #region Th k có room nào đang chờ
                                                if (check == 0)
                                                {
                                                    Random rd = new Random();
                                                    while (room == -1 || ServerGame.phongchoi[room].active == 1)
                                                    {
                                                        room = rd.Next(0, 15);
                                                        ServerGame.phongchoi[room] = new Room();
                                                    }
                                                    ServerGame.phongchoi[room].index1 = y;
                                                    ServerGame.phongchoi[room].active = 1;
                                                    ServerGame.phongchoi[room].count++;
                                                    if(tien >=500)
                                                        ServerGame.phongchoi[room].loaiphong = 3;
                                                    else
                                                        ServerGame.phongchoi[room].loaiphong = 2;
                                                    ASCIIEncoding encode = new ASCIIEncoding();
                                                    string send = "1"+ ServerGame.phongchoi[room].loaiphong.ToString() + room.ToString();
                                                    sendmess(send, sk);
                                                    send = "iRoom " + room.ToString() +" "+ ServerGame.phongchoi[room].loaiphong.ToString() + "x" + ServerGame.phongchoi[room].loaiphong.ToString() + " has created. " + "Number user: " + ServerGame.phongchoi[room].count.ToString() + ".";
                                                    for (int j = 0; j < 20; j++)
                                                    {
                                                        if (ServerGame.Ngchoi[j].username != "")
                                                            sendmess(send, ServerGame.Ngchoi[j].sk2);                                                    }

                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                sendmess("0", sk);
                                            }
                                        }
                                        #endregion
                                        else                                        
                                        {
                                            if (checkrequest[0] == 'n')
                                            #region Th là chơi tiếp
                                            {
                                                int indexdoithu=0;
                                                if (y != ServerGame.phongchoi[room].index2 && ServerGame.phongchoi[room].index2 != -1)
                                                {
                                                    indexdoithu = ServerGame.phongchoi[room].index2;
                                                    sendmess("n", ServerGame.Ngchoi[indexdoithu].sk);
                                                }

                                                else
                                                    if (y == ServerGame.phongchoi[room].index2 && ServerGame.phongchoi[room].index1 != -1)
                                                {
                                                    indexdoithu = ServerGame.phongchoi[room].index1;
                                                    sendmess("n", ServerGame.Ngchoi[indexdoithu].sk);
                                                }
                                            }
                                            #endregion
                                            else
                                            {
                                                #region Th là out room
                                                if (checkrequest[0] == 'q')
                                                {
                                                    if (time != 0)
                                                    {
                                                        diem = -1;
                                                        outroom(y);
                                                        time = 0;
                                                    }
                                                    else
                                                    {
                                                        outroom(y);
                                                    }

                                                }
                                                #endregion
                                            }
                                        }

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
