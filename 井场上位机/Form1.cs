using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO.Ports;
using CShapeSerialPort;
using System.IO;
using System.Threading;

namespace 井场上位机
{
    public partial class Form1 : Form
    {
        private SerialPortUtil comPort = new SerialPortUtil();//声明串口方法
        private SerialPortUtil comPort1 = new SerialPortUtil();
        int port = 8899;//定义端口号
        public IPAddress IP = IPAddress.Parse("192.168.1.1");//定义一个IP地址
        private Socket stSocket;//创建套接字 
        double windSpeed;//全局风速
        string windDir;//全局风向
        string potency="0";//全局浓度  
        int i=0;//一个计数器，用于延时判断
        bool pp;//用于记录当前是否为模拟的状态
        Socket service = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //创建一个Socket对象   
       
        public bool connect()//连接服务端
        {
            //判断tcp是否成功连接
            try
            {
                stSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);//初始化一个socket
                IPEndPoint tempRemoteIP = new IPEndPoint(IP, port);//根据ip地址和端口号创立一个终结点；
                EndPoint epTemp = (EndPoint)tempRemoteIP;
                stSocket.Connect(epTemp);//连接到远程主机
                return true;
            }
            catch (Exception)
            {
                textBox1.AppendText("连接cc3200失败");
                return false;
            }
        }

        public string getDir()//h获得风向（十六进制）
        {
            string a;
            byte[] bySend = { 0x11, 0x03, 0x00, 0x00, 0x00, 0x01, 0x86, 0x9a };
            stSocket.Send(bySend);//发送数据
            byte[] byText = new byte[7];//创建一个缓冲区
            try
            {
                stSocket.Receive(byText);
                string sTemp = byteToHexStr(byText);
                //return getReal(sTemp);
                return sTemp;
            }
            catch
            {
                a = ("风向接收失败！");
                return a;
            }
        }

        public string getStrDir(string a)//将传感器采集的数值方向信息转化为文字
        {
            int temp;
            temp = Convert.ToInt16(a, 16);
            if (windSpeed > 0.2)
            {
                if (temp >= 22.5 && temp < 67.5)
                    return "东北";
                else if (temp >= 67.5 && temp < 112.5)
                    return "东";
                else if (temp >= 112.5 && temp < 157.5)
                    return "东南";
                else if (temp >= 157.5 && temp < 202.5)
                    return "南";
                else if (temp >= 202.5 && temp < 247.5)
                    return "西南";
                else if (temp >= 247.5 && temp < 292.5)
                    return "西";
                else if (temp >= 292.5 && temp < 337.5)
                    return "西北";
                else
                    return "北";
            }
            else
                return "无风";

        }

        public string getSpe()//获得风速（十六进制）
        {
            string a;
            byte[] bySend = { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01, 0x84, 0x0a };
            stSocket.Send(bySend);//发送数据
            byte[] byText = new byte[7];//创建一个缓冲区
            try
            {
                stSocket.Receive(byText);
                string sTemp = byteToHexStr(byText);
                //return getReal(sTemp);
                return sTemp;
            }
            catch
            {
                a = ("风速接收失败！");
                return a;
            }
        }

        void comPort_DataReceived(DataReceivedEventArgs e)//触发接收事件(获得浓度)
        {
            string a = SerialPortUtil.ByteToHex(e.DataRecv);
            string temp = a[1].ToString();
            temp += a[4].ToString();
            temp += a[7].ToString();
            temp += a[10].ToString();
            int b = Convert.ToInt16(temp);
            b = b - 500;
            b = Convert.ToInt16(b * (Math.Sqrt(b))/20);
            potency = b.ToString() ;
        }

        public static string byteToHexStr(byte[] bytes)//十六进制字节转字符串
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        public static string getReal(string a)//从返回字符串中获取风向风速有效位
        {
            int i;
            string temp = null;
            for (i = 6; i < 10; i++)
            {
                temp += a[i];
            }
            return temp;
        }

