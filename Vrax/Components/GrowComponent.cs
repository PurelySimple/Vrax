using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    /// <summary>
    /// Grows the size of the entity horizontal over time
    /// </summary>
    public class GrowComponent : IComponent
    {
        public Entity Owner { get; set; }

        private int Width { get; set; }
        private double Speed { get; set; }
        private double Timer { get; set; }
        private double Duration { get; set; }

        public GrowComponent(int width, double duration, double speed)
        {
            Width = width;
            Duration = duration;
            Speed = speed;
        }

        public void Start()
        {
        }

        public void Update(double deltaTime)
        {
            Timer += deltaTime;

            float percent = (float)(Timer / Duration);
            if (percent > 1f)
                percent = 1f;

            var growth = (int)(percent * Width);

            var newSize = Owner.Rectangle;
            float deltaWidth = (growth - newSize.Width);

            if (percent < 1)
                deltaWidth += (float)(deltaTime * 40);
            else
                deltaWidth += (float)(deltaTime * Speed);

            newSize.Width = growth;
            Owner.Rectangle = newSize;

            var newPosition = Owner.Position;
            newPosition.X -= deltaWidth;
            Owner.Position = newPosition;
        }
    }
}
