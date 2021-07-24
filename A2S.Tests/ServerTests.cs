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
            Console.WriteLine(info.Name);
        }

        [TestMethod()]
        public void QueryTestTimeout()
        {
            var info = Server.Query("5.101.166.199", 27021, 15);
            Console.WriteLine(info);
        }
    }
}