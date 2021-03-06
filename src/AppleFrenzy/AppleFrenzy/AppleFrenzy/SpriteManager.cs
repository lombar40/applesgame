using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using AppleFrenzy;


namespace Apple01
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // Constant for ground vertical position
        public const int GROUND_LEVEL = 580;
        // Enum for addressing variables in array 
        enum SpriteType { bee1, bee2, bird1, bird2 };

        // System provided variable for drawing to GPU
        SpriteBatch spriteBatch;

        // Sprites
        List<Sprite> sprites = new List<Sprite>(); // bees and birds sprites, use enum to address
        UserControlledSprite player;
        List<Sprite> apples = new List<Sprite>(); // Need to query array size and update
        List<Sprite> lives = new List<Sprite>(); // Need to query array size and update
        List<Sprite> tiles = new List<Sprite>(); // Needs to be passed into a function
        
        // Sounds
        List<SoundEffect> sounds = new List<SoundEffect>();
        

        // Constructor
        public SpriteManager(Game game) : base(game) { }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// Reset game state to initial state
        /// </summary>
        public void Reset()
        {   // This is a point of possible performance optimization by NOT allocating new memory
            // when resetting, if you think about porting to WP, change this, you will need
            // to add initialization routines to several sprite ADTs. Memory operations are 
            // more expensive than CPU operations. 
            apples = new List<Sprite>();
            lives = new List<Sprite>();
            sprites = new List<Sprite>();
            LoadContent();
        }


        /// <summary>
        /// Load art assets, sounds, etc...
        /// </summary>
        protected override void LoadContent()
        {
        
            // Initialize spriteBatch object to correct GPU
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            // Load Life sprite - Player always starts with 3 lives
            lives.Add(new HudSprite(Game.Content.Load<Texture2D>(@"Images/Heart"),
                        new Vector2(20, 30), new Point(101, 171), 0.19f));
            lives.Add(new HudSprite(Game.Content.Load<Texture2D>(@"Images/Heart"),
                        new Vector2(40, 30), new Point(101, 171), 0.19f));
            lives.Add(new HudSprite(Game.Content.Load<Texture2D>(@"Images/Heart"),
                        new Vector2(60, 30), new Point(101, 171), 0.19f));

            // Load bees sprites
            sprites.Add(new BeeSprite(Game.Content.Load<Texture2D>(@"Images/Bee1"),
                        new Vector2(600, GROUND_LEVEL), new Point(24,24), 5, new Point(0,0), 
                        new Point(3,1), new Vector2(-3,0), 1f));
            sprites.Add(new BeeSprite(Game.Content.Load<Texture2D>(@"Images/Bee2"),
                        new Vector2(900, GROUND_LEVEL + 30), new Point(24, 24), 5, new Point(0, 0),
                        new Point(3, 1), new Vector2(-2, 0), 1f));

            // Load bird sprite
            sprites.Add(new BirdSprite(Game.Content.Load<Texture2D>(@"Images/Bird5"),
                        new Vector2(500, 530), new Point(47, 44), 5, new Point(0, 0),
                        new Point(9, 1), new Vector2(-2, 0), 1.17f));
            sprites.Add(new BirdSprite(Game.Content.Load<Texture2D>(@"Images/Bird5"),
                        new Vector2(870, 430), new Point(47, 44), 5, new Point(0, 0),
                        new Point(9, 1), new Vector2(-1.75f, 0), 1.17f));

            // Load player controlled character
            player = new UserControlledSprite(Game.Content.Load<Texture2D>(@"Images/Idle"),
                        new Vector2(0,GROUND_LEVEL), new Point(64, 64), 10, new Point(0, 0), new Point(1, 1),
                        new Vector2(6, 6), 1f);
            player.Initialize(Game.Services);

            // Load platform 
            tiles.Add(new PlatformSprite(Game.Content.Load<Texture2D>(@"Images/BlockB0"),
                        new Vector2(200, 535), new Point(40, 32), 5, new Vector2(0.2f, 0),
                        Game.Window.ClientBounds, 1.0f));
            tiles.Add(new PlatformSprite(Game.Content.Load<Texture2D>(@"Images/BlockB0"),
                        new Vector2(515, 415), new Point(40, 32), 5, new Vector2(0.2f, 0),
                        Game.Window.ClientBounds, 1.0f));

            // Load Apples
            loadApples();


            // Load Sound Effects
            sounds.Add(Game.Content.Load<SoundEffect>(@"Sounds\Hit3"));
            sounds.Add(Game.Content.Load<SoundEffect>(@"Sounds\AppleCollected"));
            sounds.Add(Game.Content.Load<SoundEffect>(@"Sounds\Bird03"));

            base.LoadContent();
        }

        /// <summary>
        /// Loads apples when starting a game and when
        /// there are only two remaining apples on the screen
        /// </summary>
        private void loadApples()
        {
            for (int i = 0; i < 12; ++i)
                apples.Add(new AppleSprite(Game.Content.Load<Texture2D>(@"Images\Apple"),
                    new Point(28, 32), 5, new Vector2(0, 2), Game.Window.ClientBounds, 0.60f));
        }

        // Check for apple collisions
        void AppleCollision(Sprite sprite, ref int i)
        {
            // Check for collisions
            if (sprite.collisionRect.Intersects(player.collisionRect))
            { 
                if (sprite.RectangleBottom < GROUND_LEVEL + 65)
                    ((ApplesGame)Game).AddScore(4);
                else
                    ((ApplesGame)Game).AddScore(2);

                // Play Apple Collected Sound
                sounds[1].Play();

                // Remove apple from apples list
                apples.RemoveAt(i);
                --i;

                // Check to see if we need to drop more apples
                if (apples.Count <= 3)
                    loadApples();
            }
            if (sprite.RectangleBottom >= GROUND_LEVEL + 72 || 
                sprite.collisionRect.Intersects(tiles[0].collisionRect) ||
                sprite.collisionRect.Intersects(tiles[1].collisionRect))
                sprite.stopVerticalMovement();
            
        }

        // When player gets hit, reset location and reduce lives
        void onPlayerHit()
        {

            if (lives.Count != 0)
            {
                sounds[0].Play();
                lives.RemoveAt(lives.Count - 1);
                ((ApplesGame)Game).AddScore(-4);
            }
            if (lives.Count != 0)
                player.Reset(new Vector2(0, GROUND_LEVEL));
            if (lives.Count == 0)
                player.IsAlive = false;
        }

        // Do Bird collision detection and calls its update routine if alive
        void updateBird(Sprite bird, GameTime gameTime)
        {
            if (!bird.IsAlive)
                return;

            bird.Update(gameTime, Game.Window.ClientBounds);

            // Check for bird collision detection
            if (bird.collisionRect.Intersects(player.collisionRect))
            {
                if (Math.Abs(player.RectangleBottom - bird.RectangleTop) <= 6.8f)
                {
                    bird.IsAlive = false;
                    sounds[2].Play();
                    ((ApplesGame)Game).AddScore(10);
                }
                else
                {
                    onPlayerHit();
                }
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Update player sprite
            player.Update(gameTime, Game.Window.ClientBounds, tiles);

            // This is for DEBUG Purposes only
            //if (Keyboard.GetState().IsKeyDown(Keys.P))
            //    loadApples();

            // Update bees
            sprites[(int)SpriteType.bee1].Update(gameTime, Game.Window.ClientBounds);
            sprites[(int)SpriteType.bee2].Update(gameTime, Game.Window.ClientBounds);

            // update birds
            updateBird(sprites[(int)SpriteType.bird1], gameTime);
            updateBird(sprites[(int)SpriteType.bird2], gameTime);

            // Check for bees' intersection with player
            if (sprites[(int)SpriteType.bee1].collisionRect.Intersects(player.collisionRect) ||
                sprites[(int)SpriteType.bee2].collisionRect.Intersects(player.collisionRect))
            {
                onPlayerHit();
            }

            // Update apples
            for(int i = 0; i < apples.Count; ++i)
            {
                Sprite sprite = apples[i];
                sprite.Update(gameTime, Game.Window.ClientBounds);

                // Check for collisions and update appropriately
                AppleCollision(sprite, ref i);
            }


            // Check if player is dead and end game if so
            if (!player.IsAlive)
            {
                if(player.DeathTimer > 50)
                    ((ApplesGame)Game).EndGame();
                player.DeathTimer++;
            }

            // Update platforms
            foreach(Sprite platform in tiles)
                platform.Update(gameTime, Game.Window.ClientBounds);
            

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw all sprites including player, enemies, apples, etc.. to
        /// the screen. For each sprite call it's corresponding Draw method.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            // Draw heart (life) hud
            foreach (Sprite sprite in lives)
                sprite.Draw(gameTime, spriteBatch);

            // Draw the player
            player.Draw(gameTime, spriteBatch);

            // Draw birds
            if (sprites[(int)SpriteType.bird1].IsAlive)
                sprites[(int)SpriteType.bird1].Draw(gameTime, spriteBatch);
            if (sprites[(int)SpriteType.bird2].IsAlive)
                sprites[(int)SpriteType.bird2].Draw(gameTime, spriteBatch);

            // Draw the bees
            sprites[(int)SpriteType.bee1].Draw(gameTime, spriteBatch);
            sprites[(int)SpriteType.bee2].Draw(gameTime, spriteBatch);

            // Draw the apples
            foreach (Sprite sprite in apples)
                sprite.Draw(gameTime, spriteBatch);

            // Draw platform
            foreach(Sprite platform in tiles)
                platform.Draw(gameTime, spriteBatch);
            

            

            spriteBatch.End();
            
            base.Draw(gameTime);
        }


        /// <summary>
        /// Draw the bird only if it has not been killed
        /// </summary>
        /// <param name="bird"></param>
        /// <param name="gameTime"></param>
        void drawBird(Sprite bird, GameTime gameTime)
        {
            if (bird.IsAlive)
                bird.Draw(gameTime, spriteBatch);
        }
    }
}
