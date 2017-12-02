using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    public interface IDamageHandler
    {
        void Damage(int amount, Entity source);
    }
}
