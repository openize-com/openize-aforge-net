// AForge Image Processing Library
// AForge.NET framework
//
// Copyright � AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace Openize.AForge.Imaging.NetStandard.Filters.HSL_Filters
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Base_classes;
    using Color_Filters;
    using Core.NetStandard;
    using YCbCr_Filters;

    /// <summary>
    /// Luminance and saturation linear correction.
    /// </summary>
    /// 
    /// <remarks><para>The filter operates in <b>HSL</b> color space and provides
    /// with the facility of luminance and saturation linear correction - mapping specified channels'
    /// input ranges to specified output ranges.</para>
    /// 
    /// <para>The filter accepts 24 and 32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// HSLLinear filter = new HSLLinear( );
    /// // configure the filter
    /// filter.InLuminance   = new Range( 0, 0.85f );
    /// filter.OutSaturation = new Range( 0.25f, 1 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/hsl_linear.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="LevelsLinear"/>
    /// <seealso cref="YCbCrLinear"/>
    /// 
    public class HSLLinear : BaseInPlacePartialFilter
    {
        private Range inLuminance   = new Range( 0.0f, 1.0f );
        private Range inSaturation  = new Range( 0.0f, 1.0f );
        private Range outLuminance  = new Range( 0.0f, 1.0f );
        private Range outSaturation = new Range( 0.0f, 1.0f );

        #region Public Propertis

        /// <summary>
        /// Luminance input range.
        /// </summary>
        /// 
        /// <remarks>Luminance component is measured in the range of [0, 1].</remarks>
        /// 
        public Range InLuminance
        {
            get { return this.inLuminance; }
            set { this.inLuminance = value; }
        }

        /// <summary>
        /// Luminance output range.
        /// </summary>
        /// 
        /// <remarks>Luminance component is measured in the range of [0, 1].</remarks>
        /// 
        public Range OutLuminance
        {
            get { return this.outLuminance; }
            set { this.outLuminance = value; }
        }

        /// <summary>
        /// Saturation input range.
        /// </summary>
        /// 
        /// <remarks>Saturation component is measured in the range of [0, 1].</remarks>
        /// 
        public Range InSaturation
        {
            get { return this.inSaturation; }
            set { this.inSaturation = value; }
        }

        /// <summary>
        /// Saturation output range.
        /// </summary>
        /// 
        /// <remarks>Saturation component is measured in the range of [0, 1].</remarks>
        /// 
        public Range OutSaturation
        {
            get { return this.outSaturation; }
            set { this.outSaturation = value; }
        }

        #endregion

        // format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return this.formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HSLLinear"/> class.
        /// </summary>
        /// 
        public HSLLinear( )
        {
            this.formatTranslations[PixelFormat.Format24bppRgb]  = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb]  = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
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
            int pixelSize = Image.GetPixelFormatSize( image.PixelFormat ) / 8;

            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width * pixelSize;

            RGB rgb = new RGB( );
            HSL hsl = new HSL( );

            float kl = 0, bl = 0;
            float ks = 0, bs = 0;

            // luminance line parameters
            if ( this.inLuminance.Max != this.inLuminance.Min )
            {
                kl = ( this.outLuminance.Max - this.outLuminance.Min ) / ( this.inLuminance.Max - this.inLuminance.Min );
                bl = this.outLuminance.Min - kl * this.inLuminance.Min;
            }
            // saturation line parameters
            if ( this.inSaturation.Max != this.inSaturation.Min )
            {
                ks = ( this.outSaturation.Max - this.outSaturation.Min ) / ( this.inSaturation.Max - this.inSaturation.Min );
                bs = this.outSaturation.Min - ks * this.inSaturation.Min;
            }

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * image.Stride + startX * pixelSize );

            // for each row
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                {
                    rgb.Red   = ptr[RGB.R];
                    rgb.Green = ptr[RGB.G];
                    rgb.Blue  = ptr[RGB.B];

                    // convert to HSL
                    HSL.FromRGB( rgb, hsl );

                    // do luminance correction
                    if ( hsl.Luminance >= this.inLuminance.Max )
                        hsl.Luminance = this.outLuminance.Max;
                    else if ( hsl.Luminance <= this.inLuminance.Min )
                        hsl.Luminance = this.outLuminance.Min;
                    else
                        hsl.Luminance = kl * hsl.Luminance + bl;

                    // do saturation correct correction
                    if ( hsl.Saturation >= this.inSaturation.Max )
                        hsl.Saturation = this.outSaturation.Max;
                    else if ( hsl.Saturation <= this.inSaturation.Min )
                        hsl.Saturation = this.outSaturation.Min;
                    else
                        hsl.Saturation = ks * hsl.Saturation + bs;

                    // convert back to RGB
                    HSL.ToRGB( hsl, rgb );

                    ptr[RGB.R] = rgb.Red;
                    ptr[RGB.G] = rgb.Green;
                    ptr[RGB.B] = rgb.Blue;
                }
                ptr += offset;
            }
        }
    }
}
