using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameModel;


namespace TankWars
{
    public class DrawingPanel : Panel
    {

        private World theWorld;
        private Image background;
        public DrawingPanel(World w)
        {
            DoubleBuffered = true;
            theWorld = w;
            Console.WriteLine(Directory.GetCurrentDirectory());
            background = Image.FromFile(@"..\..\..\Resources\Images\Background.png");

        }


        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);


        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            e.Graphics.TranslateTransform((int)worldX, (int)worldY);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void PlayerDrawer(object o, PaintEventArgs e)
        {
            Tank p = o as Tank;

           

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
            using (System.Drawing.SolidBrush blueBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue))
            using (System.Drawing.SolidBrush greenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green))
            {
                // Rectangles are drawn starting from the top-left corner.
                // So if we want the rectangle centered on the player's location, we have to offset it
                // by half its size to the left (-width/2) and up (-height/2)
                Rectangle r = new Rectangle(-(60 / 2), -(60 / 2), 60, 60);
                e.Graphics.FillRectangle(redBrush, r);
            }
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            Powerup p = o as Powerup;

            int width = 8;
            int height = 8;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
            using (System.Drawing.SolidBrush yellowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Yellow))
            using (System.Drawing.SolidBrush blackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
            {
                // Circles are drawn starting from the top-left corner.
                // So if we want the circle centered on the powerup's location, we have to offset it
                // by half its size to the left (-width/2) and up (-height/2)
                Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);

                /* if (p.GetKind() == 1) // red powerup
                     e.Graphics.FillEllipse(redBrush, r);
                 if (p.GetKind() == 2) // yellow powerup
                     e.Graphics.FillEllipse(yellowBrush, r);
                 if (p.GetKind() == 3) // black powerup
                     e.Graphics.FillEllipse(blackBrush, r);*/
            }


        }
        
        private void WallDrawer(object o, PaintEventArgs e)

        {
            Wall w = o as Wall;
            int size = 50;

            using (System.Drawing.SolidBrush greyBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Gray))
            {
                Rectangle r = new Rectangle(-(size / 2), -(size / 2), size, size);
                e.Graphics.FillRectangle(greyBrush, r);
            }

        }

        


        // This method is invoked when the DrawingPanel needs to be re-drawn
        protected override void OnPaint(PaintEventArgs e)
        {
            // Center the view on the middle of the world,
            // since the image and world use different coordinate systems

            int viewSize = Size.Width; // view is square, so we can just use width
            if (theWorld.GetTanks().TryGetValue(theWorld.GetPlayerId(), out Tank player))
            {
                double playerX = player.location.GetX();
                double playerY = player.location.GetY();
                e.Graphics.TranslateTransform((float)(-playerX + (viewSize / 2)), (float)(-playerY + (viewSize / 2)));
            }
            else
            {
                
                e.Graphics.TranslateTransform(viewSize / 2, viewSize / 2);
            }
            
            
            lock (theWorld)
            {


                // Draw the players
                foreach (Tank play in theWorld.GetTanks().Values)
                {
                    DrawObjectWithTransform(e, play, play.location.GetX(), play.location.GetY(), play.orientation.ToAngle(), PlayerDrawer);
                }

                // Draw the powerups
                foreach (Powerup pow in theWorld.GetPowerups().Values)
                {
                    DrawObjectWithTransform(e, pow, pow.loc.GetX(), pow.loc.GetY(), 0, PowerupDrawer);
                }

                // Do anything that Panel (from which we inherit) needs to do
                base.OnPaint(e);
            }

        }



    }
}

