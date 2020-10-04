using Mafia.Core.Roles;
using Mafia.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Attribute = Mafia.Core.Roles.Attribute;
using Mafia.Core.Network;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Mafia.Core.Roles
{
    public class Role
    {
        #region Roles
        public static HashSet<Role> Roles { get; private set; } = new HashSet<Role>();

        public static Role BODYGUARD { get; } = new Role("Bodyguard", Alignment.TOWN, new Categorie[] { Categorie.Protective, Categorie.Killing },
            new Ability("Guard", byte.MinValue, delegate (Player caster, Player target, string[] args)
            {
                if (caster.Equals(target))
                {
                    return "You can't guard yourself!";
                }
                target.Attributes.Add(new Attribute("Guarded", delegate (Player targeting, Player target, Ability targetingAbility, string[] args)
                {
                    if (targetingAbility.IsLethal)
                    {
                        caster.PerformAction(Action.Die);
                        targeting.PerformAction(Action.Die);
                        return "This player was being guarded, you die, but its guardian's too";
                    }
                    return "";
                }
                , "If someone attacks a guarded player, both the attacker and the Bodyguard die instead of the guarded player."));
                return $"{target.Name} is now guarded, if it gets any damage ";
            }
            ), "Lynch every criminal and evildoer", "A war veteran who secretly makes a living by selling protection.", new Dictionary<Investigator, string> {
                {Investigator.SHERIFF, "Not Suspicious"},{ Investigator.INVESTIGATOR, "Not Suspicious"}
            });

        /// <summary>
        /// Answers for the Look ability are : Not Suspicous, Suspicous
        /// </summary>
        public static Role SHERIFF { get; } = new Role("Sheriff", Alignment.TOWN, Categorie.Investigative, new Ability("Look", byte.MaxValue, delegate (Player caster, Player target, string[] args)
          {
              target.Role.InvestigationResults.TryGetValue(Investigator.SHERIFF, out string answer);
              return $"{answer}";
          }
        , "Check one player each day for its affiliation of last night."), "Lynch every criminal and evildoer", "A member of law enforcement, forced into hiding because of the threat of murder.", new Dictionary<Investigator, string> {
            { Investigator.SHERIFF, "Not Suspicious" }, { Investigator.INVESTIGATOR, "No Crime" }
        });

        /// <summary>
        /// Answers for the Investigate ability are : No Crime, Crime
        /// </summary>
        public static Role INVESTIGATOR { get; } = new Role("Investigator", Alignment.TOWN, Categorie.Investigative, new Ability("Investigate", byte.MaxValue, delegate (Player caster, Player target, string[] args)
          {
              string message;
              if (target.CriminalActivity.ContainsKey(caster.CurrentGame.NNights))
              {
                  target.Role.InvestigationResults.TryGetValue(Investigator.INVESTIGATOR, out string answer);
                  message = $"{target.Name} has committed {answer} tonight";
              }
              else
              {
                  message = $"{target.Name} has not commited anything tonight";
              }
              return message;
          }
                , "Check one player each day for that player's criminal record during the night."), "Lynch every criminal and evildoer", "A private sleuth, discreetly aiding the townsfolk.", new Dictionary<Investigator, string> {
                {Investigator.SHERIFF, "Not Suspicious"},{Investigator.INVESTIGATOR, "Trespassing"}
            });

        /// <summary>
        /// A role homemade by Swarm, based on the Loup Garou
        /// </summary>
        public static Role WEREWOLF { get; } = new Role("Werewolf", Alignment.WOLF, Categorie.Killing, null, "Kill all the Townsfolk and remain the only faction present in the game", "A basic werewolf, hungrily wandering",
            new Dictionary<Investigator, string> { { Investigator.SHERIFF, "Suspicious" }, { Investigator.INVESTIGATOR, "Trespassing" } });

        public static Role TOWNSFOLK { get; } = new Role("Townsfolf", Alignment.TOWN, Categorie.Benign, null, "Survive and help kill the enemies of the village", "A basic villager trying to survive",
            new Dictionary<Investigator, string> { { Investigator.INVESTIGATOR, "Not Suspicious" }, { Investigator.SHERIFF, "No Crime" } });

        #endregion

        private static readonly Dictionary<Investigator, Role> InvestToRole = new Dictionary<Investigator, Role>(){
            {Investigator.SHERIFF, SHERIFF},{Investigator.INVESTIGATOR, INVESTIGATOR}
        };

        #region Members
        public string Name { get; private set; }

        public string Goal { get; private set; }

        public string Summary { get; private set; }

        public bool VotesResolveEquality { get; private set; }

        public HashSet<Ability> Abilities { get; private set; }

        public Alignment Alignment { get; private set; }

        public HashSet<Categorie> Categories { get; private set; }

        public HashSet<Attribute> Attributes { get; private set; }

        public Dictionary<Investigator, string> InvestigationResults { get; private set; }
        #endregion

        #region Constructors
        protected Role(string name, Alignment alignment, Categorie categorie, Ability ability, string goal, string summary,
                Dictionary<Investigator, string> investigationResults, bool votesResolveEquality = false)
        {
            SetRole(name, alignment, new Categorie[] { categorie }, new Ability[] { ability }, new Attribute[0], goal,
                    summary, investigationResults, votesResolveEquality);
        }

        protected Role(string name, Alignment alignment, Categorie[] categories, Ability ability, string goal,
                string summary, Dictionary<Investigator, string> investigationResults, bool votesResolveEquality = false)
        {
            SetRole(name, alignment, categories, new Ability[] { ability }, new Attribute[0], goal, summary,
                    investigationResults, votesResolveEquality);
        }

        protected Role(string name, Alignment alignment, Categorie[] categories, Ability[] abilities,
                Attribute[] attributes, string goal, string summary,
                Dictionary<Investigator, string> investigationResults, bool votesResolveEquality = false)
        {
            SetRole(name, alignment, categories, abilities, attributes, goal, summary, investigationResults, votesResolveEquality);
        }

        /// <summary>
        /// What the constructor really does. All the other constructor only calls this Function with the right values
        /// </summary>
        /// <param name="name"></param>
        /// <param name="alignment"></param>
        /// <param name="categories"></param>
        /// <param name="abilities"></param>
        /// <param name="attributes"></param>
        /// <param name="goal"></param>
        /// <param name="summary"></param>
        /// <param name="investigationResults"></param>
        /// <param name="votesResolveEquality"></param>
        private void SetRole(string name, Alignment alignment, Categorie[] categories, Ability[] abilities,
                Attribute[] attributes, string goal, string summary,
                Dictionary<Investigator, string> investigationResults, bool votesResolveEquality)
        {
            Name = name;

            Alignment = alignment;

            Categories = new HashSet<Categorie>();
            foreach (Categorie cat in categories)
            {
                Categories.Add(cat);
            }

            Abilities = new HashSet<Ability>();
            foreach (Ability ability in abilities)
            {
                Abilities.Add(ability);
            }

            Attributes = new HashSet<Attribute>();
            foreach (Attribute attribute in attributes)
            {
                Attributes.Add(attribute);
            }

            InvestigationResults = new Dictionary<Investigator, string>(investigationResults);

            Goal = goal;
            Summary = summary;
            VotesResolveEquality = votesResolveEquality;

            Roles.Add(this);
        }
        #endregion
    }
}