        public void winDirPic(string a)//可视化风向控件
        {
            if (a == "南")
            {
                picturewinDir1.Visible = true;
                picturewinDir2.Visible = false;
                picturewinDir3.Visible = false;
                picturewinDir4.Visible = false;
                picturewinDir5.Visible = false;
                picturewinDir6.Visible = false;
                picturewinDir7.Visible = false;
                picturewinDir8.Visible = false;
            }
            else if (a == "西南")
            {
                picturewinDir1.Visible = false;
                picturewinDir2.Visible = true;
                picturewinDir3.Visible = false;
                picturewinDir4.Visible = false;
                picturewinDir5.Visible = false;
                picturewinDir6.Visible = false;
                picturewinDir7.Visible = false;
                picturewinDir8.Visible = false;
            }
            else if (a == "西")
            {
                picturewinDir1.Visible = false;
                picturewinDir2.Visible = false;
                picturewinDir3.Visible = true;
                picturewinDir4.Visible = false;
                picturewinDir5.Visible = false;
                picturewinDir6.Visible = false;
                picturewinDir7.Visible = false;
                picturewinDir8.Visible = false;
            }
            else if (a == "西北")
            {
                picturewinDir1.Visible = false;
                picturewinDir2.Visible = false;
                picturewinDir3.Visible = false;
                picturewinDir4.Visible = true;
                picturewinDir5.Visible = false;
                picturewinDir6.Visible = false;
                picturewinDir7.Visible = false;
                picturewinDir8.Visible = false;
            }
            else if (a == "北")
            {
                picturewinDir1.Visible = false;
                picturewinDir2.Visible = false;
                picturewinDir3.Visible = false;
                picturewinDir4.Visible = false;
                picturewinDir5.Visible = true;
                picturewinDir6.Visible = false;
                picturewinDir7.Visible = false;
                picturewinDir8.Visible = false;
            }
            else if (a == "东北")
            {
                picturewinDir1.Visible = false;
                picturewinDir2.Visible = false;
                picturewinDir3.Visible = false;
                picturewinDir4.Visible = false;
                picturewinDir5.Visible = false;
                picturewinDir6.Visible = true;
                picturewinDir7.Visible = false;
                picturewinDir8.Visible = false;
            }
            else if (a == "东")
            {
                picturewinDir1.Visible = false;
                picturewinDir2.Visible = false;
                picturewinDir3.Visible = false;
                picturewinDir4.Visible = false;
                picturewinDir5.Visible = false;
                picturewinDir6.Visible = false;
                picturewinDir7.Visible = true;
                picturewinDir8.Visible = false;
            }
            else if (a == "东南")
            {
                picturewinDir1.Visible = false;
                picturewinDir2.Visible = false;
                picturewinDir3.Visible = false;
                picturewinDir4.Visible = false;
                picturewinDir5.Visible = false;
                picturewinDir6.Visible = false;
                picturewinDir7.Visible = false;
                picturewinDir8.Visible = true;
            }
            else
            {
                picturewinDir1.Visible = false;
                picturewinDir2.Visible = false;
                picturewinDir3.Visible = false;
                picturewinDir4.Visible = false;
                picturewinDir5.Visible = false;
                picturewinDir6.Visible = false;
                picturewinDir7.Visible = false;
                picturewinDir8.Visible = false;
            }
        }

        public void gateLight8(string a)//西北门箭头控件
        {
            if (a == "green")
            {
                picturegateDir8b.Visible = false;
                picturegateDir8g.Visible = true;
                picturegateDir8r.Visible = false;
            }
            else if (a == "red")
            {
                picturegateDir8b.Visible = false;
                picturegateDir8g.Visible = false;
                picturegateDir8r.Visible = true;
            }
            else if (a == "blue")
            {
                picturegateDir8b.Visible = true;
                picturegateDir8g.Visible = false;
                picturegateDir8r.Visible = false;
            }
            else
            {
                picturegateDir8b.Visible = false;
                picturegateDir8g.Visible = false;
                picturegateDir8r.Visible = false;
            }
        }

