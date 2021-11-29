// Author: Grant Nations
// Author: Sebastian Ramirez
// DrawingPanel class for CS 3500 TankWars Client (PS8)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GameModel;

namespace TankWars
{
    public class DrawingPanel : Panel
    {
        private World theWorld;
        private Image background;
        private Image wallImage;
        private Image powerUpImage;

        private Image[] tankBodies;
        private Image[] tankTurrets;
        private Image[] projectiles;
        private Image[] explosion;
        private Color[] colors;

        private const int WallSize = 50;
        private const int TankSize = 60;
        private const int TurretSize = 50;
        private const int ProjectileSize = 20;
        private const int PowerupSize = 30;

        private Dictionary<int, int> explosionCounter;
        private Dictionary<int, int> beamCounter;
        private Vector2D playObjDist;

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="w"></param>
        public DrawingPanel(World w)
        {

            playObjDist = new Vector2D();
            DoubleBuffered = true;
            theWorld = w;
            background = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\Background.png"), new Size(theWorld.WorldSize, theWorld.WorldSize));
            wallImage = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\WallSprite.png"), new Size(WallSize, WallSize));
            powerUpImage = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\ChugJug.png"), new Size(PowerupSize, PowerupSize));
            explosionCounter = new Dictionary<int, int>();
            beamCounter = new Dictionary<int, int>();

            tankBodies = new Image[8];
            tankTurrets = new Image[8];
            projectiles = new Image[8];
            explosion = new Image[38];
            colors = new Color[6];

            LoadTanks();
            FillColors();

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
        /// <param name="drawer">The drawingPanel delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
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
        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            e.Graphics.DrawImage(tankBodies[t.ID % tankBodies.Length], new Point(-TankSize / 2, -TankSize / 2));
        }

        /// <summary>
        ///  TODO
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void NameDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            StringBuilder nameAndScore = new StringBuilder();
            nameAndScore.Append(t.Name);
            nameAndScore.Append(": ");
            nameAndScore.Append(t.Score);

            using (SolidBrush whiteBrush = new SolidBrush(Color.White))
            using (Font bigFont = new Font(SystemFonts.DefaultFont.FontFamily, 14, FontStyle.Regular))
            {
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                e.Graphics.DrawString(nameAndScore.ToString(), bigFont, whiteBrush, new Point(0, TankSize / 2), format);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void HealthbarDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            int tankHealth = t.HP;

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

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            e.Graphics.DrawImage(tankTurrets[t.ID % tankBodies.Length], new Point(-TurretSize / 2, -TurretSize / 2));
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
            e.Graphics.DrawImage(powerUpImage, new Point(-PowerupSize / 2, -PowerupSize / 2));
        }

