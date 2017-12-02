using LudumDare40.Vrax.Components;
using PSEngine;
using PSEngine.Core;
using System;
using System.Collections.Generic;

namespace LudumDare40.Vrax
{
    class Vrax : Game, IWorldContainer
    {
        public Font DefaultFont { get; private set; }
        public Atlas MainAtlas { get; private set; }

        private EntityFactory Factory { get; set; }
        private WaveSpawner Spawner { get; set; }

        private List<Entity> Entities { get; set; } = new List<Entity>();
        private List<ParallaxLayer> Background { get; set; } = new List<ParallaxLayer>();

        private List<IRenderable> Renderables { get; set; } = new List<IRenderable>();
        private List<Entity> ToDestroy { get; set; } = new List<Entity>();

        private Distance HealthOffset { get; set; }
        private AtlasFrame HealthFrame { get; set; }

        public Entity PlayerEntity { get; private set; }

        public static IGameAccess Game { get; private set; }
        public static IWorldContainer World { get; private set; }

        static void Main(string[] args)
        {
            var game = new Vrax();
            Game = game;
            World = game;
            game.Run();
        }

        protected override void Init(GameSettings settings)
        {
            settings.Graphics.SetNativeSize(960, 540, ScaleMode.FitWidth);
            settings.Graphics.SetScreenSize(1920, 1080, false);
            settings.Graphics.VSync = true;

            settings.UpdateHertz = 60;
            settings.Name = "Vrax - Ludum Dare 40";
        }

        protected override void OnWindowResized()
        {
        }

        protected override void Shutdown()
        {
        }

        protected override void Start()
        {
            DefaultFont = AssetCache.LoadFont("DefaultFont.fnt");
            MainAtlas = AssetCache.LoadAtlas("Atlas.json");

            // Setup slices
            var beamTexture = MainAtlas.GetFrame("beam.png");
            beamTexture.Slice = new SliceSettings()
            {
                Left = 2,
                Right = 2
            };

            //Background.Add(new ParallaxLayer(MainAtlas.GetFrames("star{0}.png"), 100, (10, 20), (1f, 1f)));
            Background.Add(new ParallaxLayer(MainAtlas.GetFrames("nebula{0}.png"), 50, (20, 45), (1f, 1f)));
            Background.Add(new ParallaxLayer(MainAtlas.GetFrames("star{0}.png"), 200, (100, 150), (1f, 1f)));

            // Load sounds
            AssetCache.LoadSound("hit0.wav");
            AssetCache.LoadSound("hit1.wav");
            AssetCache.LoadSound("hit2.wav");
            AssetCache.LoadSound("explosion0.wav");
            AssetCache.LoadSound("explosion1.wav");
            AssetCache.LoadSound("explosion2.wav");

            Factory = new EntityFactory(AssetCache);
            Spawner = new WaveSpawner(Factory);

            PlayerEntity = Factory.CreateRank1Fighter();
            PlayerEntity.Position = new Point(50, Screen.Half.Height);
            PlayerEntity.Damaged += OnPlayerDamaged;
            PlayerEntity.Destroyed += OnPlayerDestroyed;

            CreateUI();

            AddEntity(PlayerEntity);

            //var tempEnemy = Factory.CreateBoxEnemy();
            //tempEnemy.Position = new Point(Screen.Width - 50, Screen.Half.Height);
            //AddEntity(tempEnemy);

            Input.Register(Controls.Up, Keycode.W, Keycode.Up);
            Input.Register(Controls.Left, Keycode.A, Keycode.Left);
            Input.Register(Controls.Down, Keycode.S, Keycode.Down);
            Input.Register(Controls.Right, Keycode.D, Keycode.Right);
            Input.Register(Controls.Fire, MouseButton.Left, MouseButtonState.Pressed);
            Input.Register(Controls.Fire, Keycode.Space);
        }

        private void OnPlayerDestroyed(Entity obj)
        {
            var gameOverText = new Textfield(DefaultFont, "GAME OVER")
            {
                Parent = Display,
                //Scale = new Distance(2f, 2f),
                Position = Screen.Half,
                Anchor = Distance.Center,
                Color = Color.Alpha(0)
            };
            Animator.AddSequence(new[]
            {
                gameOverText.ColorTo(Color.White, 800),
                new DelayTween(2500),
                new ActionTween(() => gameOverText.Parent = null)
            });

        }

        private void OnPlayerDamaged(Entity obj)
        {
            Spawner.KillCount /= 2;
        }

        private void CreateUI()
        {
            var title = new Textfield(DefaultFont, "Vrax Ludum Dare 40 Game!")
            {
                Parent = Display,
                Position = new Point(Screen.Half.Width, -50),
                //Position = new Point(Screen.Half.Width, Screen.Half.Height * 0.3f),
                Anchor = Distance.Center
            };
            Animator.AddSequence(new[]
            {
                title.MoveTo(new Point(Screen.Half.Width, Screen.Half.Height * 0.3f), 800, Ease.BounceOut),
                new DelayTween(1000),
                title.MoveRelative(new Distance(-Screen.Width, 0), 1500, Ease.CubicOut),
                new ActionTween(() => title.Parent = null)
            });

            HealthFrame = MainAtlas.GetFrame("health.png");
            HealthOffset = new Distance(DefaultFont.GetTextWidth("Health:") + 12, 9);
        }

        protected override void Update(double deltaTime)
        {
            UpdateBackground(deltaTime);
            UpdateEntities(deltaTime);
            Spawner.Update(deltaTime);
        }

        private void UpdateBackground(double deltaTime)
        {
            foreach (var bg in Background)
            {
                bg.Update(deltaTime);
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

        protected override void Draw(Graphics g, float framePercent)
        {
            foreach (var bg in Background)
            {
                bg.Render(g);
            }

            foreach (var renderable in Renderables)
            {
                renderable.Render(g);
            }

            DrawUI(g);


            // DEBUG RENDER
            g.DrawText(DefaultFont, $"FPS: {FPS}", new Point(0, Screen.Height - DefaultFont.TextHeight));
            g.DrawText(DefaultFont, $"E: {Entities.Count}", new Point(Screen.Width, 0), TextAlign.Right);
            g.DrawText(DefaultFont, $"K: {Spawner.KillCount}", new Point(Screen.Width, 20), TextAlign.Right);
        }

        private void DrawUI(Graphics g)
        {
            g.DrawText(DefaultFont, "Health:", Point.Zero);
            for (int i = 0; i < PlayerEntity.Health; i++)
            {
                g.Draw(MainAtlas.GetFrame("health.png"), (int)HealthOffset.X + (i * (HealthFrame.Width + 12)), (int)HealthOffset.Y);
            }
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

    public enum Controls
    {
        Up,
        Down,
        Left,
        Right,
        Fire
    }
}
