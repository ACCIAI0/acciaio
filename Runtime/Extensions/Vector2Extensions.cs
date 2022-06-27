using UnityEngine;

namespace Acciaio
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Given the current Vector (x, y), it returns (x, 0)
        /// </summary>
        public static Vector2 right(this Vector2 vector) => new(vector.x, 0);

        /// <summary>
        /// Given the current Vector (x, y), it returns (0, y)
        /// </summary>
        public static Vector2 up(this Vector2 vector) => new(0, vector.y);

        /// <summary>
        /// Sizzling operator. Given the Vector (x, y), it returns (x, x)
        /// </summary>
        public static Vector2 xx(this Vector2 vector) => new(vector.x, vector.x);

        /// <summary>
        /// Sizzling operator. Given the Vector (x, y), it returns (y, y)
        /// </summary>
        public static Vector2 yy(this Vector2 vector) => new(vector.y, vector.y);

        /// <summary>
        /// Sizzling operator. Given the Vector (x, y), it returns (y, x)
        /// </summary>
        public static Vector2 yx(this Vector2 vector) => new(vector.y, vector.x);

        /// <summary>
        /// Sizzling operator. Given the Vector (x, y), it returns (x, x, x)
        /// </summary>
        public static Vector3 xxx(this Vector2 vector) => new(vector.x, vector.x, vector.x);

        /// <summary>
        /// Sizzling operator. Given the Vector (x, y), it returns (y, y, y)
        /// </summary>
        public static Vector3 yyy(this Vector2 vector) => new(vector.y, vector.y, vector.y);

        /// <summary>
        /// Sizzling operator. Given the vector (x, y), it returns (x, 0, y)
        /// </summary>
        public static Vector3 x0y(this Vector2 vector) => new(vector.x, 0, vector.y);

        /// <summary>
        /// Sizzling operator. Given the vector (x, y), it returns (x, 1, y)
        /// </summary>
        public static Vector3 y0x(this Vector2 vector) => new(vector.y, 0, vector.x);

        /// <summary>
        /// Given the Vector (x, y) and a value x', it returns (x', y)
        /// </summary>
        public static Vector2 WithX(this Vector2 vector, float x) => new(x, vector.y);

        /// <summary>
        /// Given the Vector (x, y) and a value y', it returns (x, y')
        /// </summary>
        public static Vector2 WithY(this Vector2 vector, float y) => new(vector.x, y);
    }
}