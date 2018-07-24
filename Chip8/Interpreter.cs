using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Chip8
{
    // http://devernay.free.fr/hacks/chip8/C8TECH10.HTM
    public class Interpreter
    {
        public const int ScreenWidth = 64;
        public const int ScreenHeight = 32;

        public static readonly byte[] Fontset =
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        public Interpreter()
        {
            Memory = new byte[4096];
            V = new byte[16];
            I = 0;
            Delay = 0;
            Sound = 0;
            ProgramCounter = 0x200;
            StackPointer = 0;
            Stack = new ushort[16];
            Keyboard = new bool[16];
            Display = new bool[ScreenWidth, ScreenHeight];
            LoadFontset();
        }

        public byte[] Memory { get; private set; }

        public byte[] V { get; private set; }

        public ushort I { get; private set; }

        public byte Delay { get; private set; }

        public byte Sound { get; private set; }

        public ushort ProgramCounter { get; private set; }

        public byte StackPointer { get; private set; }

        public ushort[] Stack { get; private set; }

        public bool[] Keyboard { get; private set; }

        public bool[,] Display { get; private set; }

        public void LoadApplication(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.Read(Memory, 0x200, (int)stream.Length);
            }
        }

        public void Tick()
        {
            var opcode = new Opcode(Memory[ProgramCounter], Memory[ProgramCounter + 1]);

            switch (opcode.Value & 0xF000)
            {
                case 0x0000:
                    ClearScreen();
                    break;
                case 0x1000:
                    Jump(opcode);
                    break;
                case 0x2000:
                    Call(opcode);
                    break;
                case 0x3000:
                    SkipIfEqual(opcode);
                    break;
                case 0x4000:
                    SkipIfNotEqual(opcode);
                    break;
                case 0x5000:
                    SkipIfRegistersEqual(opcode);
                    break;
                case 0x6000:
                    LoadRegisterFromData(opcode);
                    break;
                case 0x7000:
                    AddDataToRegister(opcode);
                    break;
                case 0x8000:
                    switch(opcode.N)
                    {
                        case 0x0:
                            LoadRegisterFromRegister(opcode);
                            break;
                        case 0x1:
                            BitwiseOr(opcode);
                            break;
                        case 0x2:
                            BitwiseAnd(opcode);
                            break;
                        case 0x3:
                            BitwiseXor(opcode);
                            break;
                        case 0x4:
                            AddRegister(opcode);
                            break;
                        case 0x5:
                            SubtractRegister(opcode);
                            break;
                        case 0x6:
                            Shr(opcode);
                            break;
                        case 0x7:
                            Subn(opcode);
                            break;
                        case 0xE:
                            Shl(opcode);
                            break;
                    }
                    break;
                case 0x9000:
                    SkipIfRegistersNotEqual(opcode);
                    break;
                case 0xA000:
                    LoadAddressFromData(opcode);
                    break;
                case 0xB000:
                    JumpOffset(opcode);
                    break;
                case 0xC000:
                    Random(opcode);
                    break;
                case 0xD000:
                    DrawSprite(opcode);
                    break;
                case 0xE000:
                    switch (opcode.Kk)
                    {
                        case 0x9E:
                            SkipIfKeyDown(opcode);
                            break;
                        case 0xA1:
                            SkipIfKeyUp(opcode);
                            break;
                    }
                    break;
                case 0xF000:
                    switch (opcode.Kk)
                    {
                        case 0x07:
                            LoadRegisterFromDelay(opcode);
                            break;
                        case 0x0A:
                            LoadRegisterFromKeyDown(opcode);
                            break;
                        case 0x12:
                            LoadDelayFromRegister(opcode);
                            break;
                        case 0x18:
                            LoadSoundFromRegister(opcode);
                            break;
                        case 0x1E:
                            AddRegisterToAddress(opcode);
                            break;
                        case 0x29:
                            LoadAddressFromSprite(opcode);
                            break;
                        case 0x33:
                            LoadBcdFromRegister(opcode);
                            break;
                        case 0x55:
                            LoadMemoryFromRegisters(opcode);
                            break;
                        case 0x65:
                            LoadRegistersFromMemory(opcode);
                            break;
                    }

                    break;


            }
        }

        private void LoadRegistersFromMemory(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void LoadMemoryFromRegisters(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void LoadBcdFromRegister(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void LoadAddressFromSprite(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void AddRegisterToAddress(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void LoadSoundFromRegister(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void LoadDelayFromRegister(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void LoadRegisterFromKeyDown(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void LoadRegisterFromDelay(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void SkipIfKeyUp(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void SkipIfKeyDown(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void DrawSprite(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void Random(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void JumpOffset(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void LoadAddressFromData(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void SkipIfRegistersNotEqual(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void Shl(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void Subn(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void Shr(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void SubtractRegister(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void AddRegister(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void BitwiseXor(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void BitwiseAnd(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void BitwiseOr(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void LoadRegisterFromRegister(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        private void AddDataToRegister(Opcode opcode)
        {
            V[opcode.X] = (byte)(V[opcode.X] + opcode.Kk);
        }

        private void LoadRegisterFromData(Opcode opcode)
        {
            V[opcode.X] = opcode.Kk;
            ProgramCounter += 2;
        }

        private void SkipIfRegistersEqual(Opcode opcode)
        {
            if (V[opcode.X] == V[opcode.Y])
            {
                ProgramCounter += 4;
            }
            else
            {
                ProgramCounter += 2;
            }
        }

        private void SkipIfNotEqual(Opcode opcode)
        {
            if (V[opcode.X] != opcode.Kk)
            {
                ProgramCounter += 4;
            }
            else
            {
                ProgramCounter += 2;
            }
        }

        private void SkipIfEqual(Opcode opcode)
        {
            if (V[opcode.X] == opcode.Kk)
            {
                ProgramCounter += 4;
            }
            else
            {
                ProgramCounter += 2;
            }
        }

        private void Call(Opcode opcode)
        {
            Stack[StackPointer] = ProgramCounter;
            StackPointer++;
            ProgramCounter = opcode.Nnn;
        }

        private void Jump(Opcode opcode)
        {
            ProgramCounter = opcode.Nnn;
        }

        private void ClearScreen()
        {
            Array.Clear(Display, 0, Display.Length);
        }

        private void LoadFontset()
        {
            for (var i = 0; i < Fontset.Length; i++)
            {
                Memory[i] = Fontset[i];
            }
        }
    }
}
