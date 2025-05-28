using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using System.Collections.Generic;
using System.Diagnostics;


namespace CoopTanks.Code.GameObjects
{
    class Player : GameObject
    {
        public MovementSystem Movement { get; } = new MovementSystem();
        private Keys[] _controlKeys;
        public float Speed = 64f;
        public Texture2D Texture { get; set; }
        public Texture2D TextureUp { get; set; }
        public Texture2D TextureDown { get; set; }
        public Texture2D TextureLeft { get; set; }
        public Texture2D TextureRight { get; set; }

        private static Texture2D _debugTexture;
        public Player(Vector2 initialPosition, Keys[] controls)
        {
            position = initialPosition;
            Movement.SetPosition(initialPosition);
            _controlKeys = controls;
            Texture = TextureLeft;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Отрисовка индикатора кулдауна
            if (!CanShoot)
            {
                float cooldownPercent = _shootCooldown / _shootCooldownTime;
                Vector2 barPosition = position - new Vector2(16, 30);
                Rectangle backBar = new Rectangle((int)barPosition.X, (int)barPosition.Y, 32, 5);
                Rectangle cooldownBar = new Rectangle((int)barPosition.X, (int)barPosition.Y,
                    (int)(32 * (1 - cooldownPercent)), 5);

                spriteBatch.DrawRectangle(backBar, Color.Gray);
                spriteBatch.DrawRectangle(cooldownBar, Color.Red);
            }
            spriteBatch.Draw(Texture, position, Color.White);
            DrawHitbox(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.UpdateCooldowns(gameTime);
            Movement.Update(gameTime);

            // Синхронизируем позицию
            position = Movement.CurrentPosition;

            //Rectangle playerBounds = this.GetBounds();
            //Debug.WriteLine($"Игрок: X={playerBounds.X}, Y={playerBounds.Y}, Size={playerBounds.Width}x{playerBounds.Height}");

            // Обновляем текстуру в зависимости от состояния
            UpdateDirectionTexture();

            // Обрабатываем ввод только если не двигаемся
            if (!Movement.IsMoving)
            {
                HandleInput();
            }
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

        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // При нажатии клавиши сразу меняем текстуру
            if (keyboardState.IsKeyDown(_controlKeys[0])) // Up
            {
                Texture = TextureUp;
                Movement.TryMove(MovementState.MovingUp, CanMoveTo);
                Movement.lastMove = MovementState.MovingUp;
            }
            else if (keyboardState.IsKeyDown(_controlKeys[1])) // Down
            {
                Texture = TextureDown;
                Movement.TryMove(MovementState.MovingDown, CanMoveTo);
                Movement.lastMove = MovementState.MovingDown;
            }
            else if (keyboardState.IsKeyDown(_controlKeys[2])) // Left
            {
                Texture = TextureLeft;
                Movement.TryMove(MovementState.MovingLeft, CanMoveTo);
                Movement.lastMove = MovementState.MovingLeft;
            }
            else if (keyboardState.IsKeyDown(_controlKeys[3])) // Right
            {
                Texture = TextureRight;
                Movement.TryMove(MovementState.MovingRight, CanMoveTo);
                Movement.lastMove = MovementState.MovingRight;
            }
            else if (keyboardState.IsKeyDown(_controlKeys[4])) // Shoot
            {
                Shoot();
            }
        }

        private bool CanMoveTo(Vector2 target)
        {
            var futureBounds = new Rectangle(
                (int)target.X,
                (int)target.Y,
                Texture?.Width ?? MovementSystem.TileSize, // Защита от null
                Texture?.Height ?? MovementSystem.TileSize
            );

            foreach (var wall in Walls.walls)
            {
                if (futureBounds.Intersects(wall.GetBounds()))
                {
                    return false;
                }
            }

            return true;
        }

        private Vector2 GetShootDirection()
        {
            switch (Movement.lastMove)
            {
                case MovementState.MovingUp: return -Vector2.UnitY;
                case MovementState.MovingDown: return Vector2.UnitY;
                case MovementState.MovingLeft: return -Vector2.UnitX;
                case MovementState.MovingRight: return Vector2.UnitX;
                default: return Vector2.UnitX;
            }
        }

        public void Shoot()
        {
            if (!CanShoot) return;

            // Логика создания пули
            Vector2 direction = GetShootDirection();
            Vector2 bulletSpawnPos = position + direction * 20;
            new Bullet(bulletSpawnPos, direction, this);

            // Сброс кулдауна
            ResetShootCooldown();   

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

    class Players
    {
        public static List<Player> players = new List<Player>();

        public static void AddPlayer(Player player)
        {
            players.Add(player);
        }
    }
}
