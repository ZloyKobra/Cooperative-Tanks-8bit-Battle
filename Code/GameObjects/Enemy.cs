using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CoopTanks.Code.GameObjects
{
    class Enemy : GameObject
    {
        public MovementSystem Movement;
        private List<Player> _players;
        private float _recalculateCooldown = 1f;
        private const float RecalculateInterval = 2f;
        private float _shootCooldownTime = 3f;
        public Texture2D Texture { get; set; }
        public Texture2D TextureUp { get; set; }
        public Texture2D TextureDown { get; set; }
        public Texture2D TextureLeft { get; set; }
        public Texture2D TextureRight { get; set; }

        private static Texture2D _debugTexture;

        public Enemy(Vector2 position, List<Player> players)
        {
            this.position = position;
            Movement = new MovementSystem();
            Movement.SetPosition(position);
            _players = players;
            Texture = TextureLeft;
        }

        private void FindPathToNearestPlayer()
        {
            if (_players == null || _players.Count == 0) return;

            // Находим ближайшего игрока
            Player nearestPlayer = _players
                .OrderBy(p => Vector2.Distance(position, p.position))
                .FirstOrDefault();

            if (nearestPlayer == null) return;

            // Пытаемся двигаться в сторону игрока
            Vector2 direction = nearestPlayer.position - position;
            bool moved = false;

            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                if (direction.X > 0)
                {
                    Texture = TextureUp;
                    Movement.TryMove(MovementState.MovingRight, CanMoveTo);
                }
                else
                {
                    Texture = TextureUp;
                    Movement.TryMove(MovementState.MovingLeft, CanMoveTo);
                }
            }
            else
            {
                if (direction.Y > 0)
                {
                    Texture = TextureUp;
                    Movement.TryMove(MovementState.MovingDown, CanMoveTo);
                }
                else
                {
                    Texture = TextureUp;
                    Movement.TryMove(MovementState.MovingUp, CanMoveTo);
                }
            }

            // Если не получилось двигаться в основном направлении, пробуем перпендикулярное
            if (!moved)
            {
                if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                {
                    if (direction.Y > 0)
                    {
                        Texture = TextureUp;
                        Movement.TryMove(MovementState.MovingDown, CanMoveTo);
                    }
                    else
                    {
                        Texture = TextureUp;
                        Movement.TryMove(MovementState.MovingUp, CanMoveTo);
                    }
                }
                else
                {
                    if (direction.X > 0)
                    {
                        Texture = TextureUp;
                        Movement.TryMove(MovementState.MovingRight, CanMoveTo);
                    }
                    else
                    {
                        Texture = TextureUp;
                        Movement.TryMove(MovementState.MovingLeft, CanMoveTo);
                    }
                }
            }
        }

        private bool CanMoveTo(Vector2 target)
        {
            // Создаем временный Rectangle для проверки
            var futureBounds = new Rectangle(
                (int)target.X,
                (int)target.Y,
                Texture?.Width ?? TileSize,
                Texture?.Height ?? TileSize
            );

            // Проверяем коллизии со всеми стенами
            foreach (var wall in Walls.walls)
            {
                if (futureBounds.Intersects(wall.GetBounds()))
                {
                    return false;
                }
            }

            return true;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.UpdateCooldowns(gameTime);

            // Логика стрельбы
            _shootCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_shootCooldown <= 0)
            {
                TryShoot();
                _shootCooldown = _shootCooldownTime;
            }

            // Обновляем систему движения
            Movement.Update(gameTime);
            position = Movement.CurrentPosition;

            // Проверка координат и размеров текстур
            //Rectangle enemyBounds = this.GetBounds();
            //Debug.WriteLine($"Враг: X={enemyBounds.X}, Y={enemyBounds.Y}, Size={enemyBounds.Width}x{enemyBounds.Height}");

            // Проверка снятия хп
            //Debug.WriteLine($"{healthPoint}");

            // Обновляем текстуру в зависимости от состояния
            UpdateDirectionTexture();

            // Пересчитываем путь с интервалом
            _recalculateCooldown -= deltaTime;
            if (_recalculateCooldown <= 0 && !Movement.IsMoving)
            {
                FindPathToNearestPlayer();
                _recalculateCooldown = RecalculateInterval;
            }
        }

        private void TryShoot()
        {
            if (!CanShoot) return;

            // Получаем направление к ближайшему игроку
            Player nearestPlayer = _players
                .OrderBy(p => Vector2.Distance(position, p.position))
                .FirstOrDefault();
            if (nearestPlayer == null) return;

            // Определяем основное направление (только 4 стороны)
            Vector2 direction = DetermineCardinalDirection(nearestPlayer.position - position);

            // Создаем пулю
            Vector2 bulletSpawnPos = position + direction * 20;
            new Bullet(bulletSpawnPos, direction, this);

            ResetShootCooldown();
        }

        private Vector2 DetermineCardinalDirection(Vector2 rawDirection)
        {
            // Для тайловых игр можно использовать упрощенную версию
            float angle = MathHelper.WrapAngle((float)Math.Atan2(rawDirection.Y, rawDirection.X));

            if (angle > -MathHelper.PiOver4 && angle <= MathHelper.PiOver4)
                return Vector2.UnitX; // Вправо
            else if (angle > MathHelper.PiOver4 && angle <= 3 * MathHelper.PiOver4)
                return Vector2.UnitY; // Вниз
            else if (angle > -3 * MathHelper.PiOver4 && angle <= -MathHelper.PiOver4)
                return -Vector2.UnitY; // Вверх
            else
                return -Vector2.UnitX; // Влево
        }

        private void UpdateDirectionTexture()
        {
            switch (Movement.State)
            {
                case MovementState.MovingUp:
                    Texture = TextureUp;
                    break;
                case MovementState.MovingDown:
                    Texture = TextureDown;
                    break;
                case MovementState.MovingLeft:
                    Texture = TextureLeft;
                    break;
                case MovementState.MovingRight:
                    Texture = TextureRight;
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, position, Color.White);
            DrawHitbox(spriteBatch);
        }

        public static void LoadDebugTexture(GraphicsDevice graphics)
        {
            // Создаем простую текстуру 1x1 пиксель для отрисовки линий
            _debugTexture = new Texture2D(graphics, 1, 1);
            _debugTexture.SetData(new[] { Color.White });
        }

        // отрисовка хитбокса относительно координат(положения)
        private void DrawHitbox(SpriteBatch spriteBatch)
        {
            if (_debugTexture == null) return;

            Rectangle bounds = GetBounds();
            Color hitboxColor = Color.Red * 1f; // Полупрозрачный красный

            // Отрисовываем прямоугольник хитбокса
            spriteBatch.Draw(_debugTexture, new Rectangle(bounds.X, bounds.Y, bounds.Width, 1), hitboxColor); // Верх
            spriteBatch.Draw(_debugTexture, new Rectangle(bounds.X, bounds.Y + bounds.Height - 1, bounds.Width, 1), hitboxColor); // Низ
            spriteBatch.Draw(_debugTexture, new Rectangle(bounds.X, bounds.Y, 1, bounds.Height), hitboxColor); // Лево
            spriteBatch.Draw(_debugTexture, new Rectangle(bounds.X + bounds.Width - 1, bounds.Y, 1, bounds.Height), hitboxColor); // Право
        }
    }


    class Enemies
    {
        public static List<Enemy> enemies = new List<Enemy>();

        public static void AddEnemy(Enemy enemy)
        {
            enemies.Add(enemy);
        }
    }


}

