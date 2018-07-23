using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Chip8.Tests
{
    public class ChipTests
    {
        public class Initialization
        {
            [Fact]
            public void FontsetIsStoredInInterpreterMemory()
            {
                var chip8 = new Chip();

                var actual = chip8.Memory.Take(Chip.Fontset.Length).ToArray();

                Assert.Equal(Chip.Fontset, actual);
            }
        }

        public class OpCodes
        {
            [Fact]
            public void ClearScreen()
            {
                var chip8 = new Chip();
                chip8.Display[0, 0] = true;
                chip8.Memory[0x200] = 0x00;
                chip8.Memory[0x201] = 0xE0;

                chip8.Tick();

                Assert.False(chip8.Display[0, 0]);
            }

            [Fact(Skip = "Stack NYI")]
            public void Return()
            {

            }

            [Fact]
            public void Jump()
            {
                var chip8 = new Chip();
                chip8.Memory[0x200] = 0x12;
                chip8.Memory[0x201] = 0x34;

                chip8.Tick();

                Assert.Equal(0x234, chip8.ProgramCounter);
            }

            [Fact]
            public void Call()
            {
                var chip8 = new Chip();
                var pc = chip8.ProgramCounter;
                chip8.Memory[0x200] = 0x24;
                chip8.Memory[0x201] = 0x68;

                chip8.Tick();

                Assert.Equal(pc, chip8.Stack[0]);
                Assert.Equal(1, chip8.StackPointer);
                Assert.Equal(0x468, chip8.ProgramCounter);
            }

            [Theory]
            [InlineData(0x31, 0x99, 0x99, 0x204)]
            [InlineData(0x31, 0x99, 0x50, 0x202)]
            public void SkipIfEqual(byte high, byte low, byte vx, ushort expected)
            {
                var chip8 = new Chip();
                chip8.V[1] = vx;
                chip8.Memory[0x200] = high;
                chip8.Memory[0x201] = low;

                chip8.Tick();

                Assert.Equal(expected, chip8.ProgramCounter);
            }

            [Theory]
            [InlineData(0x42, 0x77, 0x80, 0x204)]
            [InlineData(0x42, 0x77, 0x77, 0x202)]
            public void SkipIfNotEqual(byte high, byte low, byte vx, ushort expected)
            {
                var chip8 = new Chip();
                chip8.V[2] = vx;
                chip8.Memory[0x200] = high;
                chip8.Memory[0x201] = low;

                chip8.Tick();

                Assert.Equal(expected, chip8.ProgramCounter);
            }

            [Fact]
            public void SkipIfRegistersEqual()
            {
                var chip8 = new Chip();
                chip8.V[1] = 0x77;
                chip8.V[2] = 0x77;
                chip8.Memory[0x200] = 0x51;
                chip8.Memory[0x201] = 0x20;

                chip8.Tick();

                Assert.Equal(0x204, chip8.ProgramCounter);
            }
        }
    }
}
