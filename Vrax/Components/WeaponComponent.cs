using PSEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    public class WeaponComponent : IComponent
    {
        public Entity Owner { get; set; }
        public WeaponConfig Config { get; set; }
        public Distance FireOffset { get; set; }
        public Distance ProjectileDirection { get; set; }
        public bool PlayerTracking { get; set; }

        public bool TryFire { get; set; }

        public double FireTimer { get; private set; }

        public WeaponComponent(WeaponConfig config)
        {
            Config = config;
        }

        public void Start() { }

        public void Update(double deltaTime)
        {
            if (FireTimer > 0)
            {
                FireTimer -= deltaTime;

                if (FireTimer < 0)
                    FireTimer = 0;
            }
            else if (TryFire)
            {
                var projectileEntity = Config.ProjectileCreator();
                projectileEntity.Position = Owner.Position + FireOffset;
                projectileEntity.Team = Owner.Team;

                var projectile = projectileEntity.GetComponent<ProjectileComponent>();
                if (projectile == null)
                    throw new Exception("Projectile component not attached to projectile entity");

                var collision = projectileEntity.GetComponent<CollisionDamageComponent>();
                if (collision == null)
                    throw new Exception("Collision Damage component not attached to projectile entity. How is it supposed to damage?");

                collision.Damage = Config.Damage;

                projectile.Direction = ProjectileDirection;
                projectile.FiredFrom = Owner;
                FireTimer = Config.ShootSpeed;

                if (PlayerTracking)
                {
                    projectile.Direction = (Vrax.World.PlayerEntity.Position - Owner.Position).UnitVector;
                }

                Vrax.World.AddEntity(projectileEntity);

                if (Config.FireSound != null)
                    Vrax.Game.Audio.PlaySound(Config.FireSound);
            }

        }
    }
}
