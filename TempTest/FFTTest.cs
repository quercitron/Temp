using System;
using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Temp;

namespace TempTest
{
    [TestClass]
    public class FFTTest
    {
        [TestMethod]
        public void TestFFTInt1()
        {
            // (7x^4 - 4x^2 + x+5)(-2x^3-3)
            // -15-3 x+12 x^2-10 x^3-23 x^4+8 x^5-14 x^7
            int[] a = new[] { 5, 1, -4, 0, 7 };
            int[] b = new[] { -3, 0, 0, -2 };
            int[] c = new[] { -15, -3, 12, -10, -23, 8, 0, -14, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] res = FFTMultiply.Multiply(a, b);
            CollectionAssert.AreEqual(c, res);
        }

        [TestMethod]
        public void TestFFTInt2()
        {
            int[] a = new[] { 1, 1, 1 };
            int[] b = new[] { -1, 1 };
            int[] c = new[] { -1, 0, 0, 1, 0, 0, 0, 0 };
            int[] res = FFTMultiply.Multiply(a, b);
            CollectionAssert.AreEqual(c, res);
        }
    }
}
