using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    public class MovementComponent : IComponent
    {
        public double MoveSpeed { get; set; }

        public bool MoveLeft { get; set; }
        public bool MoveRight { get; set; }
        public bool MoveDown { get; set; }
        public bool MoveUp { get; set; }

        public Entity Owner { get; set; }

        public MovementComponent(double moveSpeed)
        {
            MoveSpeed = moveSpeed;
        }

        public void Start() { }

        public void Update(double deltaTime)
        {
            var newPosition = Owner.Position;

            if (MoveLeft)
            {
                newPosition.X -= (float)(deltaTime * MoveSpeed);
            }
            else if (MoveRight)
            {
                newPosition.X += (float)(deltaTime * MoveSpeed);
            }

            if (MoveUp)
            {
                newPosition.Y -= (float)(deltaTime * MoveSpeed);
            }
            else if (MoveDown)
            {
                newPosition.Y += (float)(deltaTime * MoveSpeed);
            }

            Owner.Position = newPosition;

            // Die if offscreen
            if (Owner.Position.X < -50)
            {
                Owner.MarkedForDestruction = true;
            }
        }
    }
}
