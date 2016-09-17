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
using System.Diagnostics;

namespace Camera_PTZ
{
    public struct arpaOBJ
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
    enum cameraType { pelco, nighthawk, flir } ;
    public partial class GuiMain : Form
    {

        public volatile bool connectionActive;
        Thread workerThread;
        //Socket UDPsock;
        cameraType camtype;
        public Config mConfig;
        public List<arpaOBJ> ListRadar = new List<arpaOBJ>();
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
        UdpClient radarDatalisten;
        public GuiMain()
        {
           
            InitializeComponent();
            mConfig = new Config();
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0,resolution.Height/2 );
            //this.Show();
            //UDPsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            button5.Enabled = true;
            camtype = cameraType.nighthawk;
            TopLevel = true;
            TopMost = true;
            connectionActive = false;
            radarDatalisten = new UdpClient((int)mConfig.constants[4]);
            radarDatalisten.BeginReceive(Receive, new object());
            //sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        int radarCount = 5;
        private void Receive(IAsyncResult ar)
        {
            radarCount = 5;
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, (int)mConfig.constants[4]);
            byte[] receive_byte_array = radarDatalisten.Receive(ref ip);
            for (int i = 0; i < receive_byte_array.Length; i++)
            {
                if ((receive_byte_array[i] < Convert.ToByte(' ')) || (receive_byte_array[i] > Convert.ToByte('~')))
                {
                    receive_byte_array[i] = Convert.ToByte(',');
                }
            }
            string received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
            string[] strList = received_data.Split(',');
            for (int i = 0; i < strList.Length - 4; i++)
            {
                addARPA(strList);
                //strList.(0);
            }
            radarDatalisten.BeginReceive(Receive, new object());
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

        
        private ControllerNightHawk comandNH;
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
        public void exitCam()
        {
            try
            {
                comandNH.RequestStop();
                workerThread.Abort();
                comandNH.TurnOffCam();
                tc.Disconnect();
                //Process.Start(@"C:\Windows\System32\shutdown.exe /s /t 0");
                Application.ExitThread();
                Environment.Exit(0);
                
            }
            catch (Exception e)
            {
                return;
            }
            
        }
        private void button5_Click(object sender, EventArgs e)
        {
            //_ptz.SetZoomMagnification(float.Parse(numericUpDownmagnification.Text));
            if (connectionActive)
            {
                connectionActive = false;
                if (comandNH != null)
                {
                    exitCam();

                }
                if (comandPC != null)
                {
                    comandPC.RequestStop();
                    workerThread.Abort();

                }
                Application.ExitThread();
                Environment.Exit(0);
                button5.Text = "Thoát";

                //button1.Enabled = false;
                //button2.Enabled = false;
                //button3.Enabled = false;
                //button4.Enabled = false;
                //button6.Enabled = false;
                //button8.Enabled = false;
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
        bool isWaitingForConnect = false;
        int failedconnectionCount = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(radarCount>0)radarCount--;
            if(radarCount==0)
            {
                label_radar_stat.Text = "Chưa kết nối radar";
            }
            else {
                label_radar_stat.Text = "Đã kết nối radar";
            }
            showTargets();
            if (isWaitingForConnect) return;
            BackgroundWorker bw = new BackgroundWorker();
            // this allows our worker to report progress during work
            //bw.WorkerReportsProgress = true;
            // what to do in the background thread
            bw.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {

                if (connectionActive)
                {
                    if (tc.connectionAborted)
                    {
                        connectionActive = false;
                        if (comandNH != null)
                        {
                            comandNH.RequestStop();
                            workerThread.Abort();
                            
                            tc.Disconnect();
                        }

                    }
                    return;
                }
                try
                {

                    //_ptz = new PTZFacade(IPtextBox.Text.ToString(), textBox2.Text.ToString(), textBox3.Text.ToString());
                    //_ptz.Move(x, y, z);
                    isWaitingForConnect = true;
                    tc = new TelnetConnection(mConfig.ipAdress, 23);
                    comandNH = new ControllerNightHawk(tc, this);
                    connectionActive = true;
                    workerThread = new Thread(comandNH.ListenToCommand);
                    workerThread.Start();
                    //button_Connect.Text = "Đã kết nối camera";
                    //button5.Text = "Ngắt kết nối";
                    Update();
                }
                catch (Exception ex)
                {
                    //button_Connect.Text = "Kết nối thất bại, thử lại?";
                    //MessageBox.Show("Không tìm thấy camera tại địa chỉ đã chọn.");
                    
                    return;
                }
            });

            // what to do when progress changed (update the progress bar for example)
            //bw.ProgressChanged += new ProgressChangedEventHandler(
            //delegate(object o, ProgressChangedEventArgs args)
            //{
              //  button5.Text = string.Format("{0}% Completed", args.ProgressPercentage);
            //});

            // what to do when worker completes its task (notify the user)
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate(object o, RunWorkerCompletedEventArgs args)
            {
                if (connectionActive)
                { 
                    this.textBox1.Text = "Đã kết nối camera";
                    failedconnectionCount=0;
                    isWaitingForConnect = false;
                }
                else
                {
                    failedconnectionCount++;
                    this.textBox1.Text = "Không thể kết nối đến camera."+failedconnectionCount.ToString();
                    isWaitingForConnect = false;
                }
                    
            });

