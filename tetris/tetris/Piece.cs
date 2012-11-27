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
    class Piece
    {
        /*
         * Variables
         */

        // texture that it will draw
        Texture2D blocksprite;

        // current position on the screen
        public Vector2 pos;
        Vector2 rotation_point;

        // boundingbox parameters
        // x,y - the upper left index of where the shape starts
        // width,height - the width and height in number of blocks
        //example : z block
        /*------------------
         * | 0 | 0 | 0 | 0 |
         * -----------------
         * | 0 | 1 | 1 | 0 |
         * -----------------
         * | 0 | 0 | 1 | 1 |
         * -----------------
         * | 0 | 0 | 0 | 0 |
         * -----------------
         * yelds : rectangle(1,1,3,2)
         */
        public Rectangle bounding_box;


        // used to determine shape of the block
        // for now defaulted to 4x4 matrix
        public int[,] shape;


        // Types of blocks available for createon
        public enum block { BX, LN, Z, S, T, L, LF, NONE };

        /*
         * Constructors
         */
        public block BLOCK;
        public Piece()
        {
            this.shape = new int[4, 4];
        }

        public Piece(block shape_index, Texture2D sprite , Vector2 position){
            this.blocksprite = sprite;
            this.pos = position;
            this.BLOCK = shape_index;
            this.shape = make_shape((int)shape_index);
            this.bounding_box = calculate_bounding_box();

        }

        /*
         * Member Functions
         */
         
        // changes position based on direction
        // returns true for successfull movement
        // NOTE : direction is assumed to NOT be normalized
        public void move(Vector2 position)
        {
            
            pos += position;
        }



        // Rotations, rotates the 4x4 matrix left or right
        // NOTE: we can probably change this to rotate about a certain block instead of rotating the entire 4x4 matrix
        //      this would make the rotations look nicer
        public void rotateLeft()
        {
            int[,] temp = new int[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    temp[j, i] = shape[i, 3-j];
                }
            }
            shape = temp;
            bounding_box = calculate_bounding_box();
        }
        public void rotateRight()
        {
            int[,] temp = new int[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    temp[j, i] = shape[3-i, j];
                }
            }
            shape = temp;
            bounding_box = calculate_bounding_box();
        }

        // draws the piece to the screen
        public virtual void Draw(SpriteBatch sb, int num_pix)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (shape[i, j] == 1)
                    {
                        sb.Draw(blocksprite, new Rectangle((int)pos.X + j * num_pix, (int)pos.Y + i * num_pix, num_pix, num_pix), Color.White);
                    }
                }

            }
            return;
        }

        private Rectangle calculate_bounding_box()
        {
            int xmin = 4;
            int xmax = -1;
            int ymin = 4;
            int ymax = -1;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (shape[i, j] == 1)
                    {
                        if (i < ymin)
                            ymin = i;
                        if (i > ymax)
                            ymax = i;
                        if (j < xmin)
                            xmin = j;
                        if (j > xmax)
                            xmax = j;
                    }
                }
            }
            return new Rectangle(xmin, ymin, xmax - xmin + 1, ymax - ymin + 1);
        }

        public int[,] make_shape(int index)
        {
            int[,] newshape;
            switch (index)
            {
                case 0:
                    newshape = new int[4, 4] { { 0, 0, 0, 0 }, { 0, 1, 1, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 } };
                    rotation_point = new Vector2(2, 2);
                    break;
                case 1:
                    newshape = new int[4, 4] { { 0, 0, 1, 0 }, { 0, 0, 1, 0 }, { 0, 0, 1, 0 }, { 0, 0, 1, 0 } };
                    rotation_point = new Vector2(2, 2);
                    break;
                case 2:
                    newshape = new int[4, 4] { { 0, 0, 0, 0 }, { 0, 1, 1, 0 }, { 0, 0, 1, 1 }, { 0, 0, 0, 0 } };
                    rotation_point = new Vector2(2, 2);
                    break;
                case 3:
                    newshape = new int[4, 4] { { 0, 0, 0, 0 }, { 0, 1, 1, 0 }, { 1, 1, 0, 0 }, { 0, 0, 0, 0 } };
                    rotation_point = new Vector2(2, 2);
                    break;
                case 4:
                    newshape = new int[4, 4] { { 0, 0, 0, 0 }, { 0, 1, 0, 0 }, { 1, 1, 1, 0 }, { 0, 0, 0, 0 } };
                    rotation_point = new Vector2(2, 2);
                    break;
                case 5:
                    newshape = new int[4, 4] { { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 } };
                    rotation_point = new Vector2(2, 2);
                    break;
                case 6:
                    newshape = new int[4, 4] { { 0, 0, 0, 0 }, { 0, 1, 1, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 } };
                    rotation_point = new Vector2(2, 2);
                    break;
                default:
                    newshape = new int[4, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
                    rotation_point = new Vector2(2, 2);
                    break;
            }
            return newshape;
        }

    }
}
