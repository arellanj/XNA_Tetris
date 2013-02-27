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

using Microsoft.Kinect;

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
        
        // kinect variables
        KinectSensor kinect;
        Skeleton player;
        Skeleton[] playerData;
        float distance_threshold = .2f;

        float gesture_timer;

        float p_lwrist, p_rwrist, v_lwrist, v_rwrist, p_lshoulder, p_rshoulder;


        // gestures      
        enum gesture { left, right, rotateL, rotateR };

        Texture2D style;

        // debug
        bool kinect_enable = false;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;

            this.graphics.IsFullScreen = false;
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
            // passes tells the gameworld the size of the screen
            gameworld = new world(10, 11, 50,new Vector2(this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight));
            gesture_timer = 0;
            if(kinect_enable)
                kinect = KinectSensor.KinectSensors[0];
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            if (kinect != null)
            {
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectSkeletonFrameReadyCallback);
                kinect.Start();
                kinect.ElevationAngle = 0;
            }
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);            // load all the textures that will be used in the game

            gameworld.Load(Content);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            if(kinect_enable)kinect.Stop();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        bool rpressed = false;
        bool lpressed = false;
        bool upressed = false;
        bool dpressed = false;

        protected override void Update(GameTime gameTime)
        {
            //
            // Keyboard INPUT
            //

            // ESCAPE - Allows the game to exit
            if ( Keyboard.GetState().IsKeyDown(Keys.Escape) )
                this.Exit();

            // LEFT - moves piece left
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && lpressed == false)
            {
                lpressed = true;
                gameworld.moveLeft();
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Left))
            {
                lpressed = false;
            }

            
            // RIGHT - moves piece right
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && rpressed == false)
            {
                rpressed = true;
                gameworld.moveRight();
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Right))
            {
                rpressed = false;
            }
            
            // UP - rotates clockwise
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && upressed == false)
            {
                upressed = true;
                gameworld.rotateLeft();
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Up))
            {
                upressed = false;
            }

            // DOWN - rotates counter-clockwise
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && dpressed == false)
            {
                dpressed = true;
                gameworld.rotateRight();
            }
            else if ( Keyboard.GetState().IsKeyUp(Keys.Down))
            {
                dpressed = false;
            }

            //
            // Kinect Input
            //
            if (kinect != null)
            {
                bool[] gestures = getGestures(gameTime);

                if (gestures[(int)gesture.left])
                    gameworld.moveLeft();
                if (gestures[(int)gesture.right])
                    gameworld.moveRight();
                if (gestures[(int)gesture.rotateL])
                    gameworld.rotateLeft();
                if (gestures[(int)gesture.rotateR])
                    gameworld.rotateRight();
            }

            // this is where I plan on adding the 
            // kinect interface as input to the gameworld



            gameworld.Update(gameTime);

            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // ALL DRAW FUNCTIONS MUST BE CALLED BETWEEN 
            // SPRITEBATCH.BEGIN()
            // AND SPRITEBATCH.END()
            spriteBatch.Begin();
            gameworld.Draw(spriteBatch);
            //spriteBatch.Draw(style,new Rectangle(0,0,800,600), Color.LawnGreen);
            spriteBatch.End();


            //spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
            //spriteBatch.Draw(style, new Rectangle(0, 0, 800, 600), Color.DarkGray);

           // spriteBatch.End();

            base.Draw(gameTime);
        }
    
        void kinectSkeletonFrameReadyCallback(object sender, SkeletonFrameReadyEventArgs skeletonFrames){
            using (SkeletonFrame skeleton = skeletonFrames.OpenSkeletonFrame())
            {
                if (skeleton != null)
                {
                    if (playerData == null || this.playerData.Length != skeleton.SkeletonArrayLength)
                    {
                        this.playerData = new Skeleton[skeleton.SkeletonArrayLength];
                    }
                    skeleton.CopySkeletonDataTo(playerData);
                }
            }

            if (playerData != null)
            {
                foreach (Skeleton skeleton in playerData)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        player = skeleton;
                    }
                }
            }
        }

        bool[] getGestures(GameTime gameTime)
        {

            bool[] gestures = new bool[4]{false,false,false,false};
            if (player == null) return gestures;

            if (gesture_timer >= 0)
                gesture_timer -= gameTime.ElapsedGameTime.Milliseconds;
            
            foreach (Joint j in player.Joints)
            {
                if (j.JointType == JointType.ShoulderRight)
                    p_rshoulder = j.Position.X;
                if (j.JointType == JointType.ShoulderLeft)
                    p_lshoulder = j.Position.X;
                if (j.JointType == JointType.WristRight)
                {
                    v_rwrist = j.Position.X - p_rwrist;
                    if (j.Position.X - p_rshoulder < 0)
                    {

                        if (p_rshoulder - j.Position.X  > distance_threshold && gesture_timer <= 0)
                        {
                            gesture_timer = 300.0f;
                            gestures[0] = true;
                        }

                    }
                    else
                    {
                        if ( j.Position.X - p_rshoulder > distance_threshold && gesture_timer <= 0)
                        {
                            gesture_timer = 300.0f;
                            gestures[1] = true;
                        }
                    }
                   
                    p_rwrist = j.Position.X;
                }
                if (j.JointType == JointType.WristLeft)
                {
                    v_lwrist = j.Position.X - p_lwrist;
                    if (j.Position.X - p_lshoulder > 0)
                    {
                        if (j.Position.X - p_lshoulder > distance_threshold && gesture_timer <= 0)
                        {
                            gesture_timer = 300.0f;
                            gestures[2] = true;
                        }
                        
                    }
                    else
                    {
                        if (p_lshoulder - j.Position.X > distance_threshold && gesture_timer <= 0)
                        {
                            gesture_timer = 300.0f;
                            gestures[3] = true;
                        }
                    }
 

                }
              
            }

            return gestures;
        }
    }

}
