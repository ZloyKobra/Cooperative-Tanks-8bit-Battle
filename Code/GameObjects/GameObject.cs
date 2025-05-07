using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CoopTanks.Code.GameObjects
{
    public class GameObject
    {
        public Vector2 position;
        public Vector2 previousPosition;
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

        public virtual void OnCollision(GameObject other) { }
    }
}