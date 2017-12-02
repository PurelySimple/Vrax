using PSEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax
{
    public class WeaponConfig
    {
        public double ShootSpeed { get; set; }
        public int Damage { get; set; }
        public Func<Entity> ProjectileCreator { get; set; }
        public Sound FireSound { get; set; }
    }
}
