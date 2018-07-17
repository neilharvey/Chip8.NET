using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using System;

namespace Chip8
{
    class Program
    {
        private static Cpu chip8;      

        static void Main(string[] args)
        {
            chip8 = new Cpu();
            chip8.LoadApplication("");

            using (var window = new GameWindow(640, 320, GraphicsMode.Default, "Chip-8 Emulator"))
            {
                window.VSync = VSyncMode.On;
                window.RenderFrame += Window_RenderFrame;
                window.Run(30.0);
            }
        }

        private static void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            chip8.Tick();

            if (chip8.Redraw)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);
                UpdateQuads();
                //glutSwapBuffers();
                chip8.Redraw = false;
            }
        }

        private static void UpdateQuads()
        {
            for (int y = 0; y < 32; ++y)
                for (int x = 0; x < 64; ++x)
                {
                    if (chip8.Gfx[(y * 64) + x] == false)
                    {
                        //glColor3f(0.0f, 0.0f, 0.0f);
                    }
                    else
                    {
                        //glColor3f(1.0f, 1.0f, 1.0f);
                    }

                    DrawPixel(x, y);
                }
        }

        private static void DrawPixel(int x, int y)
        {
            //glBegin(GL_QUADS);
            //glVertex3f((x * modifier) + 0.0f, (y * modifier) + 0.0f, 0.0f);
            //glVertex3f((x * modifier) + 0.0f, (y * modifier) + modifier, 0.0f);
            //glVertex3f((x * modifier) + modifier, (y * modifier) + modifier, 0.0f);
            //glVertex3f((x * modifier) + modifier, (y * modifier) + 0.0f, 0.0f);
            //glEnd();
        }
    }
}
