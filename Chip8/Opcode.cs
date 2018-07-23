using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8
{
    public class Opcode
    {
        public Opcode(byte high, byte low)
        {
            Value = (ushort)(high << 8 | low);
            X = high & 0x0F;
            Y = (low & 0xF0) >> 4;
            Kk = low;
            N = low & 0x0F;
            Nnn = (ushort)(Value & 0x0FFF);
        }

        public ushort Value { get; }

        public ushort Nnn { get; }

        public int X { get; }

        public int Y { get; }

        public int N { get; }

        public byte Kk { get; }
    }
}
