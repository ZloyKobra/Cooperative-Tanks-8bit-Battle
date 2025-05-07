using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CoopTanks.Code.GameObjects
{
    class Walls 
    {
        public static List<Wall> walls = new List<Wall>();

        public static void CreateWalls()
        {
            // Пример расположения стен
            for (int x = 0; x < 10; x++)
            {
                walls.Add(new Wall(new Vector2(x * 32, 0)));
                walls.Add(new Wall(new Vector2(x * 32, 9 * 32)));
            }
            for (int y = 1; y < 9; y++)
            {
                walls.Add(new Wall(new Vector2(0, y * 32)));
                walls.Add(new Wall(new Vector2(9 * 32, y * 32)));
            }
        }
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
