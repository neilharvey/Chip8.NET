using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace Chip8
{
    public class GameWindow : OpenTK.GameWindow
    {
        private const int Scale = 10;
        private const string WindowTitle = "Chip-8 Emulator";

        public GameWindow(Chip8 chip8) : base(
            Chip8.ScreenWidth * Scale,
            Chip8.ScreenHeight * Scale,
            GraphicsMode.Default,
            WindowTitle,
            GameWindowFlags.FixedWindow,
            DisplayDevice.Default,
            3,
            0,
            GraphicsContextFlags.ForwardCompatible)
        {
            Console.WriteLine("Open GL version: " + GL.GetString(StringName.Version));
            Chip8 = chip8 ?? throw new ArgumentNullException(nameof(chip8));
            RenderFrame += GameWindow_RenderFrame;
        }

        public Chip8 Chip8 { get; }

        private void GameWindow_RenderFrame(object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.CornflowerBlue);

            //Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref modelview);

            GL.Begin(PrimitiveType.Quads);
            UpdateQuads();
            GL.End();
            SwapBuffers();
        }

        private void UpdateQuads()
        {
            for (int y = 0; y < Chip8.ScreenHeight; ++y)
            {
                for (int x = 0; x < Chip8.ScreenWidth; ++x)
                {
                    if (Chip8.Screen[(y * 64) + x])
                    {
                        GL.Color3(0f, 0.0f, 0.0f);
                    }
                    else
                    {
                        GL.Color3(1.0f, 1.0f, 1.0f);
                    }

                    DrawPixel(x, y);
                }
            }
        }

        private void DrawPixel(int x, int y)
        {
            float vx = (x * Scale);
            float vy = (y * Scale);
           
            GL.Vertex3(vx, vy, 0);
            GL.Vertex3(vx, vy + Scale, 0);
            GL.Vertex3(vx + Scale, vy + Scale, 0);
            GL.Vertex3(vx + Scale, vy, 0);           
        }
    }
}
