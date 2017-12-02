using PSEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax.Components
{
    public class PlayerControls : IComponent
    {
        public Entity Owner { get; set; }

        private MovementComponent Movement { get; set; }
        private WeaponComponent Weapon { get; set; }

        public void Start()
        {
            Movement = Owner.GetComponent<MovementComponent>();
            Weapon = Owner.GetComponent<WeaponComponent>();
        }

        public void Update(double deltaTime)
        {
            Movement.MoveLeft = Vrax.Game.Input.IsActionKeyDown(Controls.Left);
            Movement.MoveRight = Vrax.Game.Input.IsActionKeyDown(Controls.Right);
            Movement.MoveUp = Vrax.Game.Input.IsActionKeyDown(Controls.Up);
            Movement.MoveDown = Vrax.Game.Input.IsActionKeyDown(Controls.Down);

            Weapon.TryFire = Vrax.Game.Input.IsActionKeyDown(Controls.Fire);

            // Lock player to screen
            var screen = Vrax.Game.Screen;
            if (Owner.Position.X < 0)
            {
                Movement.MoveLeft = false;
                Owner.Position = new Point(0, Owner.Position.Y);
            }
            else if (Owner.Position.X + Owner.Rectangle.Right > screen.Width)
            {
                Movement.MoveRight = false;
                Owner.Position = new Point(screen.Width - Owner.Rectangle.Right, Owner.Position.Y);
            }

            if (Owner.Position.Y < 0)
            {
                Movement.MoveUp = false;
                Owner.Position = new Point(Owner.Position.X, 0);
            }
            else if (Owner.Position.Y + Owner.Rectangle.Bottom > Vrax.Game.Screen.Height)
            {
                Movement.MoveDown = false;
                Owner.Position = new Point(Owner.Position.X, screen.Height - Owner.Rectangle.Bottom);
            }
        }
    }
}
