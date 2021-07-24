using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace A2S
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Server
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private static readonly byte[] Request = { 0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00 };

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static ServerInfo Info { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Queries the server and returns server information in accordance with A2S_Info.
        /// </summary>
        /// <param name="address">IP address of the server in string format</param>
        /// <param name="port">Port of the server</param>
        /// <param name="timeout">Timeout in seconds</param>
        /// <returns></returns>
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public struct ServerInfo
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public ServerInfo(ref BinaryReader binReader)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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

            private byte Header { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public byte Protocol { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public string Name { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public string Map { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public string Folder { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public string Game { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public short Id { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public byte Players { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public byte MaxPlayers { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public byte Bots { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public ServerTypeFlags ServerType { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public EnvironmentFlags Environment { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public VisibilityFlags Visibility { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public VacFlags Vac { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public string Version { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public ExtraDataFlags ExtraDataFlag { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

            [Flags]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public enum ExtraDataFlags : byte
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                GameId = 0x01,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                SteamId = 0x10,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                Keywords = 0x20,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                Spectator = 0x40,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                Port = 0x80
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public enum VacFlags : byte
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                Unsecured = 0,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                Secured = 1
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public enum VisibilityFlags : byte
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                Public = 0,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                Private = 1
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public enum EnvironmentFlags : byte
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                Linux = 0x6C,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                Windows = 0x77,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                Mac = 0x6D,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                MacOsX = 0x6F
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public enum ServerTypeFlags : byte
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                Dedicated = 0x64,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                NonDedicated = 0x6C,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                SourceTv = 0x70
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public ulong GameId { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public ulong SteamId { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public string Keywords { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public string Spectator { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public short SpectatorPort { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public short Port { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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
