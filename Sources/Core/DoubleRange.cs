// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace Openize.AForge.Core.NetStandard
{
    using System;

    /// <summary>
    /// Represents a double range with minimum and maximum values.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The class represents a double range with inclusive limits -
    /// both minimum and maximum values of the range are included into it.
    /// Mathematical notation of such range is <b>[min, max]</b>.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create [0.25, 1.5] range
    /// DoubleRange range1 = new DoubleRange( 0.25, 1.5 );
    /// // create [1.00, 2.25] range
    /// DoubleRange range2 = new DoubleRange( 1.00, 2.25 );
    /// // check if values is inside of the first range
    /// if ( range1.IsInside( 0.75 ) )
    /// {
    ///     // ...
    /// }
    /// // check if the second range is inside of the first range
    /// if ( range1.IsInside( range2 ) )
    /// {
    ///     // ...
    /// }
    /// // check if two ranges overlap
    /// if ( range1.IsOverlapping( range2 ) )
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </remarks>
    /// 
    [Serializable]
    public struct DoubleRange
    {
        private double min, max;

        /// <summary>
        /// Minimum value of the range.
        /// </summary>
        /// 
        /// <remarks><para>The property represents minimum value (left side limit) or the range -
        /// [<b>min</b>, max].</para></remarks>
        /// 
        public double Min
        {
            get { return this.min; }
            set { this.min = value; }
        }

        /// <summary>
        /// Maximum value of the range.
        /// </summary>
        /// 
        /// <remarks><para>The property represents maximum value (right side limit) or the range -
        /// [min, <b>max</b>].</para></remarks>
        /// 
        public double Max
        {
            get { return this.max; }
            set { this.max = value; }
        }

        /// <summary>
        /// Length of the range (deffirence between maximum and minimum values).
        /// </summary>
        public double Length
        {
            get { return this.max - this.min; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleRange"/> class.
        /// </summary>
        /// 
        /// <param name="min">Minimum value of the range.</param>
        /// <param name="max">Maximum value of the range.</param>
        /// 
        public DoubleRange( double min, double max )
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Check if the specified value is inside of the range.
        /// </summary>
        /// 
        /// <param name="x">Value to check.</param>
        /// 
        /// <returns><b>True</b> if the specified value is inside of the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsInside( double x )
        {
            return ( ( x >= this.min ) && ( x <= this.max ) );
        }

        /// <summary>
        /// Check if the specified range is inside of the range.
        /// </summary>
        /// 
        /// <param name="range">Range to check.</param>
        /// 
        /// <returns><b>True</b> if the specified range is inside of the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsInside( DoubleRange range )
        {
            return ( ( this.IsInside( range.min ) ) && ( this.IsInside( range.max ) ) );
        }

        /// <summary>
        /// Check if the specified range overlaps with the range.
        /// </summary>
        /// 
        /// <param name="range">Range to check for overlapping.</param>
        /// 
        /// <returns><b>True</b> if the specified range overlaps with the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsOverlapping( DoubleRange range )
        {
            return ( ( this.IsInside( range.min ) ) || ( this.IsInside( range.max ) ) ||
                     ( range.IsInside( this.min ) ) || ( range.IsInside( this.max ) ) );
        }

        /// <summary>
        /// Convert the signle precision range to integer range.
        /// </summary>
        /// 
        /// <param name="provideInnerRange">Specifies if inner integer range must be returned or outer range.</param>
        /// 
        /// <returns>Returns integer version of the range.</returns>
        /// 
        /// <remarks>If <paramref name="provideInnerRange"/> is set to <see langword="true"/>, then the
        /// returned integer range will always fit inside of the current single precision range.
        /// If it is set to <see langword="false"/>, then current single precision range will always
        /// fit into the returned integer range.</remarks>
        ///
        public IntRange ToIntRange( bool provideInnerRange )
        {
            int iMin, iMax;

            if ( provideInnerRange )
            {
                iMin = (int) Math.Ceiling( this.min );
                iMax = (int) Math.Floor( this.max );
            }
            else
            {
                iMin = (int) Math.Floor( this.min );
                iMax = (int) Math.Ceiling( this.max );
            }

            return new IntRange( iMin, iMax );
        }

        /// <summary>
        /// Equality operator - checks if two ranges have equal min/max values.
        /// </summary>
        /// 
        /// <param name="range1">First range to check.</param>
        /// <param name="range2">Second range to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if min/max values of specified
        /// ranges are equal.</returns>
        ///
        public static bool operator ==( DoubleRange range1, DoubleRange range2 )
        {
            return ( ( range1.min == range2.min ) && ( range1.max == range2.max ) );
        }

        /// <summary>
        /// Inequality operator - checks if two ranges have different min/max values.
        /// </summary>
        /// 
        /// <param name="range1">First range to check.</param>
        /// <param name="range2">Second range to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if min/max values of specified
        /// ranges are not equal.</returns>
        ///
        public static bool operator !=( DoubleRange range1, DoubleRange range2 )
        {
            return ( ( range1.min != range2.min ) || ( range1.max != range2.max ) );

        }

        /// <summary>
        /// Check if this instance of <see cref="Range"/> equal to the specified one.
        /// </summary>
        /// 
        /// <param name="obj">Another range to check equalty to.</param>
        /// 
        /// <returns>Return <see langword="true"/> if objects are equal.</returns>
        /// 
        public override bool Equals( object obj )
        {
            return ( obj is Range ) ? ( this == (DoubleRange) obj ) : false;
        }

        /// <summary>
        /// Get hash code for this instance.
        /// </summary>
        /// 
        /// <returns>Returns the hash code for this instance.</returns>
        /// 
        public override int GetHashCode( )
        {
            return this.min.GetHashCode( ) + this.max.GetHashCode( );
        }

        /// <summary>
        /// Get string representation of the class.
        /// </summary>
        /// 
        /// <returns>Returns string, which contains min/max values of the range in readable form.</returns>
        ///
        public override string ToString( )
        {
            return string.Format( System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", this.min, this.max );
        }
    }
}