        public void gateLight1(string a)//北门箭头控件
        {
            if (a == "green")
            {
                picturegateDir1b.Visible = false;
                picturegateDir1g.Visible = true;
                picturegateDir1r.Visible = false;
            }
            else if (a == "red")
            {
                picturegateDir1b.Visible = false;
                picturegateDir1g.Visible = false;
                picturegateDir1r.Visible = true;
            }
            else if (a == "blue")
            {
                picturegateDir1b.Visible = true;
                picturegateDir1g.Visible = false;
                picturegateDir1r.Visible = false;
            }
            else
            {
                picturegateDir1b.Visible = false;
                picturegateDir1g.Visible = false;
                picturegateDir1r.Visible = false;
            }
        }

        public void gateLight2(string a)//东北门箭头控件
        {
            if (a == "green")
            {
                picturegateDir2b.Visible = false;
                picturegateDir2g.Visible = true;
                picturegateDir2r.Visible = false;
            }
            else if (a == "red")
            {
                picturegateDir2b.Visible = false;
                picturegateDir2g.Visible = false;
                picturegateDir2r.Visible = true;
            }
            else if (a == "blue")
            {
                picturegateDir2b.Visible = true;
                picturegateDir2g.Visible = false;
                picturegateDir2r.Visible = false;
            }
            else
            {
                picturegateDir2b.Visible = false;
                picturegateDir2g.Visible = false;
                picturegateDir2r.Visible = false;
            }
        }

        public void gateLight3(string a)//东门箭头控件
        {
            if (a == "green")
            {
                picturegateDir3b.Visible = false;
                picturegateDir3g.Visible = true;
                picturegateDir3r.Visible = false;
            }
            else if (a == "red")
            {
                picturegateDir3b.Visible = false;
                picturegateDir3g.Visible = false;
                picturegateDir3r.Visible = true;
            }
            else if (a == "blue")
            {
                picturegateDir3b.Visible = true;
                picturegateDir3g.Visible = false;
                picturegateDir3r.Visible = false;
            }
            else
            {
                picturegateDir3b.Visible = false;
                picturegateDir3g.Visible = false;
                picturegateDir3r.Visible = false;
            }
        }

        public void gateLight4(string a)//东南箭头控件
        {
            if (a == "green")
            {
                picturegateDir4b.Visible = false;
                picturegateDir4g.Visible = true;
                picturegateDir4r.Visible = false;
            }
            else if (a == "red")
            {
                picturegateDir4b.Visible = false;
                picturegateDir4g.Visible = false;
                picturegateDir4r.Visible = true;
            }
            else if (a == "blue")
            {
                picturegateDir4b.Visible = true;
                picturegateDir4g.Visible = false;
                picturegateDir4r.Visible = false;
            }
            else
            {
                picturegateDir4b.Visible = false;
                picturegateDir4g.Visible = false;
                picturegateDir4r.Visible = false;
            }
        }

        public void gateLight5(string a)//南箭头控件
        {
            if (a == "green")
            {
                picturegateDir5b.Visible = false;
                picturegateDir5g.Visible = true;
                picturegateDir5r.Visible = false;
            }
            else if (a == "red")
            {
                picturegateDir5b.Visible = false;
                picturegateDir5g.Visible = false;
                picturegateDir5r.Visible = true;
            }
            else if (a == "blue")
            {
                picturegateDir5b.Visible = true;
                picturegateDir5g.Visible = false;
                picturegateDir5r.Visible = false;
            }
            else
            {
                picturegateDir5b.Visible = false;
                picturegateDir5g.Visible = false;
                picturegateDir5r.Visible = false;
            }
        }

