using PSEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LudumDare40.Vrax
{
    public class WaveSpawner : IDisposable
    {
        public event Action<int> KillCountChanged;

        public int Level { get; set; }
        private List<Wave> Waves { get; set; }

        private List<Entity> Spawned { get; set; } = new List<Entity>();
        private Random Rand { get; set; }

        public int KillCount { get; set; }

        public float WaveProgress => (float)KillCount / Waves[Level].KillRequirement;

        public WaveSpawner(EntityFactory factory)
        {
            Rand = new Random();

            Waves = new List<Wave>()
            {
                new Wave()
                {
                    KillRequirement = 5,
                    SpawnCount = 1,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(1, factory.CreateUFOEnemy)
                    })
                },
                new Wave()
                {
                    KillRequirement = 20,
                    SpawnCount = 2,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(5, factory.CreateUFOEnemy),
                        new SpawnChance(2, factory.CreateRocketLauncherEnemy)
                    })
                },
                new Wave()
                {
                    KillRequirement = 25,
                    SpawnCount = 2,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(1, factory.CreateDartEnemy),
                    })
                },
                new Wave()
                {
                    KillRequirement = 30,
                    SpawnCount = 3,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(1, factory.CreateRocketLauncherEnemy),
                        new SpawnChance(2, factory.CreateDartEnemy),
                    })
                },
                new Wave()
                {
                    KillRequirement = 40,
                    SpawnCount = 3,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(2, factory.CreateShuttleEnemy),
                        new SpawnChance(2, factory.CreateUFOEnemy),
                    })
                },
                new Wave()
                {
                    KillRequirement = 50,
                    SpawnCount = 4,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(1, factory.CreateDartEnemy),
                        new SpawnChance(2, factory.CreateUFOEnemy),
                    })
                },
                new Wave()
                {
                    KillRequirement = 55,
                    SpawnCount = 4,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(2, factory.CreateBeamEnemy),
                    })
                },
                new Wave()
                {
                    KillRequirement = 60,
                    SpawnCount = 4,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(1, factory.CreateBeamEnemy),
                        new SpawnChance(2, factory.CreateRocketLauncherEnemy),
                    })
                },
                new Wave()
                {
                    KillRequirement = 65,
                    SpawnCount = 5,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(1, factory.CreateShuttleEnemy),
                    })
                },
                new Wave()
                {
                    KillRequirement = 70,
                    SpawnCount = 5,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(2, factory.CreateShuttleEnemy),
                        new SpawnChance(2, factory.CreateRocketLauncherEnemy),
                        new SpawnChance(2, factory.CreateUFOEnemy),
                        new SpawnChance(1, factory.CreateBeamEnemy),
                    })
                },
                new Wave()
                {
                    KillRequirement = 80,
                    SpawnCount = 6,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(2, factory.CreateShuttleEnemy),
                        new SpawnChance(2, factory.CreateRocketLauncherEnemy),
                        new SpawnChance(2, factory.CreateUFOEnemy),
                        new SpawnChance(2, factory.CreateBeamEnemy),
                        new SpawnChance(2, factory.CreateDartEnemy)
                    })
                },
                new Wave()
                {
                    KillRequirement = 90,
                    SpawnCount = 8,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(2, factory.CreateShuttleEnemy),
                        new SpawnChance(2, factory.CreateRocketLauncherEnemy),
                        new SpawnChance(2, factory.CreateUFOEnemy),
                        new SpawnChance(2, factory.CreateBeamEnemy),
                        new SpawnChance(2, factory.CreateDartEnemy)
                    })
                },
                new Wave()
                {
                    KillRequirement = 100,
                    SpawnCount = 10,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(2, factory.CreateShuttleEnemy),
                        new SpawnChance(2, factory.CreateRocketLauncherEnemy),
                        new SpawnChance(2, factory.CreateUFOEnemy),
                        new SpawnChance(2, factory.CreateBeamEnemy),
                        new SpawnChance(2, factory.CreateDartEnemy)
                    })
                },
                new Wave()
                {
                    KillRequirement = 110,
                    SpawnCount = 12,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(2, factory.CreateShuttleEnemy),
                        new SpawnChance(2, factory.CreateRocketLauncherEnemy),
                        new SpawnChance(2, factory.CreateUFOEnemy),
                        new SpawnChance(2, factory.CreateBeamEnemy),
                        new SpawnChance(2, factory.CreateDartEnemy)
                    })
                },
                new Wave()
                {
                    KillRequirement = 150,
                    SpawnCount = 14,
                    Table = new WeightedTable<SpawnChance>(e => e.Weight, new[]
                    {
                        new SpawnChance(2, factory.CreateShuttleEnemy),
                        new SpawnChance(2, factory.CreateRocketLauncherEnemy),
                        new SpawnChance(2, factory.CreateUFOEnemy),
                        new SpawnChance(2, factory.CreateBeamEnemy),
                        new SpawnChance(2, factory.CreateDartEnemy)
                    })
                },
            };
        }

        public void Update(double deltaTime)
        {
            var currentWave = Waves[Level];

            while (Spawned.Count < currentWave.SpawnCount)
            {
                float spawnRange = Vrax.Game.Screen.Height * 0.8f;
                float spawnYOffset = Vrax.Game.Screen.Height * 0.1f;
                // Spawn
                var spawn = currentWave.Table.Select(Rand.Next()).SpawnMethod.Invoke();
                spawn.Position = new Point(Vrax.Game.Screen.Width, (float)(spawnRange * Rand.NextDouble()) + spawnYOffset);
                spawn.Disposed += OnEntityDisposed;
                spawn.Destroyed += OnEntityDestroyed;

                Spawned.Add(spawn);
                Vrax.World.AddEntity(spawn);
            }
        }

        private void OnEntityDestroyed(Entity obj)
        {
            KillCount++;

            Level = Waves.FindIndex(w => KillCount < w.KillRequirement);
            if (Level < 0)
                Level = Waves.Count - 1;

            KillCountChanged?.Invoke(Level);
        }

        private void OnEntityDisposed(Entity entity)
        {
            Spawned.Remove(entity);
        }

        public void Dispose()
        {
            if (Spawned != null)
            {
                foreach (var spawn in Spawned)
                {
                    spawn.Disposed -= OnEntityDisposed;
                    spawn.Destroyed -= OnEntityDisposed;

                    spawn.Dispose();
                }
                Spawned.Clear();
            }
            Spawned = null;
        }

        private class Wave
        {
            public int KillRequirement { get; set; }
            public int SpawnCount { get; set; }
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
