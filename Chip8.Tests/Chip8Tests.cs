using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Chip8.Tests
{
    public class Chip8Tests
    {
        public class Initialization
        {
            [Fact]
            public void FontsetIsStoredInInterpreterMemory()
            {
                var chip8 = new Chip8();

                var actual = chip8.Memory.Take(Chip8.Fontset.Length).ToArray();

                Assert.Equal(Chip8.Fontset, actual);
            }
        }

        public class OpCodes
        {
            [Fact]
            public void CLS()
            {
                var chip8 = new Chip8();
                chip8.Display[0, 0] = true;

                chip8.Memory[0x200] = 0x00;
                chip8.Memory[0x201] = 0xE0;
                chip8.Tick();

                Assert.False(chip8.Display[0, 0]);
            }

            [Fact(Skip = "Stack NYI")]
            public void RET()
            {

            }

            [Fact]
            public void JP()
            {
                var chip8 = new Chip8();

                chip8.Memory[0x200] = 0x12;
                chip8.Memory[0x201] = 0x34;
                chip8.Tick();

                Assert.Equal(0x234, chip8.ProgramCounter);
            }

            [Fact]
            public void CALL()
            {
                var chip8 = new Chip8();
                var pc = chip8.ProgramCounter;

                chip8.Memory[0x200] = 0x24;
                chip8.Memory[0x201] = 0x68;
                chip8.Tick();

                Assert.Equal(pc, chip8.Stack[0]);
                Assert.Equal(1, chip8.StackPointer);
                Assert.Equal(0x468, chip8.ProgramCounter);
            }

            [Fact(Skip = "NYI")]
            public void SE()
            {
            }
        }
    }
}
