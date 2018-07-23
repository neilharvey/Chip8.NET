using Xunit;

namespace Chip8.Tests
{
    public class OpcodeTests
    {
        [Fact]
        public void NnnIsLowest12Bits()
        {
            var opcode = new Opcode(0x24, 0x68);

            Assert.Equal(0x468, opcode.Nnn);
        }

        [Fact]
        public void NIsLowest4Bits()
        {
            var opcode = new Opcode(0xD1, 0x24);

            Assert.Equal(4, opcode.N);
        }

        [Fact]
        public void XIsLower4BitsOfHighByte()
        {
            var opcode = new Opcode(0x31, 0x99);

            Assert.Equal(1, opcode.X);
        }

        [Fact]
        public void YIsUpper4BitsOfLowByte()
        {
            var opcode = new Opcode(0x51, 0x20);

            Assert.Equal(2, opcode.Y);
        }

        [Fact]
        public void KkIsLowest8Bits()
        {
            var opcode = new Opcode(0x71, 0x77);

            Assert.Equal(0x77, opcode.Kk);
        }
    }
}
