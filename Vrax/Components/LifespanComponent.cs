using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    public class LifespanComponent : IComponent
    {
        public Entity Owner { get; set; }
        private double Timer { get; set; }

        public LifespanComponent(double duration)
        {
            Timer = duration;
        }

        public void Start()
        {
        }

        public void Update(double deltaTime)
        {
            if (Timer > 0)
            {
                Timer -= deltaTime;
            }
            else
                Owner.MarkedForDestruction = true;
        }
    }
}
