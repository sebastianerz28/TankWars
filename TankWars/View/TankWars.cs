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
namespace View
{
    public partial class TankWars : Form
    {
        private Controller controller = new Controller();
        public TankWars()
        {
            InitializeComponent();
            controller.ErrorOccurred += ErrorOccurredMessage;
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
