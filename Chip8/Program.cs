//#define DRAWWITHTEXTURE

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Input;
using System;

namespace Chip8
{
    class Program
    {
        static Chip chip8;

        static void Main(string[] args)
        {
            //if (args.Length != 1)
            //{
            //    Console.WriteLine("Usage: Chip8.exe chip8application\n\n");
            //    return;
            //}

            chip8 = new Chip();
            //chip8.LoadApplication(args[0]);

            using (var window = new GameWindow(chip8))
            {
                window.Run();
            }
        }
    }
}
