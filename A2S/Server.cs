using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using A2S.Structs;

namespace A2S
{
    public class Server
    {
        private static readonly byte[] Request = { 0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00 };

        private static ServerInfo Info { get; set; }

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
    }
}
