using UnityEngine;
using Random = UnityEngine.Random;

namespace Common.Helper
{
    public static class Maths
    {
        private const string StringRange = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";


        public const float TinyNum = 0.0001f;
        

        public static float Remap(float value, float fromA, float fromB, float toA, float toB)
        {
            return toA + (value - fromA) / (fromB - fromA) * (toB - toA);
        }


        public static string RandomString(int length)
        {
            var chars = new char[length];
            for (var i=0; i<length; i++)
            {
                var index = Random.Range(0, StringRange.Length);
                chars[i] = StringRange[index];
            }
            return string.Join("", chars);
        }

        public static bool Near(float a, float b)
        {
            return Mathf.Abs(a - b) <= TinyNum;
        }
    }
}