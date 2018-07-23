﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Chip8
{
    // http://devernay.free.fr/hacks/chip8/C8TECH10.HTM
    public class Chip
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

        public Chip()
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
            }
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
