﻿using System.Linq;

namespace Trulon.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Enums;
    using NPCs;
    using Items;
    using Items.Potions;
    using Config;

    public abstract class Player : Entity
    {
        private KeyboardState currentKeyboardState;
        private int velocityUp;
        private int velocityDown;
        private int velocityLeft;
        private int velocityRight;
        private IList<Potion> activePotions = new List<Potion>();
        private int inventoryIsFullTimeout;

        public EntityEquipment PlayerEquipment { get; set; }

        public IList<Potion> ActivePotions
        {
            get
            {
                return this.activePotions;

            }
        }

        public int Experience { get; set; }

        public int Coins { get; set; }

        public int SkillPoints { get; set; }

        public int AttackSkill { get; set; }

        public int DefenseSkill { get; set; }

        public int SpeedSkill { get; set; }

        public int HealthSkill { get; set; }

        public int AttackPoints
        {
            get
            {
                return this.BaseAttack + this.EquipmentBuffs["attack"] + this.AttackSkill + this.PotionBuffs["Attack"];
            }
        }

        public int DefensePoints
        {
            get
            {
                return this.BaseDefense + this.EquipmentBuffs["defense"] + this.DefenseSkill + this.PotionBuffs["Defense"];
            }
        }

        public int SpeedPoints
        {
            get
            {
                return this.BaseSpeed + this.EquipmentBuffs["speed"] + this.SpeedSkill + this.PotionBuffs["Speed"];
            }
        }

        public int HealthPoints
        {
            get
            {
                return this.BaseHealth + this.HealthSkill + this.PotionBuffs["Health"] + this.EquipmentBuffs["health"];
            }
        }

        public override int AttackRadius
        {
            get
            {
                return this.BaseAttackRadius + this.EquipmentBuffs["attackRange"] + this.PotionBuffs["AttackRange"];
            }
        }

        public Dictionary<string, int> EquipmentBuffs
        {
            get
            {
                var buffs = new Dictionary<string, int>();
                int attackBuff = 0;
                int defenseBuff = 0;
                int speedBuff = 0;
                int attackRange = 0;
                int healthBuff = 0;

                foreach (var item in this.PlayerEquipment.CurrentEquipment)
                {
                    //because the item can be removed form equipment and the slot will be set to null
                    if (item.Value != null)
                    {
                        attackBuff += item.Value.AttackPointsBuff;
                        defenseBuff += item.Value.DefensePointsBuff;
                        speedBuff += item.Value.SpeedPointsBuff;
                        attackRange += item.Value.AttackRadiusBuff;
                        healthBuff += item.Value.HealthPointsBuff;
                    }
                }
                buffs.Add("attack", attackBuff);
                buffs.Add("defense", defenseBuff);
                buffs.Add("speed", speedBuff);
                buffs.Add("attackRange", attackRange);
                buffs.Add("health", healthBuff);
                return buffs;
            }
        }

        public Dictionary<string, int> PotionBuffs
        {
            get
            {
                var buffs = new Dictionary<string, int>();
                int attackBuff = 0;
                int defenseBuff = 0;
                int speedBuff = 0;
                int healthBuff = 0;
                int attackRangeBuff = 0;

                foreach (var potion in this.ActivePotions)
                {
                    if (potion is DamagePotion)
                    {
                        attackBuff += potion.AttackPointsBuff;
                    }
                    else if (potion is DefensePotion)
                    {
                        defenseBuff += potion.DefensePointsBuff;
                    }
                    else if (potion is SpeedPotion)
                    {
                        speedBuff += potion.SpeedPointsBuff;
                    }
                    else if (potion is HealthPotion)
                    {
                        healthBuff += potion.HealthPointsBuff;
                    }
                    else if (potion is AttackRangePotion)
                    {
                        attackRangeBuff += potion.AttackRadiusBuff;
                    }
                }

                buffs.Add("Attack", attackBuff);
                buffs.Add("Health", healthBuff);
                buffs.Add("Defense", defenseBuff);
                buffs.Add("Speed", speedBuff);
                buffs.Add("AttackRange", attackRangeBuff);
                return buffs;
            }
        }

        public bool InventoryIsFull
        {
            get
            {
                if (this.inventoryIsFullTimeout > 0)
                {
                    this.inventoryIsFullTimeout--;
                    return true;
                }

                return false;
            }
        }

        public override int Level
        {
            get
            {
                int exp = this.Experience;
                if (exp >= 0 && exp < 300)
                {
                    return 1;
                }
                if (exp >= 300 && exp < 600)
                {
                    return 2;
                }
                if (exp >= 600 && exp < 900)
                {
                    return 3;
                }
                if (exp >= 900 && exp < 1200)
                {
                    return 4;
                }
                if (exp >= 1200 && exp < 1500)
                {
                    return 5;
                }

                return 6;
            }
        }

        public void Update(Map map, IList<Enemy> enemies)
        {
            base.Update();
            this.Move(map, enemies);
            //Keyboard input is in the move method which is called in the base update method
            //Make sure that player doesn't go out of bounds. T
            this.Position = new Vector2(
                MathHelper.Clamp(this.Position.X, 0, Config.ScreenWidth - this.Image.Width),
                MathHelper.Clamp(this.Position.Y, 0, Config.ScreenHeight - this.Image.Height));

            //check for timeouted potions
            for (int i = 0; i < activePotions.Count; i++)
            {
                if (activePotions[i].Countdown == 0)
                {
                    this.activePotions.Remove(activePotions[i]);
                    break;
                }
                activePotions[i].Countdown--;
            }
        }

        public IList<Enemy> GetEnemiesInRange(IList<Enemy> enemies)
        {
            var enemiesInRange = new List<Enemy>();

            foreach (var enemy in enemies)
            {
                if (this.AttackBounds.Intersects(enemy.Bounds))
                {
                    enemiesInRange.Add(enemy);
                }
            }
            return enemiesInRange;
        }

        public Ally GetAllyInRange(IList<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (this.Bounds.Intersects(entity.Bounds) && entity is Ally)
                {
                    return (Ally)entity;
                }
            }
            return null;
        }

        public void Attack(IList<Enemy> enemiesInRange)
        {
            foreach (var enemy in enemiesInRange)
            {
                enemy.HealthPoints -= this.AttackPoints;
            }
        }

        public void AddExperience(Enemy enemy)
        {
            this.Experience += enemy.ExperienceReward;
        }

        public void AddCoins(Enemy enemy)
        {
            this.Coins += enemy.CoinsReward;
        }

        public void Buy()
        {
            throw new NotImplementedException("Buy method is not implemented");
        }

        protected internal void DrinkPotion(int itemAtIndex)
        {
            if (this.Inventory.ElementAt(itemAtIndex) is Potion)
            {
                this.ActivePotions.Add(this.Inventory.ElementAt(itemAtIndex) as Potion);
                this.Inventory[itemAtIndex] = null;
            }
        }

        public void UseEquipment(int itemAtIndex)
        {
            if (itemAtIndex < this.Inventory.Length && this.Inventory[itemAtIndex] is Equipment)
            {
                var equipment = this.Inventory[itemAtIndex] as Equipment;
                //It is a bit hard to read. It means. If the key does not exists or if it is exists and the value is null
                if (!this.PlayerEquipment.CurrentEquipment.ContainsKey(equipment.Slot) ||
                    (this.PlayerEquipment.CurrentEquipment.ContainsKey(equipment.Slot)
                    && this.PlayerEquipment.CurrentEquipment[equipment.Slot] == null))
                {
                    this.PlayerEquipment.CurrentEquipment[equipment.Slot] = equipment;
                    this.Inventory[itemAtIndex] = null;
                }
            }
        }

        public void UnequipItem(EquipmentSlots slot)
        {
            if (!this.IsInventoryFull())
            {
                if (this.PlayerEquipment.CurrentEquipment.ContainsKey(slot) && this.PlayerEquipment.CurrentEquipment[slot] != null)
                {
                    this.AddToInventory(this.PlayerEquipment.CurrentEquipment[slot]);
                    this.PlayerEquipment.CurrentEquipment[slot] = null;
                }
            }
            else
            {
                this.inventoryIsFullTimeout = Config.InventoryIsFullMessageTimeout;
            }
        }

        public bool IsInventoryFull()
        {
            bool isFull = true;
            for (int i = 0; i < this.Inventory.Length; i++)
            {
                if (this.Inventory[i] == null)
                {
                    isFull = false;
                    break;
                }
            }
            return isFull;
        }

        public void DumpItem(int itemAtIndex)
        {
            if (itemAtIndex < this.Inventory.Length)
            {
                this.Inventory[itemAtIndex] = null;
            }
        }

        protected void AddSkillPoints()
        {
            throw new NotImplementedException("Buy method is not implemented");
        }

        protected void Move(Map map, IList<Enemy> enemies)
        {
            currentKeyboardState = Keyboard.GetState();

            velocityUp = SpeedPoints;
            velocityDown = SpeedPoints;
            velocityLeft = SpeedPoints;
            velocityRight = SpeedPoints;

            foreach (var obsticle in map.Obsticles)
            {
                if (this.Bounds.Intersects(obsticle.ObsticleBox))
                {
                    if (obsticle.RestrictedDirection == Direction.Up)
                    {
                        velocityUp = 0;
                    }
                    if (obsticle.RestrictedDirection == Direction.Down)
                    {
                        velocityDown = 0;
                    }
                    if (obsticle.RestrictedDirection == Direction.Left)
                    {
                        velocityLeft = 0;
                    }
                    if (obsticle.RestrictedDirection == Direction.Right)
                    {
                        velocityRight = 0;
                    }
                }
            }

            foreach (var enemy in enemies)
            {
                if (this.Bounds.Intersects(enemy.Bounds))
                {
                    if (this.PreviousDirection == "right")
                    {
                        velocityRight = 0;
                    }
                    if (this.PreviousDirection == "left")
                    {
                        velocityLeft = 0;
                    }
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                this.Position = new Vector2(this.Position.X - this.velocityLeft, this.Position.Y);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                this.Position = new Vector2(this.Position.X + this.velocityRight, this.Position.Y);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                this.Position = new Vector2(this.Position.X, this.Position.Y - this.velocityUp);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                this.Position = new Vector2(this.Position.X, this.Position.Y + this.velocityDown);
            }
        }

        public void AddToInventory(Item item)
        {
            bool isAdded = false;
            for (int i = 0; i < this.Inventory.Length; i++)
            {
                if (this.Inventory.ElementAt(i) == null)
                {
                    isAdded = true;
                    this.Inventory[i] = item;
                    break;
                }
            }
            if (!isAdded)
            {
                this.inventoryIsFullTimeout = Config.InventoryIsFullMessageTimeout;
            }
        }

        public void UseOrEquipFromInventory(Keys[] useItemKeys)
        {
            if (currentKeyboardState.GetPressedKeys().Length > 0 && useItemKeys.Contains(currentKeyboardState.GetPressedKeys()[0]))
            {
                int itemAtIndex = Array.IndexOf(useItemKeys, currentKeyboardState.GetPressedKeys()[0]);

                if (this.Inventory.ElementAt(itemAtIndex) is Potion)
                {
                    this.DrinkPotion(itemAtIndex);
                }
                else if (this.Inventory.ElementAt(itemAtIndex) is Equipment)
                {
                    this.UseEquipment(itemAtIndex);
                }
            }
        }

        public void DropItemFromInventory(Keys[] dropItemKeys)
        {
            if (currentKeyboardState.GetPressedKeys().Length > 0 && dropItemKeys.Contains(currentKeyboardState.GetPressedKeys()[0]))
            {
                int itemAtIndex = Array.IndexOf(dropItemKeys, currentKeyboardState.GetPressedKeys()[0]);
                this.DumpItem(itemAtIndex);
            }
        }

        public void UnequipItem(Keys[] unequipItemKeys)
        {
            if (currentKeyboardState.GetPressedKeys().Length > 0 && unequipItemKeys.Contains(currentKeyboardState.GetPressedKeys()[0]))
            {
                var itemAtIndex = Array.IndexOf(unequipItemKeys, currentKeyboardState.GetPressedKeys()[0]);
                switch (itemAtIndex)
                {
                    case 0:
                        this.UnequipItem(EquipmentSlots.Head);
                        break;
                    case 1:
                        this.UnequipItem(EquipmentSlots.LeftHand);
                        break;
                    case 2:
                        this.UnequipItem(EquipmentSlots.RightHand);
                        break;
                    case 3:
                        this.UnequipItem(EquipmentSlots.Body);
                        break;
                    case 4:
                        this.UnequipItem(EquipmentSlots.Feet);
                        break;
                }
            }
        }
    }
}
