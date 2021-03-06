﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AppleFrenzy;
using System;

namespace Apple01
{
    abstract class Sprite
    {
        // Current animation, only one animation may play at a time.
        public Animation currentAnimation;  
        // Length within animation frame to singal hit detection
        protected int collisionOffset;
        // sprite velocity vector
        public Vector2 velocity;
        // Position in screen coordinates
        public Vector2 position;
        // Sprite size
        protected float size;

        /* Properties */
        public bool IsAlive { get; set; }
        public int DeathTimer { get; set; }
        public bool Dead { get; set; }
        /// <summary>
        /// Returns the direction vector of the sprite
        /// </summary>
        public virtual Vector2 direction
        {
            get { return new Vector2(0,0); }
        }

        public virtual Rectangle collisionRect
        {
            get
            {
                return new Rectangle(
                        (int)position.X + collisionOffset,
                        (int)position.Y + collisionOffset,
                        currentAnimation.FrameWidth - (collisionOffset * 2),
                        currentAnimation.FrameHeight - (collisionOffset * 2));
            }
        }

        // Accessor / Mutators - helps eliminates dots elsewhere
        public float xPosition
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float yPosition
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public float xVelocity
        {
            get { return velocity.X; }
            set { velocity.X = value; }
        }

        public float yVelocity
        {
            get { return velocity.Y; }
            set { velocity.Y = value; }
        }

        public int RectangleBottom
        {
            get { return collisionRect.Bottom; }
        }

        public int RectangleTop
        {
            get { return collisionRect.Top; }
        }

        public int RectangleRight
        {
            get { return collisionRect.Right; }
        }

        public int RectangleLeft
        {
            get { return collisionRect.Left; }
        }

        /* Constructors */
        public Sprite(Texture2D image, Vector2 position, Point frameSize, int collisionOffset,
                Point currentFrame, Point sheetSize, Vector2 velocity, float size, int millisecondsPerFrame)
        {
            currentAnimation = new Animation(image, frameSize, currentFrame, sheetSize, millisecondsPerFrame);
            this.position = position;
            this.collisionOffset = collisionOffset;
            this.velocity = velocity;
            this.size = size;
        }

        public Sprite(Texture2D image, Vector2 position, Point frameSize,
                int collisionOffset, Point currentFrame, Point sheetSize, Vector2 velocity, float size)
        {
            currentAnimation = new Animation(image, frameSize, currentFrame, sheetSize);
            this.position = position;
            this.collisionOffset = collisionOffset;
            this.velocity = velocity;
            this.size = size;
        }


        /* Functions */
        // Initialize will be used to initialize any class variables and states
        public virtual void Initialize(IServiceProvider serviceProvider) { }
        public virtual void Reset() { }

        /// <summary>
        /// Stop vertical movement, useful so things don't go through ground
        /// </summary>
        public void stopVerticalMovement()
        {
            velocity.Y = 0;
        }

        /// <summary>
        /// Update the sprite by iterating through the sprite sheet and 
        /// updating the frames
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="clientBounds"></param>
        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // If enough time has passed, render next frame
            currentAnimation.nextFrame(gameTime);
        }

        /// <summary>
        /// Draw the sprite to the display
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(currentAnimation.Image, position,
                currentAnimation.frameRectangle,
                Color.White, 0, Vector2.Zero,
                size, SpriteEffects.None, 0);
        }

    }
}
