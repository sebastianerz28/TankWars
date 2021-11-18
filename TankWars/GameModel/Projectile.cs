using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace GameModel
{
    public class Projectile
    {
        [JsonProperty(PropertyName = "proj")]
        public int id { get; private set; }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D loc { get; private set; }

        [JsonProperty(PropertyName = "dir")]
        public Vector2D dir { get; private set; }

        [JsonProperty(PropertyName = "died")]
        public bool died { get; private set; }

        [JsonProperty(PropertyName = "owner")]
        public int owner { get; private set; }

        public Projectile()
        {

        }
    }
}
