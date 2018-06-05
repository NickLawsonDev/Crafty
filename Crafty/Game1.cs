//Comment out for prod
//#define DEBUGINFO

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Crafty
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Rectangle player1;
        Color[] player1Color;
        Texture2D player1Tex;
        int player1Score = 0;

        Rectangle player2;
        Color[] player2Color;
        Texture2D player2Tex;
        int player2Score = 0;

        Rectangle ball;
        Color[] ballColor;
        Texture2D ballTex;
        int ballVX;
        int ballVY;

        SpriteFont font;

        // At the top of your class:
        Texture2D pixel;

        public Game1()
        {
            //Should move this to a settings system
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 1200;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            player1 = new Rectangle
            {
                X = 100,
                Y = 400,
                Width = 50,
                Height = 100
            };
            player2 = new Rectangle
            {
                X = 1100,
                Y = 400,
                Width = 50,
                Height = 100
            };
            ball = new Rectangle
            {
                X = graphics.PreferredBackBufferWidth / 2 - 30,
                Y = graphics.PreferredBackBufferHeight / 2 - 30,
                Width = 30,
                Height = 30
            };
        }

        protected override void Initialize()
        {
            //Init player settings. Should be in an entity with an input system attached
            player1Color = new Color[player1.Width * player1.Height];
            player1Tex = new Texture2D(GraphicsDevice, player1.Width, player1.Height);

            player2Color = new Color[player2.Width * player2.Height];
            player2Tex = new Texture2D(GraphicsDevice, player2.Width, player2.Height);

            for (var i = 0; i < player1Color.Length; i++)
            {
                player1Color[i] = Color.White;
                player2Color[i] = Color.White;
            }

            player1Tex.SetData(player1Color);
            player2Tex.SetData(player2Color);

            ballColor = new Color[ball.Width * ball.Height];
            ballTex = new Texture2D(GraphicsDevice, ball.Width, ball.Height);

            //Creating the ball
            var radius = 30;
            var diam = radius / 2f;
            var diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                        ballColor[index] = Color.White;
                    else
                        ballColor[index] = Color.Transparent;
                }
            }

            ballTex.SetData(ballColor);

            var rand = new Random();
            var num = rand.Next(0, 50);

            ballVX = num > 25 ? rand.Next(5, 15) : rand.Next(-15, -5);
            ballVY = num > 25 ? rand.Next(5, 10) : rand.Next(-10, -5);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font/Debug");

            // Somewhere in your LoadContent() method:
            pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White }); // so that we can draw whatever color we want on top of it 
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Player 1 Up. Should refactor into Input system on each player controller
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                player1.Y -= 10;
            }
            //Player 1 Down
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                player1.Y += 10;
            }
            //Player 2 Up
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                player2.Y -= 10;
            }
            //Player 2 Down
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                player2.Y += 10;
            }

            if (!ball.Intersects(player1))
            {
                if (!ball.Intersects(player2))
                {
                    //Checks to see if ball hits the top or bottom of the screen
                    if (ball.Bottom >= graphics.PreferredBackBufferHeight || ball.Top <= 0)
                    {
                        ballVY = -ballVY;
                        ball.Y += ballVY;
                    }
                    else if (ball.Right >= graphics.PreferredBackBufferWidth || ball.Left <= 0)
                    {
                        ballVX = -ballVX;
                        ball.X += ballVX;
                    }
                    else 
                    {
                        ball.X += ballVX;
                        ball.Y += ballVY;
                    }
                }
                else
                {
                    //P2 Right Hit
                    if (ball.Left >= player2.Right)
                        ball.X = player2.Right;
                    //P2 Left Hit
                    else if (ball.Right >= player2.Left)
                        ball.X = player2.Left - 30;
                    //P2 Top Hit
                    else if (ball.Bottom >= player2.Top)
                        ball.Y = player2.Top - 30;
                    //P2 Bottom Hit
                    else if (ball.Top <= player2.Bottom)
                        ball.Y = player2.Bottom;

                    ballVX = -ballVX;
                    ball.X += ballVX;
                    ballVY = -ballVY;
                    ball.Y += ballVY;
                }
            }
            else
            {
                ballVX = -ballVX;
                ball.X += ballVX;
                ballVY = -ballVY;
                ball.Y += ballVY;
            }

            //MathHelper.Clamp(player1.X, 0, graphics.PreferredBackBufferWidth - ball.Width);
            player1.Y = MathHelper.Clamp(player1.Y, 0, graphics.PreferredBackBufferHeight - player1.Height);
            player2.Y = MathHelper.Clamp(player2.Y, 0, graphics.PreferredBackBufferHeight - player2.Height);
            ball.Y = MathHelper.Clamp(ball.Y, 0, graphics.PreferredBackBufferHeight - ball.Height);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(player1Tex, player1, Color.White);
            spriteBatch.Draw(player2Tex, player2, Color.White);
            spriteBatch.Draw(ballTex, ball, Color.White);


#if DEBUGINFO
            spriteBatch.DrawString(font, $"Player1 X: {player1.X}", new Vector2(10, 10), Color.Green);
            spriteBatch.DrawString(font, $"Player1 Y: {player1.Y}", new Vector2(10, 30), Color.Green);
            spriteBatch.DrawString(font, $"Player2 X: {player2.X}", new Vector2(10, 60), Color.Green);
            spriteBatch.DrawString(font, $"Player2 Y: {player2.Y}", new Vector2(10, 80), Color.Green);
            spriteBatch.DrawString(font, $"Ball X: {ball.X}", new Vector2(10, 100), Color.Green);
            spriteBatch.DrawString(font, $"Ball Y: {ball.Y}", new Vector2(10, 120), Color.Green);
            spriteBatch.DrawString(font, $"Ball VX: {ballVX}", new Vector2(10, 140), Color.Green);
            spriteBatch.DrawString(font, $"Ball VY: {ballVY}", new Vector2(10, 160), Color.Green);
            DrawBorder(ball, 1, Color.Green);
            spriteBatch.DrawString(font, $"Mouse X: {Mouse.GetState().X}", new Vector2(10, 200), Color.Green);
            spriteBatch.DrawString(font, $"Mouse Y: {Mouse.GetState().Y}", new Vector2(10, 220), Color.Green);
#endif

            spriteBatch.DrawString(font, $"Player 1 Score: {player1Score}", new Vector2(40, 20), Color.Green);
            spriteBatch.DrawString(font, $"Player 2 Score: {player2Score}", new Vector2(1040, 20), Color.Green);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            // Draw top line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

            // Draw left line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            spriteBatch.Draw(pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
            // Draw bottom line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X,
                                            rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                            rectangleToDraw.Width,
                                            thicknessOfBorder), borderColor);
        }
    }
}
