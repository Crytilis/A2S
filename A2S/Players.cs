using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace A2S
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Players
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private static readonly byte[] Request = { 0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0xFF, 0xFF, 0xFF, 0xFF };

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static Player[] PlayersArray;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private static byte Header { get; set; }

        /// <summary>
        /// Queries the server and requests information on all connected players in accordance with A2S_Player.
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
                    byte[] challengeResponse = udpClient.EndReceive(asyncResult, ref endPoint);
                    if (challengeResponse.Length == 9 && challengeResponse[4] == 0x41)
                    {
                        challengeResponse[4] = 0x55;
                        udpClient.Send(challengeResponse, challengeResponse.Length, endPoint);
                        var ms = new MemoryStream(udpClient.Receive(ref endPoint));
                        var br = new BinaryReader(ms, Encoding.UTF8);
                        ms.Seek(4, SeekOrigin.Begin);
                        Header = br.ReadByte();
                        PlayersArray = new Player[br.ReadByte()];
                        for (var i = 0; i < PlayersArray.Length; i++)
                        {
                            PlayersArray[i] = new Player(ref br);
                        }

                        br.Close();
                        ms.Close();
                    }
                }
                catch (Exception)
                {
                    udpClient.Close();
                    return null;
                }
            }
            else
            {
                udpClient.Close();
                return new TimeoutException("Server failed to respond in the time allotted");
            }
            udpClient.Close();
            return PlayersArray;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public struct Player
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public Player(ref BinaryReader binReader)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            {
                Index = binReader.ReadByte();
                Name = ReadNullTerminatedString(ref binReader);
                Score = binReader.ReadInt32();
                Duration = binReader.ReadSingle();
            }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public byte Index { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public string Name { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public int Score { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public float Duration { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public override string ToString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            {
                return Name;
            }
        }

        private static string ReadNullTerminatedString(ref BinaryReader input)
        {
            var sb = new StringBuilder();
            var read = input.ReadChar();
            while (read != '\x00')
            {
                sb.Append(read);
                read = input.ReadChar();
            }

            return sb.ToString();
        }
    }
}
