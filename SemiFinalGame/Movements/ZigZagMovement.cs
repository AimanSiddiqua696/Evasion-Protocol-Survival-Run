using SemiFinalGame.Entities;
using SemiFinalGame.Interfaces;
using System.Drawing;

namespace SemiFinalGame.Movements
{
    public class ZigZagMovement : IMovement
    {
        private float horizontalSpeed;
        private float verticalSpeed;

        private float topBound;
        private float bottomBound;

        private int verticalDirection = 1;

        public ZigZagMovement(
            float top,
            float bottom,
            float hSpeed = 2f,
            float vSpeed = 2f)
        {
            topBound = top;
            bottomBound = bottom;
            horizontalSpeed = hSpeed;
            verticalSpeed = vSpeed;
        }

        public void Move(GameObject obj, GameTime gameTime)
        {
            // Move horizontally
            float newX = obj.Position.X + horizontalSpeed;

            // Move vertically (zig-zag)
            float newY = obj.Position.Y + verticalSpeed * verticalDirection;

            // Reverse vertical direction at bounds
            if (newY <= topBound)
            {
                newY = topBound;
                verticalDirection = 1;
            }
            else if (newY >= bottomBound)
            {
                newY = bottomBound;
                verticalDirection = -1;
            }

            obj.Position = new PointF(newX, newY);
        }
    }
}

