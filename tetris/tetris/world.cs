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
        public Texture2D [] block_tex;
        public Vector2 gamesize,gridsize;
        int gridunit;
        public ulong score;
        float speed;
   
        // for use in random piece creation
        enum block { BX, LN, Z, S, T, L, LF, NONE };
        block[,] blocks;
        Piece activePiece;

        Random rand;

        public world()
        {
            score = 0;
            speed = 1.0f;
            rand = new Random();
        }
        public world(int x, int y, int unit)
        {
            gridsize = new Vector2(x, y);
            gridunit = unit;
            score = 0;
            speed = 1.0f;
            rand = new Random();
            block_tex = new Texture2D[7];
            blocks = new block[x, y];

            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    
                   blocks[j, i] = (block)rand.Next(8);
                }
            }

        }

        public void Update(GameTime gameTime){

            for (int i = 0; i < (int)gridsize.Y; i++)
            {
                bool gaps = false;
                for (int j = 0; j < (int)gridsize.X; j++)
                {
                    if ( blocks[j,i] == block.NONE) {
                        gaps = true;
                        break;
                    }
                }
                if (!gaps)
                {
                    for (int j = 0; j < (int)gridsize.X; j++)
                    {
                       blocks[j, i] = block.NONE ;
                    }

                }
            }
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
                    int index = (int)blocks[j,i];
                    if ( index >= 7 ) { continue; }
                    SB.Draw(block_tex[ index ], new Rectangle(grid_x + (j * gridunit), (i)*gridunit, 
                                                     gridunit,gridunit), Color.White);
                }
            }
        }
        
    }
}
