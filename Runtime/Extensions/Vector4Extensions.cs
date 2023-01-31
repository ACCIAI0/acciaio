using UnityEngine;

namespace Acciaio.Extensions
{
    public static class Vector4Extensions
    {
        /// <summary>
        /// Given the current Vector (x, y, z, w), it returns (x, 0, 0, 0)
        /// </summary>
        public static Vector4 Right(this Vector4 vector) => new(vector.x, 0, 0, 0);

        /// <summary>
        /// Given the current Vector (x, y, z, w), it returns (0, y, 0, 0)
        /// </summary>
        public static Vector4 Up(this Vector4 vector) => new(0, vector.y, 0, 0);

        /// <summary>
        /// Given the current Vector (x, y, z, w), it returns (0, 0, z, 0)
        /// </summary>
        public static Vector4 Forward(this Vector4 vector) => new(0, 0, vector.z, 0);

        /// <summary>
        /// Given the current Vector (x, y, z, w), it returns (0, 0, 0, w)
        /// </summary>
        public static Vector4 Fourth(this Vector4 vector) => new(0, 0, 0, vector.w);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (x, x)
        /// </summary>
        public static Vector4 xx(this Vector4 vector) => new(vector.x, vector.x);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (y, y)
        /// </summary>
        public static Vector2 yy(this Vector4 vector) => new(vector.y, vector.y);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (z, z)
        /// </summary>
        public static Vector2 zz(this Vector4 vector) => new(vector.z, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (w, w)
        /// </summary>
        public static Vector2 ww(this Vector4 vector) => new(vector.w, vector.w);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (x, x, x)
        /// </summary>
        public static Vector3 xxx(this Vector4 vector) => new(vector.x, vector.x, vector.x);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (y, y, y)
        /// </summary>
        public static Vector3 yyy(this Vector4 vector) => new(vector.y, vector.y, vector.y);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (z, z, z)
        /// </summary>
        public static Vector3 zzz(this Vector4 vector) => new(vector.z, vector.z, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (w, w, w)
        /// </summary>
        public static Vector3 www(this Vector4 vector) => new(vector.w, vector.w, vector.w);

        /// <summary>
        /// Given the Vector (x, y, z, w) and a value x', it returns (x', y, z, w)
        /// </summary>
        public static Vector4 WithX(this Vector4 vector, float x) => new(x, vector.y, vector.z, vector.w);

        /// <summary>
        /// Given the Vector (x, y, z, w) and a value y', it returns (x, y', z, w)
        /// </summary>
        public static Vector4 WithY(this Vector4 vector, float y) => new(vector.x, y, vector.z, vector.w);

        /// <summary>
        /// Given the Vector (x, y, z, w) and a value z', it returns (x, y, z', w)
        /// </summary>
        public static Vector4 WithZ(this Vector4 vector, float z) => new(vector.x, vector.y, z, vector.w);

        /// <summary>
        /// Given the Vector (x, y, z, w) and a value w', it returns (x, y, z, w')
        /// </summary>
        public static Vector4 WithW(this Vector4 vector, float w) => new Vector4(vector.x, vector.y, vector.z, w);
    }
}
