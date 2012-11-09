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
        double fall_rate;
        enum Rot{left,right};
        Piece()
        {

        }
        Piece(Texture2D sprite, Vector2 position){
            this.sprite = sprite;
            this.pos = position;
        }
        void rotate(Rot rot)
        {

        }
    }
}
