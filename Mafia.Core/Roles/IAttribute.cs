using Mafia.Core;
using Mafia.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mafia.Core.Roles
{
    public interface IAttribute
    {
        public string Name();

        public string Desc();

        public string Repercussion<TRPArgs>(Player targeting, Player target, IAbility targetAbility, TRPArgs args);
    }
}
