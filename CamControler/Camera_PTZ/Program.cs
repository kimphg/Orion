using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Text;
using System.Threading;

namespace Camera_PTZ
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PtzControl());
        }
    }
}
