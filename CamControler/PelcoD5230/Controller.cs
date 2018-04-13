using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Camera_PTZ;
namespace PelcoD5230
{
    
    public partial class Controller : Form
    {
        PTZFacade pelcoController; 
        public Controller()
        {
            InitializeComponent();
            pelcoController = new PTZFacade("192.168.1.212", "admin", "admin");

        }
        void button1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            pelcoController.StopMoving();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pelcoController.Move(PTZFacade.MoveDirection.Up, 100);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            pelcoController.Move(PTZFacade.MoveDirection.Left, 100);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pelcoController.Move(PTZFacade.MoveDirection.Right, 100);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pelcoController.Move(PTZFacade.MoveDirection.Down, 100);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pelcoController.StopMoving();
        }
    }
}
