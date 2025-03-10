// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace Openize.AForge.Math.NetStandard
{
    using System;
    using Core.NetStandard;

    /// <summary>
    /// Histogram for continuous random values.
    /// </summary>
    /// 
    /// <remarks><para>The class wraps histogram for continuous stochastic function, which is represented
    /// by integer array and range of the function. Values of the integer array are treated
    /// as total amount of hits on the corresponding subranges, which are calculated by splitting the
    /// specified range into required amount of consequent ranges.</para>
    /// 
    /// <para>For example, if the integer array is equal to { 1, 2, 4, 8, 16 } and the range is set
    /// to [0, 1], then the histogram consists of next subranges:
    /// <list type="bullet">
    /// <item>[0.0, 0.2] - 1 hit;</item>
    /// <item>[0.2, 0.4] - 2 hits;</item>
    /// <item>[0.4, 0.6] - 4 hits;</item>
    /// <item>[0.6, 0.8] - 8 hits;</item>
    /// <item>[0.8, 1.0] - 16 hits.</item>
    /// </list>
    /// </para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create histogram
    /// ContinuousHistogram histogram = new ContinuousHistogram(
    ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new Range( 0.0f, 1.0f ) );
    /// // get mean and standard deviation values
    /// Console.WriteLine( "mean = " + histogram.Mean + ", std.dev = " + histogram.StdDev );
    /// </code>
    /// </remarks>
    /// 
    [Serializable]
    public class ContinuousHistogram
    {
        private int[] values;
        private Range range;

        private float mean;
        private float stdDev;
        private float median;
        private float min;
        private float max;
        private int   total;

        /// <summary>
        /// Values of the histogram.
        /// </summary>
        /// 
        public int[] Values
        {
            get { return this.values; }
        }

        /// <summary>
        /// Range of random values.
        /// </summary>
        /// 
        public Range Range
        {
            get { return this.range; }
        }

        /// <summary>
        /// Mean value.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve mean value of the histogram.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// ContinuousHistogram histogram = new ContinuousHistogram(
        ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new Range( 0.0f, 1.0f ) );
        /// // get mean value (= 0.505 )
        /// Console.WriteLine( "mean = " + histogram.Mean.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public float Mean
        {
            get { return this.mean; }
        }

        /// <summary>
        /// Standard deviation.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve standard deviation value of the histogram.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// ContinuousHistogram histogram = new ContinuousHistogram(
        ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new Range( 0.0f, 1.0f ) );
        /// // get std.dev. value (= 0.215)
        /// Console.WriteLine( "std.dev. = " + histogram.StdDev.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public float StdDev
        {
            get { return this.stdDev; }
        }

        /// <summary>
        /// Median value.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve median value of the histogram.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// ContinuousHistogram histogram = new ContinuousHistogram(
        ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new Range( 0.0f, 1.0f ) );
        /// // get median value (= 0.500)
        /// Console.WriteLine( "median = " + histogram.Median.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public float Median
        {
            get { return this.median; }
        }

        /// <summary>
        /// Minimum value.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve minimum value of the histogram with non zero
        /// hits count.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// ContinuousHistogram histogram = new ContinuousHistogram(
        ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new Range( 0.0f, 1.0f ) );
        /// // get min value (= 0.250)
        /// Console.WriteLine( "min = " + histogram.Min.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        public float Min
        {
            get { return this.min; }
        }

        /// <summary>
        /// Maximum value.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve maximum value of the histogram with non zero
        /// hits count.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// ContinuousHistogram histogram = new ContinuousHistogram(
        ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new Range( 0.0f, 1.0f ) );
        /// // get max value (= 0.875)
        /// Console.WriteLine( "max = " + histogram.Max.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public float Max
        {
            get { return this.max; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousHistogram"/> class.
        /// </summary>
        /// 
        /// <param name="values">Values of the histogram.</param>
        /// <param name="range">Range of random values.</param>
        /// 
        /// <remarks>Values of the integer array are treated as total amount of hits on the
        /// corresponding subranges, which are calculated by splitting the specified range into
        /// required amount of consequent ranges (see <see cref="ContinuousHistogram"/> class
        /// description for more information).
        /// </remarks>
        /// 
        public ContinuousHistogram( int[] values, Range range )
        {
            this.values = values;
            this.range  = range;

            this.Update( );
        }

        /// <summary>
        /// Get range around median containing specified percentage of values.
        /// </summary>
        /// 
        /// <param name="percent">Values percentage around median.</param>
        /// 
        /// <returns>Returns the range which containes specifies percentage of values.</returns>
        /// 
        /// <remarks><para>The method calculates range of stochastic variable, which summary probability
        /// comprises the specified percentage of histogram's hits.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// ContinuousHistogram histogram = new ContinuousHistogram(
        ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new Range( 0.0f, 1.0f ) );
        /// // get 50% range
        /// Range range = histogram.GetRange( 0.5f );
        /// // show the range ([0.25, 0.75])
        /// Console.WriteLine( "50% range = [" + range.Min + ", " + range.Max + "]" );
        /// </code>
        /// </remarks>
        /// 
        public Range GetRange( float percent )
        {
            int min, max, hits;
            int h = (int) ( this.total * ( percent + ( 1 - percent ) / 2 ) );
            int n = this.values.Length;
            int nM1 = n - 1;

            // skip left portion
            for ( min = 0, hits = this.total; min < n; min++ )
            {
                hits -= this.values[min];
                if ( hits < h )
                    break;
            }
            // skip right portion
            for ( max = nM1, hits = this.total; max >= 0; max-- )
            {
                hits -= this.values[max];
                if ( hits < h )
                    break;
            }
            // return range between left and right boundaries
            return new Range(
                ( (float) min / nM1 ) * this.range.Length + this.range.Min,
                ( (float) max / nM1 ) * this.range.Length + this.range.Min );
        }

        /// <summary>
        /// Update statistical value of the histogram.
        /// </summary>
        /// 
        /// <remarks>The method recalculates statistical values of the histogram, like mean,
        /// standard deviation, etc. The method should be called only in the case if histogram
        /// values were retrieved through <see cref="Values"/> property and updated after that.
        /// </remarks>
        /// 
        public void Update( )
        {
            int hits;
            int i, n = this.values.Length;
            int nM1 = n - 1;

            float rangeLength = this.range.Length;
            float rangeMin = this.range.Min;

            this.max    = 0;
            this.min    = n;
            this.mean   = 0;
            this.stdDev = 0;
            this.total  = 0;

            double sum = 0;

            // calculate mean, min, max
            for ( i = 0; i < n; i++ )
            {
                hits = this.values[i];

                if ( hits != 0 )
                {
                    // max
                    if ( i > this.max )
                        this.max = i;
                    // min
                    if ( i < this.min )
                        this.min = i;
                }

                // accumulate total value
                this.total += hits;
                // accumulate mean value
                sum += ( ( (double) i / nM1 ) * rangeLength + rangeMin ) * hits;
            }

            if ( this.total != 0 )
            {
                this.mean = (float) ( sum / this.total );
            }

            this.min = ( this.min / nM1 ) * rangeLength + rangeMin;
            this.max = ( this.max / nM1 ) * rangeLength + rangeMin;

            // calculate stadard deviation
            sum = 0;
            double diff;

            for ( i = 0; i < n; i++ )
            {
                hits = this.values[i];
                diff = ( ( (double) i / nM1 ) * rangeLength + rangeMin ) - this.mean;
                sum += diff * diff * hits;
            }

            if ( this.total != 0 )
            {
                this.stdDev = (float) Math.Sqrt( sum / this.total );
            }

            // calculate median
            int m, halfTotal = this.total / 2;

            for ( m = 0, hits = 0; m < n; m++ )
            {
                hits += this.values[m];
                if ( hits >= halfTotal )
                    break;
            }
            this.median = ( (float) m / nM1 ) * rangeLength + rangeMin;
        }
    }
}
