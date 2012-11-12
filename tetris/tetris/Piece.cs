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
        Texture2D blocksprite;
        Vector2 pos;
        bool[,] shape;
        public Piece()
        {

        }
        public Piece(Texture2D sprite, Vector2 position){
            this.blocksprite = sprite;
            this.pos = position;
            this.shape = new bool [4,4];
        }

        public bool move(Vector2 direction)
        {
            if (CollisionCheck(direction) == true){
                return false;
            }
            return true;
        }

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
