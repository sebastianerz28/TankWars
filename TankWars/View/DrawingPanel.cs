using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameModel;
using static GameController.Controller;

namespace TankWars
{
    public class DrawingPanel : Panel
    {

        private World theWorld;
        private Image background;
        private Image wallImage;
        private Image PowerUpImage;
        

        private Image[] TankBodies;
        private Image[] TankTurrets;
        private Image[] Projectiles;
        private Image[] Explosion;
        

        private const int WallSize = 50;
        private const int TankSize = 60;
        private const int TurretSize = 50;
        private const int ProjectileSize = 20;
        private const int PowerupSize = 30;

        private Dictionary<int, int> explosionIndex; 

        public DrawingPanel(World w, Action a)
        {
           
            
            DoubleBuffered = true;
            theWorld = w;
            background = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\Background.png"), new Size(theWorld.GetWorldSize(), theWorld.GetWorldSize()));
            wallImage = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\WallSprite.png"), new Size(WallSize, WallSize));
            PowerUpImage = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\ChugJug.png"), new Size(PowerupSize, PowerupSize));
            explosionIndex = new Dictionary<int, int>();

            TankBodies = new Image[8];
            TankTurrets = new Image[8];
            Projectiles = new Image[8];
            Explosion = new Image[8];
            LoadTanks();
            
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

            e.Graphics.DrawImage(TankBodies[p.ID % TankBodies.Length], new Point(-TankSize / 2, -TankSize / 2));

            /*   e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
               using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
               {
                   // Rectangles are drawn starting from the top-left corner.
                   // So if we want the rectangle centered on the player's location, we have to offset it
                   // by half its size to the left (-width/2) and up (-height/2)
                   Rectangle r = new Rectangle(-(60 / 2), -(60 / 2), 60, 60);
                   e.Graphics.FillRectangle(redBrush, r);
               }*/
        }

        private void NameDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            StringBuilder nameAndScore = new StringBuilder();
            nameAndScore.Append(t.name);
            nameAndScore.Append(": ");
            nameAndScore.Append(t.score);

            using (SolidBrush whiteBrush = new SolidBrush(Color.White))
            using (Font bigFont = new Font(SystemFonts.DefaultFont.FontFamily, 14, FontStyle.Regular)) {
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                e.Graphics.DrawString(nameAndScore.ToString(), bigFont, whiteBrush, new Point(0, TankSize / 2), format);
            }
        }

        private void HealthbarDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            int tankHealth = t.hp;

            int leftSize = (int)(TankSize * ((double)tankHealth / 3));
            int rightSize = TankSize - leftSize;

            Rectangle leftRectangle = new Rectangle(-TankSize / 2, -TankSize / 2, leftSize, 8);
            Rectangle rightRectangle = new Rectangle((-TankSize / 2) + leftSize, -TankSize / 2, rightSize, 8);

            

            if (tankHealth == 3)
            {
                using (SolidBrush greenBrush = new SolidBrush(Color.Green))
                {
                    e.Graphics.FillRectangle(greenBrush, leftRectangle);
                }
            }
            else if (tankHealth == 2)
            {
                using (SolidBrush orangeBrush = new SolidBrush(Color.Orange))
                {
                    e.Graphics.FillRectangle(orangeBrush, leftRectangle);
                }
            }
            else if (tankHealth == 1)
            {
                using (SolidBrush redBrush = new SolidBrush(Color.Red))
                {
                    e.Graphics.FillRectangle(redBrush, leftRectangle);
                }
            }

