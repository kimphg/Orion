using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using UsbHid;
using UsbHid.USB.Classes.Messaging;
struct arpaOBJ
{
    public String id;
    /*public void setAzi(float azi)
    {
        this.azi = azi;
    }
    public void setRange(float range)
    {
        this.range = range;
    }*/
    public float range;
    public float azi;
};
namespace Camera_PTZ
{
    enum cameraType { pelco, nighthawk, flir } ;
    public partial class GuiMain : Form
    {
        bool connectionActive;
        Thread workerThread;
        //Socket UDPsock;
        cameraType camtype;
        List<arpaOBJ> ListRadar = new List<arpaOBJ>();
        //System.Windows.Forms.Timer getCamStateTimer;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myC = base.CreateParams;
                myC.ClassStyle = myC.ClassStyle | 0x200;
                return myC;
            }
        }
        //private static PTZFacade _ptz;
        private const uint PRESET_PATTERN = 8;
        TelnetConnection tc;
        public GuiMain()
        {
            InitializeComponent();
            //UDPsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            button5.Enabled = true;
            
            TopLevel = true;
            TopMost = true;
            connectionActive = false;
            //sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }


        private void button6_Click(object sender, EventArgs e)
        {
            if (!connectionActive) return;
            comandNH.SetValue(Convert.ToDouble(numericUpDown2.Value), Convert.ToDouble(numericUpDown4.Value));
        }

        private void IPtextBox_TextChanged(object sender, EventArgs e)
        {

            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_Connect_Click(object sender, EventArgs e)
        {

            button_Connect.Text = "Đang kết nối...";
            Update();
            //Update();
            switch (camtype)
            {
                case cameraType.nighthawk:
                    try
                    {

                        //_ptz = new PTZFacade(IPtextBox.Text.ToString(), textBox2.Text.ToString(), textBox3.Text.ToString());
                        //_ptz.Move(x, y, z);
                        tc = new TelnetConnection(IPtextBox.Text.ToString(), 23);
                        comandNH = new CommandTransferNH(tc,this);
                        if (textBox2.Text.Length > 0) tc.Login(textBox2.Text.ToString(), textBox3.Text.ToString(), 100);
                        button1.Enabled = true;
                        button2.Enabled = true;
                        button3.Enabled = true;
                        button4.Enabled = true;
                        //button5.Enabled = true;
                        button6.Enabled = true;
                        button8.Enabled = true;

                        button_Connect.Enabled = false;
                        connectionActive = true;
                        workerThread = new Thread(comandNH.ListenToCommand);
                        workerThread.Start();
                        button_Connect.Text = "Đã kết nối camera";
                        button5.Text = "Ngắt kết nối";
                        Update();
                    }
                    catch (Exception ex)
                    {
                        button_Connect.Text = "Kết nối thất bại, thử lại?";
                        MessageBox.Show("Không tìm thấy camera tại địa chỉ đã chọn.");

                        return;
                    }
                    break;
                case cameraType.pelco:
                    try
                    {
                        PTZFacade ptz;
                        ptz = new PTZFacade(IPtextBox.Text, "admin", "admin");
                        comandPC = new CommandTransferPelco(ptz, this);
                        //comandPC = new CommandTransferPelco( this);
                        workerThread = new Thread(comandPC.ListenToCommand);
                        workerThread.Start();
                        button1.Enabled = true;
                        button2.Enabled = true;
                        button3.Enabled = true;
                        button4.Enabled = true;
                        //button5.Enabled = true;
                        button6.Enabled = true;
                        button8.Enabled = true;
                        connectionActive = true;
                        button_Connect.Enabled = false;
                        button_Connect.Text = "Đã kết nối camera";
                        button5.Text = "Ngắt kết nối";
                        Update();
                        break;
                    }
                    catch (Exception ex)
                    {
                        button_Connect.Text = "Kết nối thất bại, thử lại?";
                        MessageBox.Show(ex.ToString());

                        return;
                    }
                default:
                    break;
            }
            
            
        }
        private CommandTransferNH comandNH;
        private CommandTransferPelco comandPC;
        private void PtzControl_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            //x = Convert.ToInt32(numericUpDown2.Value);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            //y = Convert.ToInt32(numericUpDown3.Value);
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            //z = Convert.ToInt32(numericUpDown4.Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comandNH.tiltUp();
            //_ptz.Move(PTZFacade.MoveDirection.Up, Convert.ToInt32(velocityNumber.Value));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comandNH.panLeft();//_ptz.Move(PTZFacade.MoveDirection.Left, Convert.ToInt32(velocityNumber.Value));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comandNH.panRight();// _ptz.Move(PTZFacade.MoveDirection.Right, Convert.ToInt32(velocityNumber.Value));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            switch (camtype)
            {
                case cameraType.pelco:
                    comandPC.MoveDown();
                    break;

            }
            //comandNH.tiltDown();//_ptz.Move(PTZFacade.MoveDirection.Down, Convert.ToInt32(velocityNumber.Value));
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            comandNH.Stop();//_ptz.StopMoving();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //_ptz.SetZoomMagnification(float.Parse(numericUpDownmagnification.Text));
            if (connectionActive)
            {
                connectionActive = false;
                if (comandNH != null)
                {
                    comandNH.RequestStop();
                    workerThread.Abort();
                    comandNH.TurnOffCam();
                    tc.Disconnect();
                }
                if (comandPC != null)
                {
                    comandPC.RequestStop();
                    workerThread.Abort();

                }
                button5.Text = "Thoát";
                button_Connect.Text = "Kết nối";
                button_Connect.Enabled = true;
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button6.Enabled = false;
                button8.Enabled = false;
            }
            else
            {
                //workerObject.
                
                Application.ExitThread();
                Environment.Exit(0);
                //timer1.Enabled = false;
            }
        }

        private void velocityNumber_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void ResizeEvent(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                //notifyIcon.Visible = true;
                //notifyIcon.ShowBalloonTip(500);
                //this.Hide();
            }   
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            
            //workerObject.setKzoom(float.Parse(numericUpDown1.Text));
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }



        private void label6_Click(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!connectionActive) return;
            //FF 00 09 77 00 00
            try
            {
                byte[] cmd = new byte[8];
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x09;
                cmd[3] = 0x77;
                cmd[4] = 0x00;
                cmd[5] = 0x00;
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
                List<byte> input = tc.readBinary();
                if (input.Count == 0) return;
                if (input[0] == 0xFF && input[2] == 0x09 && input[3] == 0x77)
                {
                    double camazi = (input[4] << 8 | input[5]) / 65536.0 * 360.0;
                    //MessageBox.Show("CAMAZI," + camazi.ToString());
                    string sendData = "CAMAZI," + camazi.ToString() + ",";
                    using (UdpClient c = new UdpClient(1111))
                        c.Send(Encoding.ASCII.GetBytes(sendData), sendData.Length, "127.0.0.1",2917);
                }
            }
            catch (Exception exc)
            {
                return;
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Pelco")
            {
                camtype = cameraType.pelco;
            }
            if (comboBox1.Text == "Night Hawk")
            {
                camtype = cameraType.nighthawk;
            }
            if (comboBox1.Text == "Flir")
            {
                camtype = cameraType.flir;
            }
        }

        private void timerPelcoCommand_Tick(object sender, EventArgs e)
        {
            if (comandPC!=null)
            comandPC.Update();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        internal void addARPA(string[] strList)
        {
            arpaOBJ newobj;
            newobj.id = strList[1];
            float.TryParse(strList[2], out newobj.range);
            float.TryParse(strList[3], out newobj.azi);
            bool newdata = true;
            for (int i = 0; i < ListRadar.Count; i++)
            {
                if (ListRadar[i].id == newobj.id)
                {
                    ListRadar[i] = newobj;
                    newdata = false; break;
                }
            }

            if (newdata)
            {
                ListRadar.Add(newobj);
            }
            showTargets();
            
        }

        private void showTargets()
        {
            listBox1.Items.Clear();
            for (int i = 0; i < ListRadar.Count; i++)
            {
                String ss;

                ss = "Muc tieu:" + ListRadar[i].id + " ||Cu ly:" + ListRadar[i].range + " ||Phuong vi:" + ListRadar[i].azi;
                
                listBox1.Items.Add(ss);
                if (selectedTargetIndex == i)
                {

                    listBox1.SetSelected(i, true);
                }

            }
            
        }

        internal void targetUp()
        {
            if(selectedTargetIndex<10)selectedTargetIndex += 1;
            showTargets();
        }

        internal void targetDown()
        {
            if (selectedTargetIndex >0) selectedTargetIndex -= 1;
            showTargets();
        }

        public int selectedTargetIndex { get; set; }

        internal void ShowOpTop()
        {
            //this.WindowState = FormWindowState.Minimized;
            //this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.BringToFront();
        }
        internal void HideToTray()
        {
            this.WindowState = FormWindowState.Minimized;
            
        }

        public void ViewtData(int cx, int cy)
        {
            
        }

        public void ViewtData(int cx, int cy, bool onTracking, bool isconnected)
        {
            textBox1.Text = cx.ToString() + "|" + cy.ToString() + "|Track:" + onTracking.ToString() + "|Connected:" + isconnected.ToString();
        }

        public void GotoSelectedTarget()
        {
            
        }
    }
    public class Config
    {
        public double[] constants;
        public Config()
        {
            int nparam = 10;
            constants = new double[nparam];
            try
            {
                StreamReader sr = new StreamReader("camConfig.txt");
                for (int i = 0; i < nparam; i++)
                {
                    String[] line = sr.ReadLine().Split(' ');
                    constants[i] = double.Parse(line[i]);
                }
                
            }
            catch (Exception e)
            {
                //if (constants[0]==0) constants[0] = 20;//Delta H in metres
                //1-2 elevation calibrating
                if (constants[3] == 0) constants[3] = 8;//zoomk 
                if (constants[4] == 0) constants[4] = 20;//do nhay zoom IR
                if (constants[5] == 0) constants[5] = 8;//do nhay pan
                if (constants[6] == 0) constants[6] = 5;//do nhay tilt
                if (constants[7] == 0) constants[7] = 13;//min focus
                if (constants[8] == 0) constants[8] = 8;//do nhay zoom Vissible
                
                //MessageBox.Show(e.Message);2
            }
        }
    }
    public class CommandTransferNH
    {
        //socket
        UdpClient listener;
        IPEndPoint groupEP;
        // các biến trạng thái cua joystick
        public UsbHidDevice pDevice;
        bool bt1, bt2, bt3,
             bt4, bt5, bt6,
             bt7, bt8, bt9,
             bt10, bt11, bt12;
        int cx, cy;
        int mArrow;
        // các biến trạng thái chung
        bool dialoghidden = false;
        public volatile bool tinhChinh = true;
        // trạng thái mục tiêu radar
        double bearing;// goc phuong vi
        double elevation;// goc ta`
        double range;// cu ly mt
        public bool isColorCam = false;//biến chọn camera anh nhiet hay anh mau
        private bool onTracking = false;// trang thai ba'm
        public volatile Config config;
        private volatile bool _shouldStop = false;
        TelnetConnection tc;
        GuiMain m_Gui;
        double x_track = 0, y_track = 0;
        public CommandTransferNH(TelnetConnection tconnect, GuiMain ptzControl)
        {
            //ket noi den joystick
            pDevice = new UsbHidDevice(0x046D, 0xC215);
            //pDevice.OnConnected += DeviceOnConnected;
            //pDevice.OnDisConnected += DeviceOnDisConnected;
            pDevice.DataReceived += DeviceDataReceived;
            pDevice.Connect();
            // ket noi den camera
            tc = tconnect;
            config = new Config();
            //mo socket
            listener = new UdpClient(8001);
            groupEP = new IPEndPoint(IPAddress.Any, 0);
            initCommand();
        }
        private void ThreadSafe(MethodInvoker method)
        {
            if (m_Gui.InvokeRequired)
                m_Gui.Invoke(method);
            else
                method();
        }
        private void DeviceDataReceived(byte[] data)
        {
            int newcy = (data[2] >> 2) | ((data[3] & 0x0f) << 6);
            newcy = (newcy - 512) / 16;
            int newcx = (data[1] | ((data[2] & 0x03) << 8));
            newcx = (newcx - 512) / 16;
            int newvelo = 255 - data[6];
            int new_mArrow = data[3] >> 4;
            int buttons = (data[5] | (data[7]) << 8);
            bool newbt1 = ((buttons & 0x01) > 0);
            bool newbt2 = ((buttons & 0x02) > 0);
            bool newbt3 = ((buttons & 0x04) > 0);
            bool newbt4 = ((buttons & 0x08) > 0);
            bool newbt5 = ((buttons & 0x10) > 0);
            bool newbt6 = ((buttons & 0x20) > 0);
            bool newbt7 = ((buttons & 0x40) > 0);
            bool newbt8 = ((buttons & 0x80) > 0);
            bool newbt9 = ((buttons & 0x0100) > 0);
            bool newbt10 = ((buttons & 0x0200) > 0);
            bool newbt11 = ((buttons & 0x0400) > 0);
            bool newbt12 = ((buttons & 0x0800) > 0);
            //xy motion
            if (bt1 != newbt1)
            {
                bt1 = newbt1;
                if (bt1)
                {
                    if (!dialoghidden)
                    {
                        ThreadSafe(() => m_Gui.GotoSelectedTarget());
                    }
                    else if (!onTracking)
                    {
                        byte[] dgram;// bat dau track
                        dgram = new byte[2];
                        dgram[0] = 0xff;
                        dgram[1] = 0x01;
                        listener.Send(dgram, 2, "127.0.0.1", 8000);
                        onTracking = true;
                    }
                    else
                    {
                        byte[] dgram;// ngung track
                        dgram = new byte[2];
                        dgram[0] = 0xff;
                        dgram[1] = 0x00;
                        listener.Send(dgram, 2, "127.0.0.1", 8000);
                        Stop();
                        onTracking = false;
                    }
                }


            }

            if (bt4 != newbt4)
            {
                bt4 = newbt4;

                if (bt4)// phong to cua so track
                {
                    byte[] dgram;
                    dgram = new byte[2];
                    dgram[0] = 0xff;
                    dgram[1] = 0x02;
                    listener.Send(dgram, 2, "127.0.0.1", 8000);
                }

            }
            if (bt6 != newbt6)
            {
                bt6 = newbt6;

                if (bt6)// thu nho cua so track
                {
                    byte[] dgram;
                    dgram = new byte[2];
                    dgram[0] = 0xff;
                    dgram[1] = 0x03;
                    listener.Send(dgram, 2, "127.0.0.1", 8000);
                }
            }
            if (bt12 != newbt12)
            {
                bt12 = newbt12;

                if (bt12)
                {
                    if (dialoghidden)
                    {

                        ThreadSafe(() => m_Gui.ShowOpTop());

                        dialoghidden = false;
                    }
                    else
                    {
                        ThreadSafe(() => m_Gui.HideToTray());
                        dialoghidden = true;
                    }
                }

            }

            //fine move
            if (mArrow != new_mArrow)
            {
                mArrow = new_mArrow;
                double vazi, vtilt;
                switch (mArrow)
                {
                    case 2://right
                        panRight();
                        break;
                    case 4://down
                        tiltDown();
                        break;
                    case 6://left
                        panLeft();
                        break;
                    case 0://up
                        tiltUp();
                        break;
                    default:
                        Stop();
                        break;
                }
                
            }
            if ((newcx != cx || newcy != cy) && (mArrow == 8))
            {
                ThreadSafe(() => m_Gui.ViewtData(cx, cy, onTracking, isconnected));
                double vazi, vtilt;
                cx = newcx;
                cy = newcy;
                if (Math.Abs(cx) <= 1) vazi = 0; else vazi = cx / 10.0;
                if (Math.Abs(cy) <= 1) vtilt = 0; else vtilt = -cy / 20.0;
                if (isconnected && (!onTracking)) pelco_ptz.MoveXYZ(vazi, vtilt, 0);
            }
            //bt
            if (bt3 != newbt3)
            {
                bt3 = newbt3;
                PTZFacade.ZoomDirection zoomdir;
                if (bt3)
                    zoomdir = PTZFacade.ZoomDirection.In;
                else
                    zoomdir = PTZFacade.ZoomDirection.Stop;
                if (isconnected) pelco_ptz.Zoom(zoomdir);
            }
            if (bt5 != newbt5)
            {
                bt5 = newbt5;
                PTZFacade.ZoomDirection zoomdir;
                if (bt5)
                    zoomdir = PTZFacade.ZoomDirection.Out;
                else
                    zoomdir = PTZFacade.ZoomDirection.Stop;
                if (isconnected) pelco_ptz.Zoom(zoomdir);
            }

            //double.TryParse(coor[1], out azi);//degrees
            //azi = azi / 3.141592654 * 180;
            //double.TryParse(coor[2], out y);//m
            //double.TryParse(coor[3], out range);//km
            //if(y>0)config.constants[0] = y;
            //ialpha = (unsigned short)(0xffff*alpha/(2*3.141592654));

        }
        private void initCommand()
        {

        }
        private void stabOn()
        {
            byte[] cmd = new byte[8];
            //PA: B2 A5 E6 93
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x44;
            cmd[3] = 0x77;
            cmd[4] = 0x01;
            cmd[5] = 0x01;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        private void stabOff()
        {
            byte[] cmd = new byte[8];
            //PA: B2 A5 E6 93
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x44;
            cmd[3] = 0x77;
            cmd[4] = 0x01;
            cmd[5] = 0x03;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        public void ListenToCommand()
        {
            _shouldStop = false;
            try
            {
                
                
                while (!_shouldStop)
                {
                    byte[] receive_byte_array = listener.Receive(ref groupEP);
                    string received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                    if (receive_byte_array[0] == 0xff)//nhan du lieu bam tu anh Thi
                    {

                        int temp;
                        //lay gia tri x_track
                        if (receive_byte_array[1] >> 7 > 0)
                        {

                            temp = (receive_byte_array[1] << 8) | (receive_byte_array[2]) - 1 - 0xffff;


                        }
                        else
                        {
                            temp = (receive_byte_array[1] << 8) | (receive_byte_array[2]);
                        }
                        x_track += temp / 1500.0;// thay doi tuy theo truong hop
                        //lay gia tri y_track
                        if (receive_byte_array[3] >> 7 > 0)
                        {

                            temp = (receive_byte_array[3] << 8) | (receive_byte_array[4]) - 1 - 0xffff;

                        }
                        else
                        {
                            temp = (receive_byte_array[3] << 8) | (receive_byte_array[4]);
                        }
                        y_track += temp / 2000.0;//// thay doi tuy theo truong hop
                        if (y_track > 1) y_track = 1;
                        if (x_track > 1) x_track = 1;
                        if (y_track < -1) y_track = -1;
                        if (x_track < -1) x_track = -1;
                    }
                    //lenh dieu khien bang udp--------------------------------------------
                    string[] coor = received_data.Split(' ');
                    if ((coor[0] == "PTZSET") && (coor.Length >= 4))
                    {
                        double.TryParse(coor[1], out bearing);//degrees
                        //azi = azi / 3.141592654 * 180;
                        double.TryParse(coor[2], out elevation);//m
                        double.TryParse(coor[3], out range);//km
                        //if(y>0)config.constants[0] = y;
                        //ialpha = (unsigned short)(0xffff*alpha/(2*3.141592654));
                        Update();

                    }
                    else if ((coor[0] == "AZISET") && (coor.Length >= 2))
                    {
                        byte[] cmd = new byte[8];
                        double.TryParse(coor[1], out bearing);//degrees
                        //set azi ----------------------- 
                        //MessageBox.Show("new azi tracking");
                        ushort newazi = getAzi();
                        cmd[0] = 0xFF;
                        cmd[1] = 0x00;
                        cmd[2] = 0x05;
                        cmd[3] = 0x77;
                        cmd[4] = (byte)(newazi >> 8);
                        cmd[5] = (byte)(newazi);
                        cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                        tc.Write(cmd);

                    }
                    else if ((coor[0] == "2XMUL") && (coor.Length >= 2))
                    {
                        byte[] cmd = new byte[8];
                        byte mulstat;
                        byte.TryParse(coor[1], out mulstat);//degrees
                        //set 2x off FF 00 32 77 02 xx 00= out; 01 = In, 
                        cmd[0] = 0xFF;
                        cmd[1] = 0x00;
                        cmd[2] = 0x32;
                        cmd[3] = 0x77;
                        cmd[4] = 0x02;
                        cmd[5] = mulstat;
                        cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                        tc.Write(cmd);
                    }
                    else if ((coor[0] == "PTZMOV") && (coor.Length >= 2))
                    {
                        switch (coor[1])
                        {
                            case "IIN":
                                zoomIRIn();
                                break;
                            case "IOUT":
                                zoomIROut();
                                break;
                            case "VIN":
                                if (!tinhChinh) zoomIRIn();
                                else
                                    zoomVisIn();
                                break;
                            case "VOUT":
                                if (!tinhChinh) zoomIROut();
                                else
                                    zoomVisOut();
                                break;
                            case "TCON":
                                tinhChinh = true;
                                break;
                            case "TCOF":
                                tinhChinh = false;
                                break;
                            case "UP":
                                tiltUp();
                                break;
                            case "DOWN":
                                tiltDown();
                                break;
                            case "LEFT":
                                panLeft();
                                break;
                            case "RGHT":
                                panRight();
                                break;
                            case "VNR":
                                focusVNear();
                                break;
                            case "VFR":
                                focusVFar();
                                break;
                            case "INR":
                                focusINear();
                                break;
                            case "IFR":
                                focusIFar();
                                break;
                            case "AF":
                                autoFocus();
                                break;
                            case "STOP":
                                Stop();
                                break;
                            //NSTB
                            case "NSTB":
                                stabOff();
                                break;
                            case "STB":
                                stabOn();
                                break;
                            default:
                                break;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return;
            }

        }

        public void Stop()
        {
            //MessageBox.Show("Stop");
            byte[] cmd = new byte[8];
            //stop moving
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x00;
            cmd[4] = 0x00;
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        public void StopFocus()
        {
            byte[] cmd = new byte[8];

            //stop focus
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x01;
            cmd[3] = 0x00;
            cmd[4] = 0x00;
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        public void focusINear()
        {
            byte[] cmd = new byte[8];
            uint rate = (uint)config.constants[4];
            if (rate > 127) rate = 127;
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x01;
            cmd[3] = 0x00;
            cmd[4] = (byte)(rate);
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);

        }
        public void focusVNear()
        {
            byte[] cmd = new byte[8];
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x32;
            cmd[3] = 0x77;
            cmd[4] = 0x06;
            cmd[5] = 0x01;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }

        public void panRight()
        {
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint rate = (uint)config.constants[5];
            if (!tinhChinh) rate *= 2;
            if (rate > 127) rate = 127;
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x02;
            cmd[4] = (byte)(rate);
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        public void pan(double rate)
        {
            if (Math.Abs(rate) > 1) return;
            if (rate >= 0)
            {
                byte[] cmd = new byte[8];
                //set rate
                int irate = Convert.ToInt32(rate * 127);
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x00;
                cmd[3] = 0x02;
                cmd[4] = (byte)(irate);
                cmd[5] = 0x00;
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
            }
            else
            {
                rate = -rate;
                byte[] cmd = new byte[8];
                //set rate
                int irate = Convert.ToInt32(rate * 127);
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x00;
                cmd[3] = 0x04;
                cmd[4] = (byte)(irate);
                cmd[5] = 0x00;
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
            }
        }
        public void panLeft()
        {
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint rate = (uint)config.constants[5];
            if (!tinhChinh) rate *= 2;
            if (rate > 127) rate = 127;
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x04;
            cmd[4] = (byte)(rate);
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        public void focusIFar()
        {
            byte[] cmd = new byte[8];
            uint rate = (uint)config.constants[4];
            if (rate > 127) rate = 127;
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x80;
            cmd[4] = (byte)(rate);
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        public void focusVFar()
        {
            byte[] cmd = new byte[8];

            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x32;
            cmd[3] = 0x77;
            cmd[4] = 0x06;
            cmd[5] = 0x02;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);

        }
        public void tilt(double rate)//
        {

            if (Math.Abs(rate) > 1) return;
            if (rate >= 0)
            {
                byte[] cmd = new byte[8];
                //set rate
                int irate = Convert.ToInt32(rate * 63);
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x00;
                cmd[3] = 0x08;
                cmd[4] = 0x00;
                cmd[5] = (byte)(irate);
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
            }
            else
            {
                rate = -rate;
                byte[] cmd = new byte[8];
                //set rate
                int irate = Convert.ToInt32(rate * 63);
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x00;
                cmd[3] = 0x10;
                cmd[4] = 0x00;
                cmd[5] = (byte)(irate);
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
            }
        }
        public void tiltDown()
        {

            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint rate = (uint)config.constants[6];
            if (!tinhChinh) rate *= 2;
            if (rate > 63) rate = 63;

            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x10;
            cmd[4] = 0x00;
            cmd[5] = (byte)(rate);
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }

        public void tiltUp()
        {
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint rate = (uint)config.constants[6];
            if (!tinhChinh) rate *= 2;
            if (rate > 63) rate = 63;

            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x08;
            cmd[4] = 0x00;
            cmd[5] = (byte)(rate);
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }

        public void zoomIROut()//zoom IR cam
        {
            //zoomVisOut();
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint rate = (uint)config.constants[4];
            if (!tinhChinh) rate *= 2;
            if (rate > 63) rate = 63;

            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x40;
            cmd[4] = 0x00;
            cmd[5] = (byte)(rate);
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        public void autoFocus()
        {
            byte[] cmd = new byte[8];
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x24;
            cmd[3] = 0x77;
            cmd[4] = 0x0D;
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        public void zoomVisIn()
        {
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint rate = (uint)config.constants[8];
            //if (!tinhChinh) rate *= 2;
            if (rate > 127) rate = 127;
            //ushort newazi = getAzi();
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x0E;
            cmd[3] = 0x77;
            cmd[4] = (byte)(rate);
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
            ushort focus = (ushort)config.constants[7];//set standart focus
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x08;
            cmd[3] = 0x77;
            cmd[4] = (byte)((focus >> 8) | ((byte)0x10));//focus go to min for visual
            cmd[5] = (byte)(focus);
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        public void zoomVisOut()
        {
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            double a = config.constants[8];
            //if(!tinhChinh)a*=2;
            uint rate = (uint)(255 - a);

            if (rate < 128) rate = 128;
            //ushort newazi = getAzi();
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x0E;
            cmd[3] = 0x77;
            cmd[4] = (byte)(rate);
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
            ushort focus = (ushort)config.constants[7];//set standart focus
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x08;
            cmd[3] = 0x77;
            cmd[4] = (byte)((focus >> 8) | ((byte)0x10));//focus go to min for visual
            cmd[5] = (byte)(focus);
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);

        }
        public void zoomIRIn()//zoom IR cam
        {
            //zoomVisIn();
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint rate = (uint)config.constants[4];
            //if (!tinhChinh) rate *= 2;
            if (rate > 63) rate = 63;
            ushort newazi = getAzi();
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x20;
            cmd[4] = 0x00;
            cmd[5] = (byte)(rate);
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        private ushort getAzi()
        {

            return (ushort)(0xffff * ((bearing) / (360)));
        }
        public void Update()
        {
            if (onTracking)
            {
                pan(x_track);
                tilt(y_track);
            }
            else
            {
                if (range == 0) return;
                byte[] cmd = new byte[8];
                //set azi ----------------------- 
                ushort newazi = getAzi();
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x05;
                cmd[3] = 0x77;
                cmd[4] = (byte)(newazi >> 8);
                cmd[5] = (byte)(newazi);
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
                //set elevation ---------------- 
                ushort newEL = getEL();
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x06;
                cmd[3] = 0x77;
                cmd[4] = (byte)(newEL >> 8);
                cmd[5] = (byte)(newEL);
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
                //set zoom  goto FF 00 07 77 ax xx---------------- 
                ushort newZoom = getZoomIR();//IR camera zoom
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x07;
                cmd[3] = 0x77;
                cmd[4] = (byte)((newZoom >> 8) | (0xA0));// //IR camera zoom
                cmd[5] = (byte)(newZoom);
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
                //Vis camera zoom
                newZoom = getZoomVis();
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x07;
                cmd[3] = 0x77;
                cmd[4] = (byte)((newZoom >> 8) | (0x60));// //VIS camera zoom
                cmd[5] = (byte)(newZoom);
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
                //set focus FF 00 08 77 ax xx---------------- 
                ushort focus = (ushort)config.constants[7];//set standart focus
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x08;
                cmd[3] = 0x77;
                cmd[4] = (byte)((focus >> 8) | ((byte)0x10));//focus go to min for visual
                cmd[5] = (byte)(focus);
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
            }
            //stabOff();
        }



        private ushort getZoomIR()
        {
            uint FOV = (uint)(config.constants[3] / range);//field of view in degree
            if (FOV > 255) FOV = 255;
            ushort data = 0;
            byte carry;
            carry = (byte)(FOV % 10);
            data += carry;

            FOV /= 10;
            carry = (byte)(FOV % 10);
            data += (ushort)(carry << 4);

            FOV /= 10;
            carry = (byte)(FOV % 10);
            data += (ushort)(carry << 8);
            return data;
        }
        private ushort getZoomVis()
        {
            uint FOV = (uint)(config.constants[3] / range * 2);//field of view in degree
            if (FOV > 255) FOV = 255;
            ushort data = 0;
            byte carry;
            carry = (byte)(FOV % 10);
            data += carry;

            FOV /= 10;
            carry = (byte)(FOV % 10);
            data += (ushort)(carry << 4);

            FOV /= 10;
            carry = (byte)(FOV % 10);
            data += (ushort)(carry << 8);
            return data;
        }
        private ushort getEL()
        {

            double EL = -Math.Atan(elevation / 1000 / range);
            double ELcalib = Math.Cos(Math.Abs(bearing - config.constants[1] / 57.2957795)) * config.constants[2] / 57.2957795;
            EL += ELcalib;// in radian
            EL += config.constants[0] / 57.2957795;
            if (EL < 0) EL += 6.283185307;
            return (ushort)(0xffff * (EL / (6.283185307)));
        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        // Volatile is used as hint to the compiler that this data 
        // member will be accessed by multiple threads. 


        internal void SetValue(double p1, double p3)
        {
            bearing = p1;
            range = p3;
            if (range == 0) return;
            Update();
        }

        internal void CamsSelect(bool p)
        {
            isColorCam = p;
        }

        internal void TurnOffCam()
        {
            byte[] cmd = new byte[8];
            //set elevation ---------------- 
            ushort newEL = 0xD554;
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x06;
            cmd[3] = 0x77;
            cmd[4] = (byte)(newEL >> 8);
            cmd[5] = (byte)(newEL);
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
    }
   
    public class CommandTransferPelco
    {
        public volatile bool tinhChinh = true;
        int cx, cy;
        int zoom;
        int fineMove;
        bool bt1, bt2, bt3, bt4, bt5, bt6, bt12;
        public volatile Config config;
        private volatile bool _shouldStop = false;
        PTZFacade pelco_ptz;
        //private PTZFacade ptz;
        private GuiMain ptzControl;
        public UsbHidDevice Device;
        bool onTracking;
        bool isconnected = false;
        bool dialoghidden = false;
        UdpClient listener ;
        IPEndPoint groupEP ;
        public CommandTransferPelco(PTZFacade ptz, GuiMain ptzControl)
        {

            // TODO: Complete member initialization
            this.pelco_ptz = ptz;
            isconnected = true;
            config = new Config();
            initCommand();
           
            listener = new UdpClient(8001);
            groupEP = new IPEndPoint(IPAddress.Any, 0);
            
            this.ptzControl = ptzControl;
            Device = new UsbHidDevice(0x046D, 0xC215);
            Device.OnConnected += DeviceOnConnected;
            Device.OnDisConnected += DeviceOnDisConnected;
            Device.DataReceived += DeviceDataReceived;
            Device.Connect();
            onTracking = false;
        }
        public CommandTransferPelco(  GuiMain ptzControl)
        {

            // TODO: Complete member initialization
            
            config = new Config();
            initCommand();
            
                listener = new UdpClient(8001);
                groupEP = new IPEndPoint(IPAddress.Any, 0);
            
            this.ptzControl = ptzControl;
            Device = new UsbHidDevice(0x046D, 0xC215);
            Device.OnConnected += DeviceOnConnected;
            Device.OnDisConnected += DeviceOnDisConnected;
            Device.DataReceived += DeviceDataReceived;
            Device.Connect();
            onTracking = false;
        }
        private void DeviceDataReceived(byte[] data)
        {
            int  newcy = (data[2]>>2) | ((data[3]&0x0f) << 6);
            newcy = (newcy - 512) / 16;
            int newcx = (data[1]|((data[2]&0x03)<<8));
            newcx = (newcx - 512) / 16;
            int newvelo = 255 - data[6];
                      
            int newmove = data[3]>>4;

            bool newbt1 = (((data[5] | (data[7]) << 8) & 0x01) > 0);
            bool newbt3 = (((data[5] | (data[7]) << 8) & 0x04) > 0);
            bool newbt4 = (((data[5] | (data[7]) << 8) & 0x08) > 0);
            bool newbt5 = (((data[5] | (data[7]) << 8) & 0x10) > 0);
            bool newbt6 = (((data[5] | (data[7]) << 8) & 0x20) > 0);
            bool newbt12 = (((data[5]| (data[7]) << 8) & 0x0800) > 0);
            //xy motion
            if (bt1 != newbt1)
            {
                bt1 = newbt1;
                if (bt1)
                {
                    if (!dialoghidden)
                    {
                        ThreadSafe(() => ptzControl.GotoSelectedTarget());
                    }
                    else if(!onTracking)
                    {
                        byte[] dgram;
                        dgram = new byte[2];
                        dgram[0] = 0xff;
                        dgram[1] = 0x01;
                        listener.Send(dgram, 2, "127.0.0.1", 8000);
                        onTracking = true;
                    }else
                    {
                        byte[] dgram;
                        dgram = new byte[2];
                        dgram[0] = 0xff;
                        dgram[1] = 0x00;
                        listener.Send(dgram, 2, "127.0.0.1", 8000);
                        if (isconnected) pelco_ptz.StopMoving();
                        onTracking = false;
                    }
                }
                

            }
            
            if (bt4 != newbt4)
            {
                bt4 = newbt4;

                if (bt4)
                {
                    byte[] dgram;
                    dgram = new byte[2];
                    dgram[0] = 0xff;
                    dgram[1] = 0x02;// track window  bigger
                    listener.Send(dgram, 2, "127.0.0.1", 8000);
                }

            }
            if (bt6 != newbt6)
            {
                bt6 = newbt6;

                if (bt6)
                {
                    byte[] dgram;
                    dgram = new byte[2];
                    dgram[0] = 0xff;
                    dgram[1] = 0x03;// track window  smaller
                    listener.Send(dgram, 2, "127.0.0.1", 8000);
                }
            }
            if (bt12 != newbt12)
            {
                bt12 = newbt12;

                if (bt12)
                {
                    if (dialoghidden)
                    {
                        
                        ThreadSafe(() => ptzControl.ShowOpTop());
                        
                        dialoghidden = false;
                    }else
                    {
                        ThreadSafe(() => ptzControl.HideToTray());
                        dialoghidden = true;
                    }
                }
                
            }
            
            //fine move
            if (fineMove != newmove)
            {
                fineMove = newmove;
                double vazi, vtilt;
                switch (fineMove)
                {
                    case 2://right
                        vazi = 0.7 + newvelo / 127.0;
                        vtilt = 0;
                        break;
                    case 4://down
                        if (!dialoghidden)
                        {
                            ThreadSafe(() => ptzControl.targetUp());
                            return;
                        }
                        vazi = 0;
                        vtilt = -(0.5 + newvelo / 127.0);
                        break;
                    case 6://left
                        
                        vazi = -(0.7 + newvelo / 127.0);
                        vtilt = 0;
                        break;
                    case 0://up
                        if (!dialoghidden)
                        {
                            ThreadSafe(() => ptzControl.targetDown());
                             return;
                        }
                        vazi = 0;
                        vtilt = 0.5 + newvelo / 127.0;
                        break;
                    default:
                        vazi = 0;
                        vtilt = 0;
                        break;
                }
                //vazi = vazi;
                //vtilt = vtilt;
                if (isconnected) pelco_ptz.MoveXYZ(vazi, vtilt, 0);
            }
            if ((newcx != cx || newcy != cy) && (fineMove == 8))
            {
                ThreadSafe(() => ptzControl.ViewtData(cx,cy,onTracking,isconnected));
                double vazi, vtilt;
                cx = newcx;
                cy = newcy;
                if (Math.Abs(cx) <= 1) vazi = 0; else vazi = cx / 10.0;
                if (Math.Abs(cy) <= 1) vtilt = 0; else vtilt = -cy / 20.0;
                if (isconnected&&(!onTracking)) pelco_ptz.MoveXYZ(vazi, vtilt, 0);
            }
            //bt
            if (bt3 != newbt3)
            {
                bt3 = newbt3;
                PTZFacade.ZoomDirection zoomdir;
                if (bt3)
                    zoomdir = PTZFacade.ZoomDirection.In;
                else
                    zoomdir = PTZFacade.ZoomDirection.Stop;
                if (isconnected) pelco_ptz.Zoom(zoomdir);
            }
            if (bt5 != newbt5)
            {
                bt5 = newbt5;
                PTZFacade.ZoomDirection zoomdir;
                if (bt5)
                    zoomdir = PTZFacade.ZoomDirection.Out;
                else
                    zoomdir = PTZFacade.ZoomDirection.Stop;
                if (isconnected) pelco_ptz.Zoom(zoomdir);
            }

            //double.TryParse(coor[1], out azi);//degrees
            //azi = azi / 3.141592654 * 180;
            //double.TryParse(coor[2], out y);//m
            //double.TryParse(coor[3], out range);//km
            //if(y>0)config.constants[0] = y;
            //ialpha = (unsigned short)(0xffff*alpha/(2*3.141592654));
                       
        }

        private void AppendText(string p)
        {
            //ThreadSafe(() => ptzControl.textBox1.AppendText(p + Environment.NewLine));
        }

        private void DeviceOnDisConnected()
        {
            //ThreadSafe(() => ptzControl.checkBox1.Enabled = false);

        }

        private void DeviceOnConnected()
        {
            //ThreadSafe(() => ptzControl.checkBox1.Enabled = true);
        }
        private void ThreadSafe(MethodInvoker method)
        {
            if (ptzControl.InvokeRequired)
                ptzControl.Invoke(method);
            else
                method();
        }
        public void Update()
        {
            

            
        }
        private void initCommand()
        {
            //pelco_ptz.Move(PTZFacade.MoveDirection.Down);
        }
        public void MoveDown()
        {
            if (isconnected) pelco_ptz.Move(PTZFacade.MoveDirection.Down);
        }
        private void Stop()
        {

            if (isconnected) pelco_ptz.StopMoving();
        }
        public void ListenToCommand()
        {
            _shouldStop = false;

            try
            {
                
                double x_value = 0, y_value = 0;
                int tracknum = 0;
                //bool onTracking = false;
                while (!_shouldStop)
                {
                    
                    byte[] receive_byte_array = listener.Receive(ref groupEP);
                    
                    if (receive_byte_array[0] == 0xff)//nhan du lieu bam tu anh Thi
                    {
                        
                        int temp;
                        if (receive_byte_array[1] >> 7 > 0)
                        {
                            
                            temp = (receive_byte_array[1] << 8) | (receive_byte_array[2]) - 1 - 0xffff;
                            

                        }
                        else {
                            temp = (receive_byte_array[1] << 8) | (receive_byte_array[2]);
                        }
                        x_value += temp / 15.0;// thay doi tuy theo truong hop

                        if (receive_byte_array[3] >> 7 > 0)
                        {

                            temp = (receive_byte_array[3] << 8) | (receive_byte_array[4]) - 1 - 0xffff;

                        }
                        else
                        {
                            temp = (receive_byte_array[3] << 8) | (receive_byte_array[4]);
                        }
                        y_value += temp / 20.0;//// thay doi tuy theo truong hop
                        
                        tracknum++;
                        if (tracknum > 0)
                        {
                            if (y_value > 0.8) y_value = 0.8;
                            if (x_value > 1.2) x_value = 1.2;
                            if (y_value < -0.8) y_value = -0.8;
                            if (x_value < -1.2) x_value = -1.2;
                            if (isconnected) pelco_ptz.MoveXYZ(x_value, -y_value, 0);
                            tracknum = 0;
                            x_value = 0;
                            y_value = 0;
                        }
                           
                    }
                    string received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);

                    //MessageBox.Show(received_data);
                    string[] strList = received_data.Split(',');
                    if ((strList[0] == "$RATTM") && (strList.Length > 4))//radar
                    {
                        
                        ThreadSafe(() => ptzControl.addARPA(strList));
                    }
                   
                    
                }
                
                    

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

        }




        internal void RequestStop()
        {
            listener.Close();
            _shouldStop = true;
        }
    }

}
