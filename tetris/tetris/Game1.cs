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

        Vector3 p_lwrist, p_rwrist, v_lwrist, v_rwrist, p_lshoulder, p_rshoulder;
        float vmax, vmin;


        // gestures      
        enum gesture { left, right, rotateL, rotateR, hdrop };
        int numGestures = 5;

        Texture2D style;

        // debug
        bool kinect_enable = true;     
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
            gameworld = new world(14, 15, 35,new Vector2(this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight));
            gesture_timer = 0;
            vmax = vmin = 0;
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
        bool spressed = false;

        protected override void Update(GameTime gameTime)
        {
            //
            // Keyboard INPUT
            //
            // currently it uses only one keyboard state with no memory of its previouse state the workaround is to use a *pressed variable
            // since it is using the kinect anyways, this code will be removed later

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
            else if (Keyboard.GetState().IsKeyUp(Keys.Down))
            {
                dpressed = false;
            }
            
            // SPACE - Hard Drop
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && spressed == false)
            {
                spressed = true;
                gameworld.hardDrop();
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                spressed = false;
            }

            //
            // Kinect Input
            //
            if (kinect != null)
            {
                // is there a better way to do this
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
            
            spriteBatch.End();
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
            //initialize  the return
            Console.WriteLine("( {0} )", v_rwrist.Y);
            Console.WriteLine(" Max = {0}, Min = {1}", vmax, vmin);
            vmax = Math.Max(vmax, v_rwrist.Y);
            vmin = Math.Min(vmin, v_rwrist.Y);

            bool[] gestures = new bool[numGestures];
            for (int i = 0; i < numGestures; i++)
                gestures[i] = false;
            // if there is no player present do not check the data
            if (player == null) return gestures;
            
            // only check for gestures when the timer is done
            if (gesture_timer >= 0)
            {
                gesture_timer -= gameTime.ElapsedGameTime.Milliseconds;
                return gestures;
            }

            // check all the joints in the player
            foreach (Joint j in player.Joints)
            {
                if (j.JointType == JointType.ShoulderRight)
                    p_rshoulder = new Vector3(j.Position.X, j.Position.Y, j.Position.Z);

                if (j.JointType == JointType.ShoulderLeft)
                    p_lshoulder = new Vector3(j.Position.X, j.Position.Y, j.Position.Z);

                if (j.JointType == JointType.WristRight)
                {
                    Vector3 joint = new Vector3(j.Position.X, j.Position.Y, j.Position.Z);
                    v_rwrist = joint - p_rwrist; // update the wrists velocity
                    if (joint.X - p_rshoulder.X < 0 && p_rshoulder.X - joint.X > distance_threshold)
                    {
                        gesture_timer = 300.0f;
                        gestures[(int)gesture.left] = true;
                    }
                    else if (joint.X - p_rshoulder.X > distance_threshold)
                    {
                        gesture_timer = 300.0f;
                        gestures[(int)gesture.right] = true;
                    }

                    p_rwrist = joint;
                }
                if (j.JointType == JointType.WristLeft)
                {
                    Vector3 joint = new Vector3(j.Position.X, j.Position.Y, j.Position.Z);

                    v_lwrist = joint - p_lwrist; // update the wrists velocity
                    if (joint.X - p_lshoulder.X > 0 && joint.X - p_lshoulder.X > distance_threshold)
                    {
                        gesture_timer = 300.0f;
                        gestures[(int)gesture.rotateL] = true;
                    }
                    else if (p_lshoulder.X - joint.X > distance_threshold)
                    {
                        gesture_timer = 300.0f;
                        gestures[(int)gesture.rotateR] = true;
                    }
                    p_lwrist = joint;
                }

            }
            return gestures;
        }
    }

}
