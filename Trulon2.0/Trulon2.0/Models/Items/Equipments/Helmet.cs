﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trulon.Enums;
using Trulon.Interfaces;

namespace Trulon.Models.Items.Equipments
{
    public class Helmet : Equipment, IEquipable
    {
        //public Helmet(
        //    string name = "Helmet",
        //    Texture2D image = null,
        //    Rectangle bounds = new Rectangle(),
        //    Vector2 position = new Vector2(),
        //    EquipmentSlots slot = EquipmentSlots.Head,
        //    int attackPointsBuff = 0,
        //    int defensePointsBuff = 0,
        //    int speedPointsBuff = 10)
        //    : base(name, image, bounds, position, slot)
        //{
        //    this.AttackPointsBuff = attackPointsBuff;
        //    this.DefensePointsBuff = defensePointsBuff;
        //    this.SpeedPointsBuff = speedPointsBuff;
        //}

        public int AttackPointsBuff { get; set; }
        public int DefensePointsBuff { get; set; }
        public int SpeedPointsBuff { get; set; }

        public override void Initialize(Texture2D texture, Vector2 position)
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}
