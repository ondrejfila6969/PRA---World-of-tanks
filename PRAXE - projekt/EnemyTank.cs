using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PRAXE___projekt
{
    public class EnemyTank
    {
        public Vector2 Position;
        public int HP;
        private float _speed;
        private float _bulletSpeed;
        private Texture2D _bodyTexture;
        private Texture2D _barrelTexture;
        private Texture2D _bulletTexture;
        private List<Bullet> _bullets;
        private float _shootInterval;
        private float _timeSinceLastShot;
        private int _verticalDirection = 1;
        private GraphicsDevice _graphicsDevice;

        public bool IsDestroyed { get; private set; }

        public EnemyTank(Vector2 position, float shootInterval, int hp, float bulletSpeed, float speed)
        {
            Position = position;
            HP = hp;
            _shootInterval = shootInterval;
            _bulletSpeed = bulletSpeed;
            _speed = speed;
            _bullets = new List<Bullet>();
            IsDestroyed = false;
        }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, _bodyTexture.Width, _bodyTexture.Height);

        public void LoadContent(GraphicsDevice graphicsDevice, Texture2D bodyTexture, Texture2D barrelTexture, Texture2D bulletTexture)
        {
            _graphicsDevice = graphicsDevice;
            _bodyTexture = bodyTexture;
            _barrelTexture = barrelTexture;
            _bulletTexture = bulletTexture;
        }

        public void Update(GameTime gameTime)
        {
            if (IsDestroyed)
                return;

            Position.Y += _verticalDirection * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Position.Y <= 0)
            {
                Position.Y = 0;
                _verticalDirection = 1;
            }
            else if (Position.Y + _bodyTexture.Height >= _graphicsDevice.Viewport.Height)
            {
                Position.Y = _graphicsDevice.Viewport.Height - _bodyTexture.Height;
                _verticalDirection = -1;
            }

            _timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeSinceLastShot >= _shootInterval)
            {
                _timeSinceLastShot = 0;
                Shoot();
            }

            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Position += _bullets[i].Direction * _bulletSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_bullets[i].Position.X < 0 || _bullets[i].Position.X > _graphicsDevice.Viewport.Width ||
                    _bullets[i].Position.Y < 0 || _bullets[i].Position.Y > _graphicsDevice.Viewport.Height)
                {
                    _bullets.RemoveAt(i);
                }
            }
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;
            if (HP <= 0 && !IsDestroyed)
            {
                IsDestroyed = true;
                OnDespawn();
            }
        }

        private void OnDespawn()
        {
            _bullets.Clear();
        }

        public void Shoot()
        {
            if (IsDestroyed) return;

            Vector2 barrelEnd = Position + new Vector2(-_barrelTexture.Width / 2, _bodyTexture.Height / 2);
            _bullets.Add(new Bullet(barrelEnd, new Vector2(-1, 0), 0));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsDestroyed) return;

            Vector2 tankCenter = Position + new Vector2(_bodyTexture.Width / 2, _bodyTexture.Height / 2);

            spriteBatch.Draw(
                _bodyTexture,
                tankCenter,
                null,
                Color.Red,
                0f,
                new Vector2(_bodyTexture.Width / 2, _bodyTexture.Height / 2),
                1.0f,
                SpriteEffects.None,
                0f
            );

            spriteBatch.Draw(
                _barrelTexture,
                tankCenter,
                null,
                Color.White,
                (float)Math.PI,
                new Vector2(0, _barrelTexture.Height / 2),
                1.0f,
                SpriteEffects.None,
                0f
            );

            foreach (var bullet in _bullets)
            {
                spriteBatch.Draw(_bulletTexture, bullet.Position, Color.White);
            }
        }

    }
}