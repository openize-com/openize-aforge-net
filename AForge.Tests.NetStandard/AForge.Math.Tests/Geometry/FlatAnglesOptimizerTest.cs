﻿namespace Openize.AForge.Tests.NetStandard.AForge.Math.Tests.Geometry
{
    using NUnit.Framework;
    using Openize.AForge.Math.NetStandard.Geometry;

    [TestFixture]
    public class FlatAnglesOptimizerTest
    {
        private IShapeOptimizer optimizer = new FlatAnglesOptimizer( 160 );

        
        [TestCase( new int[] { 0, 0, 10, 0, 10, 10 }, new int[] { 0, 0, 10, 0, 10, 10 } )]
        [TestCase( new int[] { 0, 0, 20, 0, 10, 1 }, new int[] { 0, 0, 20, 0, 10, 1 } )]
        [TestCase( new int[] { 0, 0, 10, 1, 20, 0, 20, 20 }, new int[] { 0, 0, 20, 0, 20, 20 } )]
        [TestCase( new int[] { 0, 0, 5, 1, 10, 0, 10, 10 }, new int[] { 0, 0, 5, 1, 10, 0, 10, 10 } )]
        [TestCase( new int[] { 0, 0, 20, 0, 20, 20, 11, 9 }, new int[] { 0, 0, 20, 0, 20, 20 } )]
        [TestCase( new int[] { 0, 0, 20, 0, 20, 20, 9, 11 }, new int[] { 0, 0, 20, 0, 20, 20 } )]
        [TestCase( new int[] { 9, 11, 0, 0, 10, 1, 20, 0, 21, 10, 20, 20 }, new int[] { 0, 0, 20, 0, 20, 20 } )]
        [TestCase( new int[] { 11, 9, 0, 0, 10, -1, 20, 0, 19, 10, 20, 20 }, new int[] { 0, 0, 20, 0, 20, 20 } )]
        public void OptimizationTest( int[] coordinates, int[] expectedCoordinates )
        {
            ShapeOptimizerTestBase.TestOptimizer( coordinates, expectedCoordinates, this.optimizer );
        }
    }
}
