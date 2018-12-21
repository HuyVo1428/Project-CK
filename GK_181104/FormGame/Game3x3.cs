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
        #region khai bao
        System.Media.SoundPlayer PlayerStart = new System.Media.SoundPlayer(@"C:\Sudoku\media\sound01.wav");
        string checkresult = "";
        public int Check = 0;
        public static int room;
        public int point = 0;
        public int time=0;
        public int stopthread = 0;
        Thread t;
        #endregion
        public SUDOKU3x3()
        {
            InitializeComponent();
            

        }
        //Load
        private void Form1_Load(object sender, EventArgs e)
        {
            PlayerStart.PlayLooping();
            button_play.Enabled = false;
            label1.Text = "ROOM:" + room.ToString();
            stopthread = 0;
            t = new Thread(receive);
            t.IsBackground = true;
            t.Start();            
            Addevent();

        }
        //Thread receive
        private void receive()
        {
            while(stopthread==0)
            {
                try
                {
                    Control1.byteReceive = new byte[82];
                    Control1.stm.Read(Control1.byteReceive, 0, 82);
                    if (Convert.ToChar(Control1.byteReceive[0]) == 'N')
                    #region Xử lý người chơi vào phòng
                    {
                        textBox82.Text += "Người chơi ";
                        int i = 1;
                        for (; Convert.ToChar(Control1.byteReceive[i]) != '.'; i++)
                        {
                            textBox82.Text += Convert.ToChar(Control1.byteReceive[i]);
                            if (Convert.ToChar(Control1.byteReceive[i + 1]) == '.')
                            {
                                textBox82.Text += " đã vào phòng.";
                                textBox82.Text += Environment.NewLine;
                            }
                        }
                        if (Control1.byteReceive[i + 1] == 'x')
                            button_play.Enabled = true;
                    }
                    #endregion
                    else
                    {
                        if (Convert.ToChar(Control1.byteReceive[0]) == 'c')
                        #region Xử lý tạo mảng game
                        {
                            for (int i = 0; i < 9; i++)
                                for (int j = 0; j < 9; j++)
                                {
                                    //Chuyển đổi kiểu dữ liệu load mảng lên textbox (=0 bỏ qua)
                                    TextBox control = (TextBox)this.Controls.Find("textBox" + Convert.ToString(i * 9 + (j + 1)), true).SingleOrDefault();
                                    control.Text = "";
                                    if (Convert.ToChar(Control1.byteReceive[i * 9 + j + 1]) != '0')
                                    {
                                        control.Text += Convert.ToChar(Control1.byteReceive[i * 9 + j + 1]);
                                        control.Enabled = false;
                                        control.BackColor = Color.Yellow;
                                    }
                                    else
                                    {
                                        control.Enabled = true;
                                        control.BackColor = Color.White;
                                    }
                                }
                            textBox82.Text += "Chủ phòng đã bắt đầu game" + Environment.NewLine;
                            point = 500;               
                            label2.Text = "Point: " + point.ToString();
                            time = 180;
                            label3.Text = time.ToString();
                        }
                        #endregion                

                        else
                        {
                            if (Convert.ToChar(Control1.byteReceive[0]) == 'r')
                            #region Xử lý kết quả check của server
                            {
                                string temp="";
                                temp += Convert.ToChar(Control1.byteReceive[1]);
                                checkresult = temp;
                            }
                            #endregion
                            else
                            {
                                if (Convert.ToChar(Control1.byteReceive[0]) == 'o')
                                #region Xử lý thông tin chơi của đối thủ
                                {
                                    string aa = "";
                                    aa += Convert.ToChar(Control1.byteReceive[1]);
                                    string bb = "";
                                    bb += Convert.ToChar(Control1.byteReceive[2]);
                                    int x = Convert.ToInt32(aa);
                                    int y = Convert.ToInt32(bb);
                                    TextBox control = (TextBox)this.Controls.Find("textBox" + Convert.ToString((x*9)+(y+1)), true).SingleOrDefault();
                                    control.Text = "";
                                    if (Convert.ToChar(Control1.byteReceive[3]) != 'x')
                                    {
                                        control.Text += Convert.ToChar(Control1.byteReceive[3]);
                                        control.BackColor = Color.Blue;
                                    }
                                    else
                                    {
                                        if (Convert.ToChar(Control1.byteReceive[3]) == 'x')
                                            control.BackColor = Color.White;
                                    }
                                }
                                #endregion
                                else
                                {
                                    if(Convert.ToChar(Control1.byteReceive[0]) == 't')
                                    #region Xử lý thời gian
                                    {
                                        time--;
                                        label3.Text = time.ToString();
                                    }
                                    #endregion
                                    else
                                    {                                        
                                        if (Convert.ToChar(Control1.byteReceive[0]) == 'f')
                                        #region Xử lý kết quả
                                        {
                                            #region Th thắng
                                            if (Convert.ToChar(Control1.byteReceive[1]) == '1')
                                            {
                                                string yourpoint = "";
                                                string friendpoint = "";
                                                int i = 2;
                                                int ishost = -1;
                                                for (; Convert.ToChar(Control1.byteReceive[i]) != '|'; i++)
                                                    yourpoint += Convert.ToChar(Control1.byteReceive[i]);
                                                i++;
                                                for (; Convert.ToChar(Control1.byteReceive[i]) != '|'; i++)
                                                    friendpoint += Convert.ToChar(Control1.byteReceive[i]);
                                                MessageBox.Show("Bạn đã thắng. Điểm của bạn: " + yourpoint + " Điểm của đối thủ: " + friendpoint + " ."
                                                    + Environment.NewLine + "Bạn được thưởng 500$");
                                                string tien = "";
                                                i++;
                                                for (; Convert.ToChar(Control1.byteReceive[i]) != '@'; i++)
                                                    tien += Convert.ToChar(Control1.byteReceive[i]);
                                                i++;
                                                if (Convert.ToChar(Control1.byteReceive[i]) == 'c')
                                                    ishost = 0;
                                                else
                                                    ishost = 1;
                                                if (Convert.ToInt32(tien) >= 500)
                                                {
                                                    DialogResult dr = new DialogResult();
                                                    dr = MessageBox.Show("Bạn còn: " + tien.ToString() + " tien. Bạn có muốn chơi tiếp !"
                                                        , "Chơi tiếp", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                                    if (dr == DialogResult.Yes)
                                                    {
                                                        if (ishost == 0)
                                                        {
                                                            try
                                                            {
                                                                Control1.sendmess("n", Control1.stm);
                                                            }
                                                            catch
                                                            {
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            Control1.sendmess("q", Control1.stm);
                                                            this.Close();
                                                            break;
                                                        }
                                                        catch
                                                        {
                                                        }
                                                    }
                                                }
                                                time = 180;
                                            }
                                            #endregion
                                            else
                                            {
                                                if (Convert.ToChar(Control1.byteReceive[1]) == '0')
                                                #region Th thua
                                                {
                                                    string yourpoint = "";
                                                    string friendpoint = "";
                                                    int i = 2;
                                                    int ishost = -1;
                                                    for (; Convert.ToChar(Control1.byteReceive[i]) != '|'; i++)
                                                        yourpoint += Convert.ToChar(Control1.byteReceive[i]);
                                                    i++;
                                                    for (; Convert.ToChar(Control1.byteReceive[i]) != '|'; i++)
                                                        friendpoint += Convert.ToChar(Control1.byteReceive[i]);
                                                    MessageBox.Show("Bạn đã thua. Điểm của bạn: " + yourpoint + " Điểm của đối thủ: " + friendpoint + " ."
                                                        + Environment.NewLine + "Bạn bị trừ 500$");
                                                    string tien = "";
                                                    i++;
                                                    for(; Convert.ToChar(Control1.byteReceive[i]) != '@'; i++)
                                                        tien += Convert.ToChar(Control1.byteReceive[i]);
                                                    i++;
                                                    if (Convert.ToChar(Control1.byteReceive[i]) == 'c')
                                                        ishost = 0;
                                                    else
                                                        ishost = 1;
                                                    if (Convert.ToInt32(tien) >= 500)
                                                    {
                                                        DialogResult dr = new DialogResult();
                                                        dr = MessageBox.Show("Bạn còn: " + tien.ToString() + " tien. Bạn có muốn chơi tiếp !"
                                                            , "Chơi tiếp", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                                        if (dr == DialogResult.Yes)
                                                        {
                                                            if (ishost == 0)
                                                            {
                                                                try
                                                                {
                                                                    Control1.sendmess("n", Control1.stm);
                                                                }
                                                                catch
                                                                {
                                                                }
                                                            }

                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                
                                                                Control1.sendmess("q", Control1.stm);
                                                                this.Close();
                                                                break;
                                                            }
                                                            catch
                                                            {
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Bạn còn: " + tien.ToString() + " tien. Bạn không đủ tiền chơi tiếp !");
                                                        this.Close();
                                                        break;
                                                    }
                                                    time = 180;
                                                }
                                                #endregion
                                                else
                                                #region Th hòa
                                                {
                                                    if (Convert.ToChar(Control1.byteReceive[1]) == '2')
                                                    {
                                                        string yourpoint = "";
                                                        string friendpoint = "";
                                                        int i = 2;
                                                        int ishost = -1;
                                                        for (; Convert.ToChar(Control1.byteReceive[i]) != '|'; i++)
                                                            yourpoint += Convert.ToChar(Control1.byteReceive[i]);
                                                        i++;
                                                        for (; Convert.ToChar(Control1.byteReceive[i]) != '|'; i++)
                                                            friendpoint += Convert.ToChar(Control1.byteReceive[i]);
                                                        MessageBox.Show("Hòa. Điểm của bạn: " + yourpoint + " Điểm của đối thủ: " + friendpoint + " .");
                                                        string tien = "";
                                                        i++;
                                                        for (; Convert.ToChar(Control1.byteReceive[i]) != '@'; i++)
                                                            tien += Convert.ToChar(Control1.byteReceive[i]);
                                                        i++;
                                                        if (Convert.ToChar(Control1.byteReceive[i]) == 'c')
                                                            ishost = 0;
                                                        else
                                                            ishost = 1;
                                                        if (Convert.ToInt32(tien) >= 500)
                                                        {
                                                            DialogResult dr = new DialogResult();
                                                            dr = MessageBox.Show("Bạn còn: " + tien.ToString() + " tien. Bạn có muốn chơi tiếp !"
                                                                , "Chơi tiếp", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                                            if (dr == DialogResult.Yes)
                                                            {
                                                                if (ishost == 0)
                                                                {
                                                                    try
                                                                    {
                                                                        Control1.sendmess("n", Control1.stm);
                                                                    }
                                                                    catch
                                                                    {
                                                                    }
                                                                }

                                                            }
                                                            else
                                                            {
                                                                try
                                                                {
                                                                    Control1.sendmess("q", Control1.stm);
                                                                    this.Close();
                                                                    break;
                                                                }
                                                                catch
                                                                {
                                                                }
                                                            }
                                                        }
                                                        time = 180;
                                                    }
                                                }
                                                #endregion
                                            }
                                            for (int i = 0; i < 9; i++)
                                                for (int j = 0; j < 9; j++)
                                                {
                                                    TextBox control = (TextBox)this.Controls.Find("textBox" + (i * 9 + (j + 1)).ToString(), true).SingleOrDefault();
                                                    control.Enabled = false;
                                                }
                                        }
                                        #endregion
                                        else
                                        {
                                            #region Xử lý chơi tiếp
                                            if (Convert.ToChar(Control1.byteReceive[0]) == 'n')
                                            {
                                                button_play.Enabled = true;
                                                textBox82.Text += "Đối thủ muốn chơi tiếp !" + Environment.NewLine;
                                            }
                                            #endregion
                                        }

                                    }
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
        //Hàm addevent cho textbox
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
                             string test = "?" + "3" + temp1 + temp2 + e.KeyChar;
                             try
                             {

                                 Control1.sendmess(test, Control1.stm);
                                 //Nhận kết quả kiểm tra từ server
                                 Thread.Sleep(400);                                 
                                 #region Xu ly khi ô nhập
                                 if (checkresult == "0")
                                 {
                                     control.BackColor = Color.Red;
                                     point = point - 100;
                                     label2.Text = "Point: " + point.ToString();
                                     checkresult = "-1";
                                 }
                                 else
                                 {
                                     if (checkresult == "1")
                                     {
                                         control.ForeColor = Color.White;
                                         control.BackColor = Color.Green;
                                         point = point + 100;
                                         label2.Text = "Point: " + point.ToString();
                                         checkresult = "-1";
                                     }
                                     else
                                     {
                                         MessageBox.Show("Đối thủ đã nhanh hơn bạn ở ô này !");
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
                             point = point - 200;
                             label2.Text = "Point: " + point.ToString();
                             string test = "?" + "3" + temp1 + temp2 +"0";
                             try
                             {

                                 Control1.sendmess(test, Control1.stm);
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
        //Nút out
        private void button2_Click(object sender, EventArgs e)
        {

            DialogResult dr = new DialogResult();
            dr = MessageBox.Show("Bạn sẽ bị xử thua, bạn chắc chán muốn thoát", "Thoát phòng", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                try
                {
                    Control1.sendmess("q", Control1.stm);
                }
                catch
                {
                }
                this.Close();
                stopthread = 1;
            }

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
        //Nút âm thanh
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
        //Nút play
        private void button_play_Click(object sender, EventArgs e)
        {
            try
            {

                Control1.sendmess("!", Control1.stm);
            }
            catch
            {

            }
            button_play.Enabled = false;
        }
    }
}
