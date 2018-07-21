﻿using OpenTK;
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
        }

        public Chip8 Chip8 { get; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Width, Height, 0, -1, 1);
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Begin(PrimitiveType.Quads);

            for (int y = 0; y < Chip8.ScreenHeight; ++y)
            {
                for (int x = 0; x < Chip8.ScreenWidth; ++x)
                {
                    DrawPixel(x, y, Chip8.Screen[(y * 64) + x]);
                }
            }

            GL.End();
            GL.Flush();
            SwapBuffers();
        }

        private void DrawPixel(int x, int y, bool state)
        {
            if (!state)
            {
                GL.Color3(0f, 0.0f, 0.0f);
            }
            else
            {
                GL.Color3(1.0f, 1.0f, 1.0f);
            }

            float vx = (x * Scale);
            float vy = (y * Scale);
           
            GL.Vertex2(vx, vy);
            GL.Vertex2(vx, vy + Scale);
            GL.Vertex2(vx + Scale, vy + Scale);
            GL.Vertex2(vx + Scale, vy);           
        }
    }
}
