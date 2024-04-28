using System;
using Common.Helper;
using UnityEngine;

namespace Common.Structure
{


    [Serializable]
    public struct Size
    {
        public int width;
        public int height;


        public static Size zero { get; } = new Size(0, 0);

        public Size(int w, int h)
        {
            width = w;
            height = h;
        }

        public Size(Vector2 v)
        {
            width = (int)v.x;
            height = (int)v.y;
        }

        public Size(Vector2Int v)
        {
            width = v.x;
            height = v.y;
        }

        public SizeF ToSizeF()
        {
            return new SizeF(width, height);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(width, height);
        }

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int(width, height);
        }

        public override string ToString()
        {
            return $"Size({width}, {height})";
        }

        public bool Equals(Size other)
        {
            return width == other.width && height == other.height;
        }

        public override bool Equals(object obj)
        {
            return obj is Size other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(width, height);
        }

        public static bool operator == (Size a, Size b)
        {
            return a.width == b.width && a.height == b.height;
        }

        public static bool operator !=(Size a, Size b)
        {
            return !(a == b);
        }
    }

    [Serializable]
    public struct SizeF
    {
        public float width;
        public float height;

        public static SizeF zero { get; } = new SizeF(0f, 0f);

        public SizeF(float w, float h)
        {
            width = w;
            height = h;
        }

        public SizeF(Vector2 v)
        {
            width = v.x;
            height = v.y;
        }

        public SizeF(Vector2Int v)
        {
            width = v.x;
            height = v.y;
        }

        public Size ToSize()
        {
            return new Size((int)width, (int)height);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(width, height);
        }

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int((int)width, (int)height);
        }

        public override string ToString()
        {
            return $"SizeF({width}, {height})";
        }

        public bool Equals(SizeF other)
        {
            return width.Equals(other.width) && height.Equals(other.height);
        }

        public override bool Equals(object obj)
        {
            return obj is SizeF other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(width, height);
        }

        public static bool operator == (SizeF a, SizeF b)
        {
            return  Maths.NearEpsilon(a.width, b.width) && Maths.NearEpsilon(a.height, b.height);
        }

        public static bool operator !=(SizeF a, SizeF b)
        {
            return !(a == b);
        }
    }

}