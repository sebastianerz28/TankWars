using System;
using System.Collections.Generic;
namespace GameModel
{
    public class World
    {
        private Dictionary<int, Wall> walls = new Dictionary<int, Wall>();
        private Dictionary<int, Tank> tanks = new Dictionary<int, Tank>();
        private Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
        private Dictionary<int, Beam> beams = new Dictionary<int, Beam>();
        private Dictionary<int, Powerup> powerups = new Dictionary<int, Powerup>();
        private int PlayerId;
        private int WorldSize;

        public Dictionary<int, Wall> GetWalls()
        {
            return walls;
        }

        public Dictionary<int, Tank> GetTanks()
        {
            return tanks;
        }

        public Dictionary<int, Projectile> GetProjectiles()
        {
            return projectiles;
        }

        public Dictionary<int, Beam> GetBeams()
        {
            return beams;
        }
        public Dictionary<int, Powerup> GetPowerups()
        {
            return powerups;
        }

        public int GetPlayerId()
        {
            return PlayerId;
        }
        public void SetPlayerID(int id)
        {
            PlayerId = id;
        }

        public int GetWorldSize()
        {
            return WorldSize;
        }

        public void SetWorldSize(int size)
        {
            WorldSize = size;
        }
    }
}
