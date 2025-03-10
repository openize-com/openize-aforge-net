// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � Andrew Kirillov, 2005-2014
// aforge.net@gmail.com
//

namespace Openize.AForge.Imaging.NetStandard.Filters.Morphology
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Base_classes;
    using Specific_Optimizations;

    /// <summary>
    /// Erosion operator from Mathematical Morphology.
    /// </summary>
    /// 
    /// <remarks><para>The filter assigns minimum value of surrounding pixels to each pixel of
    /// the result image. Surrounding pixels, which should be processed, are specified by
    /// structuring element: 1 - to process the neighbor, -1 - to skip it.</para>
    /// 
    /// <para>The filter especially useful for binary image processing, where it removes pixels, which
    /// are not surrounded by specified amount of neighbors. It gives ability to remove noisy pixels
    /// (stand-alone pixels) or shrink objects.</para>
    /// 
    /// <para>For processing image with 3x3 structuring element, there are different optimizations
    /// available, like <see cref="Erosion3x3"/> and <see cref="BinaryErosion3x3"/>.</para>
    /// 
    /// <para>The filter accepts 8 and 16 bpp grayscale images and 24 and 48 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Erosion filter = new Erosion( );
    /// // apply the filter
    /// filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample12.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/erosion.png" width="320" height="240" />
    /// </remarks>
    /// 
    /// <seealso cref="Dilatation"/>
    /// <seealso cref="Closing"/>
    /// <seealso cref="Opening"/>
    /// <seealso cref="Erosion3x3"/>
    /// <seealso cref="BinaryErosion3x3"/>
    /// 
    public class Erosion : BaseUsingCopyPartialFilter
    {
        // structuring element
        private short[,] se = new short[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        private int size = 3;

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
        /// Initializes a new instance of the <see cref="Erosion"/> class.
        /// </summary>
        /// 
        /// <remarks><para>Initializes new instance of the <see cref="Erosion"/> class using
        /// default structuring element - 3x3 structuring element with all elements equal to 1.
        /// </para></remarks>
        /// 
        public Erosion( )
        {
            // initialize format translation dictionary
            this.formatTranslations[PixelFormat.Format8bppIndexed]    = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb]       = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format48bppRgb]       = PixelFormat.Format48bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Erosion"/> class.
        /// </summary>
        /// 
        /// <param name="se">Structuring element.</param>
        /// 
        /// <remarks><para>Structuring elemement for the erosion morphological operator
        /// must be square matrix with odd size in the range of [3, 99].</para></remarks>
        /// 
        /// <exception cref="ArgumentException">Invalid size of structuring element.</exception>
        /// 
        public Erosion( short[,] se )
            : this( )
        {
            int s = se.GetLength( 0 );

            // check structuring element size
            if ( ( s != se.GetLength( 1 ) ) || ( s < 3 ) || ( s > 99 ) || ( s % 2 == 0 ) )
                throw new ArgumentException( "Invalid size of structuring element." );

            this.se = se;
            this.size = s;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect )
        {
            PixelFormat pixelFormat = sourceData.PixelFormat;

            // processing start and stop X,Y positions
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;

            // structuring element's radius
            int r = this.size >> 1;

            // flag to indicate if at least one pixel for the given structuring element was found
            bool foundSomething;

            if ( ( pixelFormat == PixelFormat.Format8bppIndexed ) || ( pixelFormat == PixelFormat.Format24bppRgb ) )
            {
                int pixelSize = ( pixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

                int dstStride = destinationData.Stride;
                int srcStride = sourceData.Stride;

                // base pointers
                byte* baseSrc = (byte*) sourceData.ImageData.ToPointer( );
                byte* baseDst = (byte*) destinationData.ImageData.ToPointer( );

                // allign pointers by X
                baseSrc += ( startX * pixelSize );
                baseDst += ( startX * pixelSize );

                if ( pixelFormat == PixelFormat.Format8bppIndexed )
                {
                    // grayscale image

                    // compute each line
                    for ( int y = startY; y < stopY; y++ )
                    {
                        byte* src = baseSrc + y * srcStride;
                        byte* dst = baseDst + y * dstStride;

                        byte min, v;

                        // loop and array indexes
                        int t, ir, jr, i, j;

                        // for each pixel
                        for ( int x = startX; x < stopX; x++, src++, dst++ )
                        {
                            min = 255;
                            foundSomething = false;

                            // for each structuring element's row
                            for ( i = 0; i < this.size; i++ )
                            {
                                ir = i - r;
                                t = y + ir;

                                // skip row
                                if ( t < startY )
                                    continue;
                                // break
                                if ( t >= stopY )
                                    break;

                                // for each structuring element's column
                                for ( j = 0; j < this.size; j++ )
                                {
                                    jr = j - r;
                                    t = x + jr;

                                    // skip column
                                    if ( t < startX )
                                        continue;
                                    if ( t < stopX )
                                    {
                                        if ( this.se[i, j] == 1 )
                                        {
                                            foundSomething = true;
                                            // get new MIN value
                                            v = src[ir * srcStride + jr];
                                            if ( v < min )
                                                min = v;
                                        }
                                    }
                                }
                            }
                            // result pixel
                            *dst = ( foundSomething ) ? min : *src;
                        }
                    }
                }
                else
                {
                    // 24 bpp color image

                    // compute each line
                    for ( int y = startY; y < stopY; y++ )
                    {
                        byte* src = baseSrc + y * srcStride;
                        byte* dst = baseDst + y * dstStride;

                        byte minR, minG, minB, v;
                        byte* p;

                        // loop and array indexes
                        int t, ir, jr, i, j;

                        // for each pixel
                        for ( int x = startX; x < stopX; x++, src += 3, dst += 3 )
                        {
                            minR = minG = minB = 255;
                            foundSomething = false;

                            // for each structuring element's row
                            for ( i = 0; i < this.size; i++ )
                            {
                                ir = i - r;
                                t = y + ir;

                                // skip row
                                if ( t < startY )
                                    continue;
                                // break
                                if ( t >= stopY )
                                    break;

                                // for each structuring element's column
                                for ( j = 0; j < this.size; j++ )
                                {
                                    jr = j - r;
                                    t = x + jr;

                                    // skip column
                                    if ( t < startX )
                                        continue;
                                    if ( t < stopX )
                                    {
                                        if ( this.se[i, j] == 1 )
                                        {
                                            foundSomething = true;
                                            // get new MIN values
                                            p = &src[ir * srcStride + jr * 3];

                                            // red
                                            v = p[RGB.R];
                                            if ( v < minR )
                                                minR = v;

                                            // green
                                            v = p[RGB.G];
                                            if ( v < minG )
                                                minG = v;

                                            // blue
                                            v = p[RGB.B];
                                            if ( v < minB )
                                                minB = v;
                                        }
                                    }
                                }
                            }
                            // result pixel
                            if ( foundSomething )
                            {
                                dst[RGB.R] = minR;
                                dst[RGB.G] = minG;
                                dst[RGB.B] = minB;
                            }
                            else
                            {
                                dst[RGB.R] = src[RGB.R];
                                dst[RGB.G] = src[RGB.G];
                                dst[RGB.B] = src[RGB.B];
                            }
                        }
                    }
                }
            }
            else
            {
                int pixelSize = ( pixelFormat == PixelFormat.Format16bppGrayScale ) ? 1 : 3;

                int dstStride = destinationData.Stride / 2;
                int srcStride = sourceData.Stride / 2;

                // base pointers
                ushort* baseSrc = (ushort*) sourceData.ImageData.ToPointer( );
                ushort* baseDst = (ushort*) destinationData.ImageData.ToPointer( );

                // allign pointers by X
                baseSrc += ( startX * pixelSize );
                baseDst += ( startX * pixelSize );

                if ( pixelFormat == PixelFormat.Format16bppGrayScale )
                {
                    // 16 bpp grayscale image

                    // compute each line
                    for ( int y = startY; y < stopY; y++ )
                    {
                        ushort* src = baseSrc + y * srcStride;
                        ushort* dst = baseDst + y * dstStride;

                        ushort min, v;

                        // loop and array indexes
                        int t, ir, jr, i, j;

                        // for each pixel
                        for ( int x = startX; x < stopX; x++, src++, dst++ )
                        {
                            min = 65535;
                            foundSomething = false;

                            // for each structuring element's row
                            for ( i = 0; i < this.size; i++ )
                            {
                                ir = i - r;
                                t = y + ir;

                                // skip row
                                if ( t < startY )
                                    continue;
                                // break
                                if ( t >= stopY )
                                    break;

                                // for each structuring element's column
                                for ( j = 0; j < this.size; j++ )
                                {
                                    jr = j - r;
                                    t = x + jr;

                                    // skip column
                                    if ( t < startX )
                                        continue;
                                    if ( t < stopX )
                                    {
                                        if ( this.se[i, j] == 1 )
                                        {
                                            foundSomething = true;
                                            // get new MIN value
                                            v = src[ir * srcStride + jr];
                                            if ( v < min )
                                                min = v;
                                        }
                                    }
                                }
                            }
                            // result pixel
                            *dst = ( foundSomething ) ? min : *src;

                        }
                    }
                }
                else
                {
                    // 48 bpp color image

                    // compute each line
                    for ( int y = startY; y < stopY; y++ )
                    {
                        ushort* src = baseSrc + y * srcStride;
                        ushort* dst = baseDst + y * dstStride;

                        ushort  minR, minG, minB, v;
                        ushort* p;

                        // loop and array indexes
                        int t, ir, jr, i, j;

                        // for each pixel
                        for ( int x = startX; x < stopX; x++, src += 3, dst += 3 )
                        {
                            minR = minG = minB = 65535;
                            foundSomething = false;

                            // for each structuring element's row
                            for ( i = 0; i < this.size; i++ )
                            {
                                ir = i - r;
                                t = y + ir;

                                // skip row
                                if ( t < startY )
                                    continue;
                                // break
                                if ( t >= stopY )
                                    break;

                                // for each structuring element's column
                                for ( j = 0; j < this.size; j++ )
                                {
                                    jr = j - r;
                                    t = x + jr;

                                    // skip column
                                    if ( t < startX )
                                        continue;
                                    if ( t < stopX )
                                    {
                                        if ( this.se[i, j] == 1 )
                                        {
                                            foundSomething = true;
                                            // get new MIN values
                                            p = &src[ir * srcStride + jr * 3];

                                            // red
                                            v = p[RGB.R];
                                            if ( v < minR )
                                                minR = v;

                                            // green
                                            v = p[RGB.G];
                                            if ( v < minG )
                                                minG = v;

                                            // blue
                                            v = p[RGB.B];
                                            if ( v < minB )
                                                minB = v;
                                        }
                                    }
                                }
                            }
                            // result pixel
                            if ( foundSomething )
                            {
                                dst[RGB.R] = minR;
                                dst[RGB.G] = minG;
                                dst[RGB.B] = minB;
                            }
                            else
                            {
                                dst[RGB.R] = src[RGB.R];
                                dst[RGB.G] = src[RGB.G];
                                dst[RGB.B] = src[RGB.B];
                            }
                        }
                    }
                }
            }
        }
    }
}