        public void gateLight6(string a)//西南箭头控件
        {
            if (a == "green")
            {
                picturegateDir6b.Visible = false;
                picturegateDir6g.Visible = true;
                picturegateDir6r.Visible = false;
            }
            else if (a == "red")
            {
                picturegateDir6b.Visible = false;
                picturegateDir6g.Visible = false;
                picturegateDir6r.Visible = true;
            }
            else if (a == "blue")
            {
                picturegateDir6b.Visible = true;
                picturegateDir6g.Visible = false;
                picturegateDir6r.Visible = false;
            }
            else
            {
                picturegateDir6b.Visible = false;
                picturegateDir6g.Visible = false;
                picturegateDir6r.Visible = false;
            }
        }

        public void gateLight7(string a)//西箭头控件
        {
            if (a == "green")
            {
                picturegateDir7b.Visible = false;
                picturegateDir7g.Visible = true;
                picturegateDir7r.Visible = false;
            }
            else if (a == "red")
            {
                picturegateDir7b.Visible = false;
                picturegateDir7g.Visible = false;
                picturegateDir7r.Visible = true;
            }
            else if (a == "blue")
            {
                picturegateDir7b.Visible = true;
                picturegateDir7g.Visible = false;
                picturegateDir7r.Visible = false;
            }
            else
            {
                picturegateDir7b.Visible = false;
                picturegateDir7g.Visible = false;
                picturegateDir7r.Visible = false;
            }
        }

        public void writeData()
        {
            StreamWriter sw = new StreamWriter(@"f:\\src.txt",true);
            sw.Write(potency);
            sw.Write(" "+windSpeed);
            sw.Write(" " + Convert.ToInt16(getReal(getDir()), 16)+"\r\n");          
            sw.Close(); 
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            comPort.DataReceived += new DataReceivedEventHandler(comPort_DataReceived);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            windSpeed = Convert.ToInt64(getReal(getSpe()), 16) / 10.00;
            labelwinSpe.Text = windSpeed.ToString("F1") + "m/s";
            windDir = labelwinDir.Text = getStrDir(getReal(getDir()));
            winDirPic(windDir);
            labelPot.Text = potency+" ppm";
            comPort.WriteData(SerialPortUtil.HexToByte("01"));
            writeData();
            string a = potency + " " + windSpeed.ToString() + " " + windDir;
    }

