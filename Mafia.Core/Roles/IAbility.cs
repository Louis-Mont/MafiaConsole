using Mafia.Core;
using Mafia.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mafia.Core.Roles
{
    /// <summary>
    /// All args must be represented ideally by a tuple for better comprehension or an object
    /// </summary>
    public interface IAbility
    {
        /// <summary>
        /// The lower the first it's executed
        /// </summary>
        /// <returns>the priority, if byte maximum value, then it is done at the end the night</returns>
        public byte Priority();

        public string Name();

        public string Desc();

        /// <summary>
        /// I advise you to not @Override it
        /// </summary>
        /// <param name="uses"></param>
        /// <returns>The number of uses this ability have still left. -1 or lower means each night it can be used</returns>
        public sbyte UsesLeft(sbyte uses)
        {
            return (sbyte)(Uses() - uses);
        }

        public sbyte Uses()
        {
            return -1;
        }

        public bool IsNight()
        {
            return true;
        }

        public bool IsDay()
        {
            return false;
        }

        public bool IsLethal()
        {
            return false;
        }

        public string Exec(Player caster, Player target, params string[] args);
    }
}
