using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Mafia.Network
{
    public class Player
    {

        ///
        /// Players
        ///
        public static HashSet<Player> ListPlayers { get; private set; } = new HashSet<Player>();

        public static HashSet<Player> getListPlayers()
        {
            return ListPlayers;
        }

        #region Mafia
        public Role Role { get; set; }

        public HashSet<IAttribute> Attributes { get; } = new HashSet<IAttribute>();

        public bool Voted { get; set; }

        /// <summary>
        /// Used abilities during the night
        /// </summary>
        public Dictionary<int, IAbility> CriminalActivity { get; set; } = new Dictionary<int, IAbility>();
        #endregion

        #region Generic
        public static bool LastResult { get; private set; } = true;

        public int PermsLevel { get; set; } = 0;

        public IPAddress IP { get; }

        public string Name { get; }

        public Player(IPAddress ip = null, string name = null, bool isHost)
        {
            IP = ip;
            Name = name;
            if (isHost)
            {
                PermsLevel = 1;
            }
            LastResult = ListPlayers.Add(this);
        }

        /// <summary>
        /// Return the Player in the list associed to the SwPlayer 
        /// </summary>
        /// <param name="ip">The ip of the Player you're looking for</param>
        /// <param name="name">Its name</param>
        /// <returns>The player of found out, null otherwise</returns>
        public static Player GetPlayer(IPAddress ip = null, string name = null)
        {
            foreach (Player p in ListPlayers)
            {
                if (p.IP.Equals(ip) || p.Name == name)
                {
                    return p;
                }
            }
            return null;
        }
        #endregion
    }
}
