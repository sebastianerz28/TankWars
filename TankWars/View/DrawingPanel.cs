﻿using System;
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
        private Image wallImage;

        private const int WallSize = 50;
        private const int TankSize = 60;
        private const int TurretSize = 50;
        private const int ProjectileSize = 30;
        private const int PowerupSize = 10;

        public DrawingPanel(World w)
        {
            DoubleBuffered = true;
            theWorld = w;
            background = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\Background.png"), new Size(theWorld.GetWorldSize(), theWorld.GetWorldSize()));
            wallImage = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\WallSprite.png"), new Size(WallSize, WallSize));
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
            {
                // Circles are drawn starting from the top-left corner.
                // So if we want the circle centered on the powerup's location, we have to offset it
                // by half its size to the left (-width/2) and up (-height/2)
                Rectangle r = new Rectangle(-(PowerupSize / 2), -(PowerupSize / 2), PowerupSize, PowerupSize);

                e.Graphics.FillEllipse(redBrush, r);
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
            e.Graphics.DrawImage(wallImage, new Point(-WallSize/2, -WallSize/2));
            //e.Graphics.DrawImage(wallImage, new Rectangle(-(WallSize / 2), -(WallSize / 2), WallSize, WallSize));

            //using (System.Drawing.TextureBrush wallBrush = new System.Drawing.TextureBrush(wallImage))
            //{
            //    Rectangle r = new Rectangle(-(WallSize / 2), -(WallSize / 2), WallSize, WallSize);
            //    e.Graphics.FillRectangle(wallBrush, r);
            //}
        }

        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            using(System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue))
            {

                e.Graphics.FillEllipse(redBrush, new Rectangle(-ProjectileSize / 2, -ProjectileSize / 2, ProjectileSize, ProjectileSize));
            }
        }

        private void ExplosionDrawer(object o, PaintEventArgs e)
        {
            using (System.Drawing.SolidBrush lavenderBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Lavender))
            {
                e.Graphics.FillRectangle(lavenderBrush, new Rectangle(-(60 / 2), -(60 / 2), 60, 60));
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

                int backgroundX = -theWorld.GetWorldSize() / 2;
                int backgroundY = -theWorld.GetWorldSize() / 2;
                e.Graphics.DrawImage(background, new Point(backgroundX, backgroundY));
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
                    if(play.hp > 0)
                        DrawObjectWithTransform(e, play, play.location.GetX(), play.location.GetY(), play.orientation.ToAngle(), PlayerDrawer);
                    if (play.died || play.hp == 0)
                    {
                        DrawObjectWithTransform(e, play, play.location.GetX(), play.location.GetY(), 0, ExplosionDrawer);
                    }

                }

                // Draw the powerups
                foreach (Powerup pow in theWorld.GetPowerups().Values)
                {
                    if(!pow.died)
                        DrawObjectWithTransform(e, pow, pow.loc.GetX(), pow.loc.GetY(), 0, PowerupDrawer);
                }

                foreach (Wall wall in theWorld.GetWalls().Values)
                {
                    int distX = (int)((wall.p1.GetX() - wall.p2.GetX()) / WallSize);
                    int distY = (int)((wall.p1.GetY() - wall.p2.GetY()) / WallSize);
                    int p2X = (int)wall.p2.GetX();
                    int p2Y = (int)wall.p2.GetY();
                    
                    for (int i = 0; i <= Math.Abs(distX == 0 ? distY : distX); i++)
                    {
                        
                        DrawObjectWithTransform(e, wall, p2X, p2Y, 0, WallDrawer);
                        if (distX != 0)
                        {
                            if(distX < 0)
                            {
                                p2X -= WallSize;
                            }
                            else
                            {
                                p2X += WallSize;
                            }
                            
                        }
                        else
                        {
                            if(distY < 0)
                            {
                                p2Y -= WallSize;
                            }
                            else
                            {
                                p2Y += WallSize;
                            }
                            
                        }
                    }

                }

                foreach(Projectile p in theWorld.GetProjectiles().Values)
                {
                    if(!p.died)
                        DrawObjectWithTransform(e, p, p.loc.GetX(), p.loc.GetY(), p.dir.ToAngle(), ProjectileDrawer);
                }

                

                

                // Do anything that Panel (from which we inherit) needs to do
                base.OnPaint(e);
            }

        }



    }
}

