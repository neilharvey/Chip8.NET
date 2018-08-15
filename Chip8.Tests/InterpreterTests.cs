using System;
using System.Linq;
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

            [Fact]
            public void Return()
            {
                chip8.Memory[0x200] = 0x24;
                chip8.Memory[0x201] = 0x00;
                chip8.Tick();
                chip8.Memory[0x400] = 0x00;
                chip8.Memory[0x401] = 0xEE;

                chip8.Tick();

                Assert.Equal(0, chip8.StackPointer);
                Assert.Equal(0x202, chip8.ProgramCounter);
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
            public void LoadRegisterFromData()
            {
                chip8.Memory[0x200] = 0x61;
                chip8.Memory[0x201] = 0x99;

                chip8.Tick();

                Assert.Equal(0x99, chip8.V[1]);
            }

            [Fact]
            public void AddDataToRegister()
            {
                chip8.V[1] = 0x15;
                chip8.Memory[0x200] = 0x71;
                chip8.Memory[0x201] = 0x10;

                chip8.Tick();

                Assert.Equal(0x25, chip8.V[1]);
            }

            [Fact]
            public void LoadRegisterFromRegister()
            {
                chip8.V[1] = 0x11;
                chip8.V[2] = 0x22;
                chip8.Memory[0x200] = 0x81;
                chip8.Memory[0x201] = 0x20;

                chip8.Tick();

                Assert.Equal(0x22, chip8.V[1]);
            }

            [Fact]
            public void BitwiseOr()
            {
                chip8.V[1] = 0b11110000;
                chip8.V[2] = 0b00111100;

                chip8.Memory[0x200] = 0x81;
                chip8.Memory[0x201] = 0x21;

                chip8.Tick();

                Assert.Equal(0b11111100, chip8.V[1]);
            }

            [Fact]
            public void BitwiseAnd()
            {
                chip8.V[1] = 0b11110000;
                chip8.V[2] = 0b00111100;

                chip8.Memory[0x200] = 0x81;
                chip8.Memory[0x201] = 0x22;

                chip8.Tick();

                Assert.Equal(0b00110000, chip8.V[1]);
            }

            [Fact]
            public void BitwiseXor()
            {
                chip8.V[1] = 0b11110000;
                chip8.V[2] = 0b00111100;
                chip8.Memory[0x200] = 0x81;
                chip8.Memory[0x201] = 0x23;

                chip8.Tick();

                Assert.Equal(0b11001100, chip8.V[1]);
            }

            [Theory]
            [InlineData(100, 100, 200, 0)]
            [InlineData(100, 200, 44, 1)]
            public void AddRegister(byte v1, byte v2, byte expectedV1, byte expectedVf)
            {
                chip8.V[1] = v1;
                chip8.V[2] = v2;
                chip8.Memory[0x200] = 0x81;
                chip8.Memory[0x201] = 0x24;

                chip8.Tick();

                Assert.Equal(expectedV1, chip8.V[1]);
                Assert.Equal(expectedVf, chip8.V[0xF]);
            }

            [Theory]
            [InlineData(200, 100, 100, 1)]
            [InlineData(100, 200, 100, 0)]
            public void SubtractRegister(byte v1, byte v2, byte expectedV1, byte expectedVf)
            {
                chip8.V[1] = v1;
                chip8.V[2] = v2;
                chip8.Memory[0x200] = 0x81;
                chip8.Memory[0x201] = 0x25;

                chip8.Tick();

                Assert.Equal(expectedV1, chip8.V[1]);
                Assert.Equal(expectedVf, chip8.V[0xF]);
            }

            [Theory]
            [InlineData(200, 100, 100, 0)]
            [InlineData(100, 200, 100, 1)]
            public void SubstractRegisterInverse(byte v1, byte v2, byte expectedV1, byte expectedVf)
            {
                chip8.V[1] = v1;
                chip8.V[2] = v2;
                chip8.Memory[0x200] = 0x81;
                chip8.Memory[0x201] = 0x27;

                chip8.Tick();

                Assert.Equal(expectedV1, chip8.V[1]);
                Assert.Equal(expectedVf, chip8.V[0xF]);
            }

            [Theory]
            [InlineData(255, 0x000, 127, 1)]
            [InlineData(254, 0x000, 127, 0)]
            public void ShiftRight(byte v1, byte v2, byte expectedV1, byte expectedVf)
            {
                chip8.V[1] = v1;
                chip8.V[2] = v2;
                chip8.Memory[0x200] = 0x81;
                chip8.Memory[0x201] = 0x26;

                chip8.Tick();

                Assert.Equal(expectedV1, chip8.V[1]);
                Assert.Equal(expectedVf, chip8.V[0xF]);
            }

            [Theory]
            [InlineData(254, 0x000, 127, 1)]
            [InlineData(127, 0x000, 254, 0)]
            public void ShiftLeft(byte v1, byte v2, byte expectedV1, byte expectedVf)
            {
                chip8.V[1] = v1;
                chip8.V[2] = v2;
                chip8.Memory[0x200] = 0x81;
                chip8.Memory[0x201] = 0x2E;

                chip8.Tick();

                Assert.Equal(expectedV1, chip8.V[1]);
                Assert.Equal(expectedVf, chip8.V[0xF]);
            }

            [Theory]
            [InlineData(0, 1, 0x204)]
            [InlineData(1, 1, 0x202)]
            public void SkipIfRegistersNotEqual(byte v1, byte v2, byte expectedProgramCounter)
            {
                chip8.V[1] = v1;
                chip8.V[2] = v2;
                chip8.Memory[0x200] = 0x91;
                chip8.Memory[0x201] = 0x20;

                chip8.Tick();

                Assert.Equal(expectedProgramCounter, chip8.ProgramCounter);
            }

            [Fact]
            public void LoadAddressFromData()
            {
                chip8.Memory[0x200] = 0xA1;
                chip8.Memory[0x201] = 0x24;

                chip8.Tick();

                Assert.Equal(0x124, chip8.I);
            }

            [Fact]
            public void JumpOffset()
            {
                chip8.V[0] = 0xFE;
                chip8.Memory[0x200] = 0xB1;
                chip8.Memory[0x201] = 0x24;

                chip8.Tick();

                Assert.Equal(0xFE + 0x124, chip8.ProgramCounter);
            }

            [Fact(Skip = "NYI")]
            public void Random()
            {
                throw new NotImplementedException();
            }

            [Fact(Skip = "NYI")]
            public void DrawSprite()
            {
                throw new NotImplementedException();
            }

            [Theory]
            [InlineData(false, 0x202)]
            [InlineData(true, 0x204)]
            public void SkipIfKeyDown(bool keyPressed, ushort expectedProgramCounter)
            {
                chip8.V[1] = 0xF;
                chip8.Keyboard[0xF] = keyPressed;
                chip8.Memory[0x200] = 0xE1;
                chip8.Memory[0x201] = 0x9E;

                chip8.Tick();

                Assert.Equal(expectedProgramCounter, chip8.ProgramCounter);
            }

            [Theory]
            [InlineData(true, 0x202)]
            [InlineData(false, 0x204)]
            public void SkipIfKeyUp(bool keyPressed, ushort expectedProgramCounter)
            {
                chip8.V[1] = 0xF;
                chip8.Keyboard[0xF] = keyPressed;
                chip8.Memory[0x200] = 0xE1;
                chip8.Memory[0x201] = 0xAE;

                chip8.Tick();

                Assert.Equal(expectedProgramCounter, chip8.ProgramCounter);
            }

            [Fact]
            public void LoadRegisterFromDelay()
            {
                chip8.Delay = 0xA;
                chip8.Memory[0x200] = 0xF1;
                chip8.Memory[0x201] = 0x07;

                chip8.Tick();

                Assert.Equal(0x0A, chip8.V[1]);
            }

            [Fact(Skip = "NYI")]
            public void LoadRegisterFromKeyDown()
            {
                // Not sure how to pause until the key is pressed.
            }

            [Fact]
            public void LoadDelayFromRegister()
            {
                chip8.V[1] = 0x0F;
                chip8.Memory[0x200] = 0xF1;
                chip8.Memory[0x201] = 0x15;

                chip8.Tick();

                Assert.Equal(0x0F, chip8.Delay);
            }

            [Fact(Skip = "NYI")]
            public void LoadSoundFromRegister()
            {
                chip8.V[1] = 0x0A;
                chip8.Memory[0x200] = 0xF1;
                chip8.Memory[0x201] = 0x18;

                chip8.Tick();

                Assert.Equal(0x0A, chip8.Sound);
            }

            [Fact]
            public void AddRegisterToAddress()
            {
                chip8.V[1] = 5;
                chip8.I = 10;
                chip8.Memory[0x200] = 0xF1;
                chip8.Memory[0x201] = 0x1E;

                chip8.Tick();

                Assert.Equal(15, chip8.I);
            }

            [Fact(Skip = "NYI")]
            public void LoadAddressFromSprite()
            {
                throw new NotImplementedException();
            }

            [Fact]
            public void LoadBcdFromRegister()
            {
                chip8.V[1] = 123;
                chip8.I = 0x400;
                chip8.Memory[0x200] = 0xF1;
                chip8.Memory[0x201] = 0x33;

                chip8.Tick();

                Assert.Equal(1, chip8.Memory[0x400]);
                Assert.Equal(2, chip8.Memory[0x401]);
                Assert.Equal(3, chip8.Memory[0x402]);
            }

            [Fact]
            public void LoadMemoryFromRegisters()
            {
                chip8.I = 0x400;
                chip8.V[0] = 1;
                chip8.V[1] = 1;
                chip8.V[2] = 2;
                chip8.V[3] = 3;
                chip8.V[4] = 5;
                chip8.Memory[0x200] = 0xF4;
                chip8.Memory[0x201] = 0x55;

                chip8.Tick();

                Assert.Equal(1, chip8.Memory[0x400]);
                Assert.Equal(1, chip8.Memory[0x401]);
                Assert.Equal(2, chip8.Memory[0x402]);
                Assert.Equal(3, chip8.Memory[0x403]);
                Assert.Equal(5, chip8.Memory[0x404]);
            }

            [Fact]
            public void LoadRegistersFromMemory()
            {
                chip8.I = 0x400;
                chip8.V[0x400] = 15;
                chip8.V[0x401] = 10;
                chip8.V[0x402] = 6;
                chip8.V[0x403] = 3;
                chip8.V[0x404] = 1;
                chip8.Memory[0x200] = 0xF4;
                chip8.Memory[0x201] = 0x65;

                chip8.Tick();

                Assert.Equal(15, chip8.V[0]);
                Assert.Equal(10, chip8.V[1]);
                Assert.Equal(6, chip8.V[2]);
                Assert.Equal(3, chip8.V[3]);
                Assert.Equal(1, chip8.V[4]);
            }
        }
    }
}
