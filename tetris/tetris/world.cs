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
   
        // for use in random piece creation
        enum block { empty, BX, LN, Z, S, T, L, LF };
        
        List<Piece> worldpieces;
        Piece activePiece;

        public Texture2D testtxtr;

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
            
            moveDown(gameTime);
        }
        
        private void moveDown(GameTime gameTime){
        
        }
        public virtual void rotateRight()
        {

        }

        public virtual void rotateLeft()
        {

        }
        public void Draw(SpriteBatch SB)
        {
            SB.Draw(screenback, new Rectangle(0, 0, (int)gamesize.X, (int)gamesize.Y), Color.White);

            // temporary gameplace
            int grid_x = ((int)gamesize.X / 2) - (((int)gridsize.X * gridunit)/2);
            SB.Draw(gridback, new Rectangle( grid_x , 0,
                       ( (int)gridsize.X * gridunit ) , (int)gridsize.Y * gridunit ), Color.White);

            // test output for grid
            for (int i = 0; i < (int)gridsize.Y; i++ )
            {
                for (int j = 0; j < (int)gridsize.X; j++)
                {
                    SB.Draw(testtxtr, new Rectangle(grid_x + (j * gridunit), (i)*gridunit, 
                                                     gridunit,gridunit), Color.White);
                }
            }
        }
        
    }
}
