using SemiFinalGame.Entities;
using SemiFinalGame.Interfaces;
using System.Drawing;

namespace SemiFinalGame.Movements
{
    public class JumpingMovement : IMovement
    {
        private float gravity;
        private float jumpForce;
        private float verticalVelocity;

        private float groundY;
        private bool isGrounded = true;

        public JumpingMovement(float groundLevel, float jumpStrength = -10f, float gravityForce = 0.5f)
        {
            groundY = groundLevel;
            jumpForce = jumpStrength;
            gravity = gravityForce;
        }

        public void Move(GameObject obj, GameTime gameTime)
        {
            // Start jump if object is on the ground
            if (isGrounded)
            {
                verticalVelocity = jumpForce;
                isGrounded = false;
            }

            // Apply gravity
            verticalVelocity += gravity;

            // Update position
            obj.Position = new PointF(
                obj.Position.X,
                obj.Position.Y + verticalVelocity
            );

            // Land on the ground
            if (obj.Position.Y >= groundY)
            {
                obj.Position = new PointF(obj.Position.X, groundY);
                verticalVelocity = 0;
                isGrounded = true;
            }
        }
    }
}
