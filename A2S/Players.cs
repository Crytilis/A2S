using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using A2S.Structs;

namespace A2S
{
    public class Players
    {
        private static readonly byte[] Request = { 0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0xFF, 0xFF, 0xFF, 0xFF };
        private static Player[] _playersArray;

        private static byte Header { get; set; }

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
                        _playersArray = new Player[br.ReadByte()];
                        for (var i = 0; i < _playersArray.Length; i++)
                        {
                            _playersArray[i] = new Player(ref br);
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
            return _playersArray.ToList();
        }

        
    }
}
