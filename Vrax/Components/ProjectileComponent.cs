using PSEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    public class ProjectileComponent : IComponent
    {
        public Entity Owner { get; set; }
        public Entity FiredFrom { get; set; }
        public Distance Direction { get; set; } = new Distance(1, 0);
        public double Speed { get; set; }
        public int Damage { get; set; }

        public void Start() { }

        public void Update(double deltaTime)
        {
            var newPosition = Owner.Position;

            newPosition.X += (float)(Direction.X * Speed * deltaTime);
            newPosition.Y += (float)(Direction.Y * Speed * deltaTime);

            Owner.Position = newPosition;

            // Destroy Projectiles off screen
            var screen = Vrax.Game.Screen;
            if (newPosition.X > screen.Width || newPosition.X < 0 ||
                newPosition.Y < 0 || newPosition.Y > screen.Height)
            {
                Owner.MarkedForDestruction = true;
            }
            else
            {
                // Check for entity collision (not efficient...)
                var worldRect = Owner.WorldRect;
                foreach (var other in Vrax.World.AllEntities())
                {
                    if (!other.IgnoreCollision && other.Team != Owner.Team && other != FiredFrom && worldRect.CollidesWith(other.WorldRect))
                    {
                        Owner.MarkedForDestruction = true;

                        var damageHandler = other.GetComponent<IDamageHandler>();
                        if (damageHandler == null)
                        {
                            other.ApplyDamage(Damage);
                        }
                        else
                        {
                            damageHandler.Damage(Damage, Owner);
                        }
                    }
                }
            }
        }
    }
}
