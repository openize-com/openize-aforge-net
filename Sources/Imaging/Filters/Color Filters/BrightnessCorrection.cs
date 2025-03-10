// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � AForge.NET, 2007-2012
// contacts@aforgenet.com
//

namespace Openize.AForge.Imaging.NetStandard.Filters.Color_Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Base_classes;
    using Core.NetStandard;

    /// <summary>
    /// Brightness adjusting in RGB color space.
    /// </summary>
    /// 
    /// <remarks><para>The filter operates in <b>RGB</b> color space and adjusts
    /// pixels' brightness by increasing every pixel's RGB values by the specified
    /// <see cref="AdjustValue">adjust value</see>. The filter is based on <see cref="LevelsLinear"/>
    /// filter and simply sets all input ranges to (0, 255-<see cref="AdjustValue"/>) and
    /// all output range to (<see cref="AdjustValue"/>, 255) in the case if the adjust value is positive.
    /// If the adjust value is negative, then all input ranges are set to
    /// (-<see cref="AdjustValue"/>, 255 ) and all output ranges are set to
    /// ( 0, 255+<see cref="AdjustValue"/>).</para>
    /// 
    /// <para>See <see cref="LevelsLinear"/> documentation for more information about the base filter.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale and 24/32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// BrightnessCorrection filter = new BrightnessCorrection( -50 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/brightness_correction.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="LevelsLinear"/>
    /// 
    public class BrightnessCorrection : BaseInPlacePartialFilter
    {
        private LevelsLinear baseFilter = new LevelsLinear( );
        private int adjustValue;

        /// <summary>
        /// Brightness adjust value, [-255, 255].
        /// </summary>
        /// 
        /// <remarks>Default value is set to <b>10</b>, which corresponds to increasing
        /// RGB values of each pixel by 10.</remarks>
        ///
        public int AdjustValue
        {
            get { return this.adjustValue; }
            set
            {
                this.adjustValue = Math.Max( -255, Math.Min( 255, value ) );

                if ( this.adjustValue > 0 )
                {
                    this.baseFilter.InRed = this.baseFilter.InGreen = this.baseFilter.InBlue = this.baseFilter.InGray =
                        new IntRange( 0, 255 - this.adjustValue );
                    this.baseFilter.OutRed = this.baseFilter.OutGreen = this.baseFilter.OutBlue = this.baseFilter.OutGray =
                        new IntRange( this.adjustValue, 255 );
                }
                else
                {
                    this.baseFilter.InRed = this.baseFilter.InGreen = this.baseFilter.InBlue = this.baseFilter.InGray =
                        new IntRange( -this.adjustValue, 255 );
                    this.baseFilter.OutRed = this.baseFilter.OutGreen = this.baseFilter.OutBlue = this.baseFilter.OutGray =
                        new IntRange( 0, 255 + this.adjustValue );
                }
            }
        }

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>See <see cref="IFilterInformation.FormatTranslations"/>
        /// documentation for additional information.</para></remarks>
        ///
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return this.baseFilter.FormatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrightnessCorrection"/> class.
        /// </summary>
        /// 
        public BrightnessCorrection( )
        {
            this.AdjustValue = 10;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrightnessCorrection"/> class.
        /// </summary>
        /// 
        /// <param name="adjustValue">Brightness <see cref="AdjustValue">adjust value</see>.</param>
        /// 
        public BrightnessCorrection( int adjustValue )
        {
            this.AdjustValue = adjustValue;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image, Rectangle rect )
        {
            this.baseFilter.ApplyInPlace( image, rect );
        }
    }
}
