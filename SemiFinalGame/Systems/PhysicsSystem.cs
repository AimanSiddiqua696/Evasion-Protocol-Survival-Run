using SemiFinalGame.Entities;
using SemiFinalGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiFinalGame.Systems
{
    public class PhysicsSystem
    {
        // Global gravity value (can be positive or negative)
        public float Gravity { get; set; } = 0.5f;

        // Apply physics to objects that have physics enabled
        //public void ApplyJump(IMovable player, float jumpForce)
        //{
        //    player.Velocity = new PointF(player.Velocity.X, jumpForce);
        //}

        public void Apply(List<GameObject> objects)
        {
            foreach (var obj in objects.OfType<IPhysicsObject>().Where(o => o.HasPhysics))
            {
                if (obj is IMovable movable)
                {
                    // Use the object custom gravity if set, otherwise use the global gravity
                    float appliedGravity = obj.CustomGravity ?? Gravity;

                    // Update the velocity of the object by applying gravity.
                    // Note: This is simple Euler integration and illustrates the physics update responsibility.
                    movable.Velocity = new PointF(
                        movable.Velocity.X, // Horizontal velocity remains unchanged
                        movable.Velocity.Y + appliedGravity // Vertical velocity changes based on gravity
                    );

                    if (obj is GameObject gameObject)
                    {
                        // Update the position of the object based on its (now-updated) velocity
                        gameObject.Position = new PointF(
                            gameObject.Position.X + movable.Velocity.X,
                            gameObject.Position.Y + movable.Velocity.Y
                        );
                    }
                }
            }
        }
    }
}
