using UnityEngine;

namespace Acciaio.Extensions
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Given the current Vector (x, y, z), it returns (x, 0, 0)
        /// </summary>
        public static Vector3 Right(this Vector3 vector) => new(vector.x, 0, 0);

        /// <summary>
        /// Given the current Vector (x, y, z), it returns (0, y, 0)
        /// </summary>
        public static Vector3 Up(this Vector3 vector) => new(0, vector.y, 0);

        /// <summary>
        /// Given the current Vector (x, y, z), it returns (0, 0, z)
        /// </summary>
        public static Vector3 Forward(this Vector3 vector) => new(0, 0, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (x, x)
        /// </summary>
        public static Vector2 xx(this Vector3 vector) => new(vector.x, vector.x);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (y, y)
        /// </summary>
        public static Vector2 yy(this Vector3 vector) => new(vector.y, vector.y);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (z, z)
        /// </summary>
        public static Vector2 zz(this Vector3 vector) => new(vector.z, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (x, y)
        /// </summary>
        public static Vector2 xy(this Vector3 vector) => new(vector.x, vector.y);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (x, z)
        /// </summary>
        public static Vector2 xz(this Vector3 vector) => new(vector.x, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (y, z)
        /// </summary>
        public static Vector2 yz(this Vector3 vector) => new(vector.y, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (y, x)
        /// </summary>
        public static Vector2 yx(this Vector3 vector) => new(vector.y, vector.x);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (z, x)
        /// </summary>
        public static Vector2 zx(this Vector3 vector) => new(vector.z, vector.x);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (z, y)
        /// </summary>
        public static Vector2 zy(this Vector3 vector) => new(vector.z, vector.y);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (x, x, x)
        /// </summary>
        public static Vector3 xxx(this Vector3 vector) => new(vector.x, vector.x, vector.x);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (y, y, y)
        /// </summary>
        public static Vector3 yyy(this Vector3 vector) => new(vector.y, vector.y, vector.y);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (z, z, z)
        /// </summary>
        public static Vector3 zzz(this Vector3 vector) => new(vector.z, vector.z, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z), it returns (z, y, x)
        /// </summary>
        public static Vector3 zyx(this Vector3 vector) => new(vector.z, vector.y, vector.x);

        /// <summary>
        /// Given the Vector (x, y, z) and a value x', it returns (x', y, z)
        /// </summary>
        public static Vector3 WithX(this Vector3 vector, float x) => new(x, vector.y, vector.z);

        /// <summary>
        /// Given the Vector (x, y, z) and a value y', it returns (x, y', z)
        /// </summary>
        public static Vector3 WithY(this Vector3 vector, float y) => new(vector.x, y, vector.z);

        /// <summary>
        /// Given the Vector (x, y, z) and a value z', it returns (x, y, z')
        /// </summary>
        public static Vector3 WithZ(this Vector3 vector, float z) => new(vector.x, vector.y, z);
    }
}
