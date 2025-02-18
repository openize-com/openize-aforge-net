﻿namespace Openize.AForge.Tests.NetStandard.AForge.Math.Tests
{
    using NUnit.Framework;
    using Openize.AForge.Math.NetStandard;

    [TestFixture]
    public class StatisticsTest
    {
        [Test]
        public void ModeTest( )
        {
            int[] values = new int[] { 1, 2, 2, 3, 3, 3 };
            int mode = Statistics.Mode( values );
            Assert.AreEqual( 3, mode );

            values = new int[] { 1, 1, 1, 2, 2, 2 };
            mode = Statistics.Mode( values );
            Assert.AreEqual( 3, mode );

            values = new int[] { 2, 2, 2, 1, 1, 1 };
            mode = Statistics.Mode( values );
            Assert.AreEqual( 0, mode );

            values = new int[] { 0, 0, 0, 0, 0, 0 };
            mode = Statistics.Mode( values );
            Assert.AreEqual( 0, mode );

            values = new int[] { 1, 1, 2, 3, 6, 8, 11, 12, 7, 3 };
            mode = Statistics.Mode( values );
            Assert.AreEqual( 7, mode );
        }
    }
}
