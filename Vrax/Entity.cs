using LudumDare40.Vrax.Components;
using PSEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LudumDare40.Vrax
{
    public class Entity : IDisposable
    {
        public event Action<Entity> Destroyed;
        public event Action<Entity> Damaged;
        public event Action<Entity> Spawned;

        public int Health { get; set; }
        public Team Team { get; set; }
        public bool IgnoreCollision { get; set; }

        public Point Position { get; set; }
        public Rect Rectangle { get; set; }

        public Rect WorldRect => new Rect((int)Position.X + Rectangle.X, (int)Position.Y + Rectangle.Y, Rectangle.Size);

        public bool MarkedForDestruction { get; set; }

        private List<IComponent> Components { get; set; } = new List<IComponent>();
        private bool DidStart { get; set; }

        public void Start()
        {
            if (DidStart)
                return;

            foreach (var component in Components)
            {
                component.Start();
            }
            DidStart = true;

            Spawned?.Invoke(this);
        }

        public void AddComponent(IComponent component)
        {
            component.Owner = this;
            Components.Add(component);
        }

        public T GetComponent<T>()
        {
            return Components.OfType<T>().FirstOrDefault();
        }

        public void Update(double deltaTime)
        {
            if (!MarkedForDestruction && Health <= 0)
            {
                MarkedForDestruction = true;
                return;
            }

            foreach (var component in Components)
            {
                component.Update(deltaTime);
            }
        }

        public void ApplyDamage(int amount)
        {
            Health -= amount;

            if (Health > 0)
            {
                Damaged?.Invoke(this);
            }
            else
            {
                MarkedForDestruction = true;
            }
        }

        public void Dispose()
        {
            MarkedForDestruction = true;
            Destroyed?.Invoke(this);
            Destroyed = null;

            Damaged = null;
            Spawned = null;
        }
    }

    public enum Team
    {
        Player,
        Enemy
    }
}
