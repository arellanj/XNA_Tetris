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
    class Particle
    {
        Color color;
        Texture2D tex;
        Vector2 pos, origin;
        float rot;
  
        public Particle(){    
        }

        public Particle(Texture2D tex, Vector2 position, Color color )
        {
            this.tex = tex;
            this.pos = position;
            this.color = color;
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            rot = 0.0f;
        }

        public void Draw( SpriteBatch SB ){
            SB.Draw(tex, pos, null, color, rot, origin,1.0f, SpriteEffects.None, 0f );
        }
        public float rotate( float radians ){
            rot += radians;
            rot %= (float)(2 * Math.PI);
            return rot += radians;
        }

    }
}
