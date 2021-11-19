using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameController;
using GameModel;
using TankWars;

namespace View
{
    public partial class TankWars : Form
    {
        private Controller controller = new Controller();
        private DrawingPanel drawer;
        private World world;
        private const int MenuSize = 60;
        private const int ViewSize = 900;
        public TankWars()
        {
            
            InitializeComponent();
            world = controller.GetWorld();
            controller.ErrorOccurred += ErrorOccurredMessage;
            controller.StartDrawWorld += Controller_StartDrawWorld;
            

            drawer = new DrawingPanel(world);
            drawer.Location = new Point(0, MenuSize);
            drawer.Size = new Size(ViewSize, ViewSize);
            this.Controls.Add(drawer);


        }



        private void Controller_StartDrawWorld()
        {
            
        }

        private void ErrorOccurredMessage(string message)
        {
            MessageBox.Show(message);
        }
        private void connectButton_Click(object sender, EventArgs e)
        {
            if(IPTextBox.Text == "" || playerNameTextBox.Text == "")
            {
                MessageBox.Show("IP or player name cannot be blank");
                return;
            }
            controller.Connect(IPTextBox.Text, playerNameTextBox.Text);
            
        }

    }
}
