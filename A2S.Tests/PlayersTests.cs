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
            var index = 0;
            var serverDict = new Dictionary<string, int>
            {
                {"46.251.238.194", 27023},
                {"46.251.235.194", 27021}
            };
            
            foreach (var serverPlayers in serverDict.Select(kvP => Players.Query(kvP.Key, kvP.Value, 5)))
            {
                index++;
                var response = string.Empty;
                if (serverPlayers is not Exception)
                {
                    foreach (var player in serverPlayers)
                    {
                        if (!string.IsNullOrWhiteSpace(player.Name))
                            response += $"[Name] {player.Name}\n\t[Score] {player.Score}\n\t[Duration] {player.Duration}\r\n\t\r\n\t";
                    }
                }
                else
                {
                    response = serverPlayers.ToString();
                }

                Console.WriteLine($"Test {index}:\n\t{response}");
            }
        }
    }
}