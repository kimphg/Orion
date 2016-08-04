using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using PositioningControlNs;
using ScriptControlNs;
using LensControlNs;

namespace Camera_PTZ
{
    public class PTZFacade
    {
        public enum MoveDirection { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft }
        public enum ZoomDirection { Stop = 0, Out = 1, In = 2 }
        public enum AutoFocus { Off = 0, On = 1 }
        public enum AutoIris { Off = 0, On = 1 }
        public enum FocusDirection { Stop = 0, Far = 1, Near = 2 }
        public enum IrisDirection { Stop = 0, Close = 1, Open = 2 }

        private PositioningControl positioningCtrl = new PositioningControl();
        private ScriptControl scriptCtrl = new ScriptControl();
        private LensControl lensCtrl = new LensControl();
        private VelocityLimits velocityLimits;

        /// <summary>
        /// Sarix cameras do not include "100 CONTINUE" in SOAP responses so tell the Windows 
        /// ServicePointManager to NOT expect it by setting the Expect100Continue attribute to false.
        /// http://pdn.pelco.com/content/web-service-proxy-class-generation
        /// </summary>
        static PTZFacade()
        {
            System.Net.ServicePointManager.Expect100Continue = false;
        }

        /// <summary>
        /// Ctor for the PTZFacade class.
        /// PTZFacade wraps related Pelco API calls into meaningful actions.
        /// </summary>
        /// <param name="socket">IP and port of a camera to connect with. format: IP:PORT</param>
        /// <param name="username">Username to use when authenticating with the camera</param>
        /// <param name="password">Password to use when authenticating with the camera</param>
        public PTZFacade(string socket, string username = "", string password = "")
        {
            if (string.IsNullOrWhiteSpace(socket))
                throw new ArgumentException(string.Format("wrong socket", "PTZFacade"));
            
            // Modify the WSDL endpoint control URL to include the camera socket specified in the parameter
            string socketRegex = string.Format("{0}[:0-9]*", "localhost");
            positioningCtrl.Url = Regex.Replace(positioningCtrl.Url, socketRegex, socket);
            scriptCtrl.Url = Regex.Replace(scriptCtrl.Url, socketRegex, socket);
            lensCtrl.Url = Regex.Replace(lensCtrl.Url, socketRegex, socket);

            // Set credentials
            NetworkCredential credentials = new NetworkCredential(username, password);
            positioningCtrl.Credentials = credentials.GetCredential(new Uri(positioningCtrl.Url), "Basic");
            scriptCtrl.Credentials = credentials.GetCredential(new Uri(scriptCtrl.Url), "Basic");
            lensCtrl.Credentials = credentials.GetCredential(new Uri(lensCtrl.Url), "Basic");

            // Attempt to get the camera's velocity limits, rethrow on exception
            try
            {
                velocityLimits = positioningCtrl.GetVelocityLimits();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Patterns
        public void StartRecordPattern(uint patternNumber)
        {
            string patternName = PatternNameFromNumber(patternNumber);
            try
            {
                scriptCtrl.BeginScript(patternName);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void StopRecordPattern(uint patternNumber)
        {
            string patternName = PatternNameFromNumber(patternNumber);
            try
            {
                scriptCtrl.EndScript(patternName);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void ExecutePattern(uint patternNumber)
        { 
            string patternName = PatternNameFromNumber(patternNumber);
            try
            {
                scriptCtrl.ExecuteScript(patternName);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void StopPattern(uint patternNumber)
        {
            string patternName = PatternNameFromNumber(patternNumber);
            try
            {
                scriptCtrl.HaltScript(patternName, 0, false);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void RemovePattern(uint patternNumber)
        {
            string patternName = PatternNameFromNumber(patternNumber);
            try
            {
                scriptCtrl.DeleteScript(patternName);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
        #endregion

        #region Presets
        public void SetPreset(uint presetNumber)
        {
            string presetName = PresetNameFromNumber(presetNumber);
            try
            {
                scriptCtrl.EndScript(presetName);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void ExecutePreset(uint presetNumber)
        {
            string presetName = PresetNameFromNumber(presetNumber);
            try
            {
                scriptCtrl.ExecuteScript(presetName);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void RemovePreset(uint presetNumber)
        {
            string presetName = PresetNameFromNumber(presetNumber);
            try
            {
                scriptCtrl.DeleteScript(presetName);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
        #endregion

        #region Continuously Move
        public void SetXYZ(int x,int y, int z)
        {
            Velocity velocity ;
            velocity = new Velocity();
            Xyz xyz = new Xyz();
            xyz.x = x;
            xyz.xSpecified = true;
            xyz.y = y;
            xyz.ySpecified = true;
            xyz.z = z;
            xyz.zSpecified = true;
            velocity.rotation = xyz;
            positioningCtrl.SetPosition(velocity);
        }
        public void MoveXYZ(double vx, double vy, double vz = 0)
        {
            Velocity velocity = new Velocity();
            Xyz rotationalXyz = new Xyz();
            rotationalXyz.x = (int)(vx * 4500.0);
            rotationalXyz.xSpecified = true;
            rotationalXyz.y = (int)(vy * 4500.0);
            rotationalXyz.ySpecified = true;
            rotationalXyz.z = (int)(vz * 4500.0);
            rotationalXyz.zSpecified = true;
            velocity.rotation = rotationalXyz;
            try
            {
                positioningCtrl.SetVelocity(velocity);


            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
        public void Move(MoveDirection direction, int speedPercentage = 100)
        {
            speedPercentage = EnsureLimits(speedPercentage, 0, 100);
            Xyz rotationalXyz = XyzFromDirection(direction, speedPercentage);
            Velocity velocity = new Velocity();
            rotationalXyz.x /= 5;
            rotationalXyz.y /= 5;
            rotationalXyz.z /= 5;
            velocity.rotation = rotationalXyz;
            try
            {
                positioningCtrl.SetVelocity(velocity);
                
                
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void StopMoving()
        {
            Velocity velocity = new Velocity();
            velocity.rotation = new Xyz() { xSpecified = true, ySpecified = true };

            try
            {
                for (int i = 0; i < 2; i++)
                    positioningCtrl.SetVelocity(velocity);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
        #endregion

        #region Zoom
        public void Zoom(ZoomDirection zoomDirection)
        {
            try
            {
                lensCtrl.Zoom((uint)zoomDirection);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public float ZoomMagnification()
        {
            uint zoomMag = 0;
            try
            {
                zoomMag = lensCtrl.GetMag();
            }
            catch (Exception e)
            {
                LogException(e);
            }
            return zoomMag / 100f;
        }

        public void SetZoomMagnification(float magnification)
        {
            magnification = EnsureLimits(magnification, 1, 70);
            magnification *= 100f;
            try
            {
                lensCtrl.SetMag((uint)magnification);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
        #endregion

        #region Focus
        public void SetAutoFocus(AutoFocus autoFocus)
        {
            try
            {
                lensCtrl.AutoFocus((int)autoFocus);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void Focus(FocusDirection focusDirection)
        {
            try
            {
                lensCtrl.Focus((uint)focusDirection);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
        #endregion

        #region Iris
        public void SetAutoIris(AutoIris autoIris)
        {
            try
            {
                lensCtrl.AutoIris((int)autoIris);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void Iris(IrisDirection irisDirection)
        {
            try
            {
                lensCtrl.Iris((uint)irisDirection);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
        #endregion

        #region Display
        /// <summary>
        /// Print out settings from the PositioningControl and LensControl
        /// </summary>
        public void DisplaySettings()
        {
            string strValue = null;
            uint uintValue = 0;
            int intValue = 0;
            AxisLimits axisLimits = null;
            Velocity velocity = null;
            VelocityLimits velocityLimits = null;
            AccelerationLimits accelerationLimits = null;
            SpeedProfile speedProfile = null;
            SpeedProfile[] speedProfiles = null;

            Console.WriteLine();

            Console.WriteLine("GetPositionEventInterval:");
            try
            {
                strValue = positioningCtrl.GetPositionEventInterval();
                Console.WriteLine(string.Format("\t{0}", strValue));
            }
            catch (Exception e)
            {
                LogException(e);
            }
            Console.WriteLine();

            Console.WriteLine("GetPositionLimits:");
            try
            {
                axisLimits = positioningCtrl.GetPositionLimits();
                Console.WriteLine(string.Format("\tX: {0}-{1}\n\tY: {2}-{3}\n\tZ: {4}-{5}", axisLimits.rotation.xMin, axisLimits.rotation.xMax,
                    axisLimits.rotation.yMin, axisLimits.rotation.yMax, axisLimits.rotation.zMin, axisLimits.rotation.zMax));
            }
            catch (Exception e)
            {
                LogException(e);
            }
            Console.WriteLine();

            Console.WriteLine("GetVelocity:");
            try
            {
                velocity = positioningCtrl.GetVelocity();
                Console.WriteLine(string.Format("\tX: {0}\n\tY: {1}\n\tZ: {2}", velocity.rotation.x, velocity.rotation.y,
                    velocity.rotation.z));
            }
            catch (Exception e)
            {
                LogException(e);
            }
            Console.WriteLine();

            Console.WriteLine("GetVelocityLimits:");
            try
            {
                velocityLimits = positioningCtrl.GetVelocityLimits();
                Console.WriteLine(string.Format("\tX: {0}-{1}\n\tY: {2}-{3}\n\tZ: {4}-{5}", velocityLimits.rotation.xMin, velocityLimits.rotation.xMax,
                    velocityLimits.rotation.yMin, velocityLimits.rotation.yMax, velocityLimits.rotation.zMin, velocityLimits.rotation.zMax));
            }
            catch (Exception e)
            {
                LogException(e);
            }
            Console.WriteLine();

            Console.WriteLine("GetVelocityUrl:");
            try
            {
                strValue = positioningCtrl.GetVelocityURL();
                Console.WriteLine(string.Format("\tUrl: {0}", strValue));
            }
            catch (Exception e)
            {
                LogException(e);
            }
            Console.WriteLine();

            Console.WriteLine("GetAllSpeedProfiles:");
            try
            {
                speedProfiles = positioningCtrl.GetAllSpeedProfiles();
                Console.WriteLine(string.Format("\tCount: {0}", speedProfiles.Length));
                foreach(SpeedProfile innerSpeedProfile in speedProfiles)
                {
                    velocityLimits = innerSpeedProfile.VelocityLimits;
                    accelerationLimits = innerSpeedProfile.AccelerationLimits;
                    Console.WriteLine(string.Format("\tId: {0}", innerSpeedProfile.id));
                    Console.WriteLine(string.Format("\tName: {0}", innerSpeedProfile.Name));
                    Console.WriteLine(string.Format("\tAcceleration Limits - X: {0}-{1}\n\tY: {2}-{3}\n\tZ: {4}-{5}", accelerationLimits.rotation.xMin,
                        accelerationLimits.rotation.xMax, accelerationLimits.rotation.yMin, accelerationLimits.rotation.yMax, accelerationLimits.rotation.zMin,
                        accelerationLimits.rotation.zMax));
                    Console.WriteLine(string.Format("\tVelocity Limits - X: {0}-{1}\n\tY: {2}-{3}\n\tZ: {4}-{5}", velocityLimits.rotation.xMin, 
                        velocityLimits.rotation.xMax, velocityLimits.rotation.yMin, velocityLimits.rotation.yMax, velocityLimits.rotation.zMin, 
                        velocityLimits.rotation.zMax));
                    if (speedProfile == null)
                        speedProfile = innerSpeedProfile;
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
            Console.WriteLine();

            Console.WriteLine("GetAzimuthZero:");
            try
            {
                intValue = positioningCtrl.GetAzimuthZero();
                Console.WriteLine(string.Format("\tAzimuth Zero: {0}", intValue));
            }
            catch (Exception e)
            {
                LogException(e);
            }
            Console.WriteLine();

            Console.WriteLine("GetEnabledSpeedProfile:");
            try
            {
                speedProfile = positioningCtrl.GetEnabledSpeedProfile();
                velocityLimits = speedProfile.VelocityLimits;
                accelerationLimits = speedProfile.AccelerationLimits;
                Console.WriteLine(string.Format("\tId: {0}", speedProfile.id));
                Console.WriteLine(string.Format("\tName: {0}", speedProfile.Name));
                Console.WriteLine(string.Format("\tAcceleration Limits - X: {0}-{1}\n\tY: {2}-{3}\n\tZ: {4}-{5}", accelerationLimits.rotation.xMin,
                    accelerationLimits.rotation.xMax, accelerationLimits.rotation.yMin, accelerationLimits.rotation.yMax, accelerationLimits.rotation.zMin,
                    accelerationLimits.rotation.zMax));
                Console.WriteLine(string.Format("\tVelocity Limits - X: {0}-{1}\n\tY: {2}-{3}\n\tZ: {4}-{5}", velocityLimits.rotation.xMin,
                    velocityLimits.rotation.xMax, velocityLimits.rotation.yMin, velocityLimits.rotation.yMax, velocityLimits.rotation.zMin,
                    velocityLimits.rotation.zMax));
            }
            catch (Exception e)
            {
                LogException(e);
            }
            Console.WriteLine();

            Console.WriteLine("GetSpeedProfile:");
            if (speedProfile != null)
            {
                try
                {
                    speedProfile = positioningCtrl.GetSpeedProfile(speedProfile.id);
                    velocityLimits = speedProfile.VelocityLimits;
                    accelerationLimits = speedProfile.AccelerationLimits;
                    Console.WriteLine(string.Format("\tId: {0}", speedProfile.id));
                    Console.WriteLine(string.Format("\tName: {0}", speedProfile.Name));
                    Console.WriteLine(string.Format("\tAcceleration Limits - X: {0}-{1}\n\tY: {2}-{3}\n\tZ: {4}-{5}", accelerationLimits.rotation.xMin,
                        accelerationLimits.rotation.xMax, accelerationLimits.rotation.yMin, accelerationLimits.rotation.yMax, accelerationLimits.rotation.zMin,
                        accelerationLimits.rotation.zMax));
                    Console.WriteLine(string.Format("\tVelocity Limits - X: {0}-{1}\n\tY: {2}-{3}\n\tZ: {4}-{5}", velocityLimits.rotation.xMin,
                        velocityLimits.rotation.xMax, velocityLimits.rotation.yMin, velocityLimits.rotation.yMax, velocityLimits.rotation.zMin,
                        velocityLimits.rotation.zMax));
                }
                catch (Exception e)
                {
                    LogException(e);
                }
            }
            Console.WriteLine();

            Console.WriteLine("GetMag:");
            try
            {
                uintValue = lensCtrl.GetMag();
                Console.WriteLine(string.Format("\tZoom Magnification: {0:0.##}X", uintValue / 100f));
            }
            catch (Exception e)
            {
                LogException(e);
            }                
            Console.WriteLine();

            Console.WriteLine("GetMaxAOV:");
            try
            {
                uintValue = lensCtrl.GetMaxAOV();
                Console.WriteLine(string.Format("\tMax Angle Of View: {0}", uintValue));
            }
            catch (Exception e)
            {
                LogException(e);
            }
            Console.WriteLine();

            Console.WriteLine("GetMaxDigitalMag:");
            try
            {
                uintValue = lensCtrl.GetMaxDigitalMag();
                Console.WriteLine(string.Format("\tMax Digital Magnification: {0:0.##}X", uintValue / 100f));
            }
            catch (Exception e)
            {
                LogException(e);
            }
            Console.WriteLine();

            Console.WriteLine("GetMaxMag:");
            try
            {
                uintValue = lensCtrl.GetMaxMag();
                Console.WriteLine(string.Format("\tMax Magnification: {0:0.##}X", uintValue / 100f));
            }
            catch (Exception e)
            {
                LogException(e);
            }
            Console.WriteLine();

            Console.WriteLine("GetMaxOpticalMag:");
            try
            {
                uintValue = lensCtrl.GetMaxOpticalMag();
                Console.WriteLine(string.Format("\tMax Optical Magnification: {0:0.##}X", uintValue / 100f));
            }
            catch (Exception e)
            {
                LogException(e);
            }
            Console.WriteLine();
        }
        #endregion

        #region Private
        /// <summary>
        /// Write the exception message after a tab
        /// </summary>
        /// <param name="e"></param>
        private void LogException(Exception e)
        {
            Console.WriteLine(string.Format("\t{0}", e.Message));
        }

        /// <summary>
        /// Ensure a numeric value is between an acceptable range.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="min">Minimum value to accept</param>
        /// <param name="max">Maximum value to accept</param>
        /// <returns></returns>
        private int EnsureLimits(int value, int min, int max)
        {
            value = Math.Min(value, max);
            value = Math.Max(value, min);
            return value;
        }

        /// <summary>
        /// Ensure a numeric value is between an acceptable range.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="min">Minimum value to accept</param>
        /// <param name="max">Maximum value to accept</param>
        /// <returns></returns>
        private float EnsureLimits(float value, float min, float max)
        {
            value = Math.Min(value, max);
            value = Math.Max(value, min);
            return value;
        }

        /// <summary>
        /// Return the standard pattern name given a number
        /// </summary>
        /// <param name="patternNumber">Pattern number</param>
        /// <returns>String representation of the pattern number</returns>
        private string PatternNameFromNumber(uint patternNumber)
        {
            return string.Format("{0}{1}", "PATTERN", patternNumber);
        }

        /// <summary>
        /// Return the standard preset name given a number
        /// </summary>
        /// <param name="presetNumber">Preset number</param>
        /// <returns>String representation of the preset number</returns>
        private string PresetNameFromNumber(uint presetNumber)
        {
            return string.Format("{0}{1}", "PRESET", presetNumber);
        }

        /// <summary>
        /// Given a direction and speed percentage, provide the x,y values.
        /// </summary>
        /// <param name="direction">Direction to move</param>
        /// <param name="speedPercentage">Percentage at which speed to move</param>
        /// <returns>Class representing the speed and direction</returns>
        private Xyz XyzFromDirection(MoveDirection direction, int speedPercentage = 100)
        {
            Xyz xyz = new Xyz();
            speedPercentage = EnsureLimits(speedPercentage, 0, 100);
            int xVal = velocityLimits.rotation.xMax * (speedPercentage / 100);
            int yVal = velocityLimits.rotation.yMax * (speedPercentage / 100);

            if (direction == MoveDirection.UpLeft || direction == MoveDirection.Left || direction == MoveDirection.DownLeft)
                xVal = -xVal;
            if (direction == MoveDirection.DownLeft || direction == MoveDirection.Down || direction == MoveDirection.DownRight)
                yVal = -yVal;

            switch (direction)
            {
                case MoveDirection.Down:
                case MoveDirection.Up:
                    xyz.y = yVal;
                    xyz.ySpecified = true;
                    break;
                case MoveDirection.Left:
                case MoveDirection.Right:
                    xyz.x = xVal;
                    xyz.xSpecified = true;
                    break;
                default:
                    xyz.x = xVal;
                    xyz.y = yVal;
                    xyz.xSpecified = true;
                    xyz.ySpecified = true;
                    break;
            }
            return xyz;
        }
        #endregion

        
    }
}
