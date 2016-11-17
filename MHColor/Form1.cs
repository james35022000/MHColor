using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;

namespace MabinogiHeroColor
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out  Rectangle lpRect);
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("USER32.DLL")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("USER32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        [System.Runtime.InteropServices.DllImport("USER32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        Bitmap MH_pic;
        Boolean left = true;
        Bitmap pic = new Bitmap(256, 256);
        int WidthOffset = 0, HeightOffset = 0, WindowsOffsetW = 0, WindowsOffsetH = 0;
        static List<RGBinfo> RGB_List = new List<RGBinfo>();
        public struct RGBinfo
        {
            public int R;
            public int G;
            public int B;
            public string name;
        };
        public class cboDataList
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
        /*------------------------------------------------------------------*/

        public Form1()
        {
            InitializeComponent();
        }
        //Initial------------------------------------------------
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string JpgPath = Application.StartupPath + "\\data\\MHColor_Start.jpg";
                MH_pic = new Bitmap(JpgPath);
                pictureBox1.Image = Image.FromFile(JpgPath);
                progressBar1.Value = 0;
                progressBar1.Maximum = 103552;
                List<cboDataList> lis_DataList = new List<cboDataList>()
            {
                new cboDataList
                {
                    Name = "一個符合",
                    Value = 1                   
                },
                new cboDataList
                {
                    Name = "兩個符合",
                    Value = 2                   
                },
                new cboDataList
                {
                    Name = "三個符合",
                    Value = 3                   
                },
                new cboDataList
                {
                    Name = "四個符合",
                    Value = 4                   
                },
                new cboDataList
                {
                    Name = "五個符合",
                    Value = 5                   
                },
                new cboDataList
                {
                    Name = "六個符合",
                    Value = 6                   
                }
            };
                comboBox1.DataSource = lis_DataList;
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "Value";
                textBox1.Text = "10";
            }
            catch
            {
                MessageBox.Show("啟動時發生錯誤!請檢查是否缺少檔案", "Error");
                Application.Exit();
            }
        }
        //擷取色板-------------------------------------------------
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.X <= 128)
                left = true;
            else
                left = false;
            ColorBoard();
        }
        private void ColorBoard()
        {
            try
            {
                Process[] process = Process.GetProcessesByName("heroes");
                /* 取得該視窗的大小與位置 */
                Rectangle screen;
                GetWindowRect(process[0].MainWindowHandle, out screen);
                screen.Width -= screen.X;
                screen.Height -= screen.Y;
                if (screen.Height - 28 == 768 || screen.Height - 28 == 864 || screen.Height - 28 == 960 || screen.Height - 28 == 1024 ||
                    screen.Height - 28 == 900 || screen.Height - 28 == 1080 || screen.Height - 28 == 800 || screen.Height - 28 == 1050)
                {
                    WindowsOffsetW = 0;
                    WindowsOffsetH = 22;
                }
                else
                {
                    WindowsOffsetW = 0;
                    WindowsOffsetH = 0;
                }
                WidthOffset = (1920 - screen.Size.Width + WindowsOffsetW)/2;
                HeightOffset = (1080 - screen.Size.Height + WindowsOffsetH)/2;
                /* 抓取截圖 */
                Bitmap PicPath = new Bitmap(screen.Width, screen.Height, PixelFormat.Format32bppArgb);
                Graphics gfx = Graphics.FromImage(PicPath);
                gfx.CopyFromScreen(screen.X, screen.Y, 0, 0, screen.Size, CopyPixelOperation.SourceCopy);
                Bitmap sourceImage = new Bitmap(PicPath);
                Graphics graphic = Graphics.FromImage(pic);
                if (left)
                {
                    graphic.DrawImage(sourceImage, new Rectangle(0, 0, 128, 256), new Rectangle(807-WidthOffset+WindowsOffsetW, 332-HeightOffset+WindowsOffsetH, 128, 256), GraphicsUnit.Pixel);
                    IntPtr MH = FindWindow(null, "新瑪奇英雄傳");
                    if (MH == IntPtr.Zero)
                        MessageBox.Show("Not Found");
                    else
                        SetForegroundWindow(MH);
                }
                else
                {
                    graphic.DrawImage(sourceImage, new Rectangle(128, 0, 128, 256), new Rectangle(935-WidthOffset+WindowsOffsetW, 332-HeightOffset+WindowsOffsetH, 128, 256), GraphicsUnit.Pixel);
                    IntPtr MH = FindWindow(null, "新瑪奇英雄傳");
                    if (MH == IntPtr.Zero)
                        MessageBox.Show("Not Found");
                    else
                        SetForegroundWindow(MH);
                }
                pictureBox1.Image = pic;
                MH_pic = pic;
            }
            catch
            {
                MessageBox.Show("請確認遊戲開啟後再進行截圖!!", "Error");
            }
        }
        //找色----------------------------------------------------
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Clear();
                database();
                progressBar1.Value = 0;
                for (int i = 0; i < 256; i++)
                {
                    for (int j = 0; j < 256; j++)
                    {
                        Color MH_col = MH_pic.GetPixel(i, j);
                        MHColor.color_map_construct.construct(Int32.Parse(MH_col.R.ToString()),
                                                              Int32.Parse(MH_col.G.ToString()),
                                                              Int32.Parse(MH_col.B.ToString()),
                                                              Int32.Parse(textBox1.Text), RGB_List, i, j);
                        progressBar1.Value += 1;
                    }
                }
                for (int i = 0; i < 176; i++)
                {
                    for (int j = 0; j < 216; j++)
                    {
                        if (Convert.ToInt32(MHColor.color_map_construct.Color_map[i, j]) +
                            Convert.ToInt32(MHColor.color_map_construct.Color_map[i + 40, j]) +
                            Convert.ToInt32(MHColor.color_map_construct.Color_map[i + 80, j]) +
                            Convert.ToInt32(MHColor.color_map_construct.Color_map[i, j + 40]) +
                            Convert.ToInt32(MHColor.color_map_construct.Color_map[i + 40, j + 40]) +
                            Convert.ToInt32(MHColor.color_map_construct.Color_map[i + 80, j + 40]) >= Convert.ToInt32(comboBox1.SelectedValue.ToString()))
                            listBox1.Items.Add(i.ToString() + " " + j.ToString());
                        progressBar1.Value += 1;
                    }
                }
            }
            catch{  }
        }
        //進度條---------------------------------------------------
        public static void database()
        {
            try
            {
                int cnt = 0;
                string input;
                string[] input_array;
                string DatabasePath = Application.StartupPath + "\\data\\RGBdatabase.txt";
                StreamReader sr = new StreamReader(DatabasePath);
                cnt = 1;
                while (!sr.EndOfStream)
                {
                    RGBinfo temp;
                    input = sr.ReadLine();
                    input_array = input.Split(' ');
                    temp.R = Int32.Parse(input_array[0]);
                    temp.G = Int32.Parse(input_array[1]);
                    temp.B = Int32.Parse(input_array[2]);
                    temp.name = input_array[3];
                    RGB_List.Add(temp);
                    cnt++;
                }
            }
            catch
            {
                MessageBox.Show("啟動時發生錯誤!請檢查是否缺少檔案", "Error");
                Application.Exit();
            }
        }
        //選取-------------------------------------------------
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show((MH_pic.GetPixel(87, 77).R.ToString()) + (MH_pic.GetPixel(88, 77).R.ToString()) + (MH_pic.GetPixel(87, 78).R.ToString()));
            try
            {
                string[] temp = listBox1.SelectedItem.ToString().Split(' ');
                //one-----------------------------------------------------------
                int X = Int32.Parse(temp[0]);
                int Y = Int32.Parse(temp[1]);
                pictureBox2.BackColor = Color.FromArgb(Int32.Parse(MH_pic.GetPixel(X, Y).R.ToString()),
                                                       Int32.Parse(MH_pic.GetPixel(X, Y).G.ToString()),
                                                       Int32.Parse(MH_pic.GetPixel(X, Y).B.ToString()));
                label3.Text = MH_pic.GetPixel(X, Y).R.ToString();
                label4.Text = MH_pic.GetPixel(X, Y).G.ToString();
                label7.Text = MH_pic.GetPixel(X, Y).B.ToString();
                //two-----------------------------------------------------------
                X = Int32.Parse(temp[0]) + 40;
                pictureBox3.BackColor = Color.FromArgb(Int32.Parse(MH_pic.GetPixel(X, Y).R.ToString()),
                                                       Int32.Parse(MH_pic.GetPixel(X, Y).G.ToString()),
                                                       Int32.Parse(MH_pic.GetPixel(X, Y).B.ToString()));
                label8.Text = MH_pic.GetPixel(X, Y).R.ToString();
                label9.Text = MH_pic.GetPixel(X, Y).G.ToString();
                label10.Text = MH_pic.GetPixel(X, Y).B.ToString();
                //three---------------------------------------------------------
                X = Int32.Parse(temp[0]) + 80;
                pictureBox4.BackColor = Color.FromArgb(Int32.Parse(MH_pic.GetPixel(X, Y).R.ToString()),
                                                       Int32.Parse(MH_pic.GetPixel(X, Y).G.ToString()),
                                                       Int32.Parse(MH_pic.GetPixel(X, Y).B.ToString()));
                label11.Text = MH_pic.GetPixel(X, Y).R.ToString();
                label12.Text = MH_pic.GetPixel(X, Y).G.ToString();
                label13.Text = MH_pic.GetPixel(X, Y).B.ToString();
                //four----------------------------------------------------------
                X = Int32.Parse(temp[0]);
                Y = Int32.Parse(temp[1]) + 40;
                pictureBox5.BackColor = Color.FromArgb(Int32.Parse(MH_pic.GetPixel(X, Y).R.ToString()),
                                                       Int32.Parse(MH_pic.GetPixel(X, Y).G.ToString()),
                                                       Int32.Parse(MH_pic.GetPixel(X, Y).B.ToString()));
                label16.Text = MH_pic.GetPixel(X, Y).R.ToString();
                label15.Text = MH_pic.GetPixel(X, Y).G.ToString();
                label14.Text = MH_pic.GetPixel(X, Y).B.ToString();
                //five----------------------------------------------------------
                X = Int32.Parse(temp[0]) + 40;
                pictureBox6.BackColor = Color.FromArgb(Int32.Parse(MH_pic.GetPixel(X, Y).R.ToString()),
                                                       Int32.Parse(MH_pic.GetPixel(X, Y).G.ToString()),
                                                       Int32.Parse(MH_pic.GetPixel(X, Y).B.ToString()));
                label19.Text = MH_pic.GetPixel(X, Y).R.ToString();
                label18.Text = MH_pic.GetPixel(X, Y).G.ToString();
                label17.Text = MH_pic.GetPixel(X, Y).B.ToString();
                //six-----------------------------------------------------------
                X = Int32.Parse(temp[0]) + 80;
                pictureBox7.BackColor = Color.FromArgb(Int32.Parse(MH_pic.GetPixel(X, Y).R.ToString()),
                                                       Int32.Parse(MH_pic.GetPixel(X, Y).G.ToString()),
                                                       Int32.Parse(MH_pic.GetPixel(X, Y).B.ToString()));
                label22.Text = MH_pic.GetPixel(X, Y).R.ToString();
                label21.Text = MH_pic.GetPixel(X, Y).G.ToString();
                label20.Text = MH_pic.GetPixel(X, Y).B.ToString();
            }
            catch { }
        }
        //點選色塊--------------------------------------------------------------------------
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                Process[] process = Process.GetProcessesByName("heroes");
                /* 取得該視窗的大小與位置 */
                Rectangle screen;
                GetWindowRect(process[0].MainWindowHandle, out screen);
                WidthOffset = (1920 - screen.Size.Width) / 2;
                HeightOffset = (1080 - screen.Size.Height) / 2;
                string[] temp = listBox1.SelectedItem.ToString().Split(' ');
                int X = Int32.Parse(temp[0]) + screen.X + 807 - WidthOffset + 1, Y = Int32.Parse(temp[1]) + screen.Y + 332 - HeightOffset + 1;
                IntPtr MH = FindWindow(null, "新瑪奇英雄傳");
                if (MH == IntPtr.Zero)
                    MessageBox.Show("Not Found");
                else
                {
                    Cursor.Position = new Point(X, Y); //滑鼠移到色點
                    SetForegroundWindow(MH);
                }
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MHColor.Form2 form2 = new MHColor.Form2();
            form2.Show();
        }

    }
}
