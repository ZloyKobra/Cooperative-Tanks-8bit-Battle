using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CoopTanks.Code.GameObjects
{
    public class Bullet : GameObject
    {
        public Vector2 Direction { get; private set; }
        public float Speed { get; private set; } = 200f; // Пикселей в секунду
        public int Damage { get; private set; } = 1;
        public GameObject Owner { get; private set; }
        public float LifeTime { get; private set; } = 2f; // Время жизни в секундах
        private float _currentLifeTime = 0f;
        private static Texture2D _debugTexture;
        private static int BulletSize = 16;

        public static new Texture2D Texture { get; set; }

        // Статический список всех пуль (для удобства обработки)
        public static List<Bullet> ActiveBullets = new List<Bullet>();

        public Bullet(Vector2 position, Vector2 direction, GameObject owner)
        {
            this.position = position + new Vector2(TileSize / 2, TileSize / 2);
            Direction = Vector2.Normalize(direction);
            Owner = owner;
            IsSolid = false; // Пули не являются препятствиями
            // Регистрируем пулю в списке активных
            ActiveBullets.Add(this);
        }

        public static void LoadDebugTexture(GraphicsDevice graphics)
        {
            // Создаем простую текстуру 1x1 пиксель для отрисовки линий
            _debugTexture = new Texture2D(graphics, 1, 1);
            _debugTexture.SetData(new[] { Color.White });
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Движение пули
            position += Direction * Speed * deltaTime;

            // Обновление времени жизни
            _currentLifeTime += deltaTime;
            if (_currentLifeTime >= LifeTime)
            {
                Destroy();
                return;
            }

            // Проверка коллизий
            CheckCollisions();
        }

        private void CheckCollisions()
        {
            Rectangle bulletBounds = GetBounds();

            // Проверка столкновений со стенами
            foreach (var wall in Walls.walls)
            {
                if (bulletBounds.Intersects(wall.GetBounds()))
                {
                    OnWallHit(wall);
                    Destroy();
                    return;
                }
            }


            foreach (var player in Players.players)
            {
                if (player != Owner && bulletBounds.Intersects(player.GetBounds()))
                {
                    OnPlayerHit(player);
                    Destroy();
                    return;
                }
            }
            
            foreach (var enemy in Enemies.enemies)
            {
                if (enemy != Owner && bulletBounds.Intersects(enemy.GetBounds()))
                {
                    OnEnemyHit(enemy);
                    Destroy();
                    return;
                }
            }

            foreach (var bullet in ActiveBullets.ToList())
            {
                if (bullet != this && bulletBounds.Intersects(bullet.GetBounds()))
                {
                    bullet.Destroy();
                    Destroy();
                    return;
                }
            }
        }

        private void OnEnemyHit(Enemy enemy)
        {
            enemy.TakeDamage(this);
        }

        private void OnPlayerHit(Player player)
        {
            player.TakeDamage(this);
        }

        private void OnWallHit(Wall wall)
        {
            // Эффекты при попадании в стену
            // Можно добавить частицы или звук
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                float rotation = (float)Math.Atan2(Direction.Y, Direction.X);
                spriteBatch.Draw(
                    Texture,
                    position,
                    null,
                    Color.White,
                    rotation,
                    new Vector2(Texture.Width / 2, Texture.Height / 2),
                    1f,
                    SpriteEffects.None,
                    0f);
            }
            DrawHitbox(spriteBatch);
        }

        // отрисовка хитбокса пули относительно её координат(положения)
        private void DrawHitbox(SpriteBatch spriteBatch)
        {
            if (_debugTexture == null) return;

            Rectangle bounds = GetBounds();
            Color hitboxColor = Color.Red * 0.5f; // Полупрозрачный красный

            // Отрисовка хитбокса
            spriteBatch.Draw(_debugTexture, new Rectangle(bounds.X, bounds.Y, bounds.Width, 1), hitboxColor); // Верх
            spriteBatch.Draw(_debugTexture, new Rectangle(bounds.X, bounds.Y + bounds.Height - 1, bounds.Width, 1), hitboxColor); // Низ
            spriteBatch.Draw(_debugTexture, new Rectangle(bounds.X, bounds.Y, 1, bounds.Height), hitboxColor); // Лево
            spriteBatch.Draw(_debugTexture, new Rectangle(bounds.X + bounds.Width - 1, bounds.Y, 1, bounds.Height), hitboxColor); // Право
        }

        public void Destroy()
        {
            // Отмечаем пулю из списка активных
            IsDestroyed = true;
            // Потом можно добавить эффект уничтожения (анимацию взрыва)
        }

        public override Rectangle GetBounds()
        {
            return new Rectangle(
                (int)position.X - BulletSize / 2,
                (int)position.Y - BulletSize / 2,
                BulletSize,
                BulletSize
            );
        }
    }
}