﻿namespace Trulon.Models.Items.Equipments
{
    using global::Trulon.Enums;

    public class Helmet : Equipment
    {
        private const Names DefaultName = Names.Helmet;
        private const EquipmentSlots DefaultSlot = EquipmentSlots.Head;
        private const int DefaultAttackPointsBuff = 0;
        private const int DefaultDefensePointsBuff = 20;
        private const int DefaultSpeedPointsBuff = 0;
        private const int DefaultPrice = 40;

        public Helmet()
        {
            this.Name = DefaultName;
            this.Slot = DefaultSlot;
            this.AttackPointsBuff = DefaultAttackPointsBuff;
            this.DefensePointsBuff = DefaultDefensePointsBuff;
            this.SpeedPointsBuff = DefaultSpeedPointsBuff;
            this.Price = DefaultPrice;
        }
    }
}
