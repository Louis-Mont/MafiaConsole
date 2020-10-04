using Mafia.Core;
using Mafia.Core.Exceptions;
using Mafia.Core.Network;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Mafia.Core.Roles
{
    /// <summary>
    /// All args must be represented ideally by a tuple for better comprehension or an object
    /// </summary>
    public class Ability
    {
        public string Name { get; private set; }

        public string Desc { get; private set; }

        /// <summary>
        /// The lower the first it's executed. If byte maximum value, then it is done at the end the night/day
        /// </summary>
        public byte Priority { get; private set; }

        /// <summary>
        /// If equal to -1, the number of uses is unlimited
        /// </summary>
        public sbyte MaxUses { get; private set; }

        public sbyte Uses { get; private set; } = 0;

        public delegate string FExec(Player caster, Player target, params string[] args);

        /// <summary>
        /// What the ability does. Each call increases the number of uses by 1.
        /// </summary>
        /// <exception cref="MaxUsesException">If the number of MaxUses have been reached</exception>
        public FExec Exec
        {
            get
            {
                if (Uses >= MaxUses && MaxUses >= 0)
                {
                    throw new MaxUsesException("The number of max utilisations have been reached");
                }
                Uses += 1;
                return exec;
            }
            private set
            {
                exec = value;
            }
        }

        private FExec exec;

        /// <summary>
        /// Abilities during night can be executed in order depending on their priority
        /// </summary>
        public bool IsNight { get; private set; }

        /// <summary>
        /// Abilities during the day can be executed anytime during the Voting phase
        /// </summary>
        public bool IsDay { get; private set; }

        public bool IsLethal { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of the ability. If null, sets its name to Abil{Random Int}</param>
        /// <param name="priority">The lower the first it's executed. If byte maximum value, then it is done at the end the night/day</param>
        /// <param name="exec">What the ability does</param>
        /// <param name="desc"></param>
        /// <param name="maxUses">If equal to -1, the number of uses is unlimited</param>
        /// <param name="isNight">Abilities during night can be executed in order depending on their priority</param>
        /// <param name="isDay">Abilities during the day can be executed anytime during the Voting phase</param>
        /// <param name="isLethal"></param>
        /// <exception cref="ArgumentNullException">If the ability is null</exception>
        public Ability(string name, byte priority, FExec exec, string desc = "", sbyte maxUses = -1, bool isNight = true, bool isDay = false, bool isLethal = false)
        {
            Name = name ?? $"Abil{new Random().Next(int.MaxValue)}";
            Priority = priority;
            Exec = exec ?? throw new ArgumentNullException("Abilities needs something for them to be done when executed");
            Desc = desc;
            MaxUses = maxUses;
            IsNight = isNight;
            IsDay = isDay;
            IsLethal = isLethal;
        }
    }
}
