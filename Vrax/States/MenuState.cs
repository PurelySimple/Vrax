using System;
using System.Collections.Generic;
using System.Text;
using PSEngine;
using PSEngine.Core;

namespace LudumDare40.Vrax.States
{
    public class MenuState : IGameState
    {
        public IGameState NextState { get; private set; }
        private DisplayContainer Display { get; set; }
        private EntityFactory Factory { get; set; }

        private Size Screen => Vrax.Game.Screen;
        private Image TitleImage { get; set; }

        public MenuState(EntityFactory factory)
        {
            Factory = factory;

            Display = new DisplayContainer()
            {
                Parent = Vrax.Game.Display
            };

            var assetCache = Vrax.Game.AssetCache;
            var titleTexture = assetCache.Get<Texture>("Title.png");

            TitleImage = new Image(titleTexture)
            {
                Parent = Display,
                Anchor = new Distance(0.5f, 0),
                Position = new Point(Screen.Half.Width, 30)
            };
            AnimateTitle();

            var font = assetCache.Get<Font>("DefaultFont.fnt");
            var menuOptions = new List<(string label, Action callback)>()
            {
                ( "Play", OnPlayPressed ),
                ( "Toggle Music", OnToggleMusic ),
                ( "Help", OnHelpPressed ),
            };

            var textColor = Color.FromRGB(0xbbbbbb);

            int y = 180;
            for (int i = 0; i < menuOptions.Count; i++)
            {
                var tf = new Textfield(font, menuOptions[i].label)
                {
                    Parent = Display,
                    Position = new Point(Screen.Half.Width - 90, y + (i * 60)),
                    Color = textColor
                };

                var button = new TextButton(
                    new TextButton.State()
                    {
                        Scale = 1f,
                        Color = textColor
                    },
                    new TextButton.State()
                    {
                        Scale = 1f,
                        Color = Color.White
                    },
                    new TextButton.State()
                    {
                        Scale = 0.95f,
                        Color = textColor
                    });

                button.ClickEvent += menuOptions[i].callback;
                tf.Attach(button);
            }

            new Textfield(font, "Ludum Dare 40 \"The more you have, the worse it is.\"")
            {
                Parent = Display,
                Scale = new Distance(0.5f, 0.5f),
                Position = Screen,
                Anchor = Distance.One,
                Align = TextAlign.Right
            };
        }

        private void OnHelpPressed()
        {
            NextState = new HelpState(Factory);
        }

        private void OnToggleMusic()
        {
            var audio = Vrax.Game.Audio;
            audio.MuteMusic(!audio.IsMusicMuted);
            if (!audio.IsMusicMuted)
                audio.PlayMusic(Vrax.Game.AssetCache.Get<Music>("song.mp3"), -1, 1500);
        }

        private void OnPlayPressed()
        {
            NextState = new CombatState(Factory);
        }

        private void AnimateTitle()
        {
            var sequence = new TweenSequence(new[]
            {
                TitleImage.ColorTo(Color.FromRGB(0x6828A8), 3000),
                TitleImage.ColorTo(Color.FromRGB(0x0074A5), 3000),
                TitleImage.ColorTo(Color.FromRGB(0x00A51B), 3000),
            });
            sequence.CompleteEvent += AnimateTitle;
            Vrax.Game.Animator.Add(sequence);
        }

        public void Draw(Graphics g)
        {
        }

        public void Update(double deltaTime)
        {
        }

        public void Dispose()
        {
            Display.Dispose();
            Display = null;
            NextState = null;
        }
    }
}
