using Mafia.Core;
using Mafia.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mafia.Core.Roles
{
    public class Attribute
    {
        public string Name { get; private set; }

        public string Desc { get; private set; }

        public delegate string FRepercussion(Player targeting, Player target, Ability targetAbility, params string[] args);

        public FRepercussion Repercussion { get; private set; }

        public Attribute(string name, FRepercussion repercussion, string desc="")
        {
            Name = name ?? $"Attr{new Random().Next(int.MaxValue)}";
            Repercussion = repercussion??throw new NotImplementedException("");
            Desc = desc;
        }
    }
}
