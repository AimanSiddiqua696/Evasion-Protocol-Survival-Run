using SemiFinalGame.Entities;
using SemiFinalGame.Interfaces;
using System.Drawing;

namespace SemiFinalGame.Movements
{
    public class VerticalPatrolMovement : IMovement
    {
        private float topBound;
        private float bottomBound;
        private float speed;

        public VerticalPatrolMovement(float top, float bottom, float patrolSpeed = 2f)
        {
            topBound = top;
            bottomBound = bottom;
            speed = patrolSpeed;
        }

        public void Move(GameObject obj, GameTime gameTime)
        {
            // Move vertically
            obj.Position = new PointF(
                obj.Position.X,
                obj.Position.Y + speed
            );

            // Reverse direction when bounds are reached
            if (obj.Position.Y <= topBound)
            {
                obj.Position = new PointF(obj.Position.X, topBound);
                speed = Math.Abs(speed);   // Move down
            }
            else if (obj.Position.Y >= bottomBound)
            {
                obj.Position = new PointF(obj.Position.X, bottomBound);
                speed = -Math.Abs(speed);  // Move up
            }
        }
    }
}

