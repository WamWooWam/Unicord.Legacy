using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib
{
    class TestClass
    {
        private IntPtr nativeObject;

        public static implicit operator bool(TestClass c)
        {
            return c.nativeObject != IntPtr.Zero;
        }

        public static bool operator ==(TestClass lhs, TestClass rhs)
        {
            return lhs.nativeObject == rhs.nativeObject;
        }
    }
}
