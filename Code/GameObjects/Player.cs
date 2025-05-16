using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;


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
        
        //private Animation _animationUp, _animationDown, _animationLeft, _animationRight; Потом реализовать

        public Player(Vector2 initialPosition, Keys[] controls)
        {
            position = initialPosition;
            Movement.SetPosition(initialPosition);
            _controlKeys = controls;
            //Texture = TextureLeft;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, position, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            Movement.Update(gameTime);

            // Синхронизируем позицию
            position = Movement.CurrentPosition;

            // Обновляем текстуру в зависимости от состояния
            // UpdateDirectionTexture();

            // Обрабатываем ввод только если не двигаемся
            if (!Movement.IsMoving)
            {
                HandleInput();
            }

            /*switch (Movement.State)
            {
                case MovementState.MovingUp:
                    _animationUp.Update(gameTime);
                    Texture = _animationUp.CurrentFrame;
                    break;
                    // ... аналогично для других направлений ...
            }*/
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

        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // При нажатии клавиши сразу меняем текстуру
            if (keyboardState.IsKeyDown(_controlKeys[0])) // Up
            {
                //Texture = TextureUp;
                Movement.TryMove(MovementState.MovingUp, CanMoveTo);
            }
            else if (keyboardState.IsKeyDown(_controlKeys[1])) // Down
            {
                //Texture = TextureDown;
                Movement.TryMove(MovementState.MovingDown, CanMoveTo);
            }
            else if (keyboardState.IsKeyDown(_controlKeys[2])) // Left
            {
                //Texture = TextureLeft;
                Movement.TryMove(MovementState.MovingLeft, CanMoveTo);
            }
            else if (keyboardState.IsKeyDown(_controlKeys[3])) // Right
            {
                //Texture = TextureRight;
                Movement.TryMove(MovementState.MovingRight, CanMoveTo);
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
    }
}