        private void button3_Click(object sender, EventArgs e)

        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "启动服务")
            {
                try
                {
                    connect();
                    comPort.PortName = "COM4";
                    comPort1.PortName = "COM3";
                    comPort.OpenPort();
                    comPort1.OpenPort();
                    timer1.Start();
                    pp = false;
                    textBox1.AppendText("启动成功！");
                    timer3.Start();
                    button1.Text = "停止服务";
                    button2.Enabled = true;             
                }
                catch
                {
                    textBox1.AppendText("启动失败！");
                }
            }
            else
            {
                timer1.Stop();
                timer3.Stop();
                comPort.ClosePort();
                comPort1.ClosePort();
                labelwinDir.Text = "NULL";
                labelwinSpe.Text = "NULL";
                labelPot.Text = "NULL";
                if (button2.Text == "取消模拟")
                {
                    button2_Click(sender, e);
                    button2.Text = "应急模拟";

                }
                button2.Enabled = false;
                button1.Text = "启动服务";
                winDirPic("qqq");
                stSocket.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "应急模拟")
            {
                pp = true;
                timer2.Start();
                textBox1.AppendText("泄漏发生");
                button2.Text = "取消模拟";           
            }
            else
            {
                pp = false;
                timer2.Stop();
                gateLight1("blue");
                gateLight2("blue");
                gateLight3("blue");
                gateLight4("blue");
                gateLight5("blue");
                gateLight6("blue");
                gateLight7("blue");
                gateLight8("blue");
                comPort1.WriteData(SerialPortUtil.HexToByte("20"));
                textBox1.AppendText("取消警报");
                button2.Text = "应急模拟";   
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            string g = "green";
            string r = "red";
            string b = "blue";
            if (windDir == "北")
            {
                gateLight1(g);
                gateLight2(g);
                gateLight3(r);
                gateLight4(r);
                gateLight5(r);
                gateLight6(r);
                gateLight7(r);
                gateLight8(g);
                comPort1.WriteData(SerialPortUtil.HexToByte("11"));
            }
            else if (windDir == "东北")
            {
                gateLight1(g);
                gateLight2(g);
                gateLight3(g);
                gateLight4(r);
                gateLight5(r);
                gateLight6(r);
                gateLight7(r);
                gateLight8(r);
                comPort1.WriteData(SerialPortUtil.HexToByte("12"));
            }
            else if (windDir == "东")
            {
                gateLight1(r);
                gateLight2(g);
                gateLight3(g);
                gateLight4(g);
                gateLight5(r);
                gateLight6(r);
                gateLight7(r);
                gateLight8(r);
                comPort1.WriteData(SerialPortUtil.HexToByte("13"));
            }
            else if (windDir == "东南")
            {
                gateLight1(r);
                gateLight2(r);
                gateLight3(g);
                gateLight4(g);
                gateLight5(g);
                gateLight6(r);
                gateLight7(r);
                gateLight8(r);
                comPort1.WriteData(SerialPortUtil.HexToByte("14"));
            }
            else if (windDir == "南")
            {
                gateLight1(r);
                gateLight2(r);
                gateLight3(r);
                gateLight4(g);
                gateLight5(g);
                gateLight6(g);
                gateLight7(r);
                gateLight8(r);
                comPort1.WriteData(SerialPortUtil.HexToByte("15"));
            }
            else if (windDir == "西南")
            {
                gateLight1(r);
                gateLight2(r);
                gateLight3(r);
                gateLight4(r);
                gateLight5(g);
                gateLight6(g);
                gateLight7(g);
                gateLight8(r);
                comPort1.WriteData(SerialPortUtil.HexToByte("16"));
            }
            else if (windDir == "西")
            {
                gateLight1(r);
                gateLight2(r);
                gateLight3(r);
                gateLight4(r);
                gateLight5(r);
                gateLight6(g);
                gateLight7(g);
                gateLight8(g);
                comPort1.WriteData(SerialPortUtil.HexToByte("17"));
            }
            else if (windDir == "西北")
            {
                gateLight1(g);
                gateLight2(r);
                gateLight3(r);
                gateLight4(r);
                gateLight5(r);
                gateLight6(r);
                gateLight7(g);
                gateLight8(g);
                comPort1.WriteData(SerialPortUtil.HexToByte("18"));
            }
            else if (windDir == "无风")
            {
                gateLight1(g);
                gateLight2(g);
                gateLight3(g);
                gateLight4(g);
                gateLight5(g);
                gateLight6(g);
                gateLight7(g);
                gateLight8(g);
                comPort1.WriteData(SerialPortUtil.HexToByte("10"));
            }
            else
            {
                gateLight1(b);
                gateLight2(b);
                gateLight3(b);
                gateLight4(b);
                gateLight5(b);
                gateLight6(b);
                gateLight7(b);
                gateLight8(b);
                comPort1.WriteData(SerialPortUtil.HexToByte("10"));
            }
        }

        private void picturegateDir8r_Click(object sender, EventArgs e)
        {

        }

        private void picturegateDir3g_Click(object sender, EventArgs e)
        {

        }

        private void labelPot_Click(object sender, EventArgs e)
        {

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (pp == false)
            {
                if (Convert.ToInt16(potency) > 10)
                {
                    i++;
                }
                else
                {
                    i = 0;
                }
                if (i > 5)
                {
                    button2_Click(sender, e);
                    i = 0;
                }
            }
            else
            {
                if (Convert.ToInt16(potency) <= 8)
                {
                    i++;
                }
                else
                {
                    i = 0;
                }
                if (i > 5)
                {
                    button2_Click(sender, e);
                    i = 0;
                }
            }
        }
    }
}
