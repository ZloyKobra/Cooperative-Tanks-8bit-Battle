﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CoopTanks.Code.GameObjects
{
    class Walls 
    {
        public static List<Wall> walls = new List<Wall>();
    }

    class Wall : GameObject
    {
        public static new Texture2D Texture { get; set; }

        public Wall(Vector2 initialPosition) 
        { 
            position = initialPosition;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, position, Color.White);
        }

        public override void Update(GameTime gameTime) { }

        public override Rectangle GetBounds()
        {
            return new Rectangle(
                (int)position.X,
                (int)position.Y,
                Texture.Width,
                Texture.Height
            );
        }
    }
}
