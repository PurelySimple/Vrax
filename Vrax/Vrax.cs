using LudumDare40.Vrax.Components;
using LudumDare40.Vrax.States;
using PSEngine;
using PSEngine.Core;
using System;
using System.Collections.Generic;

namespace LudumDare40.Vrax
{
    class Vrax : Game
    {
        public Font DefaultFont { get; private set; }
        public Atlas MainAtlas { get; private set; }

        private EntityFactory Factory { get; set; }
        private IGameState State { get; set; }

        private List<ParallaxLayer> Background { get; set; } = new List<ParallaxLayer>();

        public static IGameAccess Game { get; private set; }
        public static IWorldContainer World { get; private set; }
        public static bool EnableDebug { get; set; }

        static void Main(string[] args)
        {
            var game = new Vrax();
            Game = game;
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
            AssetCache.LoadTexture("Title.png");

            Audio.PlayMusic(AssetCache.LoadMusic("song.mp3"));

            // Setup slices
            var beamTexture = MainAtlas.GetFrame("beam.png");
            beamTexture.Slice = new SliceSettings()
            {
                Left = 2,
                Right = 2
            };

            var meterHolder = MainAtlas.GetFrame("meterholder.png");
            meterHolder.Slice = new SliceSettings()
            {
                Left = 8,
                Right = 8
            };

            var meter = MainAtlas.GetFrame("meter.png");
            meter.Slice = new SliceSettings()
            {
                Left = 2,
                Right = 2
            };

            Background.Add(new ParallaxLayer(MainAtlas.GetFrames("nebula{0}.png"), 50, (20, 45), (1f, 1f)));
            Background.Add(new ParallaxLayer(MainAtlas.GetFrames("star{0}.png"), 200, (100, 150), (1f, 1f)));

            // Load sounds
            AssetCache.LoadSound("hit0.wav");
            AssetCache.LoadSound("hit1.wav");
            AssetCache.LoadSound("hit2.wav");
            AssetCache.LoadSound("explosion0.wav");
            AssetCache.LoadSound("explosion1.wav");
            AssetCache.LoadSound("explosion2.wav");
            AssetCache.LoadSound("transform.wav");
            AssetCache.LoadSound("shieldup.wav");
            AssetCache.LoadSound("shielddown.wav");

            Factory = new EntityFactory(AssetCache);

            Input.Register(Controls.Cancel, Keycode.Escape);
            Input.Register(Controls.Up, Keycode.W, Keycode.Up);
            Input.Register(Controls.Left, Keycode.A, Keycode.Left);
            Input.Register(Controls.Down, Keycode.S, Keycode.Down);
            Input.Register(Controls.Right, Keycode.D, Keycode.Right);
            Input.Register(Controls.Fire, MouseButton.Left, MouseButtonState.Pressed);
            Input.Register(Controls.Fire, Keycode.Space);

            Input.Register(Controls.TransformSmaller, Keycode.Q);
            Input.Register(Controls.TransformSmaller, MouseButton.X1, MouseButtonState.Released);
            Input.Register(Controls.TransformBigger, Keycode.E);
            Input.Register(Controls.TransformBigger, MouseButton.X2, MouseButtonState.Released);

            var action = Input.Register(Controls.DisableMusic, Keycode.M);
            action.OnReleased += OnDisableMusic;

            action = Input.Register(Controls.Debug, Keycode.F1);
            action.OnReleased += () => EnableDebug = !EnableDebug;

            State = new MenuState(Factory);
        }

        private void OnDisableMusic()
        {
            Audio.StopMusic();
        }

        protected override void Update(double deltaTime)
        {
            UpdateBackground(deltaTime);
            State?.Update(deltaTime);

            if (State?.NextState != null)
            {
                var newState = State.NextState;
                State.Dispose();
                State = newState;

                World = newState as IWorldContainer;
            }
        }

        private void UpdateBackground(double deltaTime)
        {
            foreach (var bg in Background)
            {
                bg.Update(deltaTime);
            }
        }

        protected override void Draw(Graphics g, float framePercent)
        {
            foreach (var bg in Background)
            {
                bg.Render(g);
            }

            State?.Draw(g);

            // DEBUG
            if (EnableDebug)
                g.DrawText(DefaultFont, $"FPS: {FPS}", new Point(0, Screen.Height - DefaultFont.TextHeight));
        }
    }

    public enum Controls
    {
        Cancel,
        Up,
        Down,
        Left,
        Right,
        Fire,
        DisableMusic,
        TransformSmaller,
        TransformBigger,

        Debug
    }
}
