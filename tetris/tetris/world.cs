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
        // Textures for the background and the playable area's background
        public Texture2D gridback, screenback; 

        // the different textures for the block types
        // size depends on the different number of blocks
        public Texture2D [] block_tex;
        
        // gamesize - the length and height of the playable area in 
        //            pixels
        // gridsize - the number of rows and columns that the game will have
        public Vector2 gamesize,gridsize;
        
        // the size in pixels of each block
        int gridunit;

        // current total score
        public ulong score;

        // falling speed of the blocks
        // should probably change to int
        float speed;
   
        // Types of blocks available for createon
        enum block { BX, LN, Z, S, T, L, LF, NONE };

        // the texture grid 
        // will contain what texture goes in the block
        block[][] blocks;

        // movable Piece that is added to blocks
        // when a collision is detected
        Piece activePiece;

        // random number generator
        Random rand;

        //
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
            blocks = new block[y][];


            // test to populate the playable area
            for (int i = 0; i < y; i++)
            {
                blocks[i] = new block[x];
                for (int j = 0; j < x; j++)
                {
                   blocks[i][j] = (block)rand.Next(8);
                }
            }

        }

        // takes in a row index and returns true if the row is completed
        bool row_full( int row )
        {
            foreach ( block b in blocks[row] ){
                if (b == block.NONE) return false;
            }
            return true;
        }

        // Updates the game by:
        //     moving the activePiece down
        //     TODO: checking for collision
        //     TODO: adding piece to backround if colided
        //     removing completed rows
        //     TODO: moving the rows down after some have been removed
        //     increasing  score
        public void Update(GameTime gameTime)
        {
            moveDown(gameTime);

            // NOTE: could possibly change to integers?
            float addedscore = 0;
            // more rows at a time give you a higher multiplier
            float multiplier = 1.0f;

            // Checks for completed rows
            for (int i = 0; i < (int)gridsize.Y; i++)
            {
                
                if (row_full(i))
                {
                    addedscore += 100 * multiplier;
                    multiplier += .5f;
                    for (int j = 0; j < (int)gridsize.X; j++)
                    {
                       blocks[i][j] = block.NONE ;
                    }

                }
            }
            score += (ulong)addedscore;
        }
        
        // moves down active Piece
        private void moveDown(GameTime gameTime){
        
        }

        // Rotates activePiece
        // Note : can possibly merge rotations into one function
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

            // outputs the blocks in the back
            for (int i = 0; i < (int)gridsize.Y; i++ )
            {
                for (int j = 0; j < (int)gridsize.X; j++)
                {
                    int index = (int)blocks[i][j];
                    if ( index >= 7 ) { continue; }
                    SB.Draw(block_tex[ index ], new Rectangle(grid_x + (j * gridunit), (i)*gridunit, 
                                                     gridunit,gridunit), Color.White);
                }
            }
        }
        
    }
}
