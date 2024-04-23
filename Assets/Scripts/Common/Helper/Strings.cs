using System;

namespace Common.Helper
{
    public static class Strings
    {
        public static string Uuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}