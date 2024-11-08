﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace PRAXE___projekt
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Vector2 _tankPosition;
        private float _tankSpeed = 100f;
        private Texture2D _tankBodyTexture;
        private Texture2D _explosionTexture;
        private Texture2D _tankBarrelTexture;
        private Texture2D _bulletTexture;
        private float _rotationAngle;
        private List<Bullet> _bullets = new List<Bullet>();
        private float _bulletSpeed = 200f;
        private EnemyTank _enemyTank;
        private Song _backgroundMusic;
        private Texture2D _backgroundTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _tankPosition = new Vector2(200, 200);
            _enemyTank = new EnemyTank(new Vector2(600, 200), 1, 100, _bulletSpeed, 50f);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _tankBodyTexture = new Texture2D(GraphicsDevice, 60, 40);
            Color[] bodyData = new Color[60 * 40];
            for (int i = 0; i < bodyData.Length; i++) bodyData[i] = Color.DarkGreen;
            _tankBodyTexture.SetData(bodyData);

            _tankBarrelTexture = new Texture2D(GraphicsDevice, 40, 10);
            Color[] barrelData = new Color[40 * 10];
            for (int i = 0; i < barrelData.Length; i++) barrelData[i] = Color.Gray;
            _tankBarrelTexture.SetData(barrelData);

            _bulletTexture = new Texture2D(GraphicsDevice, 8, 4);
            Color[] bulletData = new Color[8 * 4];
            for (int i = 0; i < bulletData.Length; i++) bulletData[i] = Color.Black;
            _bulletTexture.SetData(bulletData);

            _explosionTexture = Content.Load<Texture2D>("boom");

            _enemyTank.LoadContent(GraphicsDevice, _tankBodyTexture, _tankBarrelTexture, _bulletTexture, _explosionTexture);

            _backgroundTexture = Content.Load<Texture2D>("background");

            _backgroundMusic = Content.Load<Song>("Soldier of Heaven");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Play(_backgroundMusic);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.A))
                _rotationAngle -= 2f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboardState.IsKeyDown(Keys.D))
                _rotationAngle += 2f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 movementDirection = new Vector2((float)Math.Cos(_rotationAngle), (float)Math.Sin(_rotationAngle));
            if (keyboardState.IsKeyDown(Keys.W))
            {
                Vector2 newPosition = _tankPosition + movementDirection * _tankSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (newPosition.X >= 0 && newPosition.X + _tankBodyTexture.Width <= GraphicsDevice.Viewport.Width)
                    _tankPosition.X = newPosition.X;

                if (newPosition.Y >= 0 && newPosition.Y + _tankBodyTexture.Height <= GraphicsDevice.Viewport.Height)
                    _tankPosition.Y = newPosition.Y;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                Vector2 newPosition = _tankPosition - movementDirection * _tankSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (newPosition.X >= 0 && newPosition.X + _tankBodyTexture.Width <= GraphicsDevice.Viewport.Width)
                    _tankPosition.X = newPosition.X;

                if (newPosition.Y >= 0 && newPosition.Y + _tankBodyTexture.Height <= GraphicsDevice.Viewport.Height)
                    _tankPosition.Y = newPosition.Y;
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 barrelEnd = _tankPosition + new Vector2(
                    (float)Math.Cos(_rotationAngle) * (_tankBodyTexture.Width / 2 + _tankBarrelTexture.Width / 2),
                    (float)Math.Sin(_rotationAngle) * (_tankBodyTexture.Width / 2 + _tankBarrelTexture.Width / 2)
                );
                _bullets.Add(new Bullet(barrelEnd, Vector2.Normalize(movementDirection), _rotationAngle));
            }

            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Position += _bullets[i].Direction * _bulletSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_bullets[i].Position.X < 0 || _bullets[i].Position.X > GraphicsDevice.Viewport.Width ||
                    _bullets[i].Position.Y < 0 || _bullets[i].Position.Y > GraphicsDevice.Viewport.Height)
                {
                    _bullets.RemoveAt(i);
                }
            }

            _enemyTank.Update(gameTime);

            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                if (_enemyTank.Bounds.Contains(_bullets[i].Position))
                {
                    _enemyTank.TakeDamage(5);
                    _bullets.RemoveAt(i);
                }
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();

            _spriteBatch.Draw(_backgroundTexture, Vector2.Zero, Color.White);

            Vector2 tankCenter = _tankPosition + new Vector2(_tankBodyTexture.Width / 2, _tankBodyTexture.Height / 2);

            _spriteBatch.Draw(
                _tankBodyTexture,
                tankCenter,
                null,
                Color.White,
                _rotationAngle,
                new Vector2(_tankBodyTexture.Width / 2, _tankBodyTexture.Height / 2),
                1.0f,
                SpriteEffects.None,
                0f
            );

            _spriteBatch.Draw(
                _tankBarrelTexture,
                tankCenter,
                null,
                Color.White,
                _rotationAngle,
                new Vector2(0, _tankBarrelTexture.Height / 2),
                1.0f,
                SpriteEffects.None,
                0f
            );

            foreach (var bullet in _bullets)
            {
                _spriteBatch.Draw(_bulletTexture, bullet.Position, Color.White);
            }

            _enemyTank.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}