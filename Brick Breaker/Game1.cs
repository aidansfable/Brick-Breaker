using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Brick_Breaker
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WINDOW_WIDTH = 800;
        const int WINDOW_HEIGHT = 600;

        int Score;
        int Deaths;

        Texture2D background_t;
        Texture2D bat_t;
        Texture2D ball_t;

        Texture2D brick_t;
        List<Vector2> brick_p = new List<Vector2>();
        List<Color> brick_c = new List<Color>();

        Vector2 bat_p;
        Vector2 ball_p;
        Vector2 velocity = new Vector2(0.4f, 0.4f);

        SoundEffect ball_on_bat;
        SoundEffect ball_on_sides;
        SoundEffect breaks_brick;
        SoundEffect death;

        SpriteFont font;
        Vector2 font_p;

        // Create random number generator
        Random RanNumGen = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            bat_t = Content.Load<Texture2D>("Bat");
            bat_p = new Vector2((WINDOW_WIDTH / 2) - 65, 560);

            ball_t = Content.Load<Texture2D>("Ball");
            ball_p = new Vector2((WINDOW_WIDTH / 2) - 10, (WINDOW_HEIGHT / 2) - 10);

            // Load and spawn bricks
            brick_t = Content.Load<Texture2D>("Brick");

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 25; x++)
                {
                    brick_p.Add(new Vector2(x * brick_t.Width, y * brick_t.Height));
                    brick_c.Add(new Color(RanNumGen.Next(255), RanNumGen.Next(255), RanNumGen.Next(255)));
                }

                font = Content.Load<SpriteFont>("NewSpriteFont");

                ball_on_bat = Content.Load<SoundEffect>("ball against bat");

                ball_on_sides = Content.Load<SoundEffect>("ball against wall");

                breaks_brick = Content.Load<SoundEffect>("breaks brick");

                death = Content.Load<SoundEffect>("death");

                // TODO: use this.Content to load your game content here
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Makes ball move

            ball_p.X += (int)(velocity.X * gameTime.ElapsedGameTime.Milliseconds);
            ball_p.Y += (int)(velocity.Y * gameTime.ElapsedGameTime.Milliseconds);

            // Makes ball collide with bat
            // Creates Rectangles

            Rectangle ball_rect =
                new Rectangle((int)ball_p.X, (int)ball_p.Y,
                ball_t.Width, ball_t.Height);

            Rectangle bat_rect =
                new Rectangle((int)bat_p.X, (int)bat_p.Y,
                bat_t.Width, bat_t.Height);

            // Actual Collisions

            if (ball_rect.Intersects(bat_rect) && velocity.Y > 0)
            {
                    velocity.Y *= -1;
                    ball_on_bat.Play();
                }

            // Makes ball collide with and remove bricks
            // Creates Rectangles
            for (int bricks = brick_p.Count - 1; bricks >= 0; bricks--)
            {
                Rectangle brick_rect = new Rectangle(
                    (int)brick_p[bricks].X,
                    (int)brick_p[bricks].Y,
                    brick_t.Width, brick_t.Height);

                // Actual Collisions
                if (ball_rect.Intersects(brick_rect))
                {
                    velocity.X = Math.Abs(velocity.X); 
                    velocity.Y *= -1;
                    brick_p.RemoveAt(bricks);
                    brick_c.RemoveAt(bricks);

                    // Score: Bricks broken
                    Score++;
                    // Play Sound effect
                    breaks_brick.Play();
                }
                // Makes game exit when there are no balls left
            if (brick_p.Count <= 0) 
                Exit();

                }


            // TODO: Add your update logic here

            var keyboardState = Keyboard.GetState();

            // This makes the bat move left and right when the coresponding keys are pressed

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                bat_p.X += 0.6f * gameTime.ElapsedGameTime.Milliseconds;
            }
            else if (keyboardState.IsKeyDown(Keys.Left))
            {
                bat_p.X -= 0.6f * gameTime.ElapsedGameTime.Milliseconds;
            }

            // Left, right, and top collision
            // Right
            if (ball_p.X > WINDOW_WIDTH - 20)
            {
                velocity.X *= -1;
                ball_on_sides.Play();
            }

            // Left
            if (ball_p.X < 0)
            {
                velocity.X *= -1;
                ball_on_sides.Play();
            }

            // Top
            if (ball_p.Y < 0)
            {
                velocity.Y *= -1;
                ball_on_sides.Play();
            }

            // Ball respawn and number lost
           
            // Balls Lost (score keep and sound effect lay)
            if (ball_p.Y > WINDOW_HEIGHT - 20)
            {
                ball_p = new Vector2((WINDOW_WIDTH / 2) - 10, (WINDOW_HEIGHT / 2) - 10);
                Deaths++;
                death.Play();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            // This makes all the sprites appear on the screen

            spriteBatch.Begin();

            spriteBatch.Draw(bat_t, bat_p, Color.White);

            spriteBatch.Draw(ball_t, ball_p, Color.White);

            for (int i = 0; i < brick_p.Count; i++)
            {
                spriteBatch.Draw(brick_t, brick_p[i], brick_c[i]);
            }

            spriteBatch.DrawString(font, "Bricks Broken: " + Score, new Vector2((WINDOW_WIDTH/2) - 140, 10), Color.White);

            spriteBatch.DrawString(font, "Balls Lost: " + Deaths, new Vector2((WINDOW_WIDTH / 2) + 20, 10), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
