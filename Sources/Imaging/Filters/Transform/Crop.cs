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
    /// Crop an image.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The filter crops an image providing a new image, which contains only the specified
    /// rectangle of the original image.</para>
    /// 
    /// <para>The filter accepts 8 and 16 bpp grayscale images and 24, 32, 48 and 64 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Crop filter = new Crop( new Rectangle( 75, 75, 320, 240 ) );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/crop.jpg" width="320" height="240" />
    /// </remarks>
    /// 
    public class Crop : BaseTransformationFilter
    {
        private Rectangle rect;

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
        /// Rectangle to crop.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return this.rect; }
            set { this.rect = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crop"/> class.
        /// </summary>
        /// 
        /// <param name="rect">Rectangle to crop.</param>
        /// 
        public Crop( Rectangle rect )
        {
            this.rect = rect;

            this.formatTranslations[PixelFormat.Format8bppIndexed]    = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb]       = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb]       = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb]      = PixelFormat.Format32bppArgb;
            this.formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format48bppRgb]       = PixelFormat.Format48bppRgb;
            this.formatTranslations[PixelFormat.Format64bppArgb]      = PixelFormat.Format64bppArgb;
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
            return new Size( this.rect.Width, this.rect.Height );
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
            // validate rectangle
            Rectangle srcRect = this.rect;
            srcRect.Intersect( new Rectangle( 0, 0, sourceData.Width, sourceData.Height ) );

            int xmin = srcRect.Left;
            int ymin = srcRect.Top;
            int ymax = srcRect.Bottom - 1;
            int copyWidth = srcRect.Width;

            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;
            int pixelSize = Image.GetPixelFormatSize( sourceData.PixelFormat ) / 8;
            int copySize  = copyWidth * pixelSize;

            // do the job
            byte* src = (byte*) sourceData.ImageData.ToPointer( ) + ymin * srcStride + xmin * pixelSize;
            byte* dst = (byte*) destinationData.ImageData.ToPointer( );

            if ( this.rect.Top < 0 )
            {
                dst -= dstStride * this.rect.Top;
            }
            if ( this.rect.Left < 0 )
            {
                dst -= pixelSize * this.rect.Left;
            }

            // for each line
            for ( int y = ymin; y <= ymax; y++ )
            {
                SystemTools.CopyUnmanagedMemory( dst, src, copySize );
                src += srcStride;
                dst += dstStride;
            }
        }
    }
}
