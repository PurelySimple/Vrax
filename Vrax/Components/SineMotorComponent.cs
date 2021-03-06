﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    public class SineMotorComponent : IComponent
    {
        public Entity Owner { get; set; }

        private MovementComponent Movement { get; set; }
        private float Speed { get; set; }

        private double Timer { get; set; }

        public SineMotorComponent(float revolutionSpeed)
        {
            Speed = revolutionSpeed;
        }

        public void Start()
        {
            Movement = Owner.GetComponent<MovementComponent>();

            Movement.MoveLeft = true;

            // Random starting direction
            var rand = new Random();
            Timer = rand.NextDouble() * 2 * Math.PI;
        }

        public void Update(double deltaTime)
        {
            Timer += (deltaTime * 2 * Math.PI * Speed);
            var sine = Math.Sin(Timer);

            Movement.MoveUp = sine < 0;
            Movement.MoveDown = sine > 0;
        }
    }
}
