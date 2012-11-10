using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace tetris
{
    class world
    {
        Texture2D gridback, screenback; 
        Vector2 gridsize;
        ulong score;
        float speed;


        public world()
        {
            score = 0;
            speed = 1.0f;
        }
        public world(int x, int y)
        {
            gridsize = new Vector2(x, y);
            score = 0;
            speed = 1.0f;
        }

        public void Update(GameTime gameTime){

        }

        public void Draw()
        {

        }
        
    }
}
