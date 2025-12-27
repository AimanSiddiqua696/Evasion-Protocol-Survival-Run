using SemiFinalGame.Entities;
using SemiFinalGame.Interfaces;
using System.Drawing;

namespace SemiFinalGame.Movements
{
    public enum PatrolAxis
    {
        Horizontal,
        Vertical
    }

    public class PatrolMovement : IMovement
    {
        private float minBound;
        private float maxBound;
        private float speed;
        private PatrolAxis axis;

        public PatrolMovement(float min, float max, PatrolAxis patrolAxis, float patrolSpeed = 2f)
        {
            minBound = min;
            maxBound = max;
            axis = patrolAxis;
            speed = patrolSpeed;
        }

        public void Move(GameObject obj, GameTime gameTime)
        {
            if (axis == PatrolAxis.Horizontal)
            {
                obj.Position = new PointF(
                    obj.Position.X + speed,
                    obj.Position.Y
                );

                if (obj.Position.X <= minBound)
                {
                    obj.Position = new PointF(minBound, obj.Position.Y);
                    speed = Math.Abs(speed);
                }
                else if (obj.Position.X >= maxBound)
                {
                    obj.Position = new PointF(maxBound, obj.Position.Y);
                    speed = -Math.Abs(speed);
                }
            }
            else // Vertical patrol
            {
                obj.Position = new PointF(
                    obj.Position.X,
                    obj.Position.Y + speed
                );

                if (obj.Position.Y <= minBound)
                {
                    obj.Position = new PointF(obj.Position.X, minBound);
                    speed = Math.Abs(speed);
                }
                else if (obj.Position.Y >= maxBound)
                {
                    obj.Position = new PointF(obj.Position.X, maxBound);
                    speed = -Math.Abs(speed);
                }
            }
        }
    }
}
