using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private Texture2D _tankBarrelTexture;
        private Texture2D _bulletTexture;

        private float _rotationAngle; // Úhel natočení hlavně podle myši
        private List<Bullet> _bullets = new List<Bullet>();
        private float _bulletSpeed = 200f;
        private int _bulletWidth = 8;
        private int _bulletHeight = 4;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _tankPosition = new Vector2(200, 200); // Počáteční souřadnice tanku
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Textura těla tanku
            _tankBodyTexture = new Texture2D(GraphicsDevice, 60, 40);
            Color[] bodyData = new Color[60 * 40];
            for (int i = 0; i < bodyData.Length; i++) bodyData[i] = Color.DarkGreen;
            _tankBodyTexture.SetData(bodyData);

            // Textura hlavně tanku
            _tankBarrelTexture = new Texture2D(GraphicsDevice, 40, 10);
            Color[] barrelData = new Color[40 * 10];
            for (int i = 0; i < barrelData.Length; i++) barrelData[i] = Color.Gray;
            _tankBarrelTexture.SetData(barrelData);

            // Textura projektilu
            _bulletTexture = new Texture2D(GraphicsDevice, _bulletWidth, _bulletHeight);
            Color[] bulletData = new Color[_bulletWidth * _bulletHeight];
            for (int i = 0; i < bulletData.Length; i++) bulletData[i] = Color.Black;
            _bulletTexture.SetData(bulletData);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            // Pohyb W, A, S, D + kontrola hranic
            if (keyboardState.IsKeyDown(Keys.W) && _tankPosition.Y > 0)
                _tankPosition.Y -= _tankSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.S) && _tankPosition.Y < GraphicsDevice.Viewport.Height - _tankBodyTexture.Height)
                _tankPosition.Y += _tankSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.A) && _tankPosition.X > 0)
                _tankPosition.X -= _tankSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.D) && _tankPosition.X < GraphicsDevice.Viewport.Width - _tankBodyTexture.Width)
                _tankPosition.X += _tankSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Výpočet úhlu natočení hlavně podle směru k myši
            Vector2 direction = new Vector2(mouseState.X, mouseState.Y) - (_tankPosition + new Vector2(_tankBodyTexture.Width / 2, _tankBodyTexture.Height / 2));
            _rotationAngle = (float)Math.Atan2(direction.Y, direction.X);

            // Střelba, když uživatel klikne levým tlačítkem myši
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // Výpočet konce hlavně
                Vector2 barrelEnd = _tankPosition + new Vector2(
                    (float)Math.Cos(_rotationAngle) * (_tankBodyTexture.Width / 2 + _tankBarrelTexture.Width / 2),
                    (float)Math.Sin(_rotationAngle) * (_tankBodyTexture.Width / 2 + _tankBarrelTexture.Width / 2)
                );

                Vector2 bulletDirection = Vector2.Normalize(new Vector2(mouseState.X, mouseState.Y) - barrelEnd);

                // Přidání nového projektilu do listu
                _bullets.Add(new Bullet(barrelEnd, bulletDirection, _rotationAngle));
            }

            // Aktualizace pozice všech projektilů
            for (int i = 0; i < _bullets.Count; i++)
            {
                _bullets[i].Position += _bullets[i].Direction * _bulletSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();

            // Vykreslení těla tanku
            _spriteBatch.Draw(_tankBodyTexture, _tankPosition, Color.White);

            // Vykreslení hlavně tanku s rotací
            Vector2 barrelOrigin = new Vector2(0, _tankBarrelTexture.Height / 2);
            Vector2 barrelPosition = _tankPosition + new Vector2(_tankBodyTexture.Width / 2, _tankBodyTexture.Height / 2);

            _spriteBatch.Draw(
                _tankBarrelTexture,
                barrelPosition,
                null,
                Color.White,
                _rotationAngle,
                barrelOrigin,
                1.0f,
                SpriteEffects.None,
                0f
            );

            // Vykreslení všech projektilů
            foreach (var bullet in _bullets)
            {
                _spriteBatch.Draw(_bulletTexture, bullet.Position, null, Color.White, bullet.Rotation, new Vector2(0, _bulletHeight / 2), 1.0f, SpriteEffects.None, 0f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
