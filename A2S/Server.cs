using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace A2S
{
    public class Server
    {
        private static readonly byte[] Request = { 0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00 };

        public static ServerInfo Info { get; set; }

        public static dynamic Query(string address, int port, int timeout)
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(address), Convert.ToUInt16(port));
            using var udpClient = new UdpClient();
            udpClient.Send(Request, Request.Length, endPoint);
            var asyncResult = udpClient.BeginReceive(null, null);
            asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeout));
            if (asyncResult.IsCompleted)
            {
                try
                {
                    var receivedData = udpClient.EndReceive(asyncResult, ref endPoint);
                    var challengeResponse = receivedData;
                    if (challengeResponse.Length == 25 && challengeResponse[4] == 0x41)
                    {
                        challengeResponse[4] = 0x54;
                        udpClient.Send(challengeResponse, challengeResponse.Length, endPoint);
                        var receivedAsync = udpClient.BeginReceive(null, null);
                        if (receivedAsync.IsCompleted)
                        {
                            receivedData = udpClient.EndReceive(receivedAsync, ref endPoint);
                        }
                    }
                    var memSteam = new MemoryStream(receivedData);
                    var binReader = new BinaryReader(memSteam, Encoding.UTF8);
                    memSteam.Seek(4, SeekOrigin.Begin);
                    Info = new ServerInfo(ref binReader);
                    return Info;
                }
                catch (Exception)
                {
                    udpClient.Close();
                    return null;
                }
            }

            udpClient.Close();
            return new TimeoutException("Server failed to respond in the time allotted");
        }

        public struct ServerInfo
        {
            public ServerInfo(ref BinaryReader binReader)
            {
                Header = binReader.ReadByte();
                Protocol = binReader.ReadByte();
                Name = ReadNullTerminatedString(ref binReader);
                Map = ReadNullTerminatedString(ref binReader);
                Folder = ReadNullTerminatedString(ref binReader);
                Game = ReadNullTerminatedString(ref binReader);
                Id = binReader.ReadInt16();
                Players = binReader.ReadByte();
                MaxPlayers = binReader.ReadByte();
                Bots = binReader.ReadByte();
                ServerType = (ServerTypeFlags)binReader.ReadByte();
                Environment = (EnvironmentFlags)binReader.ReadByte();
                Visibility = (VisibilityFlags)binReader.ReadByte();
                Vac = (VacFlags)binReader.ReadByte();
                Version = ReadNullTerminatedString(ref binReader);
                ExtraDataFlag = (ExtraDataFlags)binReader.ReadByte();
                GameId = 0;
                SteamId = 0;
                Keywords = null;
                Spectator = null;
                SpectatorPort = 0;
                Port = 0;
                if (ExtraDataFlag.HasFlag(ExtraDataFlags.Port))
                {
                    Port = binReader.ReadInt16();
                }
                

                if (ExtraDataFlag.HasFlag(ExtraDataFlags.SteamId))
                {
                    SteamId = binReader.ReadUInt64();
                }

                if (ExtraDataFlag.HasFlag(ExtraDataFlags.Spectator))
                {
                    SpectatorPort = binReader.ReadInt16();
                    Spectator = ReadNullTerminatedString(ref binReader);
                }

                if (ExtraDataFlag.HasFlag(ExtraDataFlags.Keywords))
                {
                    Keywords = ReadNullTerminatedString(ref binReader);
                }

                if (ExtraDataFlag.HasFlag(ExtraDataFlags.GameId))
                {
                    GameId = binReader.ReadUInt64();
                }
            }

            public byte Header { get; set; }
            public byte Protocol { get; set; }
            public string Name { get; set; }
            public string Map { get; set; }
            public string Folder { get; set; }
            public string Game { get; set; }
            public short Id { get; set; }
            public byte Players { get; set; }
            public byte MaxPlayers { get; set; }
            public byte Bots { get; set; }
            public ServerTypeFlags ServerType { get; set; }
            public EnvironmentFlags Environment { get; set; }
            public VisibilityFlags Visibility { get; set; }
            public VacFlags Vac { get; set; }
            public string Version { get; set; }
            public ExtraDataFlags ExtraDataFlag { get; set; }

            [Flags]
            public enum ExtraDataFlags : byte
            {
                GameId = 0x01,
                SteamId = 0x10,
                Keywords = 0x20,
                Spectator = 0x40,
                Port = 0x80
            }

            public enum VacFlags : byte
            {
                Unsecured = 0,
                Secured = 1
            }

            public enum VisibilityFlags : byte
            {
                Public = 0,
                Private = 1
            }

            public enum EnvironmentFlags : byte
            {
                Linux = 0x6C,
                Windows = 0x77,
                Mac = 0x6D,
                MacOsX = 0x6F
            }

            public enum ServerTypeFlags : byte
            {
                Dedicated = 0x64,
                Nondedicated = 0x6C,
                SourceTv = 0x70
            }

            public ulong GameId { get; set; }
            public ulong SteamId { get; set; }
            public string Keywords { get; set; }
            public string Spectator { get; set; }
            public short SpectatorPort { get; set; }
            public short Port { get; set; }
        }

        private static string ReadNullTerminatedString(ref BinaryReader input)
        {
            var sb = new StringBuilder();
            var reader = input.ReadChar();
            while (reader != '\x00')
            {
                sb.Append(reader);
                reader = input.ReadChar();
            }

            return sb.ToString();
        }
    }
}
