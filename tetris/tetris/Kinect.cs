using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectTracking
{

    /// <summary>
    /// 
    /// Simple Kinect Interface
    ///     
    /// 
    /// </summary>
    class Kinect
    {
        // kinect variables
        KinectSensor kinectSensor = null;

        public Skeleton player;

        Skeleton[] playerData;

        // USE to see if the kinect is enabled
        public bool enabled
        {
            get
            {
                return kinectSensor != null;
            }
        }
        public Kinect()
        {
        }
        ~Kinect()
        {
            if(enabled)
            kinectSensor.Stop();
        }

        public void pause()
        {
            kinectSensor.Stop();
        }
        
        public void start()
        {
            kinectSensor.Start();
        }
        
        public void initialize( int elevationAngle = 0 )
        {

            try { kinectSensor = KinectSensor.KinectSensors[0]; }
            catch (Exception e)
            {
                Console.WriteLine("kinect not detected, continuing with kinect disabled {0}",e);
                return;
            }

            // limits elevation angle to keep the motors from trying too extreme an angle
            if (elevationAngle >= 26 )
            {
                elevationAngle = 26;
            }
            else if (elevationAngle <= -26)
            {
                elevationAngle = -26;
            }

            // Only initializes Skeletal Tracking
            kinectSensor.SkeletonStream.Enable();

            // set a call back function to process skeleton data
            kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectSkeletonFrameReadyCallback);
            kinectSensor.Start();
            kinectSensor.ElevationAngle = elevationAngle;

        }

        // Process skeleton data
        void kinectSkeletonFrameReadyCallback(object sender, SkeletonFrameReadyEventArgs skeletonFrames)
        {
            // Open the skeleton
            using (SkeletonFrame skeleton = skeletonFrames.OpenSkeletonFrame())
            {

                // ensure that there is a skeleton
                if (skeleton != null)
                {
                    // if there are no players or a new player has entered or left
                    // resize playerdata to fit exactly all the players
                    if (playerData == null || this.playerData.Length != skeleton.SkeletonArrayLength)
                    {
                        this.playerData = new Skeleton[skeleton.SkeletonArrayLength];
                    }

                    // store info on all players
                    skeleton.CopySkeletonDataTo(playerData);
                }
            }

            if (playerData != null)
            {
                foreach (Skeleton skeleton in playerData)
                {
                    // if a player is being tracked
                    // that is the player we want to focus on
                    // NOTE: the kinect's default is to track the first two skeletons that it sees,
                    //      this means that if left unchecked, when a person is playing a one player game
                    //      if the kinect "sees" another player it could focus on them and not the Actual player
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        player = skeleton;
                    }
                }
            }
        }
    }
}
