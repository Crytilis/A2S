using Microsoft.VisualStudio.TestTools.UnitTesting;
using A2S;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2S.Tests
{
    [TestClass()]
    public class PlayersTests
    {
        [TestMethod()]
        public void QueryTest()
        {
            var players = Players.Query("46.251.238.194", 27023, 5);
            foreach (var player in players)
            {
                if (player.Name != string.Empty)
                    Console.WriteLine($"[Name] {player.Name}\n\t[Score] {player.Score}\n\t[Duration] {player.Duration}");
            }
        }

        [TestMethod()]
        public void QueryTestTimeout()
        {
            var players = Players.Query("5.101.166.199", 27021, 5);
            Console.WriteLine(players);
        }
    }
}