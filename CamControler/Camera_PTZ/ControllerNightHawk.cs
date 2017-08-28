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
using System.Diagnostics;
//PA = B2 A5 E6 93
namespace Camera_PTZ
{
    public class ControllerNightHawk
    {
        //socket
        UdpClient listener;
        IPEndPoint groupEP;
        // các biến trạng thái cua joystick
        public UsbHidDevice pDevice;
        bool  bt1 = false, bt2 = false, bt3 = false,
              bt4 = false, bt5 = false, bt6 = false,
              bt7 = false, bt8 = false, bt9 = false,
             bt10 = false, bt11 = false, bt12 = false;
        int joystick_x, joystick_y;
        int focusIr = 95;// tu 0 den 100
        int focusVis = 955;// tu 900 den 1000
        int zoomFovVis=16;// gia tri tu 0 den 16
        int zoomFovIr = 8;// gia tri tu 0 den 8
        double joystick_sensitive;//joystick sensitive
        double realWeight = 1 / 6.0;
        int mArrow;
        // các biến trạng thái chung
        private volatile bool _shouldStop = false;
        public bool isVisCam = false;//biến chọn camera ảnh nhiệt/ảnh màu
        public bool fogFilter = false;
        private bool onTracking = false;// trang thai ba'm
        bool dialoghidden = false;
        public volatile bool tinhChinh = true;
        // trạng thái mục tiêu radar
        double bearing;// goc phuong vi
        double deltaH = 40;// chenh lech do cao
        double range;// cu ly mt
        double curCamAzi;
        double deltaAzi;
        double curCamEle=0;
        //bool onRadarTracking = false;
        System.Threading.Timer mUpdateTimer;
        System.Threading.Timer timer200ms;
        bool joystickOnline = false;
        TelnetConnection tc;
        GuiMain m_Gui;
        double x_sum = 0, y_sum = 0;
        private int errCount =-50;
        bool isSimulation = false;
        public ControllerNightHawk(TelnetConnection tconnect, GuiMain ptzControl)
        {
            //restartTracker();// restart chuong trinh cua anh Thi
            //ket noi den joystick
            pDevice = new UsbHidDevice(0x046D, 0xC215);
            //pDevice.OnConnected += DeviceOnConnected;
            //pDevice.OnDisConnected += DeviceOnDisConnected;
            pDevice.DataReceived += DeviceDataReceived;
            joystickOnline = pDevice.Connect();
            // ket noi den camera
            tc = tconnect;
            m_Gui = ptzControl;
            //mo socket
            mUpdateTimer = new System.Threading.Timer(TimerCallback, null, 100, 100);
            timer200ms = new System.Threading.Timer(Timer200, null, 500, 500);
            listener = new UdpClient(8001);
            groupEP = new IPEndPoint(IPAddress.Any, 0);
            initCommand();
        }
        public ControllerNightHawk(GuiMain ptzControl)
        {
            //restartTracker();// restart chuong trinh cua anh Thi
            //ket noi den joystick
            pDevice = new UsbHidDevice(0x046D, 0xC215);
            //pDevice.OnConnected += DeviceOnConnected;
            //pDevice.OnDisConnected += DeviceOnDisConnected;
            pDevice.DataReceived += DeviceDataReceived;
            joystickOnline = pDevice.Connect();
            // ket noi den camera
            isSimulation = true;
            m_Gui = ptzControl;
            //mo socket
            mUpdateTimer = new System.Threading.Timer(TimerCallback, null, 100, 100);
            timer200ms = new System.Threading.Timer(Timer200, null, 500, 500);
            listener = new UdpClient(8001);
            groupEP = new IPEndPoint(IPAddress.Any, 0);
            initCommand();
        }
        void restartTracker()
        {
            try
            {
                //foreach (var process in Process.GetProcessesByName("TrackCam"))
                //{
                //    process.Kill();
                //}
                //System.Threading.Thread.Sleep(300);
                Process process2 = new Process();
                process2.StartInfo.FileName = m_Gui.mConfig.trackerFileName;
                process2.StartInfo.WorkingDirectory = m_Gui.mConfig.WorkingDir;
                process2.Start();

                errCount = 0;
            }
            catch (Exception e)
            {
                return;
            }
           
        }
        void startRangeFinder()
        {
            //FF 00 0F 77 26 00
            byte[] cmd = new byte[8];
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x0F;
            cmd[3] = 0x77;
            cmd[4] = 0x26;
            cmd[5] = 0x01;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);//azi
        }
        private void Timer200(object state)
        {

            try// check connection state
            {
                errCount++;
                if (errCount > 50) { 
                    restartTracker();
                    errCount = 0;
                }
                queryLensValue();
                queryRangeFinder();
                getLensValue();
                //read status values from camera
                
                //else if (input[0] == 0xFF && input[2] == 0x0A && input[3] == 0x77)
                //{
                //    double curCamEle = ((input[4] << 8) | input[5]) * 0.0054931640625;/// 65536.0 * 360.0;
                //    ThreadSafe(() => m_Gui.ViewtData(joystick_x, joystick_y, (curCamEle), onTracking, true));
                //}
            }
            catch (Exception exc)
            {
                return;
            }
        }

      

