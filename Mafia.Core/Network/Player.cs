using Mafia.Core.Roles;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Action = Mafia.Core.Roles.Action;
using Attribute = Mafia.Core.Roles.Attribute;

namespace Mafia.Core.Network
{
    public class Player
    {

        #region Mafia
        public Role Role { get; set; }

        public HashSet<Attribute> Attributes { get; } = new HashSet<Attribute>();

        /// <summary>
        /// The Game the player is currently in
        /// </summary>
        public Game CurrentGame;

        public bool HasVoted { get; set; }

        #region DeadOrAlive
        public bool IsDead
        {
            get
            {
                return isDead;
            }
            set
            {
                isDead = value;
                isAlive = !value;
            }
        }

        private bool isDead;

        public bool IsAlive
        {
            get
            {
                return isAlive;
            }
            set
            {
                isAlive = value;
                isDead = !value;
            }
        }

        private bool isAlive;
        #endregion

        public void PerformAction(Action action)
        {
            switch (action)
            {
                case Action.Die:
                    IsDead = true;
                    break;
            }
        }

        public void Vote(Player target)
        {
            CurrentGame.Votes.Add(target, CurrentGame.Votes.ContainsKey(target) ? 1 : CurrentGame.Votes[target] + 1);
        }

        /// <summary>
        /// Used abilities during the nights, indexed by the number of the night he did it
        /// </summary>
        public Dictionary<byte, Ability> CriminalActivity { get; set; } = new Dictionary<byte, Ability>();
        #endregion

        #region Generic
        public static bool LastResult { get; private set; } = true;

        public byte PermsLevel { get; set; } = 0;

        public IPAddress IP { get; }

        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of the player. If null or nothing, sets its name to Player{Random Int}</param>
        /// <param name="ip"></param>
        /// <param name="isHost"></param>
        public Player(string name = "", IPAddress ip = null, bool isHost = false)
        {
            IP = ip;
            Name = name == "" || name == null ? $"Player{new Random().Next(int.MaxValue)}" : name;
            if (isHost)
            {
                PermsLevel = 1;
            }
        }

        /// <summary>
        /// Return the Player in the list
        /// </summary>
        /// <param name="listPlayers">The list of the player you're looking in for</param>
        /// <param name="ip">The ip of the Player you're looking for</param>
        /// <param name="name">Its name</param>
        /// <returns>The player of found out, null otherwise</returns>
        public static Player GetPlayer(List<Player> listPlayers, string name = null, IPAddress ip = null)
        {
            foreach (Player p in listPlayers)
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
