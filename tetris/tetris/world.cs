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

        // falling speed of the blocks in steps per second
        // should probably change to int
        float step_time;
   

        // the texture grid 
        // will contain what texture goes in the block
        Piece.block[][] blocks;

        // movable Piece that is added to blocks
        // when a collision is detected
        Piece activePiece;
        bool isactive;

        // random number generator
        Random rand;

        Vector2 startpos;

        //
        public world()
        {
            score = 0;
            step_time = 1000.0f;
            rand = new Random();
        }


        public world(int x, int y, int unit, Vector2 gamesize)
        {
            gridsize = new Vector2(x, y);
            this.gamesize = gamesize;
            gridunit = unit;
            score = 0;
            step_time = 1000.0f;
            rand = new Random();
            block_tex = new Texture2D[7];
            blocks = new Piece.block[y][];

            startpos = new Vector2( ((int)this.gamesize.X / 2) - 2*unit,0 );
            isactive = false;

            
            // test to populate the playable area
            for (int i = 0; i < y; i++)
            {
                blocks[i] = new Piece.block[x];
                for (int j = 0; j < x; j++)
                {
                   blocks[i][j] = Piece.block.NONE;
                }
            }

        }

        // takes in a row index and returns true if the row is completed
        bool row_full( int row )
        {
            
            foreach ( Piece.block b in blocks[row] ){
                if (b == Piece.block.NONE) return false;
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
            if (isactive == false)
            {
                int piece = rand.Next(7);
                activePiece = new Piece((Piece.block)piece, block_tex[piece],startpos);
                isactive = true;
                
            }
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
                       blocks[i][j] = Piece.block.NONE ;
                    }
                    for (int j = i; j > 0; j-- )
                        swap_rows(j, j - 1);

                }
            }
            score += (ulong)addedscore;
        }

        void swap_rows(int index1, int index2)
        {
            Piece.block[] temp = blocks[index1];
            blocks[index1] = blocks[index2];
            blocks[index2] = temp;
        }
        
        // moves down active Piece
        float elapsed_t;
        private void moveDown(GameTime gameTime)
        {
            elapsed_t += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsed_t > step_time)
            {
                elapsed_t = 0;
                Vector2 oldpos = activePiece.pos;
                activePiece.move(new Vector2(0, gridunit)); 
                switch (collision())
                {
                    case 1:
                        activePiece.pos = oldpos;
                        break;
                    case 2:
                        activePiece.pos = oldpos;
                        addpiece();
                        break;
                    default:
                        break;
                }
                //activePiece.rotateLeft();

            }
        }

        // Rotates activePiece
        // Note : can possibly merge rotations into one function
        public void rotateRight()
        {
            activePiece.rotateRight();
        }
        public void rotateLeft()
        {
            activePiece.rotateLeft();
        }

        public void moveLeft()
        {
            activePiece.move(new Vector2(-gridunit, 0));
        }
        
        public void moveRight()
        {
            activePiece.move(new Vector2(gridunit, 0));
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

            activePiece.Draw(SB, gridunit);
        }

        private int collision()
        {
            // returns:
            // 0  - no collision
            // 1  - wall/ side collisions ** might need to change based on type of movement
            // 2  - brick collisions in the gameworld or the bottom

            // there is some terrible stuff happening here
            //     it can probably be fixed by having active piece's position 
            //     be updated through the index in the grid as opposed to by pixel
            int X = ((int)activePiece.pos.X - (int)gridsize.X / 2) / gridunit + (int)activePiece.bounding_box.X;
            int Y = ((int)activePiece.pos.Y - (int)gridsize.Y / 2) / gridunit + (int)activePiece.bounding_box.Y;
            if ( X < 0 || X + activePiece.bounding_box.Width >= gridsize.X) 
                return 1;
            if (Y + activePiece.bounding_box.Height >= gridsize.Y) 
                return 2;
            for (int i = 0; i < (int)activePiece.bounding_box.Height; i++)
            {
                for (int j = 0; j < (int)activePiece.bounding_box.Width; j++)
                {
                    if( activePiece.shape[i,j] == 1 && blocks[Y+i][X+j] != Piece.block.NONE)
                    return 2;
                }
            }
                
            return 0; 
        }

        private void addpiece()
        {
            int X = ((int)activePiece.pos.X - (int)gridsize.X/2) / gridunit + (int)activePiece.bounding_box.X;
            int Y = ((int)activePiece.pos.Y - (int)gridsize.Y/2) / gridunit + (int)activePiece.bounding_box.Y;
            for (int i = 0; i < (int)activePiece.bounding_box.Height; i++)
            {
                for (int j = 0; j < (int)activePiece.bounding_box.Width; j++)
                {
                    if (activePiece.shape[i, j] == 1){
                    } blocks[Y + i][X + j] = activePiece.BLOCK;
                }
            }
            isactive = false;
        }
    }

}