        private void queryRangeFinder()
        {
            byte[] cmd = new byte[8];
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x4B;
            cmd[3] = 0x77;
            cmd[4] = 0x00;
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);//azi
        }



        private void getLensValue()
        {
            while (true)
            {
                List<byte> input = tc.readBinary();
                if (input.Count == 0) break;
                int i = 0;
                while (true)
                {
                    int pos = i * 7;
                    i++;
                    if (pos + 6 > input.Count) break;
                    if(input[pos] != 0xFF)break;
                    if ( input[pos + 2] == 0x09 && input[pos + 3] == 0x77)//curCamAzi
                    {
                        curCamAzi = ((input[pos + 4] << 8) | input[pos + 5]) * 0.0054931640625 - m_Gui.mConfig.elevationErr;/// 65536.0 * 360.0;
                        if (curCamAzi >= 360) curCamAzi -= 360.0;
                        if (curCamAzi < 0) curCamAzi += 360.0;
                        deltaAzi = bearing - curCamAzi;
                        if (deltaAzi > 180) deltaAzi -= 360;
                        if (deltaAzi < -180) deltaAzi += 360;
                        //ThreadSafe(() => m_Gui.ViewtData(joystick_x, joystick_y, (curCamAzi-config.constants[0]), onTracking, true));
                    }
                    else if ( input[pos + 2] == 0x0A && input[pos + 3] == 0x77)//curCamEle
                    {
                        curCamEle = ((input[pos + 4] << 8) | input[pos + 5]) * 0.0054931640625;/// 65536.0 * 360.0;
                        if (curCamEle > 180) curCamEle -= 360;                                                                       /// 

                    }
                    else if ( input[pos + 2] == 0x50 && input[pos + 3] == 0x77 )//Focus value
                    {
                        curCamFocus = (((input[pos + 4]&0x0F) << 8) | input[pos + 5]) ;/// 65536.0 * 360.0;
                        return;
                    }
                    else if (input[pos + 2] == 0x4B && input[pos + 3] == 0x77)
                    {
                        curRFvalue =  ((input[pos + 4]  << 8) | input[pos + 5]);/// 65536.0 * 360.0;;
                    }
                }
            }
        }

        private void queryLensValue()
        {
            
            byte[] cmd = new byte[8];
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x09;
            cmd[3] = 0x77;
            cmd[4] = 0x00;
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);//azi
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x0A;
            cmd[3] = 0x77;
            cmd[4] = 0x00;
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);//ele
            return;
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x50;
            cmd[3] = 0x77;
            cmd[4] = 0xC0;
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
            
        }
        public void StartRadarTargetTrack()
        {
            
            if (range == 0) return;
            stabOn();
            Thread.Sleep(500);
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            sendAziCtrl();
            

            Thread.Sleep(1000);
            
            //setStandartFocus();
            Thread.Sleep(300);
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
            Thread.Sleep(300);
            //set zoom  goto FF 00 07 77 ax xx---------------- 
            int fov = getZoomIR();//IR camera zoom
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x07;
            cmd[3] = 0x77;
            cmd[4] = (byte)((fov >> 8) | (0xB0));// //IR camera zoom
            cmd[5] = (byte)(fov);
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
            Thread.Sleep(300);
            //Vis camera zoom
            fov = getZoomVis();
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x07;
            cmd[3] = 0x77;
            cmd[4] = (byte)((fov >> 8) | (0x70));// //VIS camera zoom
            cmd[5] = (byte)(fov);
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
            //
            
        }

        private void setStandartFocus()
        {
             focusVis = 956;//set standart focus
             focusIr = 95;
             UpdateFocus();
        }
        private void TimerCallback(object state)
        {
            if (m_Gui.connectionActive||isSimulation) Update();
        }
        private void ThreadSafe(MethodInvoker method)
        {
            if(m_Gui!=null)
            {
                if (m_Gui.InvokeRequired)
                    m_Gui.Invoke(method);
                else
                    method();
            }
            
        }
        private void DeviceDataReceived(byte[] data)
        {
            
            int newjy = (data[2] >> 2) | ((data[3] & 0x0f) << 6);
            joystick_y = (newjy - 512) / 8;// giá trị từ -64 đến 64
            int newjx = (data[1] | ((data[2] & 0x03) << 8));
            joystick_x = (newjx - 512) / 8;// giá trị từ -64 đến 64
            joystick_sensitive = (255.0 - Convert.ToDouble(data[6]))/255.0;// giá trị từ 0 đến 255
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
            if (newbt1 && newbt2 && newbt11)
            {
                this.TurnOffCam();
                ThreadSafe(() => m_Gui.exitCam());
                return;
            }

            if (bt1 != newbt1)
            {
                bt1 = newbt1;
                if (bt2)
                {
                    startRangeFinder();
                }
                else if (bt1)
                {
                    if (!dialoghidden)
                    {
                        if (m_Gui.ListRadar.Count>0&&m_Gui.ListRadar.Count > m_Gui.selectedTargetIndex)
                        {
                            this.bearing = m_Gui.ListRadar[(m_Gui.selectedTargetIndex)].azi + m_Gui.mConfig.aziError;
                            if (bearing >= 360) bearing -= 360;
                            if (bearing < 0) bearing += 360;
                            this.range = m_Gui.ListRadar[(m_Gui.selectedTargetIndex)].range * 1.852;
                            this.StartRadarTargetTrack();
                        }
                        //ThreadSafe(() => m_Gui.ViewtData(x_track, y_track, (curCamAzi), onTracking, true));
                        
                    }
                    else if (!onTracking)
                    {
                        byte[] dgram;// bắt đầu track
                        dgram = new byte[2];
                        dgram[0] = 0xff;
                        dgram[1] = 0x01;
                        listener.Send(dgram, 2, "127.0.0.1", 8000);
                        
                        //while (listener.Receive(ref groupEP).Length > 0) { }
                    }
                    else
                    {
                        byte[] dgram;// ngung track
                        dgram = new byte[2];
                        dgram[0] = 0xff;
                        dgram[1] = 0x00;
                        listener.Send(dgram, 2, "127.0.0.1", 8000);
                        Stop();
                        resettracking();
                        onTracking = false;
                    }
                }


            }

            if (bt4 != newbt4)
            {
                bt4 = newbt4;
                if (bt11 && bt4)
                {
                    if (isVisCam)
                    {
                        autoFocusVis();
                    }
                    else {
                        autoFocusIr();
                    }
                }
                else  if (bt4)
                {
                    // chuyen camera
                    if (isVisCam) CamsSelect(false);
                    else CamsSelect(true);
                }
            }
            if (bt9 != newbt9)
            {
                bt9 = newbt9;
                if (bt11 && bt9)
                {
                    toggleTubolenceMitig();
                }
                else if (bt9)
                {
                    // thu nhỏ cửa sổ bám
                    byte[] dgram;
                    dgram = new byte[2];
                    dgram[0] = 0xff;
                    dgram[1] = 0x03;
                    listener.Send(dgram, 2, "127.0.0.1", 8000);
                    
                }
            }
            if (bt10 != newbt10)
            {
                bt10 = newbt10;
                if(bt11&&bt10)
                {
                    // bat/tat loc suong mu
                    toggleFogFilter();
                }
                else if (bt10)
                {
                    //Phong to cua so bam
                    byte[] dgram;
                    dgram = new byte[2];
                    dgram[0] = 0xff;
                    dgram[1] = 0x02;
                    listener.Send(dgram, 2, "127.0.0.1", 8000);
                }
                
                
            }
            if (bt11 != newbt11)
            {
                bt11 = newbt11;
            //    if (bt11) stabOn();
            //    else 
            //    stabOff();
            }
            if (bt7 != newbt7)
            {
                bt7 = newbt7;
                
            }
            if (bt8 != newbt8)
            {
                bt8 = newbt8;
                if (bt11 && bt8)
                {
                    toggleMul2x();
                }
                
            }
            if (bt6 != newbt6)
            {
                bt6 = newbt6;

                if (bt6)
                {
                    
                    if (isRecording)
                    {
                        //tat ghi luu
                        isRecording = false;
                        byte[] dgram;
                        dgram = new byte[2];
                        dgram[0] = 0xfa;
                        dgram[1] = 0x00;
                        listener.Send(dgram, 2, "127.0.0.1", 8000);
                    }
                    else {
                        //bat ghi luu
                        isRecording = true;
                        byte[] dgram;
                        dgram = new byte[2];
                        dgram[0] = 0xfa;
                        dgram[1] = 0x01;
                        listener.Send(dgram, 2, "127.0.0.1", 8000);
                    }
                }
            }
            if (bt12 != newbt12)
            {
                bt12 = newbt12;
                if (bt12&bt11)// 
                {
                    toggleStab();
                }
                else if (bt12)// ẩn/hiện giao diện hiển thị
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
                            break;
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
                            break;
                        }
                        tiltUp();
                        break;
                    default:
                        Stop();
                        break;
                }

            }
            if (bt2 != newbt2)
            {
                bt2 = newbt2;
                if (!bt2) Stop();
            }
            
            
            //bt
            if (bt3 != newbt3)
            {
                bt3 = newbt3;

                if (bt3)
                {
                    ZoomActiveOut();
                    
                }
            }
            if (bt5 != newbt5)
            {
                bt5 = newbt5;

                if (bt5)
                {
                    ZoomActiveIn();
                    
                }
                
            }

            //double.TryParse(coor[1], out azi);//degrees
            //azi = azi / 3.141592654 * 180;
            //double.TryParse(coor[2], out y);//m
            //double.TryParse(coor[3], out range);//km
            //if(y>0)config.constants[0] = y;
            //ialpha = (unsigned short)(0xffff*alpha/(2*3.141592654));

        }

        private void toggleStab()
        {
            if (stabilizOn)
            {
                
                stabOff();
            }
            else 
            {
                
                stabOn();
            }
        }

        private void toggleMul2x()
        {
            if (teleMul)
            {
                byte[] cmd = new byte[8]; 
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x32;
                cmd[3] = 0x77;
                cmd[4] = 0x02;
                cmd[5] = 0x00;
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
                teleMul = false;
            }
            else {
                byte[] cmd = new byte[8];
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x32;
                cmd[3] = 0x77;
                cmd[4] = 0x02;
                cmd[5] = 0x01;
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);

                teleMul = true;

            }
        }
        
        private void toggleTubolenceMitig()
        {
            if (turboLence)
            {
                byte[] cmd = new byte[8];
                //PA: B2 A5 E6 93
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x24;
                cmd[3] = 0x77;
                cmd[4] = 0x13;
                cmd[5] = 0x00;
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
                turboLence = false;

            }
            else {
                byte[] cmd = new byte[8];
                //PA: B2 A5 E6 93
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x24;
                cmd[3] = 0x77;
                cmd[4] = 0x13;
                cmd[5] = 0x10;
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
                turboLence = true;
            }
            
        }

        private void toggleFogFilter()
        {
            if (fogFilter)
            {
                byte[] cmd = new byte[8];
                //PA: B2 A5 E6 93
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x11;
                cmd[3] = 0x77;
                cmd[4] = 0x32;
                cmd[5] = 0x00;
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
                fogFilter = false;
            }
            else
            {
                byte[] cmd = new byte[8];
                //PA: B2 A5 E6 93
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x11;
                cmd[3] = 0x77;
                cmd[4] = 0x32;
                cmd[5] = 0x02;
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
                fogFilter = true;
            }
        }
        double sumWeight = 1 / 400.0;
        
        private void resettracking()
        {
            x_sum = 0;
            y_sum = 0;
            x_target = 0;
            y_target = 0;
            sumWeight = 1 / 400.0;
            realWeight = 1 / 6.0;
        }

        
        private void initCommand()
        {
            //restartTracker();
            UpdateZoom();
            UpdateFocus();
            CamsSelect(true);// chon camera Vis
            //stabOff();
            stabOn();
            resettracking();
            ThreadSafe(() => m_Gui.HideToTray());
            dialoghidden = true;
            
            //setMaxGoToRate();
        }
        private void stabOn()
        {
            

            stabilizOn = true;
            if (isSimulation) return;
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
            stabilizOn = false;
            if (isSimulation) return;
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
        void setMaxGoToRate()
        { 
            //byte[] cmd = new byte[8];
            //cmd[0] = 0xB2;
            //cmd[1] = 0xA5;
            //cmd[2] = 0xE6;
            //cmd[3] = 0x93;
            //cmd[4] = 0x42;
            //cmd[5] = 0x00;
            //cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            //tc.Write(cmd);
        
        }
        double x_target=0, y_target=0;
        private bool turboLence = false;
        private bool teleMul = false;
        private bool stabilizOn = false;
        private bool isRecording = false;
   
        private double curCamFocus;
        private double simBearing = 0;
        private double simElevation = 0;
        private double simBearingRate = 0;
        private double simElevationRate = 0;
        private double simFOV = 30;
        private double curRFvalue;
        
        
        public void ListenToCommand()
        {
            _shouldStop = false;
            Process process2 = new Process();
            process2.StartInfo.FileName = m_Gui.mConfig.trackerFileName;
            process2.StartInfo.WorkingDirectory = m_Gui.mConfig.WorkingDir;
            if(File.Exists(process2.StartInfo.FileName))
            process2.Start();
            else
                MessageBox.Show("Không tìm thấy phần mềm TrackCam");
            if (isSimulation)
            { 
            }
            else
            { //open camera
                byte[] cmd = new byte[8];
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x40;
                cmd[3] = 0x77;
                cmd[4] = 0x07;
                cmd[5] = 0x00;
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
            }
            
            while (!_shouldStop)
            {
                try
                {

                    byte[] receive_byte_array = listener.Receive(ref groupEP);

                    errCount = 0;

                    if (receive_byte_array[0] == 0xff && receive_byte_array.Length == 5)//nhan du lieu bam tu anh Thi
                    {

                        int temp;
                        //lay gia tri x_track
                        if (receive_byte_array[1] == 0
                            && receive_byte_array[2] == 0
                            && receive_byte_array[3] == 0
                            && receive_byte_array[4] == 0)
                        {
                            if (onTracking)
                            {
                                onTracking = false;
                                Stop();
                                resettracking();
                            }
                        }
                        else
                        {
                            onTracking = true;
                            if (receive_byte_array[1] >> 7 > 0)
                            {

                                temp = (receive_byte_array[1] << 8) | (receive_byte_array[2]) - 1 - 0xffff;

                            }
                            else
                            {
                                temp = (receive_byte_array[1] << 8) | (receive_byte_array[2]);
                            }

                            // tich luy 
                            x_target = (Convert.ToDouble(temp) / 50.0);
                            x_sum += x_target;
                            //lay gia tri y_track
                            if (receive_byte_array[3] >> 7 > 0)
                            {

                                temp = (receive_byte_array[3] << 8) | (receive_byte_array[4]) - 1 - 0xffff;

                            }
                            else
                            {
                                temp = (receive_byte_array[3] << 8) | (receive_byte_array[4]);
                            }
                            
                            y_target = (Convert.ToDouble(temp) / 50.0);
                            y_sum += y_target;
                             

                        }
                    }
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
                        double.TryParse(coor[2], out deltaH);//m
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
                        sendAziCtrl();

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
                                focusVisDec();
                                break;
                            case "VFR":
                                //focusVFar();
                                break;
                            case "INR":
                                focusINear();
                                break;
                            case "IFR":
                                focusIFar();
                                break;
                            case "AF":
                                //autoFocusIr();
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


                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    continue;
                }
                
            }
            timer200ms.Change(Timeout.Infinite, Timeout.Infinite);
            mUpdateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            
        }
        public void Update()
        {
            
            //if (!joystickOnline) joystickOnline = pDevice.Connect();
            if (isSimulation)
            {
                simBearing += simBearingRate;
                simElevation += simElevationRate;
                if (simBearing < 0) simBearing += 360;
                if (simBearing > 360) simBearing -= 360;
                if (simElevation < -50) simElevation -= simElevationRate;
                if (simElevation > 60) simElevation -= simElevationRate;
                sendUDPControl(simBearing, simElevation, simFOV);
            }
            if (onTracking)
            {

                if (Math.Abs(x_sum) > 10)
                {
                    x_sum /= 2;
                    realWeight *= 1.3;
                }
                if (Math.Abs(y_sum) > 10)
                {
                    y_sum /=2;
                    realWeight *= 1.3;
                }
                //Debug.WriteLine("w="+realWeight.ToString());
                double panControl = x_target*m_Gui.mConfig.trackSensitive;
                double tiltControl =  - y_target * m_Gui.mConfig.trackSensitive;
                pan(panControl);
                tilt(tiltControl);//!!!
                //ThreadSafe(() => m_Gui.ViewtData(joystick_x, joystick_y, x_track, onTracking, true));
                
            }
            if (bt2)
            {
                double vPan = Convert.ToDouble(joystick_x / 64.0);// giá trị vPan từ -1 đến 1
                double vTilt = Convert.ToDouble(joystick_y / 64.0);// giá trị vTilt từ -1 đến 1
                pan(vPan * joystick_sensitive);
                tilt(vTilt * joystick_sensitive);
            }
            if (bt11)
            {
            }
            else 
            {
                if (bt7) focusVisDec();
                if (bt8) focusVisInc();

            }

            String str = "";
            if (stabilizOn) str += "Tự ổn định|";
            if (onTracking) str += "Bám MT|";
            if (teleMul) str += "2x Zoom|";
            if (fogFilter) str += "Lọc mù|";
            if (turboLence) str += "Tăng nhạy nhiệt|";
            

            if (isSimulation)
            {
                curCamEle = simElevation;
                curCamAzi = simBearing;
            }

            str = "Phương vị: " + curCamAzi.ToString("0.##") + " |" + "Góc tà: " + curCamEle.ToString("0.##") + " \r\n|" + "Khoảng cách: " + curRFvalue.ToString() + " |" + "Tiêu cự: " + curCamFocus.ToString() + " |" + str;
            ThreadSafe(() => m_Gui.ViewtData(str));
            /*
            if (isSimulation)
            {
                ThreadSafe(() => m_Gui.ViewtData(stabilizOn, onTracking, teleMul, fogFilter, turboLence, simBearing, simElevation));
            }
            else ThreadSafe(() => m_Gui.ViewtData(stabilizOn, onTracking, teleMul, fogFilter, turboLence, bearing, curCamEle, curRFvalue));
            */
            //if (bt7)
            //{
            //    if (m_Gui.selectedTargetIndex < m_Gui.ListRadar.Count)
            //    {
            //        this.bearing = m_Gui.ListRadar[(m_Gui.selectedTargetIndex)].azi + config.constants[9];
            //        this.range = m_Gui.ListRadar[(m_Gui.selectedTargetIndex)].range * 1.852;
            //        double speed = deltaAzi / 70;
            //        if (speed > 0.5) speed = 0.5;
            //        if (speed < -0.5) speed = -0.5;
            //        pan(speed);
            //    }
            //}
            //stabOff();

        }

        private void UpdateFocus()
        {
            if (isSimulation)
            {
                return;
            }
            else if (isVisCam)
            {
                byte a1, a2, a3;
                a1 = (byte)(focusVis / 100);// chu so hang tram
                a2 = (byte)((focusVis % 100) / 10);// chu so hang chuc
                a3 = (byte)((focusVis % 10));
                byte[] cmd = new byte[8];
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x08;
                cmd[3] = 0x77;
                cmd[4] = (byte)((a1) | ((byte)0x10));//Vis focus go to 
                cmd[5] = (byte)((a2 << 4) + a3);
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
                //byte[] dgram;
                //dgram = new byte[2];
                //dgram[0] = 0xfd;
                //dgram[1] = (byte)(focusVis - 900);
                //listener.Send(dgram, 2, "127.0.0.1", 8000);//send focus
            }
            else
            {
                byte a1, a2, a3;
                a1 = (byte)(focusIr / 100);// chu so hang tram
                a2 = (byte)((focusIr % 100) / 10);// chu so hang chuc
                a3 = (byte)((focusIr % 10));
                byte[] cmd = new byte[8];
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x08;
                cmd[3] = 0x77;
                cmd[4] = (byte)((a1) | ((byte)0x20));//Ir focus go to 
                cmd[5] = (byte)((a2 << 4) + a3);
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
                //byte[] dgram;
                //dgram = new byte[2];
                //dgram[0] = 0xfd;
                //dgram[1] = (byte)(focusIr);
                //listener.Send(dgram, 2, "127.0.0.1", 8000);//send focus
            }
            
        }

        
        public void Stop()
        {
            if (isSimulation)
            {
                simBearingRate = 0;
                simElevationRate = 0;
                return;
            }
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
        public void StopFocusIR()
        {
            if (isSimulation)
            {
                return;
            }
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
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x08;
            cmd[4] = 0x00;
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
            
        }
        public void focusINear()
        {
            if (isSimulation)
            {
                return;
            }
            byte[] cmd = new byte[8];
            uint rate = 0;// (uint)m_Gui.mConfig.constants[11];
            
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
        public void focusVisDec()
        {
            if (isSimulation)
            {
                return;
            }
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
        public void focusVisInc()
        {
            if (isSimulation)
            {
                return;
            }
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
        public void panRight()
        {
            if (isSimulation)
            {
                return;
            }
            byte[] cmd = new byte[8];
            uint rate = (uint)(joystick_sensitive * 30.0);
            if (rate > 63) rate = 63;
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
            if (isSimulation)
            {
                simBearingRate = rate*simFOV/30;
                return;
            }
            if (Math.Abs(rate) > 1) return;
            if (rate >= 0)
            {
                byte[] cmd = new byte[8];
                //set rate
                int irate = Convert.ToInt32(Math.Abs(rate) * 63);
                
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
                int irate = Convert.ToInt32(Math.Abs(rate) * 63);
                if (irate == 0) irate++;
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
            if (isSimulation)
            {
                return;
            }
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint rate = (uint)(joystick_sensitive * 30.0);
            if (rate > 63) rate = 63;
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
            if (isSimulation)
            {
                return;
            }
            byte[] cmd = new byte[8];
            uint rate = (uint)m_Gui.mConfig.focusrate;
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
            if (isSimulation)
            {
                return;
            }
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
        public void focusVStop()
        {
            if (isSimulation)
            {
                return;
            }
            byte[] cmd = new byte[8];
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x32;
            cmd[3] = 0x77;
            cmd[4] = 0x06;
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);

        }
        public void tilt(double rate)//
        {
            if (Math.Abs(rate) > 1) return;
            if (isSimulation)
            {
                simElevationRate = rate*simFOV/30;
                return;
            }
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
            if (isSimulation)
            {
                return;
            }
            byte[] cmd = new byte[8];
            uint rate = (uint)(joystick_sensitive * 30.0);
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
            if (isSimulation)
            {
                return;
            }
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint rate = (uint)(joystick_sensitive * 30.0);
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
            if (isSimulation)
            {
                return;
            }
            //zoomVisOut();
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint rate = 15;//(uint)m_Gui.mConfig.constants[4];
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
        public void autoFocusVis()
        {

            if (isSimulation)
            {
                return;
            }
            byte[] cmd = new byte[8];
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x32;
            cmd[3] = 0x77;
            cmd[4] = 0x04;
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        public void autoFocusIr()
        {
            if (isSimulation)
            {
                return;
            }
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
            if (isSimulation)
            {
                return;
            }
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
            if (isSimulation)
            {
                return;
            }
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint rate = (uint)m_Gui.mConfig.zoomrate;
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
            
        }
        public void zoomVisOut()
        {
            if (isSimulation)
            {
                return;
            }
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint a = (uint)m_Gui.mConfig.zoomrate;
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
            

        }
        void ZoomActiveIn()
        {
            
            if (isVisCam)
            {
                if (zoomFovVis > 0) zoomFovVis--;
            }
            else{
                if (zoomFovIr > 0) zoomFovIr--;
            }
            
            UpdateZoom();
        }
        void ZoomActiveOut()
        {
            
            if (isVisCam)
            {
                if (zoomFovVis < 16) zoomFovVis++;
            }
            else
            {
                if (zoomFovIr <8) zoomFovIr++;
            }
            
            UpdateZoom();
        }
        void sendUDPControl(double pan, double tilt, double zoom)
        {

            string str = pan.ToString() + ";" + tilt.ToString() + ";" + zoom.ToString() + ";";
            byte[] data = Encoding.ASCII.GetBytes(str);
            var client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888); // endpoint where server is listening (testing localy)
            client.Connect(ep);
            // send data
            client.Send(data, data.Length);

        }
        void UpdateZoom()
        {
            
            int FOV;
            if(isVisCam)
            {
                switch (zoomFovVis)
                {
                    case 0:
                        focusVis = 957;
                        FOV = 3;
                        break;
                    case 1:
                        focusVis = 956;
                        FOV = 5;
                        break;
                    case 2:
                        focusVis = 956;
                        FOV = 7; break;
                    case 3:
                        focusVis = 955;
                        FOV = 9; break;
                    case 4:
                        focusVis = 955;
                        FOV = 12; break;
                    case 5:
                        focusVis = 955;
                        FOV = 18; break;
                    case 6:
                        focusVis = 955;
                        FOV = 24; break;
                    case 7:
                        focusVis = 954;
                        FOV = 32; break;
                    case 8:
                        focusVis = 954;
                        FOV = 48; break;
                    case 9:
                        focusVis = 953;
                        FOV = 60; break;
                    case 10:
                        focusVis = 953;
                        FOV = 90; break;
                    case 11:
                        focusVis = 952;
                        FOV = 120; break;
                    case 12:
                        focusVis = 952;
                        FOV = 150; break;
                    case 13:
                        focusVis = 951;
                        FOV = 200; break;
                    case 14:
                        focusVis = 951;
                        FOV = 300; break;
                    case 15:
                        focusVis = 951;
                        FOV = 400; break;
                    case 16:
                        focusVis = 950;
                        FOV = 500; break;
                    default:
                        return;
                }
                if (isSimulation)
                {
                    simFOV = FOV/10.0;
                }
                else
                {
                    byte[] cmd = new byte[8];
                    cmd[0] = 0xFF;
                    cmd[1] = 0x00;
                    cmd[2] = 0x07;
                    cmd[3] = 0x77;
                    cmd[4] = (byte)((FOV >> 8) | (0x70));// Vis camera zoom
                    cmd[5] = (byte)(FOV);
                    cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                    tc.Write(cmd);
                }
                
                byte[] dgram;
                dgram = new byte[3];
                dgram[0] = 0xfe;
                dgram[1] = (byte)((50000/FOV)>>8);
                dgram[2] = (byte)(50000/FOV);
                listener.Send(dgram, 3, "127.0.0.1", 8000);//send zoom vis
            }
            else {
                switch (zoomFovIr)
                {
                    case 0:
                        focusIr = 93;
                        FOV = 5;
                        break;
                    case 1:
                        focusIr = 95;
                        FOV = 7;
                        break;
                    case 2:
                        focusIr = 96;
                        FOV = 9; break;
                    case 3:
                        focusIr = 97;
                        FOV = 13; break;
                    case 4:
                        focusIr = 98;
                        FOV = 15; break;
                    case 5:
                        focusIr = 98;
                        FOV = 24; break;
                    case 6:
                        focusIr = 99;
                        FOV = 30; break;
                    case 7:
                        focusIr = 99;
                        FOV = 45; break;
                    case 8:
                        focusIr = 100;
                        FOV = 60; break;
                    default:
                        return;
                }
                if (isSimulation)
                {
                    simFOV = FOV/10.0;
                }
                else
                {
                    byte[] cmd = new byte[8];
                    cmd[0] = 0xFF;
                    cmd[1] = 0x00;
                    cmd[2] = 0x07;
                    cmd[3] = 0x77;
                    cmd[4] = (byte)((FOV >> 8) | (0xB0));// //Ir camera zoom
                    cmd[5] = (byte)(FOV);
                    cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                    tc.Write(cmd);
                }
                
                byte[] dgram;
                dgram = new byte[3];
                dgram[0] = 0xfe;
                dgram[1] = (byte)((6000 / FOV)>>8);
                dgram[2] = (byte)(6000 / FOV);
                listener.Send(dgram, 3, "127.0.0.1", 8000);//send zoom vis
            }

            UpdateFocus();
        }
        public void zoomIRIn()//zoom in IR cam
        {
            if (isSimulation)
            {
                return;
            }
            //zoomVisIn();
            byte[] cmd = new byte[8];
            //set azi ----------------------- 
            uint rate = 15;
            if (rate > 63) rate = 63;
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x20;
            cmd[4] = 0x00;
            cmd[5] = (byte)(rate);
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        void zoomStep(int a)// gia tri 1 hoac 2
        {
            if (isSimulation)
            {
                return;
            }
            byte[] cmd = new byte[8];
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x32;
            cmd[3] = 0x77;
            cmd[4] = 0x05;
            cmd[5] = (byte)(a);
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
            
        }
        public void zoomIRStop()//zoom IR cam stop
        {
            if (isSimulation)
            {
                return;
            }
            //zoomVisIn();
            byte[] cmd = new byte[8];
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x00;
            cmd[3] = 0x00;
            cmd[4] = 0x00;// may be 20 !!!!!!!!!1
            cmd[5] = 0x00;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }
        private void sendAziCtrl()
        {
            if (isSimulation)
            {
                
                return;
            }

            byte[] cmd = new byte[8];
            ushort newazi =  (ushort)(0xffff * ((bearing) / (360.0)));
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x05;
            cmd[3] = 0x77;
            cmd[4] = (byte)(newazi >> 8);
            cmd[5] = (byte)(newazi);
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
        }



        private int getZoomIR()
        {
            int FOV = (int)(m_Gui.mConfig.targetSize / range);//60/(range/3)
            if (FOV < 5) FOV = 5;
            if (FOV > 60) FOV = 60;
            if (FOV >= 60) zoomFovIr = 8;
            else if (FOV >= 45) zoomFovIr = 7;
            else if (FOV >= 30) zoomFovIr = 6;
            else if (FOV >= 24) zoomFovIr = 5;
            else if (FOV >= 15) zoomFovIr = 4;
            else if (FOV >= 13) zoomFovIr = 3;
            else if (FOV >= 9) zoomFovIr = 2;
            else if (FOV >= 7) zoomFovIr = 1;
            else if (FOV >= 5) zoomFovIr = 0;
            return FOV;
        }
        private int getZoomVis()
        {
            int FOV = (int)(m_Gui.mConfig.targetSize / range);//60/(range/3)
            if (FOV < 3) FOV = 3;
            if (FOV > 600) FOV = 600;
            if (FOV >= 600) zoomFovIr = 16;
            else if (FOV >= 450) zoomFovVis = 15;
            else if (FOV >= 300) zoomFovVis = 14;
            else if (FOV >= 200) zoomFovVis = 13;
            else if (FOV >= 150) zoomFovVis = 12;
            else if (FOV >= 120) zoomFovVis = 11;
            else if (FOV >= 90) zoomFovVis = 10;
            else if (FOV >= 60) zoomFovVis = 9;
            else if (FOV >= 48) zoomFovVis = 8;
            else if (FOV >= 32) zoomFovVis = 7;
            else if (FOV >= 24) zoomFovVis = 6;
            else if (FOV >= 18) zoomFovVis = 5;
            else if (FOV >= 12) zoomFovVis = 4;
            else if (FOV >= 9) zoomFovVis = 3;
            else if (FOV >= 7) zoomFovVis = 2;
            else if (FOV >= 5) zoomFovVis = 1;
            else if (FOV >= 3) zoomFovVis = 0;
            return FOV;
        }
        private ushort getEL()
        {
            double EL = -Math.Atan(deltaH / 1000 / range);
            EL += m_Gui.mConfig.elevationErr / 360.0 * 6.283185307;
            //double ELcalib = Math.Cos(Math.Abs(bearing - config.constants[1] / 57.2957795)) * config.constants[2] / 57.2957795;
            //EL += ELcalib;// in radian
            //EL += config.constants[0] / 57.2957795;
            if (EL < 0) EL += 6.283185307;
            return (ushort)(0xffff * (EL / (6.283185307)));
        }
        public void RequestStop()
        {
            _shouldStop = true;
            mUpdateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            timer200ms.Change(Timeout.Infinite,Timeout.Infinite);
        }
        // Volatile is used as hint to the compiler that this data 
        // member will be accessed by multiple threads. 



        internal void CamsSelect(bool p)
        {
            isVisCam = p;
            if (isSimulation)
            {
                UpdateZoom();
            }
            else {
                byte[] cmd = new byte[8];
                cmd[0] = 0xFF;
                cmd[1] = 0x00;
                cmd[2] = 0x0F;
                cmd[3] = 0x77;
                cmd[4] = 0x24;
                if (p)
                {
                    cmd[4] = 0x00;

                }
                else
                {
                    cmd[4] = 0x01;

                }
                cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
                tc.Write(cmd);
            }
            
            //chuyen kenh hinh anh cua anh Thi
            if (p)
            {
                
                byte[] dgram;
                dgram = new byte[2];
                dgram[0] = 0xff;
                dgram[1] = 0x04;
                listener.Send(dgram, 2, "127.0.0.1", 8000);
            }
            else
            {
                
                byte[] dgram;
                dgram = new byte[2];
                dgram[0] = 0xff;
                dgram[1] = 0x05;
                listener.Send(dgram, 2, "127.0.0.1", 8000);
            }
        }

        internal void TurnOffCam()
        {

            tilt(-1.0);
            byte[] cmd = new byte[8];
            //cover camera
            cmd[0] = 0xFF;
            cmd[1] = 0x00;
            cmd[2] = 0x40;
            cmd[3] = 0x77;
            cmd[4] = 0x07;
            cmd[5] = 0x01;
            cmd[6] = (byte)(cmd[1] + cmd[2] + cmd[3] + cmd[4] + cmd[5]);
            tc.Write(cmd);
            System.Threading.Thread.Sleep(500);
            return;
        }
    }
   
}
