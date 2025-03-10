// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � AForge.NET, 2005-2010
// contacts@aforgenet.com
//

namespace Openize.AForge.Imaging.NetStandard.Filters.Morphology
{
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using _2_Source_filters;
    using Base_classes;

    /// <summary>
    /// Top-hat operator from Mathematical Morphology.
    /// </summary>
    /// 
    /// <remarks><para>Top-hat morphological operator <see cref="Subtract">subtracts</see>
    /// result of <see cref="Opening">morphological opening</see> on the input image
    /// from the input image itself.</para>
    /// 
    ///  <para>Applied to binary image, the filter allows to get all those object (their parts)
    ///  which were removed by <see cref="Opening">opening</see> filter, but never restored.</para>
    /// 
    /// <para>The filter accepts 8 and 16 bpp grayscale images and 24 and 48 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// TopHat filter = new TopHat( );
    /// // apply the filter
    /// filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample12.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/tophat.png" width="320" height="240" />
    /// </remarks>
    /// 
    /// <see cref="BottomHat"/>
    /// 
    public class TopHat : BaseInPlaceFilter
    {
        private Opening opening = new Opening( );
        private Subtract subtract = new Subtract( );

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return this.formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopHat"/> class.
        /// </summary>
        /// 
        public TopHat( )
        {
            // initialize format translation dictionary
            this.formatTranslations[PixelFormat.Format8bppIndexed]    = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb]       = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format48bppRgb]       = PixelFormat.Format48bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopHat"/> class.
        /// </summary>
        /// 
        /// <param name="se">Structuring element to pass to <see cref="Opening"/> operator.</param>
        /// 
        public TopHat( short[,] se ) : this( )
        {
            this.opening = new Opening( se );
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image )
        {
            // perform opening on the source image
            UnmanagedImage openedImage = this.opening.Apply( image );
            // subtract opened image from source image
            this.subtract.UnmanagedOverlayImage = openedImage;
            this.subtract.ApplyInPlace( image );

            openedImage.Dispose( );
        }
    }
}
