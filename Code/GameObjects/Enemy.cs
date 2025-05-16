using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoopTanks.Code.GameObjects
{
    class EnemyTank : GameObject
    {
        public MovementSystem _movement;
        private List<Player> _players;
        private float _recalculateCooldown = 1f;
        private const float RecalculateInterval = 2f;
        public Texture2D Texture { get; set; }

        public EnemyTank(Vector2 position, List<Player> players)
        {
            this.position = position;
            _movement = new MovementSystem();
            _movement.SetPosition(position);
            _players = players;
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
                    //Texture = TextureUp;
                    moved = _movement.TryMove(MovementState.MovingRight, CanMoveTo);
                else
                    //Texture = TextureUp;
                    moved = _movement.TryMove(MovementState.MovingLeft, CanMoveTo);
            }
            else
            {
                if (direction.Y > 0)
                    //Texture = TextureUp;
                    moved = _movement.TryMove(MovementState.MovingDown, CanMoveTo);
                else
                    //Texture = TextureUp;
                    moved = _movement.TryMove(MovementState.MovingUp, CanMoveTo);
            }

            // Если не получилось двигаться в основном направлении, пробуем перпендикулярное
            if (!moved)
            {
                if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                {
                    if (direction.Y > 0)
                        //Texture = TextureUp;
                        _movement.TryMove(MovementState.MovingDown, CanMoveTo);
                    else
                        //Texture = TextureUp;
                        _movement.TryMove(MovementState.MovingUp, CanMoveTo);
                }
                else
                {
                    if (direction.X > 0)
                        //Texture = TextureUp;
                        _movement.TryMove(MovementState.MovingRight, CanMoveTo);
                    else
                        //Texture = TextureUp;
                        _movement.TryMove(MovementState.MovingLeft, CanMoveTo);
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

            // Обновляем систему движения
            _movement.Update(gameTime);
            position = _movement.CurrentPosition;

            // Обновляем текстуру в зависимости от состояния
            // UpdateDirectionTexture();

            /*switch (Movement.State)
            {
                case MovementState.MovingUp:
                    _animationUp.Update(gameTime);
                    Texture = _animationUp.CurrentFrame;
                    break;
                    // ... аналогично для других направлений ...
            }*/
            // Пересчитываем путь с интервалом
            _recalculateCooldown -= deltaTime;
            if (_recalculateCooldown <= 0 && !_movement.IsMoving)
            {
                FindPathToNearestPlayer();
                _recalculateCooldown = RecalculateInterval;
            }
        }

        /*private void UpdateDirectionTexture()
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
                    // Idle сохраняет последнюю текстуру
            }
        }*/

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, position, Color.White);
        }
    }
}
