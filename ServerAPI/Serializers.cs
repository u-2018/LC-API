using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LC_API.ServerAPI
{
#pragma warning disable
    public static partial class Networking
    {
        /// <summary>
        /// Serializes a <see cref="Vector2"/>.
        /// </summary>
        [Serializable]
        public struct Vector2S
        {
            public float x;
            public float y;

            public Vector2S(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public static implicit operator Vector2(Vector2S vector2S) => vector2S.vector2;

            public static implicit operator Vector2S(Vector2 vector2) => new Vector2S(vector2.x, vector2.y);


            [NonSerialized]
            private Vector2? v2 = null;
            public Vector2 vector2
            {
                get
                {
                    if (!v2.HasValue) v2 = new Vector2(x, y);
                    return v2.Value;
                }
            }
        }

        /// <summary>
        /// Serializes a <see cref="Vector2Int"/>.
        /// </summary>
        [Serializable]
        public struct Vector2IntS
        {
            public int x;
            public int y;

            public Vector2IntS(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static implicit operator Vector2Int(Vector2IntS vector2S) => vector2S.vector2;

            public static implicit operator Vector2IntS(Vector2Int vector2) => new Vector2IntS(vector2.x, vector2.y);


            [NonSerialized]
            private Vector2Int? v2 = null;
            public Vector2Int vector2
            {
                get
                {
                    if (!v2.HasValue) v2 = new Vector2Int(x, y);
                    return v2.Value;
                }
            }
        }

        /// <summary>
        /// Serializes a <see cref="Vector3"/>.
        /// </summary>
        [Serializable]
        public struct Vector3S
        {
            public float x;
            public float y;
            public float z;

            public Vector3S(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static implicit operator Vector3(Vector3S vector3S) => vector3S.vector3;

            public static implicit operator Vector3S(Vector3 vector3) => new Vector3S(vector3.x, vector3.y, vector3.z);


            [NonSerialized]
            private Vector3? v3 = null;
            public Vector3 vector3
            {
                get
                {
                    if (!v3.HasValue) v3 = new Vector3(x, y, z);
                    return v3.Value;
                }
            }
        }

        /// <summary>
        /// Serializes a <see cref="Vector3Int"/>.
        /// </summary>
        [Serializable]
        public struct Vector3IntS
        {
            public int x;
            public int y;
            public int z;

            public Vector3IntS(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static implicit operator Vector3Int(Vector3IntS vector3S) => vector3S.vector3;

            public static implicit operator Vector3IntS(Vector3Int vector3) => new Vector3IntS(vector3.x, vector3.y, vector3.z);


            [NonSerialized]
            private Vector3Int? v3 = null;
            public Vector3Int vector3
            {
                get
                {
                    if (!v3.HasValue) v3 = new Vector3Int(x, y, z);
                    return v3.Value;
                }
            }
        }

        /// <summary>
        /// Serializes a <see cref="Vector4"/>.
        /// </summary>
        [Serializable]
        public struct Vector4S
        {
            public float x;
            public float y;
            public float z;
            public float w;

            public Vector4S(float x, float y, float z, float w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

            public static implicit operator Vector4(Vector4S vector4S) => vector4S.Vector4;

            public static implicit operator Vector4S(Vector4 vector4) => new Vector4S(vector4.x, vector4.y, vector4.z, vector4.w);


            [NonSerialized]
            private Vector4? v4 = null;
            public Vector4 Vector4
            {
                get
                {
                    if (!v4.HasValue) v4 = new Vector4(x, y, z, w);
                    return v4.Value;
                }
            }
        }

        /// <summary>
        /// Serializes a <see cref="Quaternion"/>.
        /// </summary>
        [Serializable]
        public struct QuaternionS
        {
            public float x;
            public float y;
            public float z;
            public float w;

            public QuaternionS(float x, float y, float z, float w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

            public static implicit operator Quaternion(QuaternionS quaternionS) => quaternionS.Quaternion;

            public static implicit operator QuaternionS(Quaternion quaternion) => new QuaternionS(quaternion.x, quaternion.y, quaternion.z, quaternion.w);


            [NonSerialized]
            private Quaternion? q = null;
            public Quaternion Quaternion
            {
                get
                {
                    if (!q.HasValue) q = new Quaternion(x, y, z, w);
                    return q.Value;
                }
            }
        }

        /// <summary>
        /// Serializes a <see cref="Color"/>.
        /// </summary>
        [Serializable]
        public struct ColorS
        {
            public float r;
            public float g;
            public float b;
            public float a;

            public ColorS(float r, float g, float b, float a)
            {
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }

            public static implicit operator Color(ColorS colorS) => colorS.Color;

            public static implicit operator ColorS(Color color) => new ColorS(color.r, color.g, color.b, color.a);


            [NonSerialized]
            private Color? c = null;
            public Color Color
            {
                get
                {
                    if (!c.HasValue) c = new Color(r, g, b, a);
                    return c.Value;
                }
            }
        }

        /// <summary>
        /// Serializes a <see cref="Color32"/>.
        /// </summary>
        [Serializable]
        public struct Color32S
        {
            public byte r;
            public byte g;
            public byte b;
            public byte a;

            public Color32S(byte r, byte g, byte b, byte a)
            {
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }

            public static implicit operator Color32(Color32S colorS) => colorS.Color;

            public static implicit operator Color32S(Color32 color) => new Color32S(color.r, color.g, color.b, color.a);


            [NonSerialized]
            private Color32? c = null;
            public Color32 Color
            {
                get
                {
                    if (!c.HasValue) c = new Color32(r, g, b, a);
                    return c.Value;
                }
            }
        }

        /// <summary>
        /// Serializes a <see cref="Ray"/>.
        /// </summary>
        [Serializable]
        public struct RayS
        {
            public Vector3S origin;
            public Vector3S direction;

            public RayS(Vector3 origin, Vector3 direction)
            {
                this.origin = origin;
                this.direction = direction;
            }

            public static implicit operator Ray(RayS rayS) => rayS.Ray;

            public static implicit operator RayS(Ray ray) => new RayS(ray.origin, ray.direction);


            [NonSerialized]
            private Ray? r = null;
            public Ray Ray
            {
                get
                {
                    if (!r.HasValue) r = new Ray(origin, direction);
                    return r.Value;
                }
            }
        }

        /// <summary>
        /// Serializes a <see cref="Ray2D"/>.
        /// </summary>
        [Serializable]
        public struct Ray2DS
        {
            public Vector2S origin;
            public Vector2S direction;

            public Ray2DS(Vector2 origin, Vector2 direction)
            {
                this.origin = origin;
                this.direction = direction;
            }

            public static implicit operator Ray2D(Ray2DS ray2DS) => ray2DS.Ray;

            public static implicit operator Ray2DS(Ray2D ray2D) => new Ray2DS(ray2D.origin, ray2D.direction);


            [NonSerialized]
            private Ray2D? r = null;
            public Ray2D Ray
            {
                get
                {
                    if (!r.HasValue) r = new Ray2D(origin, direction);
                    return r.Value;
                }
            }
        }
    }
#pragma warning restore
}
