using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using UsbHid;
using UsbHid.USB.Classes.Messaging;
namespace Camera_PTZ
{
    public class ControllerNightHawk
    {
        //socket
        UdpClient listener;
        IPEndPoint groupEP;
        // các biến trạng thái cua joystick
        public UsbHidDevice pDevice;
        bool bt1 = false, bt2 = false, bt3 = false,
             bt4 = false, bt5 = false, bt6 = false,
             bt7 = false, bt8 = false, bt9 = false,
             bt10 = false, bt11 = false, bt12 = false;
        int joystick_x, joystick_y;
        int mArrow;
        // các biến trạng thái chung
        private volatile bool _shouldStop = false;
        public bool isColorCam = false;//biến chọn camera ảnh nhiệt/ảnh màu
        private bool onTracking = false;// trang thai ba'm
        bool dialoghidden = false;
        public volatile bool tinhChinh = true;
        // trạng thái mục tiêu radar
        double bearing;// goc phuong vi
        double elevation;// goc ta`
        double range;// cu ly mt
        System.Threading.Timer mUpdateTimer;
        public volatile Config config;

        TelnetConnection tc;
        GuiMain m_Gui;
        double x_track = 0, y_track = 0;
        public ControllerNightHawk(TelnetConnection tconnect, GuiMain ptzControl)
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
            m_Gui = ptzControl;
            //mo socket
            mUpdateTimer = new System.Threading.Timer(TimerCallback, null, 30, 30);
            listener = new UdpClient(8001);
            groupEP = new IPEndPoint(IPAddress.Any, 0);
            initCommand();
        }
        public void SetToRadarTarget()
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
        private void TimerCallback(object state)
        {

            if (m_Gui.connectionActive) Update();
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
            int newjy = (data[2] >> 2) | ((data[3] & 0x0f) << 6);
            newjy = (newjy - 512) / 16;// giá trị từ -32 đến 32
            int newjx = (data[1] | ((data[2] & 0x03) << 8));
            newjx = (newjx - 512) / 16;// giá trị từ -32 đến 32
            int newvelo = 255 - data[6];// giá trị từ 0 đến 255
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
                        this.bearing = m_Gui.ListRadar[(m_Gui.selectedTargetIndex)].azi+config.constants[9];
                        this.range = m_Gui.ListRadar[(m_Gui.selectedTargetIndex)].range*1.852;
                        this.SetToRadarTarget();
                    }
                    else if (!onTracking)
                    {
                        byte[] dgram;// bắt đầu track
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
                if (bt4)// phóng to cửa sổ bám
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

                if (bt6)// thu nhỏ cửa sổ bám
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
                if (bt12)// ẩn/hiện giao diện hiển thị
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
                switch (mArrow)
                {
                    case 2://right

                        panRight();
                        break;
                    case 4://down
                        if (!dialoghidden)
                        {
                            ThreadSafe(() => m_Gui.targetDown());
                            return;
                        }
                        tiltDown();
                        break;
                    case 6://left
                        panLeft();
                        break;
                    case 0://up
                        if (!dialoghidden)
                        {
                            ThreadSafe(() => m_Gui.targetUp());
                            return;
                        }
                        tiltUp();
                        break;
                    default:
                        Stop();
                        break;
                }

            }
            bt2 = newbt2;
            if (bt2 && (mArrow == 8))
            {

                ThreadSafe(() => m_Gui.ViewtData(joystick_x, joystick_y, onTracking, true));
                //double vazi, vtilt;
                joystick_x = newjx;
                joystick_y = newjy;
                //if (Math.Abs(joystick_x) <= 1) vazi = 0; else vazi = joystick_x / 10.0;
                //if (Math.Abs(joystick_y) <= 1) vtilt = 0; else vtilt = -joystick_y / 20.0;
                //if ( (!onTracking)) pelco_ptz.MoveXYZ(vazi, vtilt, 0);

            }
            else
            {
                joystick_x = 0;
                joystick_y = 0;

            }

            //bt
            if (bt3 != newbt3)
            {
                bt3 = newbt3;

                if (bt3)
                {
                    if (isColorCam)
                        zoomVisIn();
                    else
                        zoomIRIn();
                }
                else
                {
                    zoomVisStop();
                    zoomIRStop();
                }

            }
            if (bt5 != newbt5)
            {
                bt5 = newbt5;

                if (bt5)
                {
                    if (isColorCam)
                        zoomVisOut();
                    else
                        zoomIROut();
                }
                else
                {
                    zoomVisStop();
                    zoomIRStop();
                }
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
                    string[] strList = received_data.Split(',');

                    for (int i = 0; i < strList.Length - 4; i++)
                    {
                        if ((strList[i] == "$RATTM"))//radar
                        {

                            ThreadSafe(() => m_Gui.addARPA(strList));
                        }
                        //strList.(0);
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
                int irate = Convert.ToInt32(Math.Abs(rate) * 127);
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
                int irate = Convert.ToInt32(Math.Abs(rate) * 127);
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
                int irate = Convert.ToInt32(Math.Abs(rate) * 63);
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
                int irate = Convert.ToInt32(Math.Abs(rate) * 63);
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
        public void zoomVisStop()
        {
            byte[] cmd = new byte[8];
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x0E;
            cmd[3] = 0x77;
            cmd[4] = 0x00;
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
            cmd[4] = (byte)((focus >> 8) | ((byte)0x10));//focus go to min for visible
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
        public void zoomIRIn()//zoom in IR cam
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
        public void zoomIRStop()//zoom IR cam stop
        {
            //zoomVisIn();
            byte[] cmd = new byte[8];
            ushort newazi = getAzi();
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x20;
            cmd[4] = 0x00;
            cmd[5] = 0x00;
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
            else if (bt2)
            {
                double vPan = Convert.ToDouble(joystick_x / 32.0);// giá trị vPan từ -1 đến 1
                double vTilt = Convert.ToDouble(joystick_y / 32.0);// giá trị vTilt từ -1 đến 1
                pan(vPan / 5);
                tilt(vTilt / 5);
            }
            else if (bt7)
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
   
}