        private void WallDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.DrawImage(wallImage, new Point(-WallSize / 2, -WallSize / 2));
        }

        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile p = o as Projectile;
            Image texture = projectiles[p.Owner % projectiles.Length];
            e.Graphics.DrawImage(texture, new Point(-ProjectileSize / 2, -ProjectileSize / 2));
        }

        private void ExplosionDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            e.Graphics.DrawImage(explosion[explosionCounter[t.ID]++ % (explosion.Length - 1)], new Point(-TankSize / 2, -TankSize / 2));

        }


        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void BeamDrawer(object o, PaintEventArgs e)
        {
            Beam b = o as Beam;
            using (SolidBrush rainbowBrush = new SolidBrush(ChooseColor(beamCounter[b.ID]++)))
            {

                e.Graphics.FillRectangle(rainbowBrush, new Rectangle(0, 0, 2, theWorld.WorldSize));
            }
            ;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void SupportBeamDrawer(object o, PaintEventArgs e)
        {
            Beam b = o as Beam;
            using (SolidBrush rainbowBrush = new SolidBrush(ChooseColor(beamCounter[b.ID])))
            {

                e.Graphics.FillRectangle(rainbowBrush, new Rectangle(0, 0, 2, 60 - beamCounter[b.ID]));
            }
            ;
        }

        /// <summary>
        /// This method is invoked when the DrawingPanel needs to be re-drawn
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Center the view on the middle of the world,
            // since the image and world use different coordinate systems
            lock (theWorld)
            {
                int viewSize = Size.Width; // view is square, so we can just use width
                if (theWorld.Tanks.TryGetValue(theWorld.PlayerID, out Tank tank))
                {
                    double playerX = tank.Location.GetX();
                    double playerY = tank.Location.GetY();
                    e.Graphics.TranslateTransform((float)(-playerX + (viewSize / 2)), (float)(-playerY + (viewSize / 2)));

                    int backgroundX = -theWorld.WorldSize / 2;
                    int backgroundY = -theWorld.WorldSize / 2;
                    e.Graphics.DrawImage(background, new Point(backgroundX, backgroundY));
                }

                // Draw the players
                foreach (Tank t in theWorld.Tanks.Values)
                {
                    playObjDist = t.Location - theWorld.Tanks[theWorld.PlayerID].Location;
                    if (Math.Abs(playObjDist.GetX()) < 900 && (Math.Abs(playObjDist.GetY()) < 900))
                    {
                        if (t.HP > 0)
                        {

                            DrawObjectWithTransform(e, t, t.Location.GetX(), t.Location.GetY(), t.Orientation.ToAngle(), TankDrawer);
                            DrawObjectWithTransform(e, t, t.Location.GetX(), t.Location.GetY(), t.Aiming.ToAngle(), TurretDrawer);
                            DrawObjectWithTransform(e, t, t.Location.GetX(), t.Location.GetY() + 5, 0, NameDrawer);
                            DrawObjectWithTransform(e, t, t.Location.GetX(), t.Location.GetY() - 10, 0, HealthbarDrawer);
                        }

                        else if (t.Died || t.HP == 0)
                        {
                            if (explosionCounter.ContainsKey(t.ID))
                            {
                                DrawObjectWithTransform(e, t, t.Location.GetX(), t.Location.GetY(), 0, ExplosionDrawer);
                            }
                        }
                    }
                }

                // Draw the powerups
                foreach (Powerup pow in theWorld.Powerups.Values)
                {
                    playObjDist = pow.Location - theWorld.Tanks[theWorld.PlayerID].Location;
                    if (Math.Abs(playObjDist.GetX()) < 900 && (Math.Abs(playObjDist.GetY()) < 900))
                        DrawObjectWithTransform(e, pow, pow.Location.GetX(), pow.Location.GetY(), 0, PowerupDrawer);
                }

                foreach (Wall wall in theWorld.Walls.Values)
                {

                    int distX = (int)((wall.P1.GetX() - wall.P2.GetX()) / WallSize);
                    int distY = (int)((wall.P1.GetY() - wall.P2.GetY()) / WallSize);
                    int p2X = (int)wall.P2.GetX();
                    int p2Y = (int)wall.P2.GetY();

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

                foreach (Projectile p in theWorld.Projectiles.Values)
                {
                    playObjDist = p.Location - theWorld.Tanks[theWorld.PlayerID].Location;
                    if (Math.Abs(playObjDist.GetX()) < 900 && (Math.Abs(playObjDist.GetY()) < 900))
                        DrawObjectWithTransform(e, p, p.Location.GetX(), p.Location.GetY(), p.Direction.ToAngle(), ProjectileDrawer);

                }

                foreach (Beam b in theWorld.Beams.Values)
                {
                    if (beamCounter[b.ID] <= 60)
                    {

                        DrawObjectWithTransform(e, b, b.Origin.GetX(), b.Origin.GetY(), b.Direction.ToAngle() - 180, BeamDrawer);
                        DrawObjectWithTransform(e, b, b.Origin.GetX(), b.Origin.GetY(), (b.Direction.ToAngle() - 180) + 30, SupportBeamDrawer);
                        DrawObjectWithTransform(e, b, b.Origin.GetX(), b.Origin.GetY(), (b.Direction.ToAngle() - 180) - 30, SupportBeamDrawer);
                        DrawObjectWithTransform(e, b, b.Origin.GetX(), b.Origin.GetY(), (b.Direction.ToAngle() - 180) + 20, SupportBeamDrawer);
                        DrawObjectWithTransform(e, b, b.Origin.GetX(), b.Origin.GetY(), (b.Direction.ToAngle() - 180) - 20, SupportBeamDrawer);
                        DrawObjectWithTransform(e, b, b.Origin.GetX(), b.Origin.GetY(), (b.Direction.ToAngle() - 180) + 10, SupportBeamDrawer);
                        DrawObjectWithTransform(e, b, b.Origin.GetX(), b.Origin.GetY(), (b.Direction.ToAngle() - 180) - 10, SupportBeamDrawer);

                    }
                }
                // Do anything that Panel (from which we inherit) needs to do
                base.OnPaint(e);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="counter"></param>
        /// <returns></returns>
        private Color ChooseColor(int counter)
        {
            if (counter < 10)
                return colors[0];
            else if (counter < 20)
                return colors[1];
            else if (counter < 30)
                return colors[2];
            else if (counter < 40)
                return colors[3];
            else if (counter < 50)
                return colors[4];
            else
                return colors[5];
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> GetExplosionCounter()
        {

            return explosionCounter;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> GetBeamCounter()
        {
            return beamCounter;
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void LoadTanks()
        {
            tankBodies[0] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\BlueTank.png"), new Size(TankSize, TankSize));
            tankTurrets[0] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\BlueTurret.png"), new Size(TurretSize, TurretSize));

            tankBodies[1] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\DarkTank.png"), new Size(TankSize, TankSize));
            tankTurrets[1] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\DarkTurret.png"), new Size(TurretSize, TurretSize));

            tankBodies[2] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\GreenTank.png"), new Size(TankSize, TankSize));
            tankTurrets[2] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\GreenTurret.png"), new Size(TurretSize, TurretSize));

            tankBodies[3] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\LightGreenTank.png"), new Size(TankSize, TankSize));
            tankTurrets[3] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\LightGreenTurret.png"), new Size(TurretSize, TurretSize));

            tankBodies[4] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\OrangeTank.png"), new Size(TankSize, TankSize));
            tankTurrets[4] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\OrangeTurret.png"), new Size(TurretSize, TurretSize));

            tankBodies[5] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\PurpleTank.png"), new Size(TankSize, TankSize));
            tankTurrets[5] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\PurpleTurret.png"), new Size(TurretSize, TurretSize));

            tankBodies[6] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\RedTank.png"), new Size(TankSize, TankSize));
            tankTurrets[6] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\RedTurret.png"), new Size(TurretSize, TurretSize));

            tankBodies[7] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\YellowTank.png"), new Size(TankSize, TankSize));
            tankTurrets[7] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\YellowTurret.png"), new Size(TurretSize, TurretSize));

            projectiles[0] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-blue.png"), new Size(ProjectileSize, ProjectileSize));
            projectiles[1] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-brown.png"), new Size(ProjectileSize, ProjectileSize));
            projectiles[2] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-green.png"), new Size(ProjectileSize, ProjectileSize));
            projectiles[3] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-grey.png"), new Size(ProjectileSize, ProjectileSize));
            projectiles[4] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-red.png"), new Size(ProjectileSize, ProjectileSize));
            projectiles[5] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-violet.png"), new Size(ProjectileSize, ProjectileSize));
            projectiles[6] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-white.png"), new Size(ProjectileSize, ProjectileSize));
            projectiles[7] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\shot-yellow.png"), new Size(ProjectileSize, ProjectileSize));


            for (int i = 0; i < explosion.Length; i++)
            {
                explosion[i] = new Bitmap(Image.FromFile(@"..\..\..\Resources\Images\frame_" + i + "_delay-0.03s.png"), new Size(TankSize, TankSize));
            }

        }

        /// <summary>
        /// TODO
        /// </summary>
        private void FillColors()
        {
            colors[0] = Color.Red;
            colors[1] = Color.Orange;
            colors[2] = Color.Yellow;
            colors[3] = Color.Green;
            colors[4] = Color.Blue;
            colors[5] = Color.Purple;
        }


    }
}

