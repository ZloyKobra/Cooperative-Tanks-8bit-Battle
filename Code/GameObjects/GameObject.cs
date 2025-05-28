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
        public bool IsDestroyed { get; set; } = false;

        private const int Damage = 1;

        public int healthPoint = 3;

        protected float _shootCooldown = 0f;
        protected float _shootCooldownTime = 1f; // Время между выстрелами по умолчанию

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
                TileSize,
                TileSize
            );
        }

        public virtual void TakeDamage(GameObject damageDealer)
        {
            // Уменьшаем HP у текущего объекта (который получает урон)
            this.healthPoint -= Damage;

            // Проверяем смерть объекта
            if (this.healthPoint <= 0)
            {
                //Destroy();
            }
        }

        public virtual void UpdateCooldowns(GameTime gameTime)
        {
            if (_shootCooldown > 0)
                _shootCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public bool CanShoot => _shootCooldown <= 0;

        protected void ResetShootCooldown()
        {
            _shootCooldown = _shootCooldownTime;
        }
    }
}