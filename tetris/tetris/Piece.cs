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
        Vector2 pos;

        // used to determine shape of the block
        // for now defaulted to 4x4 matrix
        bool[,] shape;

        /*
         * Constructors
         */
         
        public Piece(){}
        public Piece(Texture2D sprite , Vector2 position){
            this.blocksprite = sprite;
            this.pos = position;
            this.shape = new bool [4,4];
        }

        // changes position based on direction
        // returns true for successfull movement
        // NOTE : direction is assumed to NOT be normalized
        public bool move(Vector2 direction)
        {
            if (CollisionCheck(direction) == true){
                return false;
            }
            pos += direction;
            return true;
        }

        // draws the piece to the screen
        public virtual void Draw(SpriteBatch sb)
        {
            return;
        }

        public bool CollisionCheck(Vector2 direction)
        {
            throw new NotImplementedException();
        }

    }
}
