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
        public Texture2D gridback, screenback;
 
        public Vector2 gamesize,gridsize;
        int gridunit;
        public ulong score;
        float speed;


        public world()
        {
            score = 0;
            speed = 1.0f;
        }
        public world(int x, int y, int unit)
        {
            gridsize = new Vector2(x, y);
            gridunit = unit;
            score = 0;
            speed = 1.0f;
        }

        public void Update(GameTime gameTime){

        }

        public void Draw(SpriteBatch SB)
        {
            SB.Draw(screenback, new Rectangle(0, 0, (int)gamesize.X, (int)gamesize.Y), Color.White);

            // temporary gameplace
            SB.Draw(gridback, new Rectangle(((int)gamesize.X / 2) - (((int)gridsize.X * gridunit)/2) , 0,
                       ( (int)gridsize.X * gridunit ) , (int)gridsize.Y * gridunit ), Color.White);
        }
        
    }
}
