using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LC_API.Networking.Serializers
{
#pragma warning disable
    /// <summary>
    /// Serializes a <see cref="Vector2"/>.
    /// </summary>
    public struct Vector2S
    {
        public float x { get; set; }
        public float y { get; set; }

        public Vector2S(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2(Vector2S vector2S) => vector2S.vector2;

        public static implicit operator Vector2S(Vector2 vector2) => new Vector2S(vector2.x, vector2.y);


        private Vector2? v2 = null;

        [JsonIgnore]
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
    public struct Vector2IntS
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vector2IntS(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2Int(Vector2IntS vector2S) => vector2S.vector2;

        public static implicit operator Vector2IntS(Vector2Int vector2) => new Vector2IntS(vector2.x, vector2.y);


        private Vector2Int? v2 = null;

        [JsonIgnore]
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
    public struct Vector3S
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public Vector3S(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3(Vector3S vector3S) => vector3S.vector3;

        public static implicit operator Vector3S(Vector3 vector3) => new Vector3S(vector3.x, vector3.y, vector3.z);


        private Vector3? v3 = null;

        [JsonIgnore]
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
    public struct Vector3IntS
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public Vector3IntS(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3Int(Vector3IntS vector3S) => vector3S.vector3;

        public static implicit operator Vector3IntS(Vector3Int vector3) => new Vector3IntS(vector3.x, vector3.y, vector3.z);


        private Vector3Int? v3 = null;

        [JsonIgnore]
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
    public struct Vector4S
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }

        public Vector4S(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator Vector4(Vector4S vector4S) => vector4S.Vector4;

        public static implicit operator Vector4S(Vector4 vector4) => new Vector4S(vector4.x, vector4.y, vector4.z, vector4.w);


        private Vector4? v4 = null;

        [JsonIgnore]
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
    public struct QuaternionS
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }

        public QuaternionS(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator Quaternion(QuaternionS quaternionS) => quaternionS.Quaternion;

        public static implicit operator QuaternionS(Quaternion quaternion) => new QuaternionS(quaternion.x, quaternion.y, quaternion.z, quaternion.w);


        private Quaternion? q = null;

        [JsonIgnore]
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
    public struct ColorS
    {
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }
        public float a { get; set; }

        public ColorS(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static implicit operator Color(ColorS colorS) => colorS.Color;

        public static implicit operator ColorS(Color color) => new ColorS(color.r, color.g, color.b, color.a);


        private Color? c = null;

        [JsonIgnore]
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
    public struct Color32S
    {
        public byte r { get; set; }
        public byte g { get; set; }
        public byte b { get; set; }
        public byte a { get; set; }

        public Color32S(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static implicit operator Color32(Color32S colorS) => colorS.Color;

        public static implicit operator Color32S(Color32 color) => new Color32S(color.r, color.g, color.b, color.a);


        private Color32? c = null;

        [JsonIgnore]
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
    public struct RayS
    {
        public Vector3S origin { get; set; }
        public Vector3S direction { get; set; }

        public RayS(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        public static implicit operator Ray(RayS rayS) => rayS.Ray;

        public static implicit operator RayS(Ray ray) => new RayS(ray.origin, ray.direction);


        private Ray? r = null;

        [JsonIgnore]
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
    public struct Ray2DS
    {
        public Vector2S origin { get; set; }
        public Vector2S direction { get; set; }

        public Ray2DS(Vector2 origin, Vector2 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        public static implicit operator Ray2D(Ray2DS ray2DS) => ray2DS.Ray;

        public static implicit operator Ray2DS(Ray2D ray2D) => new Ray2DS(ray2D.origin, ray2D.direction);


        private Ray2D? r = null;

        [JsonIgnore]
        public Ray2D Ray
        {
            get
            {
                if (!r.HasValue) r = new Ray2D(origin, direction);
                return r.Value;
            }
        }
    }
#pragma warning restore
}
