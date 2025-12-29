//using SemiFinalGame.Core;
using SemiFinalGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SemiFinalGame.Entities
{
    public class GameObject : IDrawable, IUpdatable, IMovable, ICollidable, IPhysicsObject 
    {
        // Position of the object in the game world (encapsulated state)
        public PointF Position { get; set; }

        // Size of the object (width and height)
        // Used to compute the bounding box for collision detection (single responsibility: size responsibility)
        public SizeF Size { get; set; }

        // Velocity of the object (speed and direction)
        // Part of the IMovable contract and used by physics systems to update positions
        public PointF Velocity { get; set; } = PointF.Empty;

        // Whether this object is active (used for cleanup)
        public bool IsActive { get; set; } = true;

        // Physics related properties (optional)
        public bool HasPhysics { get; set; } = false;

        // Custom gravity 
        public float? CustomGravity { get; set; } = null;

        // Mark this object as a rigid body. When a rigid body collides, it should stop
        // (velocity will be cleared and physics disabled to prevent gravity from moving it).
        public bool IsRigidBody { get; set; } = false;

        // Optional sprite for rendering
        public Image? Sprite { get; set; } = null;

        // Bounds of the object for collision detection
        // Exposed as a computed property rather than stored state (keeps consistency)
        public RectangleF Bounds => new RectangleF(Position, Size);

        // Default update advances position by velocity.
        // Derived classes should call unless they fully replace movement behavior.
        // Demonstrates polymorphism and the Template Method pattern: derived types customize behavior.
        public virtual void Update(GameTime gameTime)
        {
            Position = new PointF(Position.X + Velocity.X, Position.Y + Velocity.Y);
        }

        // Draw the object. If a <see cref="Sprite"/> is set it will be drawn, otherwise a default rectangle.
        // This is an example of polymorphism and the Open/Closed Principle: new rendering options can be added in subclasses by overriding Draw.
        public virtual void Draw(Graphics graphics)
        {
            if (Sprite != null)
            {
                graphics.DrawImage(Sprite, Bounds);
            }
            else
            {
                using (Brush brush = new SolidBrush(Color.Gray)) // Default color
                {
                    graphics.FillRectangle(brush, Bounds);
                }
            }
        }

        // Called when this object collides with another. Default does nothing.
        // Override in derived classes to implement domain-specific reactions (damage, pickup, deactivate).
        // Demonstrates polymorphism and single responsibility: collision reaction is left to the object itself.
        public virtual void OnCollision(GameObject other)
        {
            // Default behavior: Do nothing
        }
    }
}

