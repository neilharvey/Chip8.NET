using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Chip8.Tests
{
    public class InterpreterTests
    {
        public class Initialization
        {
            [Fact]
            public void FontsetIsStoredInInterpreterMemory()
            {
                var chip8 = new Interpreter();

                var actual = chip8.Memory.Take(Interpreter.Fontset.Length).ToArray();

                Assert.Equal(Interpreter.Fontset, actual);
            }
        }

        public class OpCodes
        {
            private Interpreter chip8;

            public OpCodes()
            {
                chip8 = new Interpreter();
            }

            [Fact]
            public void ClearScreen()
            {
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
                chip8.Memory[0x200] = 0x12;
                chip8.Memory[0x201] = 0x34;

                chip8.Tick();

                Assert.Equal(0x234, chip8.ProgramCounter);
            }

            [Fact]
            public void Call()
            {
                chip8.Memory[0x200] = 0x24;
                chip8.Memory[0x201] = 0x68;

                chip8.Tick();

                Assert.Equal(0x200, chip8.Stack[0]);
                Assert.Equal(1, chip8.StackPointer);
                Assert.Equal(0x468, chip8.ProgramCounter);
            }

            [Theory]
            [InlineData(0x31, 0x99, 0x99, 0x204)]
            [InlineData(0x31, 0x99, 0x50, 0x202)]
            public void SkipIfEqual(byte high, byte low, byte vx, ushort expected)
            {
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
                chip8.V[2] = vx;
                chip8.Memory[0x200] = high;
                chip8.Memory[0x201] = low;

                chip8.Tick();

                Assert.Equal(expected, chip8.ProgramCounter);
            }

            [Fact]
            public void SkipIfRegistersEqual()
            {
                chip8.V[1] = 0x77;
                chip8.V[2] = 0x77;
                chip8.Memory[0x200] = 0x51;
                chip8.Memory[0x201] = 0x20;

                chip8.Tick();

                Assert.Equal(0x204, chip8.ProgramCounter);
            }

            [Fact]
            public void LoadData()
            {
                chip8.Memory[0x200] = 0x61;
                chip8.Memory[0x201] = 0x99;

                chip8.Tick();

                Assert.Equal(0x99, chip8.V[1]);
            }

            [Fact]
            public void AddData()
            {
                chip8.V[1] = 0x15;
                chip8.Memory[0x200] = 0x71;
                chip8.Memory[0x201] = 0x10;

                chip8.Tick();

                Assert.Equal(0x25, chip8.V[1]);
            }
        }
    }
}
