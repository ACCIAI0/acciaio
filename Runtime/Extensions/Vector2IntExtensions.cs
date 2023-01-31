using UnityEngine;

namespace Acciaio.Extensions
{
    public static class Vector2IntExtensions
    {
        /// <summary>
        /// Given the current Vector (x, y), it returns (x, 0)
        /// </summary>
        public static Vector2Int Right(this Vector2Int vector) => new(vector.x, 0);

        /// <summary>
        /// Given the current Vector (x, y), it returns (0, y)
        /// </summary>
        public static Vector2Int Up(this Vector2Int vector) => new(0, vector.y);

        /// <summary>
        /// Sizzling operator. Given the Vector (x, y), it returns (x, x)
        /// </summary>
        public static Vector2Int xx(this Vector2Int vector) => new(vector.x, vector.x);

        /// <summary>
        /// Sizzling operator. Given the Vector (x, y), it returns (y, y)
        /// </summary>
        public static Vector2Int yy(this Vector2Int vector) => new(vector.y, vector.y);

        /// <summary>
        /// Sizzling operator. Given the Vector (x, y), it returns (y, x)
        /// </summary>
        public static Vector2Int yx(this Vector2Int vector) => new(vector.y, vector.x);

        /// <summary>
        /// Sizzling operator. Given the Vector (x, y), it returns (x, x, x)
        /// </summary>
        public static Vector3Int xxx(this Vector2Int vector) => new(vector.x, vector.x, vector.x);

        /// <summary>
        /// Sizzling operator. Given the Vector (x, y), it returns (y, y, y)
        /// </summary>
        public static Vector3Int yyy(this Vector2Int vector) => new(vector.y, vector.y, vector.y);

        /// <summary>
        /// Sizzling operator. Given the vector (x, y), it returns (x, 0, y)
        /// </summary>
        public static Vector3Int x0y(this Vector2Int vector) => new(vector.x, 0, vector.y);

        /// <summary>
        /// Sizzling operator. Given the vector (x, y), it returns (x, 1, y)
        /// </summary>
        public static Vector3Int y0x(this Vector2Int vector) => new(vector.y, 0, vector.x);

        /// <summary>
        /// Given the Vector (x, y) and a value x', it returns (x', y)
        /// </summary>
        public static Vector2Int WithX(this Vector2Int vector, int x) => new(x, vector.y);

        /// <summary>
        /// Given the Vector (x, y) and a value y', it returns (x, y')
        /// </summary>
        public static Vector2Int WithY(this Vector2Int vector, int y) => new(vector.x, y);
    }
}
