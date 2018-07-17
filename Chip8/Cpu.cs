using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8
{
    public class Cpu
    {
        public bool Redraw { get; set; }

        public bool[] Gfx { get; private set; }

        public Cpu()
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
