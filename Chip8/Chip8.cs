using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8
{
    public class Chip8
    {
        public bool Redraw { get; set; }

        public bool[] Gfx { get; private set; }

        public bool[] key { get; private set; }

        public Chip8()
        {
        }

        public void LoadApplication(string path)
        {
        }

        public void Tick()
        {
        }

        private void Reset()
        {
            Gfx = new bool[64 * 32];
        }
    }
}
