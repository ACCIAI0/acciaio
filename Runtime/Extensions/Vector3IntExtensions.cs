using UnityEngine;

namespace Acciaio
{
    public static class Vector3IntExtensions
    {
        /// <summary>
        /// Given the current Vector (x, y, z), it returns (x, 0, 0)
        /// </summary>
        public static Vector3Int x(this Vector3Int vector) => new Vector3Int(vector.x, 0, 0);

        /// <summary>
        /// Given the current Vector (x, y, z), it returns (0, y, 0)
        /// </summary>
        public static Vector3Int y(this Vector3Int vector) => new Vector3Int(0, vector.y, 0);

        /// <summary>
        /// Given the current Vector (x, y, z), it returns (0, 0, z)
        /// </summary>
        public static Vector3Int z(this Vector3Int vector) => new Vector3Int(0, 0, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (x, x)
        /// </summary>
        public static Vector2Int xx(this Vector3Int vector) => new Vector2Int(vector.x, vector.x);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (y, y)
        /// </summary>
        public static Vector2Int yy(this Vector3Int vector) => new Vector2Int(vector.y, vector.y);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (z, z)
        /// </summary>
        public static Vector2Int zz(this Vector3Int vector) => new Vector2Int(vector.z, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (x, y)
        /// </summary>
        public static Vector2Int xy(this Vector3Int vector) => new Vector2Int(vector.x, vector.y);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (x, z)
        /// </summary>
        public static Vector2Int xz(this Vector3Int vector) => new Vector2Int(vector.x, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (y, z)
        /// </summary>
        public static Vector2Int yz(this Vector3Int vector) => new Vector2Int(vector.y, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (y, x)
        /// </summary>
        public static Vector2Int yx(this Vector3Int vector) => new Vector2Int(vector.y, vector.x);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (z, x)
        /// </summary>
        public static Vector2Int zx(this Vector3Int vector) => new Vector2Int(vector.z, vector.x);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (z, y)
        /// </summary>
        public static Vector2Int zy(this Vector3Int vector) => new Vector2Int(vector.z, vector.y);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (x, x, x)
        /// </summary>
        public static Vector3Int xxx(this Vector3Int vector) => new Vector3Int(vector.x, vector.x, vector.x);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (y, y, y)
        /// </summary>
        public static Vector3Int yyy(this Vector3Int vector) => new Vector3Int(vector.y, vector.y, vector.y);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (z, z, z)
        /// </summary>
        public static Vector3Int zzz(this Vector3Int vector) => new Vector3Int(vector.z, vector.z, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (z, y, x)
        /// </summary>
        public static Vector3Int zyx(this Vector3Int vector) => new Vector3Int(vector.z, vector.y, vector.x);

        /// <summary>
        /// Given the Vector (x, y, z) and a value x', it returns (x', y, z)
        /// </summary>
        public static Vector3Int WithX(this Vector3Int vector, int x) => new Vector3Int(x, vector.y, vector.z);

        /// <summary>
        /// Given the Vector (x, y, z) and a value y', it returns (x, y', z)
        /// </summary>
        public static Vector3Int WithY(this Vector3Int vector, int y) => new Vector3Int(vector.x, y, vector.z);

        /// <summary>
        /// Given the Vector (x, y, z) and a value z', it returns (x, y, z')
        /// </summary>
        public static Vector3Int WithZ(this Vector3Int vector, int z) => new Vector3Int(vector.x, vector.y, z);
    }
}