            using (SolidBrush grayBrush = new SolidBrush(Color.LightGray))
            {
                e.Graphics.FillRectangle(grayBrush, rightRectangle);
            }

        }

        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            e.Graphics.DrawImage(TankTurrets[t.ID % TankBodies.Length], new Point(-TurretSize / 2, -TurretSize / 2));
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
            e.Graphics.DrawImage(PowerUpImage, new Point(-PowerupSize / 2, -PowerupSize / 2));
        }

        private void WallDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.DrawImage(wallImage, new Point(-WallSize / 2, -WallSize / 2));
        }

        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile p = o as Projectile;
            Image texture = Projectiles[p.owner % Projectiles.Length];
            e.Graphics.DrawImage(texture, new Point(-ProjectileSize / 2, -ProjectileSize / 2));
        }

        private void ExplosionDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            e.Graphics.DrawImage(Explosion[theWorld.GetExplosionCounter()[t.ID]++], new Point(-TankSize / 2, -TankSize / 2));
            
        }



        private void BeamDrawer(object o, PaintEventArgs e)
        {

            using (SolidBrush goldBrush = new SolidBrush(Color.Gold))
            {
                e.Graphics.FillRectangle(goldBrush, new Rectangle(-(TankSize / 2), -(TankSize / 2), 60, 100000));
            }
        }

        // This method is invoked when the DrawingPanel needs to be re-drawn
        protected override void OnPaint(PaintEventArgs e)
        {
            // Center the view on the middle of the world,
            // since the image and world use different coordinate systems
            lock (theWorld)
            {
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

                // Draw the players
                foreach (Tank play in theWorld.GetTanks().Values)
                {
                    if (play.hp > 0)
                    {
                        
                        DrawObjectWithTransform(e, play, play.location.GetX(), play.location.GetY(), play.orientation.ToAngle(), PlayerDrawer);
                        DrawObjectWithTransform(e, play, play.location.GetX(), play.location.GetY(), play.aiming.ToAngle(), TurretDrawer);
                        DrawObjectWithTransform(e, play, play.location.GetX(), play.location.GetY() + 5, 0, NameDrawer);
                        DrawObjectWithTransform(e, play, play.location.GetX(), play.location.GetY() - 10, 0, HealthbarDrawer);
                    }

                   else if (play.died || play.hp == 0)
                    {
                        if(theWorld.GetExplosionCounter().ContainsKey(play.ID))
                        {
                            if (theWorld.GetExplosionCounter()[play.ID] == 7)
                            {
                                theWorld.GetExplosionCounter()[play.ID] = 0;
                            }

                            if (theWorld.GetExplosionCounter()[play.ID] !=  7)
                            {
                                DrawObjectWithTransform(e, play, play.location.GetX(), play.location.GetY(), 0, ExplosionDrawer);
                            }
                        }
                       
                        
                    }

                }

                // Draw the powerups
                foreach (Powerup pow in theWorld.GetPowerups().Values)
                {
                    if (!pow.died)
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
                            if (distX < 0)
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
                            if (distY < 0)
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

                foreach (Projectile p in theWorld.GetProjectiles().Values)
                {
                    if (!p.died)
                        DrawObjectWithTransform(e, p, p.loc.GetX(), p.loc.GetY(), p.dir.ToAngle(), ProjectileDrawer);
                }

                // Do anything that Panel (from which we inherit) needs to do
                base.OnPaint(e);
                // TODO: move this outside of lock?
            }

        }

        private void LoadTanks()
        {
            TankBodies[0] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\BlueTank.png"), new Size(TankSize, TankSize));
            TankTurrets[0] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\BlueTurret.png"), new Size(TurretSize, TurretSize));

            TankBodies[1] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\DarkTank.png"), new Size(TankSize, TankSize));
            TankTurrets[1] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\DarkTurret.png"), new Size(TurretSize, TurretSize));

            TankBodies[2] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\GreenTank.png"), new Size(TankSize, TankSize));
            TankTurrets[2] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\GreenTurret.png"), new Size(TurretSize, TurretSize));

            TankBodies[3] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\LightGreenTank.png"), new Size(TankSize, TankSize));
            TankTurrets[3] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\LightGreenTurret.png"), new Size(TurretSize, TurretSize));

            TankBodies[4] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\OrangeTank.png"), new Size(TankSize, TankSize));
            TankTurrets[4] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\OrangeTurret.png"), new Size(TurretSize, TurretSize));

            TankBodies[5] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\PurpleTank.png"), new Size(TankSize, TankSize));
            TankTurrets[5] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\PurpleTurret.png"), new Size(TurretSize, TurretSize));

            TankBodies[6] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\RedTank.png"), new Size(TankSize, TankSize));
            TankTurrets[6] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\RedTurret.png"), new Size(TurretSize, TurretSize));

            TankBodies[7] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\YellowTank.png"), new Size(TankSize, TankSize));
            TankTurrets[7] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\YellowTurret.png"), new Size(TurretSize, TurretSize));

            Projectiles[0] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-blue.png"), new Size(ProjectileSize, ProjectileSize));
            Projectiles[1] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-brown.png"), new Size(ProjectileSize, ProjectileSize));
            Projectiles[2] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-green.png"), new Size(ProjectileSize, ProjectileSize));
            Projectiles[3] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-grey.png"), new Size(ProjectileSize, ProjectileSize));
            Projectiles[4] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-red.png"), new Size(ProjectileSize, ProjectileSize));
            Projectiles[5] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-violet.png"), new Size(ProjectileSize, ProjectileSize));
            Projectiles[6] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-white.png"), new Size(ProjectileSize, ProjectileSize));
            Projectiles[7] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-yellow.png"), new Size(ProjectileSize, ProjectileSize));


            for (int i = 0; i < Explosion.Length; i++)
                Explosion[i] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\explosion_" + i + ".png"), new Size(TankSize, TankSize));
        }


    }
}

