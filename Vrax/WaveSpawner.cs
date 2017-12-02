using PSEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax
{
    public class WaveSpawner
    {
        public int Level { get; set; }
        private List<Wave> Waves { get; set; }

        private List<Entity> Spawned { get; set; } = new List<Entity>();
        private Random Rand { get; set; }

        public int KillCount { get; set; }

        public WaveSpawner(EntityFactory factory)
        {
            Rand = new Random();

            Waves = new List<Wave>()
            {
                new Wave()
                {
                    Count = 1,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(1, factory.CreateShuttleEnemy)
                    })
                },
                new Wave()
                {
                    Count = 2,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(3, factory.CreateShuttleEnemy),
                        new SpawnChance(1, factory.CreateRocketLauncherEnemy)
                    })
                },
                new Wave()
                {
                    Count = 3,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(2, factory.CreateShuttleEnemy),
                        new SpawnChance(2, factory.CreateRocketLauncherEnemy)
                    })
                },
                new Wave()
                {
                    Count = 4,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(2, factory.CreateShuttleEnemy),
                        new SpawnChance(1, factory.CreateRocketLauncherEnemy),
                        new SpawnChance(2, factory.CreateUFOEnemy),
                    })
                },
                new Wave()
                {
                    Count = 5,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(1, factory.CreateRocketLauncherEnemy),
                        new SpawnChance(4, factory.CreateUFOEnemy),
                    })
                },
                new Wave()
                {
                    Count = 6,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(2, factory.CreateRocketLauncherEnemy),
                        new SpawnChance(2, factory.CreateUFOEnemy),
                        new SpawnChance(2, factory.CreateBeamEnemy),
                    })
                }
            };
        }

        public void Update(double deltaTime)
        {
            Level = (KillCount / 10);
            if (Level >= Waves.Count)
                Level = Waves.Count - 1;

            var currentWave = Waves[Level];

            while (Spawned.Count < currentWave.Count)
            {
                // Spawn
                var spawn = currentWave.Table.Select(Rand.Next()).SpawnMethod.Invoke();
                spawn.Position = new Point(Vrax.Game.Screen.Width, (float)(Vrax.Game.Screen.Height * 0.8f * Rand.NextDouble()));
                spawn.Disposed += OnEntityDisposed;
                spawn.Destroyed += OnEntityDestroyed;

                Spawned.Add(spawn);
                Vrax.World.AddEntity(spawn);
            }
        }

        private void OnEntityDestroyed(Entity obj)
        {
            KillCount++;
        }

        private void OnEntityDisposed(Entity entity)
        {
            Spawned.Remove(entity);
        }

        private class Wave
        {
            public int Count { get; set; }
            public WeightedTable<SpawnChance> Table { get; set; }
        }

        private class SpawnChance
        {
            public Func<Entity> SpawnMethod;
            public int Weight;

            public SpawnChance(int weight, Func<Entity> Spawner)
            {
                SpawnMethod = Spawner;
                Weight = weight;
            }
        }
    }
}
