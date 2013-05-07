using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using KinectTracking;
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
        Kinect kinect;
        float distance_threshold = .2f;

        float gesture_timer;

        Vector3 p_lwrist, p_rwrist, v_lwrist, v_rwrist, p_lshoulder, p_rshoulder;
        Vector3 hip_pos;
        float vmax, vmin;


        // gestures      
        enum gesture { left, right, rotateL, rotateR, hdrop };
        int numGestures = 5;

        Texture2D style;

        // debug
        bool kinect_enable = true;

        // hand image
        Texture2D hand;
        Vector2 hand_r_pos;
        Vector2 hand_l_pos;


        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 720;

            this.graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            kinect = new Kinect();
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
            gameworld = new world(14, 20, 35,new Vector2(this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight));
            gesture_timer = 0;
            vmax = vmin = 0;
            hand_r_pos = Vector2.Zero;
            hand_l_pos = Vector2.Zero;
            if(kinect_enable)
                kinect.initialize(10);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);            // load all the textures that will be used in the game
            hand = Content.Load<Texture2D>("hand");
            gameworld.Load(Content);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //if(kinect_enable)kinect.Stop();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        // MUST CHANGE FOR KEYBOARD STUFF
        KeyboardState prev;

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
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && prev.IsKeyUp(Keys.Left))
            {
                gameworld.moveLeft();
            }

            
            // RIGHT - moves piece right
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && prev.IsKeyUp(Keys.Right))
            {
                gameworld.moveRight();
            }
            
            // UP - rotates clockwise
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && prev.IsKeyUp(Keys.Up))
            {
                gameworld.rotateLeft();
            }

            // DOWN - rotates counter-clockwise
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && prev.IsKeyUp(Keys.Down))
            {
                gameworld.rotateRight();
            }

            // SPACE - Hard Drop
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && prev.IsKeyUp(Keys.Space))
            {
                gameworld.hardDrop();
            }

            // P - pause
            if (Keyboard.GetState().IsKeyDown(Keys.P) && prev.IsKeyUp(Keys.P))
            {
                gameworld.pause();
            }
            prev = Keyboard.GetState();
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
            
            gameworld.Draw(spriteBatch);

            spriteBatch.Begin();
            spriteBatch.Draw(hand, new Rectangle(this.graphics.PreferredBackBufferWidth / 2 + (int)hand_r_pos.X, this.graphics.PreferredBackBufferHeight / 2 + (int)hand_r_pos.Y, 20, 20), Color.White);
            spriteBatch.Draw(hand, new Rectangle(this.graphics.PreferredBackBufferWidth / 2 + (int)hand_l_pos.X, this.graphics.PreferredBackBufferHeight / 2 + (int)hand_l_pos.Y, 20, 20), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
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
            if (kinect.player == null) return gestures;
            
            // only check for gestures when the timer is done
            if (gesture_timer >= 0)
            {
                gesture_timer -= gameTime.ElapsedGameTime.Milliseconds;
                return gestures;
            }

            // check all the joints in the player
            foreach (Joint j in kinect.player.Joints)
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
                        gesture_timer = 400.0f;
                        gestures[(int)gesture.rotateL] = true;
                    }
                    else if (p_lshoulder.X - joint.X > distance_threshold)
                    {
                        gesture_timer = 400.0f;
                        gestures[(int)gesture.rotateR] = true;
                    }
                    p_lwrist = joint;
                }
                if (j.JointType == JointType.HandRight)
                {
                    hand_r_pos = new Vector2(j.Position.X * this.graphics.PreferredBackBufferWidth - hip_pos.X, -j.Position.Y * this.graphics.PreferredBackBufferWidth + hip_pos.Y);
                }
                if (j.JointType == JointType.HandLeft)
                {
                    hand_l_pos = new Vector2(j.Position.X * this.graphics.PreferredBackBufferWidth - hip_pos.X, -j.Position.Y * this.graphics.PreferredBackBufferWidth + hip_pos.Y);
                }
                if(j.JointType == JointType.HipCenter)
                {
                    hip_pos = new Vector3(j.Position.X, j.Position.Y, j.Position.Z);
                }

            }
            return gestures;
        }
    }

}
