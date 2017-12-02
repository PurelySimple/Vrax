using LudumDare40.Vrax.Components;
using PSEngine;
using PSEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax
{
    public class EntityFactory
    {
        private AssetCache Cache { get; set; }
        private Atlas MainAtlas { get; set; }
        private Random Rand { get; set; }

        private AtlasFrame BoxEnemyAsset { get; set; }
        private AtlasFrame LaserAsset { get; set; }
        private AtlasFrame EnemyShot { get; set; }

        private Sound LaserSound { get; set; }
        private List<Sound> ExplodeSounds { get; set; }

        public EntityFactory(AssetCache assetCache)
        {
            Rand = new Random();
            Cache = assetCache;
            MainAtlas = assetCache.Get<Atlas>("Atlas.json");
            if (MainAtlas == null)
            {
                MainAtlas = assetCache.LoadAtlas("Atlas.json");
            }

            BoxEnemyAsset = MainAtlas.GetFrame("enemy.png");
            LaserAsset = MainAtlas.GetFrame("laser.png");
            EnemyShot = MainAtlas.GetFrame("enemy_shot.png");

            LaserSound = assetCache.LoadSound("laserfire.wav");

            ExplodeSounds = new List<Sound>()
            {
                assetCache.Get<Sound>("explosion0.wav"),
                assetCache.Get<Sound>("explosion1.wav"),
                assetCache.Get<Sound>("explosion2.wav"),
            };
        }

        public Entity CreateStartingPlayer()
        {
            var result = new Entity()
            {
                Health = 5,
                Rectangle = new Rect(8, 4, 25, 9),
                Team = Team.Player
            };
            var shipFrames = MainAtlas.GetFrames("ship{0}.png");
            result.AddComponent(new RenderComponent(shipFrames, 0.2d));
            result.AddComponent(new MovementComponent(300d));
            result.AddComponent(new PlayerControls());
            result.AddComponent(new InvulnerableDamageHandler(1d));

            // Starting weapon
            result.AddComponent(new WeaponComponent(new WeaponConfig()
            {
                ProjectileCreator = CreateLaser,
                ShootSpeed = 0.1,
                Damage = 1,
            })
            {
                FireOffset = new Distance(32, 9),
                ProjectileDirection = new Distance(1f, 0)
            });

            result.Destroyed += SpawnExplosionOnDeath;

            return result;
        }

        public Entity CreateLaser()
        {
            var result = new Entity()
            {
                Health = 1,
                Rectangle = new Rect(0, 0, 12, 2),
                IgnoreCollision = true
            };
            result.AddComponent(new CollisionDamageComponent(0) { DestroyOnCollide = true });
            result.AddComponent(new RenderComponent(LaserAsset));
            result.AddComponent(new ProjectileComponent()
            {
                Speed = 500
            });

            return result;
        }

        public Entity CreateEnemyShot()
        {
            var result = new Entity()
            {
                Health = 1,
                Rectangle = new Rect(0, 0, 10, 5),
                IgnoreCollision = true
            };
            result.AddComponent(new CollisionDamageComponent(0) { DestroyOnCollide = true });
            result.AddComponent(new RenderComponent(EnemyShot));
            result.AddComponent(new ProjectileComponent()
            {
                Speed = 350
            });

            return result;
        }

        public Entity CreateOrbShot()
        {
            var result = new Entity()
            {
                Health = 1,
                Rectangle = new Rect(0, 0, 8, 8),
                IgnoreCollision = true
            };
            result.AddComponent(new CollisionDamageComponent(0) { DestroyOnCollide = true });
            result.AddComponent(new RenderComponent(MainAtlas.GetFrame("orb.png")));
            result.AddComponent(new ProjectileComponent()
            {
                Speed = 400
            });

            return result;
        }

        public Entity CreateRocketShot()
        {
            var result = new Entity()
            {
                Health = 1,
                Rectangle = new Rect(0, 0, 16, 8),
                IgnoreCollision = true
            };
            result.AddComponent(new RenderComponent(MainAtlas.GetFrame("rocket.png")));
            result.AddComponent(new ProjectileComponent()
            {
                Speed = 50
            });
            result.AddComponent(new CollisionDamageComponent(0) { DestroyOnCollide = true });
            result.AddComponent(new AccelerationComponent(350, 1, Ease.Linear));
            result.Destroyed += SpawnExplosionOnDeath;

            return result;
        }

        public Entity CreateExplosion()
        {
            var result = new Entity()
            {
                Health = 1,
                Rectangle = new Rect(0, 0, 1, 1),
                IgnoreCollision = true
            };

            result.AddComponent(new RenderComponent(MainAtlas.GetFrames("splosion_{0}.png"), 0.1f));
            result.AddComponent(new LifespanComponent(0.49f));

            result.Spawned += e =>
            {
                Vrax.Game.Audio.PlaySound(ExplodeSounds[Rand.Next() % ExplodeSounds.Count]);
            };

            return result;
        }

        public Entity CreateBoxEnemy()
        {
            var result = new Entity()
            {
                Health = 1,
                Rectangle = new Rect(0, 0, 15, 16),
                Team = Team.Enemy
            };

            var weapon = new WeaponConfig()
            {
                Damage = 1,
                ProjectileCreator = CreateEnemyShot,
                ShootSpeed = 1
            };

            result.AddComponent(new RenderComponent(BoxEnemyAsset));
            result.AddComponent(new MovementComponent(100));
            result.AddComponent(new CollisionDamageComponent(1));
            result.AddComponent(new WeaponComponent(weapon)
            {
                ProjectileDirection = new Distance(-1, 0),
                FireOffset = new Distance(0, 8),
                TryFire = true
            });
            result.AddComponent(new SineMotorComponent(0.5f));

            result.Destroyed += SpawnExplosionOnDeath;

            return result;
        }

        public Entity CreateRocketLauncherEnemy()
        {
            var result = new Entity()
            {
                Health = 1,
                Rectangle = new Rect(0, 0, 38, 29),
                Team = Team.Enemy
            };

            var weapon = new WeaponConfig()
            {
                Damage = 2,
                ProjectileCreator = CreateRocketShot,
                ShootSpeed = 1
            };

            result.AddComponent(new RenderComponent(MainAtlas.GetFrame("rocketlauncher.png")));
            result.AddComponent(new MovementComponent(75)
            {
                MoveLeft = true
            });
            result.AddComponent(new CollisionDamageComponent(1));
            result.AddComponent(new WeaponComponent(weapon)
            {
                ProjectileDirection = new Distance(-1, 0),
                FireOffset = new Distance(2, 0),
                TryFire = true
            });

            result.Destroyed += SpawnExplosionOnDeath;

            return result;
        }

        public Entity CreateUFOEnemy()
        {
            var result = new Entity()
            {
                Health = 1,
                Rectangle = new Rect(0, 0, 29, 16),
                Team = Team.Enemy
            };

            var weapon = new WeaponConfig()
            {
                Damage = 1,
                ProjectileCreator = CreateOrbShot,
                ShootSpeed = 1.5
            };

            result.AddComponent(new RenderComponent(MainAtlas.GetFrame("ufo.png")));
            result.AddComponent(new MovementComponent(75)
            {
                MoveLeft = true
            });
            result.AddComponent(new CollisionDamageComponent(1));
            result.AddComponent(new WeaponComponent(weapon)
            {
                ProjectileDirection = new Distance(-1, 0),
                FireOffset = new Distance(2, 0),
                TryFire = true,
                PlayerTracking = true
            });
            result.AddComponent(new SineMotorComponent(1f));

            result.Destroyed += SpawnExplosionOnDeath;

            return result;
        }

        private void SpawnExplosionOnDeath(Entity entity)
        {
            var explosion = CreateExplosion();
            explosion.Position = entity.Position + new Distance((entity.Rectangle.Right / 2) - 12, (entity.Rectangle.Bottom / 2) - 12);
            Vrax.World.AddEntity(explosion);
        }
    }
}
