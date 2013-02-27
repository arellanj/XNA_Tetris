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

        // font
        private SpriteFont scorefont;

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

        public void Load(ContentManager Content)
        {
            screenback = Content.Load<Texture2D>("Backgrounds\\Back");
            gridback = Content.Load<Texture2D>("Backgrounds\\GameArea");

            // NOTE :  the order here matters!
            // it will be compared with the enumerated type blocks
            // TODO : rename the textures to the color of the blocks instead of
            //        using the name of the piece
            block_tex[0] = Content.Load<Texture2D>("Shape Textures\\Box");
            block_tex[1] = Content.Load<Texture2D>("Shape Textures\\Bar");
            block_tex[2] = Content.Load<Texture2D>("Shape Textures\\Z");
            block_tex[3] = Content.Load<Texture2D>("Shape Textures\\S");
            block_tex[4] = Content.Load<Texture2D>("Shape Textures\\T");
            block_tex[5] = Content.Load<Texture2D>("Shape Textures\\L");
            block_tex[6] = Content.Load<Texture2D>("Shape Textures\\J");

            scorefont = Content.Load<SpriteFont>("scoreFont");
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
            if (game_lost())
            {
                reset();
            }

            if (isactive == false)
            {
                int piece = rand.Next(7);
                activePiece = new Piece((Piece.block)piece, block_tex[piece], startpos);

                Vector2 start_offset = new Vector2(0, activePiece.bounding_box.Y + activePiece.bounding_box.Height);
                activePiece.move(-start_offset*gridunit);
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
            int[,] oldshape = activePiece.shape;
            Rectangle oldBB = activePiece.bounding_box;
            activePiece.rotateRight();
            if (collision() != 0)
            {
                activePiece.shape = oldshape;
                activePiece.bounding_box = oldBB;
            }
        }
        public void rotateLeft()
        {
            int[,] oldshape = activePiece.shape;
            Rectangle oldBB = activePiece.bounding_box;

            activePiece.rotateLeft();
            if (collision() != 0)
            {
                activePiece.shape = oldshape;
                activePiece.bounding_box = oldBB;
            }
        }

        public void moveLeft()
        {
            Vector2 oldpos = activePiece.pos;
            activePiece.move(new Vector2(-gridunit, 0));
            if (collision() != 0)
            {
                activePiece.pos = oldpos;
            }
        }

        public void moveRight()
        {
            Vector2 oldpos = activePiece.pos;
            activePiece.move(new Vector2(gridunit, 0));
            if (collision() != 0)
            {
                activePiece.pos = oldpos;
            }
        }

        public void hardDrop()
        {
            Vector2 oldpos = activePiece.pos;
            do
            {
                oldpos = activePiece.pos;
                activePiece.move(new Vector2(0, gridunit));
            } while (collision() != 2);
            activePiece.pos = oldpos;
            addpiece();
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

            // draws the active piece
            activePiece.Draw(SB, gridunit);

            // draws the score
            SB.DrawString(scorefont,"SCORE:"+score,new Vector2(0,0),Color.White);
        }

        private void reset()
        {
            activePiece = new Piece();
            isactive = false;

            for (int i = 0; i < gridsize.Y; i++)
            {
                for (int j = 0; j < gridsize.X; j++)
                {
                    blocks[i][j] = Piece.block.NONE;
                }
            }

            score = 0;
        }

        private int collision()
        {
            // returns:
            // 0  - no collision
            // 1  - wall/ side collision 
            // 2  - brick collisions in the gameworld or the bottom

            // there is some terrible stuff happening here
            //     it can probably be fixed by having active piece's position 
            //     be updated through the index in the grid as opposed to by pixel
            // X - the x index in grid of the upper left x index in gamegrid that activePiece is in
            // Y - the y index in grid of the upper left y index in gamegrid that activePiece is in
            int grid_x = ((int)gamesize.X / 2) - (((int)gridsize.X * gridunit) / 2);
            int X = (((int)activePiece.pos.X - grid_x) / gridunit) + (int)activePiece.bounding_box.X;
            int Y = ((int)activePiece.pos.Y ) / gridunit + (int)activePiece.bounding_box.Y;

            if ( X < 0 || X + activePiece.bounding_box.Width > gridsize.X) 
                return 1; // collision with the sides of the game grid

            if (Y + activePiece.bounding_box.Height > gridsize.Y) 
                return 2; // Collision with the bottom of the game grid

            for (int i = 0; i < (int)activePiece.bounding_box.Height; i++)
            {
                for (int j = 0; j < (int)activePiece.bounding_box.Width; j++)
                {
                    // if the index is inside a valix range
                    if (Y + i < gridsize.Y && Y + i >= 0 && X + j < gridsize.X && X + j >=0)
                    {
                        if (activePiece.shape[(int)activePiece.bounding_box.Y + i, (int)activePiece.bounding_box.X + j] == 1 && blocks[Y + i][X + j] != Piece.block.NONE)
                            return 2; // collision with the inactive blocks
                    }
                }
            }
                
            return 0; 
        }

        private bool game_lost()
        {
            for (int i = 0; i < gridsize.X; i++)
            {
                if (blocks[0][i] != Piece.block.NONE) return true;
            }
            return false;
        }

        private void addpiece()
        {

            int grid_x = ((int)gamesize.X / 2) - (((int)gridsize.X * gridunit) / 2);
            int X = (((int)activePiece.pos.X - grid_x) / gridunit) + (int)activePiece.bounding_box.X;
            int Y = ((int)activePiece.pos.Y) / gridunit + (int)activePiece.bounding_box.Y;
            for (int i = 0; i < (int)activePiece.bounding_box.Height; i++)
            {
                for (int j = 0; j < (int)activePiece.bounding_box.Width; j++)
                {
                    if (activePiece.shape[(int)activePiece.bounding_box.Y + i, (int)activePiece.bounding_box.X + j] == 1)
                    {
                        // if the index is inside a valix range
                        if (Y + i < gridsize.Y && Y + i >= 0 && X + j < gridsize.X && X + j >= 0)
                        {
                            blocks[Y + i][X + j] = activePiece.BLOCK;
                        }
                    }
                }
            }
            isactive = false;
        }
    }

}
