using PSEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    public interface IComponent
    {
        Entity Owner { get; set; }
        void Start();
        void Update(double deltaTime);
    }

    public interface IRenderable
    {
        void Render(Graphics g);
    }
}
