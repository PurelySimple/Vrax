using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    public class CollisionDamageComponent : IComponent
    {
        public Entity Owner { get; set; }
        public int Damage { get; set; }

        public CollisionDamageComponent(int damage)
        {
            Damage = damage;
        }

        public void Start() { }

        public void Update(double deltaTime)
        {
            // Check for entity collision (not efficient...)
            var worldRect = Owner.WorldRect;
            foreach (var other in Vrax.World.AllEntities())
            {
                if (!other.IgnoreCollision && other.Team != Owner.Team && other != Owner && worldRect.CollidesWith(other.WorldRect))
                {
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
