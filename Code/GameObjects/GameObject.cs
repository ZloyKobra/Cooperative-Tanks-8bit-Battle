using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CoopTanks.Code.GameObjects
{
    // Базовый класс для игровых объектов
    public class GameObject
    {
        public const int TileSize = 32;
        public Vector2 position;
        static public Texture2D Texture { get; set; }
        public bool IsSolid { get; set; } = true;

        public GameObject()
        {
            position = Vector2.Zero;
        }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual void Update(GameTime gameTime) { }

        public virtual Rectangle GetBounds()
        {
            return new Rectangle(
                (int)position.X,
                (int)position.Y,
                Texture?.Width ?? 0,
                Texture?.Height ?? 0
            );
        }
    }
}