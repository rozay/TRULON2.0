﻿using System;
using System.Web.UI.WebControls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trulon.Config;
using Trulon.Enums;
using Trulon.Interfaces;

namespace Trulon.Models.Items.Equipments
{
    public class Vest : Equipment
    {
        private const string DefaultName = "Vest";
        private const EquipmentSlots DefaultSlot = EquipmentSlots.Body;
        private const int DefaultAttackPointsBuff = 0;
        private const int DefaultDefensePointsBuff = 10;
        private const int DefaultSpeedPointsBuff = 0;
        private const int DefaultAttackRadiusBuff = 0;

        public Vest()
        {
            this.Name = DefaultName;
            this.Slot = DefaultSlot;
            this.AttackPointsBuff = DefaultAttackPointsBuff;
            this.DefensePointsBuff = DefaultDefensePointsBuff;
            this.SpeedPointsBuff = DefaultSpeedPointsBuff;
            //this.Initialize();
        }
    }
}
