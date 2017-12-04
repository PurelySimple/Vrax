using System;
using System.Collections.Generic;
using System.Text;
using PSEngine;
using PSEngine.Core;

namespace LudumDare40.Vrax.States
{
    class HelpState : IGameState
    {
        public IGameState NextState { get; private set; }
        private EntityFactory Factory { get; set; }

        private DisplayContainer Display { get; set; }

        private int PageIndex { get; set; }
        private List<DisplayContainer> Pages { get; set; } = new List<DisplayContainer>();

        private Image LeftArrow { get; set; }
        private Image RightArrow { get; set; }

        private Atlas MainAtlas { get; set; }
        private Font DefaultFont { get; set; }
        private Font HelpFont { get; set; }

        private Size Screen => Vrax.Game.Screen;

        public HelpState(EntityFactory factory)
        {
            Factory = factory;
            MainAtlas = Vrax.Game.AssetCache.Get<Atlas>("Atlas.json");
            DefaultFont = Vrax.Game.AssetCache.Get<Font>("DefaultFont.fnt");

            HelpFont = Vrax.Game.AssetCache.Get<Font>("HelpFont.fnt");
            if (HelpFont == null)
            {
                HelpFont = Vrax.Game.AssetCache.LoadFont("HelpFont.fnt");
            }

            Display = new DisplayContainer()
            {
                Parent = Vrax.Game.Display
            };

            CreateBaseUI();

            // Create pages
            CreateControlsPage();
            CreateUIHelpPage();
            CreateTransformPage();
            CreateAboutPage();

            Display.Add(Pages[0]);
        }

        private void CreateBaseUI()
        {
            var screen = Vrax.Game.Screen;
            var chevronTexture = MainAtlas.GetFrame("chevron.png");

            var backImage = new Image(chevronTexture)
            {
                Parent = Display,
                Scale = new Distance(-1f, 1), // flip
                Position = new Point(10 + chevronTexture.Width, 10)
            };
            var backButton = new Button(
                new Button.State() { },
                new Button.State() { },
                new Button.State()
                {
                    Scale = 0.95f
                });
            backButton.ClickEvent += OnExitHelp;
            backImage.Attach(backButton);

            LeftArrow = new Image(chevronTexture)
            {
                Parent = Display,
                Scale = new Distance(-1f, 1),
                Position = new Point(screen.Half.Width - chevronTexture.Width - 10, screen.Height - chevronTexture.Height - 10)
            };
            var leftButton = new Button(new Button.State(), new Button.State(), new Button.State()
            {
                Scale = 0.95f
            });
            leftButton.ClickEvent += OnLeftClicked;
            LeftArrow.Attach(leftButton);

            RightArrow = new Image(chevronTexture)
            {
                Parent = Display,
                Position = new Point(screen.Half.Width + 10, screen.Height - chevronTexture.Height - 10)
            };
            var rightButton = new Button(new Button.State(), new Button.State(), new Button.State()
            {
                Scale = 0.95f
            });
            rightButton.ClickEvent += OnRightClicked;
            RightArrow.Attach(rightButton);
        }

        private void OnRightClicked()
        {
            if (PageIndex < Pages.Count - 1)
            {
                // Remove current page
                var current = Pages[PageIndex];
                current.Parent = null;

                PageIndex++;
                Pages[PageIndex].Parent = Display;
            }
        }

        private void OnLeftClicked()
        {
            if (PageIndex > 0)
            {
                var current = Pages[PageIndex];
                current.Parent = null;

                PageIndex--;
                Pages[PageIndex].Parent = Display;
            }
        }

        private void OnExitHelp()
        {
            NextState = new MenuState(Factory);
        }

        private void CreateControlsPage()
        {
            var page = new DisplayContainer();
            Pages.Add(page);

            float y = 40;
            float x = Screen.Width * 0.3f;

            var tf = new Textfield(HelpFont, "Controls")
            {
                Parent = page,
                Position = new Point(Screen.Half.Width, y),
                Anchor = new Distance(0.5f, 0),
                Scale = new Distance(1.5f, 1.5f),
                Align = TextAlign.Center
            };
            y += tf.TextSize.Height + 20;

            tf = new Textfield(HelpFont, "Movement: WASD or Arrow keys")
            {
                Parent = page,
                Position = new Point(x, y)
            };
            y += tf.TextSize.Height + 10;

            tf = new Textfield(HelpFont, "M: Turn off music")
            {
                Parent = page,
                Position = new Point(x, y)
            };
            y += tf.TextSize.Height + 10;

            tf = new Textfield(HelpFont, "Prev Ship: Q")
            {
                Parent = page,
                Position = new Point(x, y)
            };
            y += tf.TextSize.Height + 10;

            tf = new Textfield(HelpFont, "Next Ship: E")
            {
                Parent = page,
                Position = new Point(x, y)
            };
            y += tf.TextSize.Height + 10;

            tf = new Textfield(HelpFont, "Fire: Spacebar or Left Mouse Button")
            {
                Parent = page,
                Position = new Point(x, y)
            };
            y += tf.TextSize.Height + 10;
        }

        private void CreateUIHelpPage()
        {
            var page = new DisplayContainer();
            Pages.Add(page);

            float y = 40;
            float x = Screen.Width * 0.3f;

            var helpTexture = Vrax.Game.AssetCache.Get<Texture>("uihelp.png");
            if (helpTexture == null)
            {
                helpTexture = Vrax.Game.AssetCache.LoadTexture("uihelp.png");
            }

            var img = new Image(helpTexture)
            {
                Parent = page,
                Position = new Point(Screen.Half.Width, y),
                Anchor = new Distance(0.5f, 0)
            };
            y += img.Size.Y + 30;

            var tf = new Textfield(HelpFont, "Objective: Kill enemies to advance your Wave Level. Different enemies spawn on different waves. The higher the wave level, the harder and much worse your life will be.")
            {
                Parent = page,
                MaxWidth = 600,
                Position = new Point(Screen.Width * 0.2f, y)
            };
        }

        private void CreateTransformPage()
        {
            var page = new DisplayContainer();
            Pages.Add(page);

            float x = Screen.Half.Width;
            float y = 30;

            new Image(TryLoad("helptransform0.png"))
            {
                Parent = page,
                Position = new Point(x, y),
                Anchor = new Distance(0.5f, 0)
            };
            y += 200;

            new Image(TryLoad("helptransform1.png"))
            {
                Parent = page,
                Position = new Point(x, y),
                Anchor = new Distance(0.5f, 0)
            };
        }

        private void CreateAboutPage()
        {
            var page = new DisplayContainer();
            Pages.Add(page);

            var tf = new Textfield(HelpFont, "This game was made completely in 48 hours by Mark Troyer on Dec 1-3, 2017 for the Ludum Dare 40 Compo. I had fun making it and I hope you enjoy playing it!")
            {
                Parent = page,
                Position = Screen.Half,
                Anchor = Distance.Center,
                MaxWidth = (int)(Screen.Width * 0.8f)
            };
        }

        private Texture TryLoad(string path)
        {
            var texture = Vrax.Game.AssetCache.Get<Texture>(path);

            if (texture != null)
                return texture;

            return Vrax.Game.AssetCache.LoadTexture(path);
        }

        public void Dispose()
        {
            Display.Dispose();

            foreach (var page in Pages)
                page.Dispose();
        }

        public void Draw(Graphics g) { }

        public void Update(double deltaTime) { }
    }
}
