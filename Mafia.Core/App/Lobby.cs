using Mafia.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mafia.Core.App
{
    public class Lobby
    {
        public Game Game { get; private set; }

        public HashSet<Player> ListPlayers { get; set; } = new HashSet<Player>();

        public Player Host { get; private set; }

        public Lobby(Player host)
        {
            Host = host;
            ListPlayers.Add(host);
        }

        public void LaunchGame()
        {
            Game = new Game(ListPlayers);
        }
    }
}
