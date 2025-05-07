using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CoopTanks.Code
{
    public enum MovementState
    {
        Idle,
        MovingUp,
        MovingDown,
        MovingLeft,
        MovingRight
    }

    public class MovementSystem
    {
        public const int TileSize = 32;
        public MovementState State { get; private set; } = MovementState.Idle;
        public Vector2 TargetPosition { get; private set; }
        public Vector2 CurrentPosition { get; private set; }

        private float _moveSpeed = 2f;
        private const float SnapThreshold = 0.5f; // Новый порог для "прилипания"

        public bool IsMoving => State != MovementState.Idle;

        public void Update(GameTime gameTime)
        {
            if (!IsMoving) return;

            Vector2 direction = TargetPosition - CurrentPosition;
            float distance = direction.Length();

            // Если очень близко к цели - завершаем движение
            if (distance < SnapThreshold)
            {
                CurrentPosition = TargetPosition;
                State = MovementState.Idle;
                return;
            }

            // Нормализуем направление и двигаемся
            direction.Normalize();
            CurrentPosition += direction * Math.Min(_moveSpeed, distance); // Не превышаем целевую позицию
        }

        public bool TryMove(MovementState direction, Func<Vector2, bool> canMoveTo)
        {
            if (IsMoving) return false;

            Vector2 newTarget = CurrentPosition;
            switch (direction)
            {
                case MovementState.MovingUp:
                    newTarget.Y -= TileSize;
                    break;
                case MovementState.MovingDown:
                    newTarget.Y += TileSize;
                    break;
                case MovementState.MovingLeft:
                    newTarget.X -= TileSize;
                    break;
                case MovementState.MovingRight:
                    newTarget.X += TileSize;
                    break;
                default:
                    return false;
            }

            if (canMoveTo(newTarget))
            {
                TargetPosition = newTarget;
                State = direction;
                return true;
            }

            return false;
        }

        // Добавляем метод для инициализации позиции
        public void SetPosition(Vector2 position)
        {
            CurrentPosition = position;
            TargetPosition = position;
            State = MovementState.Idle;
        }
    }
}