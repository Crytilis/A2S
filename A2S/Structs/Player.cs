using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A2S.Utility;

namespace A2S.Structs
{
    public struct Player
    {
        public Player(ref BinaryReader binReader)
        {
            Index = binReader.ReadByte();
            Name = Helpers.ReadNullTerminatedString(ref binReader);
            Score = binReader.ReadInt32();
            Duration = binReader.ReadSingle();
        }

        public byte Index { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public float Duration { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
