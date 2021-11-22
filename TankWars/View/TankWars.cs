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
            controller.UpdateArrived += OnFrame;
            controller.WorldSizeArrived += InitializeDrawer;

            // Set up key and mouse handlers
            this.KeyDown += HandleKeyDown;
            this.KeyUp += HandleKeyUp;

        }

        private void InitializeDrawer()
        {
            // Place and add the drawing panel
            drawer = new DrawingPanel(world);
            drawer.Location = new Point(0, MenuSize);
            drawer.Size = new Size(ViewSize, ViewSize);
            MethodInvoker invoker = new MethodInvoker(() => Controls.Add(drawer));
            Invoke(invoker);

            drawer.MouseDown += HandleMouseDown;
            drawer.MouseUp += HandleMouseUp;
            drawer.MouseMove += HandleMouseMove;
        }

        private void HandleMouseMove(object sender, EventArgs e)
        {

            controller.HandleMouseMove(drawer.PointToClient(Cursor.Position), ViewSize);
        }

        /// <summary>
        /// Handle mouse up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            controller.CancelMouseRequest(e);
        }

        /// <summary>
        /// Handle mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            controller.HandleMouseRequest(e);
        }


        /// <summary>
        /// Key up handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {

            controller.CancelMoveRequest(e);
        }

        /// <summary>
        /// Key down handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            // TODO: added feature
            //if (e.KeyCode == Keys.Escape)
            //    Application.Exit();


            controller.HandleMoveRequest(e);

            //// Prevent other key handlers from running
            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        /// <summary>
        /// Handler for the controller's UpdateArrived event
        /// </summary>
        private void OnFrame()
        {
            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            MethodInvoker invoker = new MethodInvoker(() => this.Invalidate(true));
            // TODO: fix this bug
            this.Invoke(invoker);
        }

        private void ErrorOccurredMessage(string message)
        {
            MessageBox.Show(message);
            connectButton.Enabled = true;
            IPTextBox.Enabled = true;
            playerNameTextBox.Enabled = true;
        }
        private void connectButton_Click(object sender, EventArgs e)
        {
            if (IPTextBox.Text == "" || playerNameTextBox.Text == "")
            {
                MessageBox.Show("IP or player name cannot be blank");
                return;
            }
            controller.Connect(IPTextBox.Text, playerNameTextBox.Text);
            connectButton.Enabled = false;
            IPTextBox.Enabled = false;
            playerNameTextBox.Enabled = false;
        }

    }
}
