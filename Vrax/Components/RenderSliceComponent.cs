using System;
using System.Collections.Generic;
using System.Text;
using PSEngine;

namespace LudumDare40.Vrax.Components
{
    class RenderSliceComponent : IComponent, IRenderable
    {
        public Entity Owner { get; set; }

        private IDrawableAsset Asset { get; set; }

        public RenderSliceComponent(IDrawableAsset asset)
        {
            Asset = asset;
        }

        public void Render(Graphics g)
        {
            g.Draw(Asset, Owner.WorldRect, Color.White);
        }

        public void Start()
        {
        }

        public void Update(double deltaTime)
        {
        }
    }
}
