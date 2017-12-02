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

        public bool TryFire { get; set; }

        private double FireTimer { get; set; }

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

                projectile.Direction = ProjectileDirection;
                projectile.Damage = Config.Damage;
                projectile.FiredFrom = Owner;
                FireTimer = Config.ShootSpeed;

                Vrax.World.AddEntity(projectileEntity);

                if (Config.FireSound != null)
                    Vrax.Game.Audio.PlaySound(Config.FireSound);
            }

        }
    }
}