            bw.RunWorkerAsync();
            return;
            //________________________________________

            
            //FF 00 09 77 00 00
            
           
        }

        private void label5_Click(object sender, EventArgs e)
        {

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
            for(int i = 0;i<strList.Length-4;i++)
            {
                if (strList[i] == "$RATTM")
                {
                    arpaOBJ newobj;
                    newobj.id = strList[1];
                    float.TryParse(strList[2], out newobj.range);
                    float.TryParse(strList[3], out newobj.azi);
                    bool newdata = true;
                    for (int j = 0; j < ListRadar.Count; j++)
                    {
                        if (ListRadar[j].id == newobj.id)
                        {
                            ListRadar[j] = newobj;
                            newdata = false; break;
                        }
                    }

                    if (newdata)
                    {
                        ListRadar.Add(newobj);
                    }
                    
                }
            }
            
            
        }

        private void showTargets()
        {
            listBox1.Invoke( (MethodInvoker)delegate ()
            {
                listBox1.Items.Clear();
            });
            
            for (int i = 0; i < ListRadar.Count; i++)
            {
                String ss;

                ss = "MT số:" + ListRadar[i].id + " ||Cự ly:" + ListRadar[i].range + " ||P.vị:" + ListRadar[i].azi;
                listBox1.Invoke((MethodInvoker)delegate()
                {
                    listBox1.Items.Add(ss);
                });
                
                if (selectedTargetIndex == i)
                {
                    listBox1.Invoke((MethodInvoker)delegate()
                    {
                        listBox1.SetSelected(i, true);
                    });
                    
                }

            }
            
        }

        internal void targetUp()
        {
            if(selectedTargetIndex>0)selectedTargetIndex -= 1;
            showTargets();
        }

        internal void targetDown()
        {
            if (selectedTargetIndex < ListRadar.Count-1) selectedTargetIndex += 1;
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

        public void ViewtData(int cx, int cy,double azi, bool onTracking, bool isconnected)
        {
            textBox1.Text = "X:"+cx.ToString() + "|Y:" +"|Azi:"+azi.ToString("0.0") + cy.ToString() + "|Track:" + onTracking.ToString() + "|Connected:" + isconnected.ToString();
        }

        public void  GotoSelectedTarget()
        {
            
        }

        private void button_add_target_Click(object sender, EventArgs e)
        {
            arpaOBJ newobj;
            newobj.id = textBox_t_num.Text;
            float.TryParse(textBox_t_range.Text, out newobj.range);
            newobj.range /= 1.852f;
            float.TryParse(textBox_t_bearing.Text, out newobj.azi);
            bool newdata = true;
            for (int j = 0; j < ListRadar.Count; j++)
            {
                if (ListRadar[j].id == newobj.id)
                {
                    ListRadar[j] = newobj;
                    newdata = false; break;
                }
            }

            if (newdata)
            {
                ListRadar.Add(newobj);
            }
        }
    }
    public class Config
    {
        public double[] constants;
        public String ipAdress;
        public String trackerFile;
        public String WorkingDir;
        public Config()
        {
            int nparam = 20;
            constants = new double[nparam];
            
            double[] constantsDefault = new double[nparam];
            constantsDefault[2] = 0;// chuan phuong bac
            constantsDefault[3] = 8;//zoomk 
            constantsDefault[4] = 2917;//cong du lieu radar
            constantsDefault[5] = 10;//do nhay pan
            constantsDefault[6] = 8;//do nhay tilt
            constantsDefault[7] = 13;//min focus
            constantsDefault[8] = 15;//do nhay zoom Vissible--
            constantsDefault[9] = 0;//
            constantsDefault[10] = 5;//do nhay focus IR--

            try
            {
                StreamReader sr = new StreamReader(@"C:\Program Files\NHCamera\cam_config.txt");
                for (int i = 0; i < nparam; i++)
                {
                    String str = sr.ReadLine();
                    // doc cac gia tri config
                    if (str != null)
                    {
                        String[] line = str.Split(';');
                        if (i == 0)
                        {
                            ipAdress = line[0];

                        }
                        else
                        if (i == 1)
                        {
                            trackerFile = line[0];
                            String[] strList = trackerFile.Split('\\');
                            strList[strList.Length - 1] = null;
                            WorkingDir = string.Join("\\",strList);
                            

                        }
                        else if (i > 1)
                        {
                            constants[i] = double.Parse(line[0]);
                        }
                    }
                    else
                    {
                        constants[i] = constantsDefault[i];
                    }
                }
                
            }
            catch (Exception e)
            {
                //if (constants[0]==0) constants[0] = 20;//Delta H in metres
                //1-2 elevation calibrating
                if (ipAdress==null) ipAdress = "192.168.150.92";
                if (trackerFile == null) trackerFile = "C:\\Program Files\\NHCamera\\TrackCam.exe";
                String[] strList = trackerFile.Split('\\');
                strList[strList.Length - 1] = null;
                WorkingDir = strList.ToString();
                for (int i = 0; i < nparam; i++)
                {
                    constants[i] = constantsDefault[i];
                }
                //MessageBox.Show(e.Message);2
            }
        }
    }
    
    public class CommandTransferPelco
    {
        public volatile bool tinhChinh = true;
        int cx, cy;
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
                //ThreadSafe(() => ptzControl.ViewtData(cx,cy,onTracking,isconnected));
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
