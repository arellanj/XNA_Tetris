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
        public Texture2D block_tex;

        // warning textures;
        public Texture2D border, warning;
        
        // gamesize - the length and height of the playable area in 
        //            pixels
        // gridsize - the number of rows and columns that the game will have
        public Vector2 gamesize,gridsize;
        
        // the size in pixels of each block
        int gridunit;

        // warning system
        bool give_warning;
        float warningOffset; 
        int warnalpha;

        // current total score
        public ulong score;

        // level
        public int level;

        // falling speed of the blocks in steps per second
        // should probably change to int
        float stepTime;

        // Pause boolean
        bool paused;
   

        // the texture grid 
        // will contain what texture goes in the block
        Piece.block[][] blocks;

        // movable Piece that is added to blocks
        // when a collision is detected
        Piece nextPiece;
        Piece activePiece;
        bool isactive;

        // random number generator
        Random rand;

        Vector2 startpos, shelfpos;

        // font
        private SpriteFont scorefont;

        //
        public world()
        {
            score = 0;
            stepTime = 1000.0f;
            rand = new Random();
        }


        public world(int x, int y, int unit, Vector2 gamesize)
        {
            gridsize = new Vector2(x, y);
            this.gamesize = gamesize;
            gridunit = unit;
            score = 0;
            level = 1;
            stepTime = 1000.0f;

            paused = false;

            rand = new Random();
   
            blocks = new Piece.block[y][];

            startpos = new Vector2( ((int)this.gamesize.X / 2) - 2*unit,2*unit );
            shelfpos = new Vector2(0, 0);
            isactive = false;

            int piece = rand.Next(7);
            int r, g, b;
            r = rand.Next(85, 255);
            g = rand.Next(85, 255);
            b = rand.Next(85, 255);
            nextPiece = new Piece((Piece.block)piece, new Color(r, g, b), shelfpos);

            
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

            block_tex = Content.Load<Texture2D>("Shape Textures\\Bar");
            border = Content.Load<Texture2D>("bounds");
            warning = Content.Load<Texture2D>("warnborder");


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
            if (paused) return;

            if (game_lost())
            {
                reset();
            }

            if (give_warning = warn_player())
            {
               
                float sinval = (float)Math.Sin(gameTime.TotalGameTime.Milliseconds * 2*Math.PI / 1000);
                warnalpha = (int) (255*(( sinval / 2) + 1));
                
            }
            
            if (isactive == false)
            {
                int piece = rand.Next(7);
                int r, g, b;
                r = rand.Next(85,255);
                g = rand.Next(85,255);
                b = rand.Next(85, 255);
                nextPiece.pos = startpos;
                activePiece = nextPiece;
                nextPiece = new Piece((Piece.block)piece, new Color(r, g, b), shelfpos);

                Vector2 start_offset = new Vector2(0, activePiece.bounding_box.Y + activePiece.bounding_box.Height);
                activePiece.move(-start_offset*gridunit);
                isactive = true;
                
            }
            moveDown(gameTime);
            


            // NOTE: could possibly change to integers?
            int addedscore = 0;   
            // more rows at a time give you a higher multiplier
            int multiplier = 1;

            // Checks for completed rows
            for (int i = 0; i < (int)gridsize.Y; i++)
            {
                
                if (row_full(i))
                {
                    addedscore += 100 * multiplier*level;
                    multiplier += 2;
                    for (int j = 0; j < (int)gridsize.X; j++)
                    {
                       blocks[i][j] = Piece.block.NONE ;
                    }
                    for (int j = i; j > 0; j-- )
                        swap_rows(j, j - 1);

                }
            }
            score += (ulong)addedscore;

            check_level();

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
            if (elapsed_t > stepTime)
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
            if (paused) return;
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
            if (paused) return;
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
            if (paused) return;
            Vector2 oldpos = activePiece.pos;
            activePiece.move(new Vector2(-gridunit, 0));
            if (collision() != 0)
            {
                activePiece.pos = oldpos;
            }
        }

        public void moveRight()
        {
            if (paused) return;
            Vector2 oldpos = activePiece.pos;
            activePiece.move(new Vector2(gridunit, 0));
            if (collision() != 0)
            {
                activePiece.pos = oldpos;
            }
        }

        public void hardDrop()
        {
            if (paused) return;
            Vector2 oldpos = activePiece.pos;
            do
            {
                oldpos = activePiece.pos;
                activePiece.move(new Vector2(0, gridunit));
            } while (collision() != 2);
            activePiece.pos = oldpos;
            addpiece();
        }

        public void pause()
        {
            paused = !paused;
        }

        public void Draw(SpriteBatch SB)
        {
            int grid_x = ((int)gamesize.X / 2) - (((int)gridsize.X * gridunit) / 2);
            int grid_y = 0;// ((int)gamesize.Y / 2) - (((int)gridsize.Y * gridunit) / 2);

            SB.Begin();
            {
                SB.Draw(screenback, new Rectangle(0, 0, (int)gamesize.X, (int)gamesize.Y), Color.White);

                // temporary gameplace
                SB.Draw(gridback, new Rectangle(grid_x, grid_y,
                       ((int)gridsize.X * gridunit), (int)gridsize.Y * gridunit), Color.White);
            }
            SB.End();

            SB.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
            {
                for (int i = 0; i < (int)gridsize.X; i++)
                {
                    SB.Draw(border, new Rectangle(grid_x + (i * gridunit), grid_y + (2) * gridunit,
                                                     gridunit, gridunit), Color.White);
                }
            }
            SB.End();

            SB.Begin();

            // outputs the blocks in the back
            for (int i = 0; i < (int)gridsize.Y; i++)
            {
                for (int j = 0; j < (int)gridsize.X; j++)
                {
                    int index = (int)blocks[i][j];
                    if (index >= 7) { continue; }
                    SB.Draw(block_tex, new Rectangle(grid_x + (j * gridunit), grid_y + (i) * gridunit,
                                                     gridunit, gridunit), Color.White);
                }
            }



            // draws the active piece
            activePiece.Draw(SB, gridunit, block_tex);
            // draws the active piece
            nextPiece.Draw(SB, gridunit, block_tex);

            // draws the score
            SB.DrawString(scorefont, "SCORE:" + score, new Vector2(0, 0), Color.White);

            SB.End();
            if (give_warning)
            {
                SB.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
                {
                    Color alpha = Color.White;
                    alpha.A = (byte)warnalpha;
                    SB.Draw(warning, new Rectangle(0, 0, (int)gamesize.X, (int)gamesize.Y), alpha);
                }
                SB.End();
            }
            if (paused)
            {
                SB.Begin();
                {
                    Color alpha = Color.Black;
                    alpha.A = 200;
                    SB.Draw(border, new Rectangle(0, 0, (int)gamesize.X, (int)gamesize.Y), alpha);
                    SB.DrawString(scorefont, "PAUSED", new Vector2(gamesize.X / 2, gamesize.Y / 2), Color.White);
                }
                SB.End();
            }
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
            level = 1;
            stepTime = 1000.0f;
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
            int grid_y = 0; // ((int)gamesize.Y / 2) - (((int)gridsize.Y * gridunit) / 2);
            int X = (((int)activePiece.pos.X - grid_x) / gridunit) + (int)activePiece.bounding_box.X;
            int Y = (((int)activePiece.pos.Y - grid_y) / gridunit) + (int)activePiece.bounding_box.Y;

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
                if (blocks[2][i] != Piece.block.NONE) return true;
            }
            return false;
        }


        private bool warn_player()
        {
            for (int i = 0; i < gridsize.X; i++)
            {
                if (blocks[3][i] != Piece.block.NONE) return true;
            }
            return false;
        }
        private void check_level()
        {
            if (score >(ulong) level * 1000)
            {
                level++;
                stepTime *= 0.9f;
            }
        }
        private void addpiece()
        {

            int grid_x = ((int)gamesize.X / 2) - (((int)gridsize.X * gridunit) / 2);
            int grid_y = 0;// ((int)gamesize.Y / 2) - (((int)gridsize.Y * gridunit) / 2);
            int X = (((int)activePiece.pos.X - grid_x) / gridunit) + (int)activePiece.bounding_box.X;
            int Y = (((int)activePiece.pos.Y - grid_y)) / gridunit + (int)activePiece.bounding_box.Y;
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
