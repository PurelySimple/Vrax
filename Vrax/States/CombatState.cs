using System;
using System.Collections.Generic;
using System.Text;
using PSEngine;
using PSEngine.Core;
using LudumDare40.Vrax.Components;

namespace LudumDare40.Vrax.States
{
    class CombatState : IGameState, IWorldContainer
    {
        private const int MeterWidth = 161;
        private const int MeterHeight = 30;

        private const int KillsPerTransform = 10;

        public IGameState NextState { get; private set; }
        public Entity PlayerEntity { get; private set; }

        private Font DefaultFont { get; set; }
        private Atlas MainAtlas { get; set; }

        private EntityFactory Factory { get; set; }
        private WaveSpawner Spawner { get; set; }

        private List<Entity> Entities { get; set; } = new List<Entity>();
        private List<IRenderable> Renderables { get; set; } = new List<IRenderable>();
        private List<Entity> ToDestroy { get; set; } = new List<Entity>();

        private Distance HealthOffset { get; set; }
        private AtlasFrame HealthFrame { get; set; }
        private Distance TransformOffset { get; set; }
        private AtlasFrame TransformFrame { get; set; }

        private DisplayContainer UILayer { get; set; }
        private Image MeterHolder { get; set; }
        private Image Meter { get; set; }
        private Textfield LevelCounterTextfield { get; set; }
        private Textfield KillCounterTextfield { get; set; }
        private float MeterPercent { get; set; }

        private Image ShieldImage { get; set; }
        private Textfield ShieldCounter { get; set; }

        private DisplayContainer Display => Vrax.Game.Display;
        private Size Screen => Vrax.Game.Screen;
        private AssetCache AssetCache => Vrax.Game.AssetCache;

        private int TransformsAvailable { get; set; }
        private int PartialTransforms { get; set; }
        private List<Func<Entity>> TransformOrder { get; set; }
        private int TransformIndex { get; set; }

        public CombatState(EntityFactory factory)
        {
            DefaultFont = AssetCache.Get<Font>("DefaultFont.fnt");
            MainAtlas = AssetCache.Get<Atlas>("Atlas.json");


            Factory = factory;
            Spawner = new WaveSpawner(factory);

            TransformOrder = new List<Func<Entity>>()
            {
                Factory.CreateStartingPlayer,
                Factory.CreateRank1Fighter,
                Factory.CreateBomber,
                Factory.CreateHovercraft,
            };

            CreateUI();

            SpawnPlayer();
        }

        private void CreateUI()
        {
            UILayer = new DisplayContainer()
            {
                Parent = Display
            };

            var title = new Textfield(DefaultFont, "Begin!")
            {
                Parent = UILayer,
                Position = new Point(Screen.Half.Width, -50),
                Anchor = Distance.Center
            };
            Vrax.Game.Animator.AddSequence(new[]
            {
                title.MoveTo(new Point(Screen.Half.Width, Screen.Half.Height * 0.4f), 800, Ease.BounceOut),
                new DelayTween(1500),
                title.MoveRelative(new Distance(-Screen.Width, 0), 1500, Ease.CubicOut),
                new ActionTween(() => title.Parent = null)
            });

            HealthFrame = MainAtlas.GetFrame("health.png");
            HealthOffset = new Distance(DefaultFont.GetTextWidth("HP:") + 12, 9);
            TransformFrame = MainAtlas.GetFrame("transform.png");
            TransformOffset = new Distance(DefaultFont.GetTextWidth("TX:") + 12, 35);

            MeterHolder = new Image(MainAtlas.GetFrame("meterholder.png"))
            {
                Parent = UILayer,
                FixedSize = new Size(171, 40),
                Position = new Point((Screen.Width - 171) * 0.5f, Screen.Height - 50)
            };
            Meter = new Image(MainAtlas.GetFrame("meter.png"))
            {
                Parent = UILayer,
                Position = MeterHolder.Position + new Distance(5, 5),
                FixedSize = new Size(0, MeterHeight)
            };
            KillCounterTextfield = new Textfield(DefaultFont, "0")
            {
                Parent = UILayer,
                Position = new Point(Screen.Half.Width, MeterHolder.Position.Y + 5),
                Color = Color.Black,
                Align = TextAlign.Center,
                Anchor = new Distance(0.5f, 0)
            };
            Spawner.KillCountChanged += OnKillCountChanged;

            var counterHolder = new Image(MainAtlas.GetFrame("meterholder.png"))
            {
                Parent = UILayer,
                FixedSize = new Size(45, 40),
                Position = MeterHolder.Position - new Distance(55, 0)
            };

            LevelCounterTextfield = new Textfield(DefaultFont, "0")
            {
                Parent = UILayer,
                Position = counterHolder.Position + new Distance(22, 5),
                Color = Color.Black,
                Align = TextAlign.Center,
                Anchor = new Distance(0.5f, 0)
            };

            // Shield counter
            ShieldImage = new Image(MainAtlas.GetFrame("shieldicon.png"))
            {
                Parent = UILayer,
                Position = MeterHolder.Position + new Distance(MeterWidth + 40, 20),
                Anchor = Distance.Center,
                Color =  Color.None
            };

            ShieldCounter = new Textfield(DefaultFont, "0")
            {
                Parent = UILayer,
                Position = ShieldImage.Position,
                Anchor = Distance.Center,
                Scale = new Distance(0.5f, 0.5f),
                Color = Color.None
            };
        }

