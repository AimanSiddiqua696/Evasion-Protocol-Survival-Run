using SemiFinalGame.Entities;
using SemiFinalGame.Interfaces;
using System;
using System.Drawing;

namespace SemiFinalGame.Movements
{
    public class PatrolRandomMovement : IMovement
    {
        private Random random;
        private float speed;
        private PointF targetPoint;

        public PatrolRandomMovement(float patrolSpeed = 1.5f)
        {
            speed = patrolSpeed;
            random = new Random();
            targetPoint = GetRandomPoint();
        }

        public void Move(GameObject obj, GameTime gameTime)
        {
            // Direction vector from enemy to target point
            float dx = targetPoint.X - obj.Position.X;
            float dy = targetPoint.Y - obj.Position.Y;

            // Distance between enemy and target point
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            // If reached target, pick a new random point
            if (distance < 1f)
            {
                targetPoint = GetRandomPoint();
                return;
            }

            // Normalize direction
            dx /= distance;
            dy /= distance;

            // Move enemy toward target point
            obj.Position = new PointF(
                obj.Position.X + dx * speed,
                obj.Position.Y + dy * speed
            );
        }

        private PointF GetRandomPoint()
        {
            // You can adjust bounds according to your game world
            float x = (float)(random.NextDouble() * 800); // example width
            float y = (float)(random.NextDouble() * 600); // example height
            return new PointF(x, y);
        }
    }
}
