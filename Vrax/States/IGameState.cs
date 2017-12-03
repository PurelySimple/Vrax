using PSEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.States
{
    public interface IGameState : IDisposable
    {
        IGameState NextState { get; }

        void Update(double deltaTime);
        void Draw(Graphics g);
    }
}
