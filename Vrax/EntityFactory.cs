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
                Health = 3,
                Rectangle = new Rect(0, 2, 50, 14),
                Team = Team.Player
            };
            result.AddComponent(new RenderComponent(MainAtlas.GetFrame("fighter0.png")));
            result.AddComponent(new MovementComponent(300d));
            result.AddComponent(new PlayerControls());
            result.AddComponent(new InvulnerableDamageHandler(1.5d));

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

        public Entity CreateRank1Fighter()
        {
            var result = new Entity()
            {
                Health = 3,
                Rectangle = new Rect(0, 2, 52, 20),
                Team = Team.Player
            };
            result.AddComponent(new RenderComponent(MainAtlas.GetFrame("fighter1.png")));
            result.AddComponent(GetBurnerAnim(new Distance(0, 9)));
            result.AddComponent(new MovementComponent(250d));
            result.AddComponent(new PlayerControls());
            result.AddComponent(new InvulnerableDamageHandler(1.5d));

            // Starting weapon
            result.AddComponent(new WeaponComponent(new WeaponConfig()
            {
                ProjectileCreator = CreateLaser,
                ShootSpeed = 0.1,
                Damage = 1,
            })
            {
                FireOffset = new Distance(28, 9),
                ProjectileDirection = new Distance(1f, 0)
            });
            // Wing weapon
            result.AddComponent(new WeaponComponent(new WeaponConfig()
            {
                ProjectileCreator = CreateLaser,
                ShootSpeed = 0.3,
                Damage = 1,
            })
            {
                TryFire = true, // Autofire
                FireOffset = new Distance(34, 21),
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

        public Entity CreateBeamShot()
        {
            var result = new Entity()
            {
                Health = 1,
                Rectangle = new Rect(0, 0, 10, 9),
                IgnoreCollision = true
            };
            result.AddComponent(new RenderSliceComponent(MainAtlas.GetFrame("beam.png")));
            result.AddComponent(new ProjectileComponent()
            {
                Speed = 0,
            });
            result.AddComponent(new CollisionDamageComponent(0));
            result.AddComponent(new GrowComponent(200, 0.8, 250));

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

        public Entity CreateShuttleEnemy()
        {
            var result = new Entity()
            {
                Health = 1,
                Rectangle = new Rect(0, 0, 35, 19),
                Team = Team.Enemy
            };

            var weapon = new WeaponConfig()
            {
                Damage = 1,
                ProjectileCreator = CreateEnemyShot,
                ShootSpeed = 3
            };

            result.AddComponent(new RenderComponent(MainAtlas.GetFrame("shuttle.png")));
            result.AddComponent(GetJetAnim(new Distance(36, 1)));
            result.AddComponent(GetJetAnim(new Distance(36, 15)));
            result.AddComponent(new MovementComponent(150));
            result.AddComponent(new CollisionDamageComponent(1));
            result.AddComponent(new WeaponComponent(weapon)
            {
                ProjectileDirection = new Distance(-1, 0),
                FireOffset = new Distance(0, 8),
                TryFire = true
            });
            result.AddComponent(new SineMotorComponent(0.8f));

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

        public Entity CreateBeamEnemy()
        {
            var result = new Entity()
            {
                Health = 10,
                Rectangle = new Rect(0, 0, 49, 33),
                Team = Team.Enemy
            };

            var weapon = new WeaponConfig()
            {
                Damage = 2,
                ProjectileCreator = CreateBeamShot,
                ShootSpeed = 4
            };

            result.AddComponent(new RenderComponent(MainAtlas.GetFrame("beamship.png")));
            result.AddComponent(new MovementComponent(40)
            {
                MoveLeft = true
            });
            result.AddComponent(new CollisionDamageComponent(1));
            result.AddComponent(new WeaponComponent(weapon)
            {
                ProjectileDirection = new Distance(-1, 0),
                FireOffset = new Distance(2, 12),
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

        private RenderComponent GetBurnerAnim(Distance offset)
        {
            return new RenderComponent(MainAtlas.GetFrames("burner{0}.png"), 0.1)
            {
                Offset = offset + new Distance(-16, 0)
            };
        }

        private RenderComponent GetJetAnim(Distance offset)
        {
            return new RenderComponent(MainAtlas.GetFrames("jet{0}.png"), 0.1)
            {
                Offset = offset
            };
        }

        private void SpawnExplosionOnDeath(Entity entity)
        {
            var explosion = CreateExplosion();
            explosion.Position = entity.Position + new Distance((entity.Rectangle.Right / 2) - 12, (entity.Rectangle.Bottom / 2) - 12);
            Vrax.World.AddEntity(explosion);
        }
    }
}
