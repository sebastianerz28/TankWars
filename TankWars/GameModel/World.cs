// Author: Grant Nations
// Author: Sebastian Ramirez
// World class for CS 3500 TankWars Client (PS8)

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
        private int playerID;
        private int worldSize;

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Wall> GetWalls()
        {
            return walls;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Tank> GetTanks()
        {
            return tanks;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Projectile> GetProjectiles()
        {
            return projectiles;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Beam> GetBeams()
        {
            return beams;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Powerup> GetPowerups()
        {
            return powerups;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public int GetPlayerId()
        {
            return playerID;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="id"></param>
        public void SetPlayerID(int id)
        {
            playerID = id;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public int GetWorldSize()
        {
            return worldSize;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="size"></param>
        public void SetWorldSize(int size)
        {
            worldSize = size;
        }
    }
}
