using System;
using System.Collections.Generic;
namespace GameModel
{
    public class World
    {
        public Dictionary<int, Wall> walls = new Dictionary<int, Wall>();
        public Dictionary<int, Tank> tanks = new Dictionary<int, Tank>();
        public Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
        public Dictionary<int, Beam> beams = new Dictionary<int, Beam>();
        public Dictionary<int, Powerup> powerups = new Dictionary<int, Powerup>();

    }
}
