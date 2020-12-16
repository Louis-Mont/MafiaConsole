using Mafia.Core.Network;
using Mafia.Core.Roles;
using Mafia.Core.App;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.Core.Test
{
    public class Tests
    {
        Game game;

        HashSet<Player> ListPlayers;

        Player caster;
        Player target;

        [SetUp]
        public void Setup()
        {
            caster = new Player("PC");
            target = new Player("PT");
            game = new Game(new HashSet<Player>(){
                {caster},{target} });
        }

        [Test]
        public void TestInvestigator()
        {
            caster.Role = Role.INVESTIGATOR;
            target.Role = Role.TOWNSFOLK;
            Assert.AreEqual(caster.Role.Abilities.ToList()[0].Exec(caster, target), "PT has not commited anything tonight");
        }

        [Test]
        public void TestSheriff()
        {
            caster.Role = Role.SHERIFF;
            target.Role = Role.TOWNSFOLK;
            Assert.AreEqual(caster.Role.Abilities.ToList()[0].Exec(caster, target), "No Crime");
        }

        /*[Test]
        public void TestLists()
        {
            var playerOrderFirst = new List<int>();
            playerOrderFirst.Add(1);
            
            playerOrderFirst.Insert(playerOrderFirst.IndexOf(1), 2);
            playerOrderFirst.Insert(playerOrderFirst.IndexOf(1), 3);
            playerOrderFirst.Add(4);
            playerOrderFirst.Insert(playerOrderFirst.IndexOf(2), 5);
            playerOrderFirst.Add(6);

        }*/
    }
}