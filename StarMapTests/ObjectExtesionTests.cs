using System;
using StarMap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StarMapTests
{
    [TestClass]
    public class ObjectExtesionTests
    {
        [TestMethod]
        public void ObjectExt_Clamp()
        {
            float one = 1;

            Assert.AreEqual(one, one.Clamp(-10, 10));
            Assert.AreEqual(one, one.Clamp(0, 10));
            Assert.AreEqual(one, one.Clamp(1, 10));
            Assert.AreEqual(0, one.Clamp(-1, 0));
            Assert.AreEqual(0, one.Clamp(0, 0));
            Assert.AreEqual(one, one.Clamp(0, float.PositiveInfinity));
            Assert.AreEqual(one, one.Clamp(1, float.PositiveInfinity));
            Assert.AreEqual(2, one.Clamp(2, float.PositiveInfinity));
            Assert.IsTrue(float.IsNaN(float.NaN.Clamp(float.NegativeInfinity, float.PositiveInfinity)), "Failed NaN test.");
            Assert.IsTrue(float.IsNaN(float.NaN.Clamp(0, 1)), "Failed NaN test.");
            Assert.AreEqual(0, float.NaN.Clamp(0, 0), "Failed NaN test.");

            Assert.AreEqual(2, one.Clamp(2, 10));

            bool caughtex = false;
            try
            {
                one.Clamp(0, -10);
            }
            catch (ArgumentOutOfRangeException)
            {
                caughtex = true;
            }
            Assert.IsTrue(caughtex, "ArgumentOutOfRangeException expected but not found.");
            Assert.AreEqual(one, 1, "Testee was modified during testing.");
        }

        [TestMethod]
        public void ObjectExt_Lerp()
        {
            Assert.AreEqual(0, ObjectExtensions.Lerp(0, 1, 0));
            Assert.AreEqual(0.5f, ObjectExtensions.Lerp(0, 1, 0.5f));
            Assert.AreEqual(1, ObjectExtensions.Lerp(0, 1, 1));

            Assert.AreEqual(0, ObjectExtensions.Lerp(0, -1, 0));
            Assert.AreEqual(-0.5f, ObjectExtensions.Lerp(0, -1, 0.5f));
            Assert.AreEqual(-1, ObjectExtensions.Lerp(0, -1, 1));
        }
    }
}
