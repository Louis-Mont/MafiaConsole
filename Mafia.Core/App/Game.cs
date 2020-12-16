using Mafia.Core;
using Mafia.Core.Network;
using Mafia.Core.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Action = Mafia.Core.Roles.Action;

namespace Mafia.Core.App
{
    public class Game
    {
        #region GameConfig
        public Dictionary<Role, byte> RoleComp { get; set; } = new Dictionary<Role, byte>
        {
            {Role.BODYGUARD, 1},
            {Role.INVESTIGATOR, 1},
            {Role.SHERIFF, 1 }
        };

        #region Time Variables
        /// <summary>
        /// The time the players have before they can vote, do day actions,...
        /// In seconds
        /// </summary>
        public short PreVoteTime { get; set; }

        /// <summary>
        /// The amount of time the players have to vote, in seconds
        /// </summary>
        public short VotingTime { get; set; }

        /// <summary>
        /// The time the different roles have to take an action during the night, in seconds
        /// </summary>
        public short ActionTime { get; set; }

        /// <summary>
        /// The time the player, usually the major, have to resolve and equality
        /// </summary>
        public short ResolveEqualityTime { get; set; }
        #endregion
        #endregion

        /// <summary>
        /// All the players present in the game, dead or alive
        /// </summary>
        public static HashSet<Player> ListPlayers { get; private set; }

        private List<(Player, Ability)> nightFlow = new List<(Player, Ability)>();

        #region DayNight
        public byte NDays { get; private set; } = 0;

        public bool IsDay
        {
            get { return isDay; }
            set
            {
                IsNight = !value;
                isDay = value;
            }
        }

        private bool isDay = true;

        public byte NNights { get; private set; } = 0;

        public bool IsNight
        {
            get { return isNight; }
            set
            {
                IsDay = !value;
                isNight = value;
            }
        }

        private bool isNight = false;
        #endregion

        public bool IsVotingTime { get; set; } = false;

        public bool IsGameStarted { get; set; } = false;

        public Dictionary<Player, int> Votes = new Dictionary<Player, int>();

        public Game(HashSet<Player> listPlayers)
        {
            if (listPlayers.Count < 2)
            {
                throw new Exception("Not enough players to start a game");
            }
            foreach (Player p in listPlayers)
            {
                p.CurrentGame = this;
            }
            ListPlayers = listPlayers;
        }

        #region Déroulement de la Game
        #region Step by step
        public void Start()
        {
            foreach (var p in ListPlayers)
            {
                foreach (var abil in p.Role.Abilities)
                {
                    if (abil.IsNight)
                    {
                        nightFlow.Insert(abil.Priority, (p, abil));
                    }
                }
            }
            IsGameStarted = true;
            PreVote();
        }

        private void PreVote()
        {
            if (!IsGameFinished())
            {
                var preVoteTimer = new Timer(PreVoteTime * 1000);
                App.currentServer.SendMessage("Start of the day");
                preVoteTimer.Elapsed += PreVoteTimer_Elapsed;
                preVoteTimer.Start();
            }
            else
            {
                EndGame();
            }
        }

        private void PreVoteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Vote();
        }

        private void Vote()
        {
            App.currentServer.AwaitMessage = Message.Vote;
            if (NDays > 0)
            {
                var voteTimer = new Timer(VotingTime * 1000);
                App.currentServer.SendMessage("Start of the votes");
                IsVotingTime = true;
                voteTimer.Elapsed += VoteTimer_Elapsed;
                voteTimer.Start();
            }
        }

        private void VoteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IsVotingTime = false;
            ResolveVotes();
        }

        private void Night()
        {
            var playerOrderFirst = new List<(int, Player)>();
            // The player that get their action effectued last, only if the priority of one of the abilities is byte max value
            var playerOrderLast = new HashSet<Player>();

            App.currentServer.SendMessage("The Sun is setting, good night !");
            IsNight = true;
            foreach (var p in ListPlayers)
            {
                foreach (var abil in p.Role.Abilities)
                {
                    if (abil.Priority == byte.MaxValue)
                    {
                        playerOrderLast.Add(p);
                    }
                    else
                    {
                        playerOrderFirst.Add((abil.Priority, p));
                    }
                }
            }
            var orderedList = new List<(int, Player)>();
            foreach (var order in playerOrderFirst)
            {

            }
            playerOrderFirst.Sort();

            foreach (var p in playerOrderFirst)
            {

            }
        }

        private void NightTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EndGame()
        {

        }

        #endregion

        #region InGameFunctions
        private bool IsGameFinished()
        {
            Alignment align = ListPlayers.ToList()[0].Role.Alignment;
            foreach (var p in ListPlayers)
            {
                if (p.IsAlive && align != p.Role.Alignment)
                {
                    return false;
                }
            }
            return true;
        }

        private void ResolveVotes()
        {
            IsVotingTime = false;
            var playersVoted = new Dictionary<Player, int>();
            int maxVotes = -1;
            foreach (var playerVotes in Votes)
            {
                if (playerVotes.Value == maxVotes)
                {
                    playersVoted.Add(playerVotes.Key, playerVotes.Value);
                }
                else if (playerVotes.Value > maxVotes)
                {
                    playersVoted.Clear();
                    playersVoted.Add(playerVotes.Key, playerVotes.Value);
                    maxVotes = playerVotes.Value;
                }
            }
            if (playersVoted.Count == 0)
            {
                App.currentServer.SendMessage("Noone has voted, no player will die");
            }
            else if (playersVoted.Count > 1)
            {
                EqualityResolution(playersVoted.Keys.ToList());
                App.currentServer.SendMessage("");
            }
            else
            {
                Player voted = playersVoted.ToList()[0].Key;
                voted.PerformAction(Action.Die);
                App.currentServer.SendMessage($"{voted.Name} is the player most voted, he die");
            }
        }

        private void EqualityResolution(List<Player> votedPlayers)
        {
            Player resolver = null;
            foreach (var p in ListPlayers)
            {
                if (p.Role.VotesResolveEquality)
                {
                    resolver = p;
                    break;
                }
            }

            if (resolver == null)
            {
                App.currentServer.SendMessage("Noone dies this turn (tie)");
                Night();
            }
            else
            {
                string message = "";
                foreach (var vp in votedPlayers)
                {
                    message += $"{vp.Name} and ";
                }
                message = message.Remove(message.Length - 4);
                message += "have been voted";
                App.currentServer.SendMessage(message);
                message = "Type vote ";
                foreach (var vp in votedPlayers)
                {
                    message += $"{vp.Name} or ";
                }
                message = message.Remove(message.Length - 4);
                message += $"To kill one of the {votedPlayers.Count} players that have been equally voted.";
                App.currentServer.SendMessage(message, resolver);
                var resolveEquality = new Timer(ResolveEqualityTime);
                resolveEquality.Elapsed += ResolveEquality_Elapsed;
                resolveEquality.Start();
            }
        }

        private void ResolveEquality_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.currentServer.AwaitMessage = Message.EqualityResolve;
            Night();
        }
        #endregion
        #endregion
    }
}
