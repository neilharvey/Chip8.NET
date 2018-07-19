using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
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
            Load += GameWindow_Load;
            UpdateFrame += GameWindow_UpdateFrame;
            RenderFrame += GameWindow_RenderFrame;
        }

        public Chip8 Chip8 { get; }

        private void GameWindow_Load(object sender, EventArgs e)
        {
            // Create texture
        }

        private void GameWindow_UpdateFrame(object sender, FrameEventArgs e)
        {
            Chip8.Tick();

            if (Chip8.Redraw)
            {
                // Update texture       
                Chip8.Redraw = false;
            }
        }

        private void GameWindow_RenderFrame(object sender, FrameEventArgs e)
        {
            GL.ClearColor(Color4.Purple);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            SwapBuffers();
        }
    }
}