        private void OnKillCountChanged(int level)
        {
            var newPercent = Spawner.WaveProgress;
            if (newPercent > 1)
                newPercent = 1;

            Meter.FixedSize = new Size((int)(MeterWidth * newPercent), MeterHeight);

            KillCounterTextfield.Text = Spawner.KillCount.ToString();
            LevelCounterTextfield.Text = level.ToString();

            // Update transform count
            PartialTransforms++;
            if (PartialTransforms == KillsPerTransform)
            {
                TransformsAvailable++;
                PartialTransforms = 0;
            }
        }

        private void SpawnPlayer()
        {
            //PlayerEntity = Factory.CreateBomber();
            PlayerEntity = TransformOrder[TransformIndex].Invoke();
            PlayerEntity.Position = new Point(50, Screen.Half.Height);
            PlayerEntity.Destroyed += OnPlayerDestroyed;
            AddEntity(PlayerEntity);
        }

        private void TransformPlayer()
        {
            TransformsAvailable--;

            var newPlayerEntity = TransformOrder[TransformIndex].Invoke();
            newPlayerEntity.Position = PlayerEntity.Position;
            newPlayerEntity.Destroyed += OnPlayerDestroyed;

            Entities.Remove(PlayerEntity);
            PlayerEntity.Dispose();

            PlayerEntity = newPlayerEntity;
            AddEntity(PlayerEntity);

            var shieldComponent = PlayerEntity.GetComponent<ShieldComponent>();
            if (shieldComponent != null)
            {
                ShieldCounter.Color = Color.White;
                ShieldImage.Color = Color.White;

                ShieldCounter.Text = shieldComponent.ShieldEntity.Health.ToString();
                shieldComponent.ShieldEntity.Damaged += e => ShieldCounter.Text = e.Health.ToString();
            }
            else
            {
                ShieldCounter.Color = Color.None;
                ShieldImage.Color = Color.None;
            }

            Vrax.Game.Audio.PlaySound(Vrax.Game.AssetCache.Get<Sound>("transform.wav"));
        }

        public void Draw(Graphics g)
        {
            foreach (var renderable in Renderables)
            {
                renderable.Render(g);
            }

            DrawUI(g);

            // DEBUG RENDER
            //g.DrawText(DefaultFont, $"E: {Entities.Count}", new Point(Screen.Width, 0), TextAlign.Right);
            //g.DrawText(DefaultFont, $"K: {Spawner.KillCount}", new Point(Screen.Width, 20), TextAlign.Right);
        }

        public void Update(double deltaTime)
        {
            var input = Vrax.Game.Input;
            if (!PlayerEntity.MarkedForDestruction && TransformsAvailable > 0)
            {
                if (input.IsActionKeyReleased(Controls.TransformSmaller))
                {
                    if (TransformIndex > 0)
                    {
                        TransformIndex--;
                        TransformPlayer();
                    }
                }
                else if (input.IsActionKeyReleased(Controls.TransformBigger))
                {
                    if (TransformIndex < TransformOrder.Count - 1)
                    {
                        TransformIndex++;
                        TransformPlayer();
                    }
                }
            }

            UpdateEntities(deltaTime);
            Spawner.Update(deltaTime);
        }

        public void Dispose()
        {
            Spawner.Dispose();
            UILayer.Dispose();

            foreach (var entity in Entities)
            {
                entity.Dispose();
            }

            Entities = null;
            Renderables = null;
            ToDestroy = null;

            NextState = null;
        }

        private void DrawUI(Graphics g)
        {
            g.DrawText(DefaultFont, "HP:", Point.Zero);
            for (int i = 0; i < PlayerEntity.Health; i++)
            {
                g.Draw(HealthFrame, (int)HealthOffset.X + (i * (HealthFrame.Width + 12)), (int)HealthOffset.Y);
            }

            g.DrawText(DefaultFont, "TX:", new Point(0, 25));
            for (int i = 0; i < TransformsAvailable; i++)
            {
                g.Draw(TransformFrame, (int)TransformOffset.X + (i * (TransformFrame.Width + 12)), (int)TransformOffset.Y);
            }
        }

        private void UpdateEntities(double deltaTime)
        {
            Renderables.Clear();
            ToDestroy.Clear();

            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                var entity = Entities[i];
                entity.Update(deltaTime);

                if (entity.MarkedForDestruction)
                {
                    ToDestroy.Add(entity);
                }
                else
                {
                    var renderables = entity.GetComponents<IRenderable>();
                    foreach (var renderable in renderables)
                    {
                        Renderables.Add(renderable);
                    }
                }
            }

            foreach (var destroyedEntity in ToDestroy)
            {
                Entities.Remove(destroyedEntity);
                destroyedEntity.Dispose();
            }
        }

        private void OnPlayerDestroyed(Entity obj)
        {
            var display = Vrax.Game.Display;
            var screen = Vrax.Game.Screen;

            var gameOverText = new Textfield(DefaultFont, "GAME OVER")
            {
                Parent = display,
                Position = screen.Half,
                Anchor = Distance.Center,
                Color = Color.Alpha(0)
            };
            Vrax.Game.Animator.AddSequence(new[]
            {
                gameOverText.ColorTo(Color.White, 800),
                new DelayTween(2500),
                new ActionTween(() => gameOverText.Parent = null),
                new ActionTween(ReturnToMenu)
            });
        }

        private void ReturnToMenu()
        {
            NextState = new MenuState(Factory);
        }

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
            entity.Start();
        }

        public IReadOnlyList<Entity> AllEntities()
        {
            return Entities;
        }
    }
}
