using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2S.Utility
{
    internal static class Helpers
    {
        public static string ReadNullTerminatedString(ref BinaryReader input)
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
