using PSEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    public class AccelerationComponent : IComponent
    {
        public Entity Owner { get; set; }

        private ProjectileComponent Projectile { get; set; }
        private double Duration { get; set; }
        private double Power { get; set; }
        private IEase Ease { get; set; }

        private double Timer { get; set; }
        private double BaseSpeed { get; set; }

        public AccelerationComponent(double power, double duration, IEase ease)
        {
            Duration = duration;
            Power = power;
            Ease = ease;
        }

        public void Start()
        {
            Projectile = Owner.GetComponent<ProjectileComponent>();
            if (Projectile == null)
                throw new Exception("AccelerationComponent only works on projectils");

            BaseSpeed = Projectile.Speed;
        }

        public void Update(double deltaTime)
        {
            Timer += deltaTime;

            double newSpeed = (Ease.GetProgress(Timer / Duration) * Power) + BaseSpeed;
            Projectile.Speed = newSpeed;
        }
    }
}
