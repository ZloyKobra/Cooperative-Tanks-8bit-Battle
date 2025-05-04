using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;

namespace CoopTanks.Code
{
    public class GameObject
    {
        public Vector2 position;

        static public Texture2D Texture { get; set; }

        public GameObject()
        {
            position = Vector2.Zero;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public virtual void Update(GameTime gameTime) { }
    }

    class Player : GameObject
    {
        public float Speed = 100f;
        public new Texture2D Texture { get; set; }

        public Player(Vector2 initialPosition)
        {
            position = initialPosition;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, this.position, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.W)) position.Y -= Speed * deltaTime;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) position.Y += Speed * deltaTime;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) position.X += Speed * deltaTime;
            if (Keyboard.GetState().IsKeyDown(Keys.A)) position.X -= Speed * deltaTime;
        }
    }
}