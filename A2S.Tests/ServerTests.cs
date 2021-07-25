using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace A2S.Tests
{
    [TestClass()]
    public class ServerTests
    {
        [TestMethod()]
        public void QueryTest()
        {
            var serverDict = new Dictionary<string, int>
            {
                {"5.101.166.197", 27021},
                {"5.101.166.199", 27021}
            };

            foreach (var server in serverDict.Select(kvP => Server.Query(kvP.Key, kvP.Value, 5)))
            {
                if (server is not Exception)
                {
                    var response = $@"
                Protocol: {server.Protocol}
                Name: {server.Name}
                Map: {server.Map}
                Folder: {server.Folder}
                Game: {server.Game}
                ID: {server.Id}
                Players: {server.Players}
                Max Players: {server.MaxPlayers}
                Bots: {server.Bots}
                Server Type: {server.ServerType}
                Environment: {server.Environment}
                Visibility: {server.Visibility}
                VAC: {server.Vac}
                Version: {server.Version}
                ExtraDataFlags:
                    Port: {server.Port}
                    SteamId: {server.SteamId}
                    Spectator:
                        Name: {server.Spectator}
                        Port: {server.SpectatorPort}
                    Keywords: {server.Keywords}
                    GameId: {server.GameId}";

                    Console.WriteLine($"Test #1:\n\t{response}");
                }
                else
                {
                    Console.WriteLine($"Test #2:\n\t{server}");
                }
            }
        }
    }
}