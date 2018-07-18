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
        const int SCREEN_WIDTH = 64;
        const int SCREEN_HEIGHT = 32;
        const int modifier = 10;

        static Chip8 myChip8 = new Chip8();

        static int display_width = SCREEN_WIDTH * modifier;
        static int display_height = SCREEN_HEIGHT * modifier;

        static ushort[,,] screenData = new ushort[SCREEN_HEIGHT, SCREEN_WIDTH, 3];

        static void Main(string[] args)
        {
            //if (args.Length != 1)
            //{
            //    Console.WriteLine("Usage: myChip8.exe chip8application\n\n");
            //    return;
            //}

            //myChip8.LoadApplication(args[1]);

            using (var window = new GameWindow(display_width, display_height))
            {
                window.Title = "Chip-8 Emulator";
                window.RenderFrame += Window_RenderFrame;
                window.Resize += Window_Resize;
                window.KeyDown += Window_KeyDown;
                window.KeyUp += Window_KeyUp;

#if DRAWWITHTEXTURE
                SetupTexture();
#endif
                window.Run();
            }
        }

        private static void SetupTexture()
        {
            // Clear screen
            for (int y = 0; y < SCREEN_HEIGHT; ++y)
            {
                for (int x = 0; x < SCREEN_WIDTH; ++x)
                {
                    screenData[y, x, 0] = screenData[y, x, 1] = screenData[y, x, 2] = 0;
                }
            }

            // Create a texture 
            GL.TexImage2D(TextureTarget2d.Texture2D, 0, TextureComponentCount.Rgb, SCREEN_WIDTH, SCREEN_HEIGHT, 0, PixelFormat.Rgb, PixelType.UnsignedByte, screenData);

            // Set up the texture
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

            // Enable textures
            GL.Enable(EnableCap.Texture2D);
        }

        private static void Window_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            var key = e.Key;

            if (key == Key.Number1) myChip8.key[0x1] = false;
            else if (key == Key.Number2) myChip8.key[0x2] = false;
            else if (key == Key.Number3) myChip8.key[0x3] = false;
            else if (key == Key.Number4) myChip8.key[0xC] = false;

            else if (key == Key.Q) myChip8.key[0x4] = false;
            else if (key == Key.W) myChip8.key[0x5] = false;
            else if (key == Key.E) myChip8.key[0x6] = false;
            else if (key == Key.R) myChip8.key[0xD] = false;

            else if (key == Key.A) myChip8.key[0x7] = false;
            else if (key == Key.S) myChip8.key[0x8] = false;
            else if (key == Key.D) myChip8.key[0x9] = false;
            else if (key == Key.F) myChip8.key[0xE] = false;

            else if (key == Key.Z) myChip8.key[0xA] = false;
            else if (key == Key.X) myChip8.key[0x0] = false;
            else if (key == Key.C) myChip8.key[0xB] = false;
            else if (key == Key.V) myChip8.key[0xF] = false;
        }

        private static void Window_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            var key = e.Key;

            if(key == Key.Escape)
            {
                return; // exit(0); -> close window?
            }

            if (key == Key.Number1) myChip8.key[0x1] = true;
            else if (key == Key.Number2) myChip8.key[0x2] = true;
            else if (key == Key.Number3) myChip8.key[0x3] = true;
            else if (key == Key.Number4) myChip8.key[0xC] = true;

            else if (key == Key.Q) myChip8.key[0x4] = true;
            else if (key == Key.W) myChip8.key[0x5] = true;
            else if (key == Key.E) myChip8.key[0x6] = true;
            else if (key == Key.R) myChip8.key[0xD] = true;

            else if (key == Key.A) myChip8.key[0x7] = true;
            else if (key == Key.S) myChip8.key[0x8] = true;
            else if (key == Key.D) myChip8.key[0x9] = true;
            else if (key == Key.F) myChip8.key[0xE] = true;

            else if (key == Key.Z) myChip8.key[0xA] = true;
            else if (key == Key.X) myChip8.key[0x0] = true;
            else if (key == Key.C) myChip8.key[0xB] = true;
            else if (key == Key.V) myChip8.key[0xF] = true;
        }

        private static void Window_Resize(object sender, EventArgs e)
        {
            //        glClearColor(0.0f, 0.0f, 0.5f, 0.0f);
            //        glMatrixMode(GL_PROJECTION);
            //        glLoadIdentity();
            //        gluOrtho2D(0, w, h, 0);
            //        glMatrixMode(GL_MODELVIEW);
            //        glViewport(0, 0, w, h);

            //        // Resize quad
            //        display_width = w;
            //        display_height = h;
        }

        private static void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            //myChip8.Tick();

            if (myChip8.Redraw)
            {
                // Clear framebuffer
                GL.Clear(ClearBufferMask.ColorBufferBit);

#if DRAWWITHTEXTURE
                updateTexture(myChip8);
#else
                UpdateQuads(myChip8);
#endif
                // Swap buffers!
                //glutSwapBuffers();

                // Processed frame
                myChip8.Redraw = false;
            }
        }

        private static void UpdateTexture(Chip8 c8)
        {
            //	// Update pixels
            //	for(int y = 0; y< 32; ++y)		
            //		for(int x = 0; x< 64; ++x)
            //			if(c8.gfx[(y * 64) + x] == 0)
            //				screenData[y][x][0] = screenData[y][x][1] = screenData[y][x][2] = 0;	// Disabled
            //			else 
            //				screenData[y][x][0] = screenData[y][x][1] = screenData[y][x][2] = 255;  // Enabled

            //	// Update Texture
            //	glTexSubImage2D(GL_TEXTURE_2D, 0 ,0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, GL_RGB, GL_UNSIGNED_BYTE, (GLvoid*) screenData);

            //        glBegin(GL_QUADS );
            //        glTexCoord2d(0.0, 0.0); glVertex2d(0.0,           0.0);
            //        glTexCoord2d(1.0, 0.0); glVertex2d(display_width, 0.0);
            //        glTexCoord2d(1.0, 1.0); glVertex2d(display_width, display_height);
            //        glTexCoord2d(0.0, 1.0); glVertex2d(0.0, display_height);
            //        glEnd();
        }

        private static void UpdateQuads(Chip8 c8)
        {
            // Draw
            for (int y = 0; y < 32; ++y)
                for (int x = 0; x < 64; ++x)
                {
                    if (c8.Gfx[(y * 64) + x] == false)
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
            //        glBegin(GL_QUADS);
            //        glVertex3f((x * modifier) + 0.0f, (y * modifier) + 0.0f, 0.0f);
            //        glVertex3f((x * modifier) + 0.0f, (y * modifier) + modifier, 0.0f);
            //        glVertex3f((x * modifier) + modifier, (y * modifier) + modifier, 0.0f);
            //        glVertex3f((x * modifier) + modifier, (y * modifier) + 0.0f, 0.0f);
            //        glEnd();
        }

    }
}
