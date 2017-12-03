using PSEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    public class InvulnerableDamageHandler : IComponent, IDamageHandler
    {
        public Entity Owner { get; set; }

        private IEnumerable<RenderComponent> Renderers { get; set; }

        private double Duration { get; set; }
        private double CooldownTimer { get; set; }

        private List<Sound> HitSounds { get; set; }
        private Random Rand { get; set; }

        public InvulnerableDamageHandler(double cooldownDuration)
        {
            Duration = cooldownDuration;
        }

        public void Damage(int amount, Entity source)
        {
            // Prevent damage if on cooldown timer
            if (CooldownTimer > 0)
                return;

            CooldownTimer = Duration;
            Owner.ApplyDamage(amount);

            if (Owner.Health > 0)
                Vrax.Game.Audio.PlaySound(HitSounds[Rand.Next() % HitSounds.Count]);
        }

        public void Start()
        {
            Renderers = Owner.GetComponents<RenderComponent>();

            Rand = new Random();
            HitSounds = new List<Sound>()
            {
                Vrax.Game.AssetCache.Get<Sound>("hit0.wav"),
                Vrax.Game.AssetCache.Get<Sound>("hit1.wav"),
                Vrax.Game.AssetCache.Get<Sound>("hit2.wav"),
            };
        }

        public void Update(double deltaTime)
        {
            if (CooldownTimer > 0)
            {
                CooldownTimer -= deltaTime;
                if (CooldownTimer < 0)
                {
                    CooldownTimer = 0;

                    foreach (var renderer in Renderers)
                        renderer.Tint = Color.White;
                }
                else
                {
                    foreach (var renderer in Renderers)
                        renderer.Tint = Color.Alpha(0.40f); // Ghost the image
                }
            }
        }
    }
}
