using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace A2S.Tests
{
    [TestClass()]
    public class ServerTests
    {
        [TestMethod()]
        public void QueryTest()
        {
            var info = Server.Query("5.101.166.197", 27021, 15);
            var response = $@"
                Protocol: {info.Protocol}
                Name: {info.Name}
                Map: {info.Map}
                Folder: {info.Folder}
                Game: {info.Game}
                ID: {info.Id}
                Players: {info.Players}
                Max Players: {info.MaxPlayers}
                Bots: {info.Bots}
                Server Type: {info.ServerType}
                Environment: {info.Environment}
                Visibility: {info.Visibility}
                VAC: {info.Vac}
                Version: {info.Version}
                ExtraDataFlags:
                    Port: {info.Port}
                    SteamId: {info.SteamId}
                    Spectator:
                        Name: {info.Spectator}
                        Port: {info.SpectatorPort}
                    Keywords: {info.Keywords}
                    GameId: {info.GameId}";
            Console.WriteLine(response);
        }

        [TestMethod()]
        public void QueryTestTimeout()
        {
            var info = Server.Query("5.101.166.199", 27021, 15);
            Console.WriteLine(info);
        }
    }
}