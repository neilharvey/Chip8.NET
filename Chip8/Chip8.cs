using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8
{
    public class Chip8
    {
        public const int ScreenWidth = 64;
        public const int ScreenHeight = 32;

        public bool Redraw { get; set; }

        public bool[] Screen { get; private set; } = new bool[ScreenWidth * ScreenHeight];

        public bool[] Keys { get; private set; }

        public Chip8()
        {
            Screen[100] = true;
            Screen[1000] = true;
        }

        public void LoadApplication(string path)
        {
           
        }

        public void Tick()
        {
        }
    }
}
