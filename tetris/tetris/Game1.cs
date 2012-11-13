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

namespace tetris
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        world gameworld;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            this.graphics.PreferredBackBufferWidth = 1024;
            this.graphics.PreferredBackBufferHeight = 800;

            //this.graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Creates a new gameworld with a playable grid of size
            // 10 columns by 11 rows
            // each cell will be 50x50 pixels
            gameworld = new world(13, 14, 55);

            // passes tells the gameworld the size of the screen
            gameworld.gamesize = new Vector2(this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);

            // load all the textures that will be used in the game
            gameworld.screenback = Content.Load<Texture2D>("Backgrounds\\Back");
            gameworld.gridback = Content.Load<Texture2D>("Backgrounds\\GameArea");

            // NOTE :  the order here matters!
            // it will be compared with the enumerated type blocks
            // TODO : rename the textures to the color of the blocks instead of
            //        using the name of the piece
            gameworld.block_tex[0] = Content.Load<Texture2D>("Shape Textures\\Box");
            gameworld.block_tex[1] = Content.Load<Texture2D>("Shape Textures\\Bar");
            gameworld.block_tex[2] = Content.Load<Texture2D>("Shape Textures\\Z");
            gameworld.block_tex[3] = Content.Load<Texture2D>("Shape Textures\\S");
            gameworld.block_tex[4] = Content.Load<Texture2D>("Shape Textures\\T");
            gameworld.block_tex[5] = Content.Load<Texture2D>("Shape Textures\\L");
            gameworld.block_tex[6] = Content.Load<Texture2D>("Shape Textures\\J");
            
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

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if ( Keyboard.GetState().IsKeyDown(Keys.Escape) )
                this.Exit();

            // TODO: Add your update logic here
            gameworld.Update(gameTime);

            // this is where I plan on adding the 
            // kinect interface as input to the gameworld

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // ALL DRAW FUNCTIONS MUST BE CALLED BETWEEN 
            // SPRITEBATCH.BEGIN()
            // AND SPRITEBATCH.END()
            spriteBatch.Begin();
            gameworld.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
