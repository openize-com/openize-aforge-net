﻿// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2010
// contacts@aforgenet.com
//

namespace Openize.AForge.Imaging.NetStandard.Filters._2_Source_filters
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Base_classes;

    /// <summary>
    /// Calculate difference between two images and threshold it.
    /// </summary>
    /// 
    /// <remarks><para>The filter produces similar result as applying <see cref="Difference"/> filter and
    /// then <see cref="Threshold"/> filter - thresholded difference between two images. Result of this
    /// image processing routine may be useful in motion detection applications or finding areas of significant
    /// difference.</para>
    /// 
    /// <para>The filter accepts 8 and 24/32color images for processing.
    /// In the case of color images, the image processing routine differences sum over 3 RGB channels (Manhattan distance), i.e.
    /// |diffR| + |diffG| + |diffB|.
    /// </para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ThresholdedDifference filter = new ThresholdedDifference( 60 );
    /// // apply the filter
    /// filter.OverlayImage = backgroundImage;
    /// Bitmap resultImage = filter.Apply( sourceImage );
    /// </code>
    /// 
    /// <para><b>Source image:</b></para>
    /// <img src="img/imaging/object.jpg" width="320" height="240" />
    /// <para><b>Background image:</b></para>
    /// <img src="img/imaging/background.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/thresholded_difference.png" width="320" height="240" />
    /// </remarks>
    /// 
    /// <seealso cref="ThresholdedEuclideanDifference"/>
    /// 
    public class ThresholdedDifference : BaseFilter2
    {
        // format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        private int threshold = 15;

        /// <summary>
        /// Difference threshold.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies difference threshold. If difference between pixels of processing image
        /// and overlay image is greater than this value, then corresponding pixel of result image is set to white; otherwise
        /// black.
        /// </para>
        /// 
        /// <para>Default value is set to <b>15</b>.</para></remarks>
        /// 
        public int Threshold
        {
            get { return this.threshold; }
            set { this.threshold = value; }
        }

        private int whitePixelsCount = 0;

        /// <summary>
        /// Number of pixels which were set to white in destination image during last image processing call.
        /// </summary>
        ///
        /// <remarks><para>The property may be useful to determine amount of difference between two images which,
        /// for example, may be treated as amount of motion in motion detection applications, etc.</para></remarks>
        ///
        public int WhitePixelsCount
        {
            get { return this.whitePixelsCount; }
        }

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>See <see cref="IFilterInformation.FormatTranslations"/> for more information.</para></remarks>
        ///
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return this.formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThresholdedDifference"/> class.
        /// </summary>
        /// 
        public ThresholdedDifference( )
        {
            // initialize format translation dictionary
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format32bppPArgb]  = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThresholdedDifference"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">Difference threshold (see <see cref="Threshold"/>).</param>
        /// 
        public ThresholdedDifference( int threshold ) : this( )
        {
            this.threshold = threshold;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="overlay">Overlay image data.</param>
        /// <param name="destinationData">Destination image data</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage overlay, UnmanagedImage destinationData )
        {
            this.whitePixelsCount = 0;

            // get source image size
            int width  = sourceData.Width;
            int height = sourceData.Height;
            int pixelSize = Bitmap.GetPixelFormatSize( sourceData.PixelFormat ) / 8;

            byte* src = (byte*) sourceData.ImageData.ToPointer( );
            byte* ovr = (byte*) overlay.ImageData.ToPointer( );
            byte* dst = (byte*) destinationData.ImageData.ToPointer( );

            if ( pixelSize == 1 )
            {
                // grayscale image
                int srcOffset = sourceData.Stride - width;
                int ovrOffset = overlay.Stride - width;
                int dstOffset = destinationData.Stride - width;

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src++, ovr++, dst++ )
                    {
                        int diff = *src - *ovr;

                        if ( diff < 0 )
                            diff = -diff;

                        if ( diff > this.threshold )
                        {
                            *dst = (byte) 255;
                            this.whitePixelsCount++;
                        }
                        else
                        {
                            *dst = 0;
                        }
                    }
                    src += srcOffset;
                    ovr += ovrOffset;
                    dst += dstOffset;
                }
            }
            else
            {
                // color image
                int srcOffset = sourceData.Stride - pixelSize * width;
                int ovrOffset = overlay.Stride - pixelSize * width;
                int dstOffset = destinationData.Stride - width;

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src += pixelSize, ovr += pixelSize, dst++ )
                    {
                        int diffR = src[RGB.R] - ovr[RGB.R];
                        int diffG = src[RGB.G] - ovr[RGB.G];
                        int diffB = src[RGB.B] - ovr[RGB.B];

                        if ( diffR < 0 )
                            diffR = -diffR;
                        if ( diffG < 0 )
                            diffG = -diffG;
                        if ( diffB < 0 )
                            diffB = -diffB;

                        if ( diffR + diffG + diffB > this.threshold )
                        {
                            *dst = (byte) 255;
                            this.whitePixelsCount++;
                        }
                        else
                        {
                            *dst = 0;
                        }
                    }
                    src += srcOffset;
                    ovr += ovrOffset;
                    dst += dstOffset;
                }
            }
        }
    }
}
