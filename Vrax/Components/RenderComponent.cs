using PSEngine;
using PSEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    public class RenderComponent : IComponent, IRenderable
    {
        public Entity Owner { get; set; }
        public Color Tint { get; set; } = Color.White;
        public Distance Offset { get; set; } = Distance.Zero;

        private List<AtlasFrame> Frames { get; set; }
        private double Speed { get; set; }

        private int FrameIndex { get; set; }
        private double Elapsed { get; set; }

        public RenderComponent(AtlasFrame frame)
        {
            Frames = new List<AtlasFrame>() { frame };
        }

        public RenderComponent(List<AtlasFrame> frames, double speed)
        {
            Frames = frames;
            Speed = speed;
        }

        public void Render(Graphics g)
        {
            g.Draw(Frames[FrameIndex], Owner.Position + Offset, Distance.Zero, 0, Distance.One, Tint);
        }

        public void Update(double deltaTime)
        {
            if (Frames.Count == 1)
                return;

            Elapsed += deltaTime;
            if (Elapsed >= Speed)
            {
                Elapsed -= Speed;

                FrameIndex = (FrameIndex + 1) % Frames.Count;
            }
        }

        public void Start()
        {
        }
    }
}
