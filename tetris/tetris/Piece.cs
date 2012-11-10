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

        Texture2D sprite;
        Vector2 pos;
        enum orientation{up, down, left, right};
        public Piece()
        {

        }
        public Piece(Texture2D sprite, Vector2 position){
            this.sprite = sprite;
            this.pos = position;
        }

        public bool move(Vector2 direction)
        {
            if (CollisionCheck(direction) == true){
                return false;
            }
            return true;
        }

        public bool CollisionCheck(Vector2 direction)
        {
            throw new NotImplementedException();
        }

    }
}
