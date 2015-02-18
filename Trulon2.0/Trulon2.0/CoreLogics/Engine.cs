﻿using System;
using System.Threading;
using Trulon.Models.Items;

namespace Trulon.CoreLogics
{
    #region Using Statements

    using System.Collections;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Config;
    using Enums;
    using Models.Entities;
    using Models.Entities.NPCs;
    using Models.Entities.NPCs.Allies;
    using Models.Entities.NPCs.Enemies;
    using Models.Entities.Players;
    using Models.Items.Equipments;
    #endregion

    #region Engine Summary
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    #endregion
    public class Engine : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D backgroundTexture;
        //Loading Entites
        private Player player;
        private Vendor vendor;
        private IList<Enemy> enemies;

        private IList<Potion> timeoutItems;

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Resources/Images";
        }

        #region Initialize Summary
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        #endregion
        protected override void Initialize()
        {
            //Sets screen size
            this.graphics.PreferredBackBufferWidth = Config.ScreenWidth;
            this.graphics.PreferredBackBufferHeight = Config.ScreenHeight;
            graphics.IsFullScreen = true;
            this.graphics.ApplyChanges();

            // TODO: Add your initialization logic here
            IsMouseVisible = true;

            //setting entites on the scene
            this.player = new Barbarian(0, 0);
            this.vendor = new Vendor(500, 500);
            this.enemies = new List<Enemy>()
            {
                new Boss(100, 200),
                new Demon(150, 0),
                new Goblin(300, 200),
                new Orc(400, 200),
                new Troll(500, 200)
            };

            this.timeoutItems = new List<Potion>();

            base.Initialize();
        }

        #region LoadContent Summary
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        #endregion
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here.
            //Load map image
            this.backgroundTexture = this.Content.Load<Texture2D>("MapImages/TrulonHomeMap");

            //Load the player resources
            this.player.Initialize(Content.Load<Texture2D>(Assets.BarbarianImages[0]), this.player.Position);

            //Load the vendor resources
            this.vendor.Initialize(Content.Load<Texture2D>(Assets.Vendor[0]), this.vendor.Position);

            foreach (var enemy in enemies)
            {
                enemy.Initialize(enemy is Goblin ? Content.Load<Texture2D>(Assets.GoblinImages[0]) : 
                Content.Load<Texture2D>(Assets.OrcImages[0]), enemy.Position);
            }

        }

        #region UnloadContent Summary
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        #endregion
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #region GameUpdate
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        #endregion
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //Save previous state of the keyboard to determine single key presses
            previousKeyboardState = currentKeyboardState;
            //Read the current state of the keyboard and store it
            currentKeyboardState = Keyboard.GetState();

            //Update player
            this.player.Update();
            //update enemies
            foreach (var enemy in enemies)
            {
                enemy.Update();
            }

            var enemiesInRange = this.player.GetEnemiesInRange(enemies);
            if (enemiesInRange.Count > 0)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Space))
                {
                     this.player.Attack(enemiesInRange);
                }
            }

            for (var i = 0; i < this.enemies.Count; i++)
            {
                if (!this.enemies[i].IsAlive)
                {
                    this.player.AddCoins(this.enemies[i]);
                    this.player.AddExperience(this.enemies[i]);
                    var equipmentDrop = ItemGenerator.GetEquipmentItem();
                    this.player.Inventory.Add(equipmentDrop);
                    var potionDrop = ItemGenerator.GetPotionItem();
                    this.player.Inventory.Add(potionDrop);
                      this.enemies.RemoveAt(i);
                    break;
                }
            }

            if (this.enemies.Count == 0)
            {
                //TODO
            }

            //Testing inventory
            //Equipment
            if (currentKeyboardState.IsKeyDown(Keys.E))
            {
                foreach (var item in this.player.Inventory)
                {
                    var equipment = item as Equipment;
                    if (equipment != null)
                    {
                        this.player.UseEquipment(equipment);
                        break;
                    }
                }
            }
            //Potions
            if (currentKeyboardState.IsKeyDown(Keys.R))
            {
                foreach (var item in this.player.Inventory)
                {
                    var potion = item as Potion;
                    if (potion != null)
                    {
                        this.player.DrinkPotion(potion);
                        this.timeoutItems.Add(potion);
                        break;
                    }
                }
            }

            //Check for timeout items
            CheckForTimedoutItems();

            base.Update(gameTime);
        }

        private void CheckForTimedoutItems()
        {
            for (int i = 0; i < timeoutItems.Count; i++)
            {
                if (timeoutItems[i].Countdown == 0)
                {
                    var item = timeoutItems[i];
                    item.HasTimedOut = true;

                    this.player.RemovePotionBuff(item);
                    this.player.Inventory.Remove(item);
                    this.timeoutItems.Remove(item);
                    break;
                }
                timeoutItems[i].Countdown--;
            }
        }

        #region GameDraw Summary
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        #endregion
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            this.spriteBatch.Begin();
            this.spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, backgroundTexture.Width, backgroundTexture.Height), Color.White);

            this.player.Draw(spriteBatch);
            this.vendor.Draw(spriteBatch);

            foreach (var enemy in enemies)
            {
                enemy.Draw(this.spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
