using UnityEngine;

namespace Acciaio
{
    public static class Vector4Extensions
    {
        /// <summary>
        /// Given the current Vector (x, y, z, w), it returns (x, 0, 0, 0)
        /// </summary>
        public static Vector4 x(this Vector4 vector) => new Vector4(vector.x, 0, 0, 0);

        /// <summary>
        /// Given the current Vector (x, y, z, w), it returns (0, y, 0, 0)
        /// </summary>
        public static Vector4 y(this Vector4 vector) => new Vector4(0, vector.y, 0, 0);

        /// <summary>
        /// Given the current Vector (x, y, z, w), it returns (0, 0, z, 0)
        /// </summary>
        public static Vector4 z(this Vector4 vector) => new Vector4(0, 0, vector.z, 0);

        /// <summary>
        /// Given the current Vector (x, y, z, w), it returns (0, 0, 0, w)
        /// </summary>
        public static Vector4 w(this Vector4 vector) => new Vector4(0, 0, 0, vector.w);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (x, x)
        /// </summary>
        public static Vector4 xx(this Vector4 vector) => new Vector2(vector.x, vector.x);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (y, y)
        /// </summary>
        public static Vector2 yy(this Vector4 vector) => new Vector2(vector.y, vector.y);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (z, z)
        /// </summary>
        public static Vector2 zz(this Vector4 vector) => new Vector2(vector.z, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (w, w)
        /// </summary>
        public static Vector2 ww(this Vector4 vector) => new Vector2(vector.w, vector.w);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (x, x, x)
        /// </summary>
        public static Vector3 xxx(this Vector4 vector) => new Vector3(vector.x, vector.x, vector.x);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (y, y, y)
        /// </summary>
        public static Vector3 yyy(this Vector4 vector) => new Vector3(vector.y, vector.y, vector.y);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (z, z, z)
        /// </summary>
        public static Vector3 zzz(this Vector4 vector) => new Vector3(vector.z, vector.z, vector.z);

        /// <summary>
        /// Sizzling operator. Give the Vector (x, y, z, w), it returns (w, w, w)
        /// </summary>
        public static Vector3 www(this Vector4 vector) => new Vector3(vector.w, vector.w, vector.w);

        /// <summary>
        /// Given the Vector (x, y, z, w) and a value x', it returns (x', y, z, w)
        /// </summary>
        public static Vector4 WithX(this Vector4 vector, float x) => new Vector4(x, vector.y, vector.z, vector.w);

        /// <summary>
        /// Given the Vector (x, y, z, w) and a value y', it returns (x, y', z, w)
        /// </summary>
        public static Vector4 WithY(this Vector4 vector, float y) => new Vector4(vector.x, y, vector.z, vector.w);

        /// <summary>
        /// Given the Vector (x, y, z, w) and a value z', it returns (x, y, z', w)
        /// </summary>
        public static Vector4 WithZ(this Vector4 vector, float z) => new Vector4(vector.x, vector.y, z, vector.w);

        /// <summary>
        /// Given the Vector (x, y, z, w) and a value w', it returns (x, y, z, w')
        /// </summary>
        public static Vector4 WithW(this Vector4 vector, float w) => new Vector4(vector.x, vector.y, vector.z, w);
    }
}
