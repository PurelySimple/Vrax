using PSEngine;
using PSEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax
{
    public class ParallaxLayer
    {
        private List<AtlasFrame> PossibleAssets { get; set; }
        private Random Rand { get; set; }
        private List<RenderObject> Objects { get; set; } = new List<RenderObject>();

        private Func<float> RollForSpeed;
        private Func<float> RollForScale;

        public ParallaxLayer(List<AtlasFrame> assets, int count, (float min, float max) speed, (float min, float max) scale)
        {
            Rand = new Random();
            PossibleAssets = assets;

            var screen = Vrax.Game.Screen;
            var speedRange = speed.max - speed.min;
            var scaleRange = scale.max - scale.min;

            RollForSpeed = () =>
            {
                return (float)(Rand.NextDouble() * speedRange) + speed.min;
            };
            RollForScale = () =>
            {
                return (scaleRange == 1f) ? 1f : (float)(Rand.NextDouble() * scaleRange) + scale.min;
            };

            for (int i = 0; i < count; i++)
            {
                float objectScale = RollForScale();
                Objects.Add(new RenderObject()
                {
                    Position = new Point(screen.Width * (float)Rand.NextDouble(), screen.Height * (float)Rand.NextDouble()),
                    Asset = PossibleAssets[Rand.Next() % PossibleAssets.Count],
                    Speed = RollForSpeed(),
                    Scale = new Distance(objectScale, objectScale)
                });
            }
        }

        public void Render(Graphics g)
        {
            foreach (var renderObject in Objects)
            {
                g.Draw(renderObject.Asset, renderObject.Position, Distance.Zero, 0, renderObject.Scale, Color.White);
            }
        }

        public void Update(double deltaTime)
        {
            var screen = Vrax.Game.Screen;
            foreach (var renderObject in Objects)
            {
                // Update position
                renderObject.Position.X -= (float)(deltaTime * renderObject.Speed);

                // is offscreen?
                int width = (int)(renderObject.Asset.GetSize().Width * renderObject.Scale.X);
                if (renderObject.Position.X + width < 0)
                {
                    // Rerandomize
                    var newScale = RollForScale();

                    renderObject.Position.X = screen.Width;
                    renderObject.Asset = PossibleAssets[Rand.Next() % PossibleAssets.Count];
                    renderObject.Speed = RollForSpeed();
                    renderObject.Scale = new Distance(newScale, newScale);
                }
            }
        }

        private class RenderObject
        {
            public Point Position;
            public IDrawableAsset Asset;
            public float Speed;
            public Distance Scale;
        }
    }
}
