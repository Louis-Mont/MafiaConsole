using Mafia.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mafia.Core.App
{
    /// <summary>
    /// All the global variables and methond concerning all the Application as a whole
    /// </summary>
    public static class App
    {
        public static Server currentServer { get; set; }

        public static Lobby currentLobby { get; set; }
    }
}
