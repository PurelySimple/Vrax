using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax
{
    public interface IWorldContainer
    {
        void AddEntity(Entity entity);
        IReadOnlyList<Entity> AllEntities();
        Entity PlayerEntity { get; }
    }
}
