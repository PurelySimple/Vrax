using System;
using System.Collections.Generic;
using System.Text;
using PSEngine;
using PSEngine.Core;

namespace LudumDare40.Vrax.Components
{
    public class ShieldComponent : IComponent, IDisposable
    {
        public Entity Owner { get; set; }
        public double Cooldown { get; set; }
        public Entity ShieldEntity { get; private set; }

        private Distance ShieldOffset { get; set; }

        private WeaponComponent Weapon { get; set; }
        private double CooldownTimer { get; set; }

        public ShieldComponent(Entity shieldEntity)
        {
            ShieldEntity = shieldEntity;
        }

        public void Start()
        {
            Weapon = Owner.GetComponent<WeaponComponent>();
            ShieldOffset = (Owner.Rectangle.Size - ShieldEntity.Rectangle.Size).Half + new Distance(Owner.Rectangle.X, Owner.Rectangle.Y);

            var shieldRender = ShieldEntity.GetComponent<RenderComponent>();
            shieldRender.Condition = () => CooldownTimer == 0;

            // Spawn shield
            ShieldEntity.Position = Owner.Position + ShieldOffset;
            Vrax.World.AddEntity(ShieldEntity);
        }

        public void Update(double deltaTime)
        {
            ShieldEntity.Position = Owner.Position + ShieldOffset;

            if (CooldownTimer > 0)
            {
                CooldownTimer -= deltaTime;
                if (CooldownTimer <= 0)
                {
                    CooldownTimer = 0;

                    if (Weapon != null && !Weapon.TryFire)
                        ShieldsUp(true);
                }
            }

            if (Weapon != null && Weapon.TryFire)
            {
                CooldownTimer = Cooldown;

                //if (!ShieldsAreDown)
                    ShieldsUp(false);
            }
        }

        private bool ShieldsAreDown => ShieldEntity.IgnoreCollision == true;

        private void ShieldsUp(bool enabled)
        {
            if (ShieldsAreDown && !enabled)
                return;
            else if (!ShieldsAreDown && enabled)
                return;

            ShieldEntity.IgnoreCollision = !enabled;

            var game = Vrax.Game;
            if (enabled)
            {
                game.Audio.PlaySound(game.AssetCache.Get<Sound>("shieldup.wav"));
            }
            else
            {
                game.Audio.PlaySound(game.AssetCache.Get<Sound>("shielddown.wav"));
            }
        }

        public void Dispose()
        {
            ShieldEntity.MarkedForDestruction = true;
            ShieldEntity = null;
        }
    }
}
