// AForge Image Processing Library
// AForge.NET framework
//
// Copyright � Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace Openize.AForge.Imaging.NetStandard.Filters.Transform
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Base_classes;
    using Core.NetStandard;

    /// <summary>
    /// Shrink an image by removing specified color from its boundaries.
    /// </summary>
    /// 
    /// <remarks><para>Removes pixels with specified color from image boundaries making
    /// the image smaller in size.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale and 24 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Shrink filter = new Shrink( Color.Black );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample2.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/shrink.jpg" width="295" height="226" />
    /// </remarks>
    /// 
    public class Shrink : BaseTransformationFilter
    {
        private Color colorToRemove = Color.FromArgb( 0, 0, 0 );
        // top-left coordinates of the object (calculated by CalculateNewImageSize())
        private int minX, minY;

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
        /// Color to remove from boundaries.
        /// </summary>
        /// 
        public Color ColorToRemove
        {
            get { return this.colorToRemove; }
            set { this.colorToRemove = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shrink"/> class.
        /// </summary>
        /// 
        public Shrink( )
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shrink"/> class.
        /// </summary>
        /// 
        /// <param name="colorToRemove">Color to remove from boundaries.</param>
        /// 
        public Shrink( Color colorToRemove ) : this( )
        {
            this.colorToRemove = colorToRemove;
        }

        /// <summary>
        /// Calculates new image size.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// 
        /// <returns>New image size - size of the destination image.</returns>
        /// 
        protected override System.Drawing.Size CalculateNewImageSize( UnmanagedImage sourceData )
        {
            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;
            int offset = sourceData.Stride -
                ( ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed ) ? width : width * 3 );

            // color to remove
            byte r = this.colorToRemove.R;
            byte g = this.colorToRemove.G;
            byte b = this.colorToRemove.B;

            this.minX = width;
            this.minY = height;
            int maxX = 0;
            int maxY = 0;

            // find rectangle which contains something except color to remove
            unsafe
            {
                byte* src = (byte*) sourceData.ImageData.ToPointer( );

                if ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed )
                {
                    // grayscale
                    for ( int y = 0; y < height; y++ )
                    {
                        for ( int x = 0; x < width; x++, src++ )
                        {
                            if ( *src != g )
                            {
                                if ( x < this.minX )
                                    this.minX = x;
                                if ( x > maxX )
                                    maxX = x;
                                if ( y < this.minY )
                                    this.minY = y;
                                if ( y > maxY )
                                    maxY = y;
                            }
                        }
                        src += offset;
                    }
                }
                else
                {
                    // RGB
                    for ( int y = 0; y < height; y++ )
                    {
                        for ( int x = 0; x < width; x++, src += 3 )
                        {
                            if (
                                ( src[RGB.R] != r ) ||
                                ( src[RGB.G] != g ) ||
                                ( src[RGB.B] != b ) )
                            {
                                if ( x < this.minX )
                                    this.minX = x;
                                if ( x > maxX )
                                    maxX = x;
                                if ( y < this.minY )
                                    this.minY = y;
                                if ( y > maxY )
                                    maxY = y;
                            }
                        }
                        src += offset;
                    }
                }
            }

            // check
            if ( ( this.minX == width ) && ( this.minY == height ) && ( maxX == 0 ) && ( maxY == 0 ) )
            {
                this.minX = this.minY = 0;
            }

            return new Size( maxX - this.minX + 1, maxY - this.minY + 1 );
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData )
        {
            // get destination image size
            int newWidth  = destinationData.Width;
            int newHeight = destinationData.Height;

            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;
            int copySize  = newWidth;

            // do the job
            byte* src = (byte*) sourceData.ImageData.ToPointer( );
            byte* dst = (byte*) destinationData.ImageData.ToPointer( );

            src += ( this.minY * srcStride );

            if ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                src += this.minX;
            }
            else
            {
                src += this.minX * 3;
                copySize *= 3;
            }

            // copy image
            for ( int y = 0; y < newHeight; y++ )
            {
                SystemTools.CopyUnmanagedMemory( dst, src, copySize );
                dst += dstStride;
                src += srcStride;
            }
        }
    }
}
