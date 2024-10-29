using Microsoft.Xna.Framework;

namespace PRAXE___projekt
{
    // Třída Bullet pro ukládání pozice a směru každého projektilu
    public class Bullet
    {
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; }
        public float Rotation { get; }
        public Bullet(Vector2 position, Vector2 direction, float rotation)
        {
            Position = position;
            Direction = direction;
            Rotation = rotation;
        }
    }
}
