// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � AForge.NET, 2005-2012
// contacts@aforgenet.com
//

namespace Openize.AForge.Imaging.NetStandard
{
    using System.ComponentModel;
    using System.Drawing;
    using Point = Core.NetStandard.Point;

    /// <summary>
    /// Image's blob.
    /// </summary>
    /// 
    /// <remarks><para>The class represents a blob - part of another images. The
    /// class encapsulates the blob itself and information about its position
    /// in parent image.</para>
    /// 
    /// <para><note>The class is not responsible for blob's image disposing, so it should be
    /// done manually when it is required.</note></para>
    /// </remarks>
    /// 
    public class Blob
    {
        // blob's image
        private UnmanagedImage image;
        // blob's image size - as original image or not
        private bool originalSize = false;

        // blob's rectangle in the original image
        private Rectangle rect;
        // blob's ID in the original image
        private int id;
        // area of the blob
        private int area;
        // center of gravity
        private Point cog;
        // fullness of the blob ( area / ( width * height ) )
        private double fullness;
        // mean color of the blob
        private Color colorMean = Color.Black;
        // color's standard deviation of the blob
        private Color colorStdDev = Color.Black;

        /// <summary>
        /// Blob's image.
        /// </summary>
        ///
        /// <remarks><para>The property keeps blob's image. In the case if it equals to <b>null</b>,
        /// the image may be extracted using <see cref="BlobCounterBase.ExtractBlobsImage( Bitmap, Blob, bool )"/>
        /// or <see cref="BlobCounterBase.ExtractBlobsImage( UnmanagedImage, Blob, bool )"/> method.</para></remarks>
        ///
        [Browsable( false )]
        public UnmanagedImage Image
        {
            get { return this.image; }
            internal set { this.image = value; }
        }

        /// <summary>
        /// Blob's image size.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies size of the <see cref="Image">blob's image</see>.
        /// If the property is set to <see langword="true"/>, the blob's image size equals to the
        /// size of original image. If the property is set to <see langword="false"/>, the blob's
        /// image size equals to size of actual blob.</para></remarks>
        /// 
        [Browsable( false )]
        public bool OriginalSize
        {
            get { return this.originalSize; }
            internal set { this.originalSize = value; }
        }

        /// <summary>
        /// Blob's rectangle in the original image.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies position of the blob in the original image
        /// and its size.</para></remarks>
        /// 
        public Rectangle Rectangle
        {
            get { return this.rect; }
        }

        /// <summary>
        /// Blob's ID in the original image.
        /// </summary>
        [Browsable( false )]
        public int ID
        {
            get { return this.id; }
            internal set { this.id = value; }
        }

        /// <summary>
        /// Blob's area.
        /// </summary>
        /// 
        /// <remarks><para>The property equals to blob's area measured in number of pixels
        /// contained by the blob.</para></remarks>
        /// 
        public int Area
        {
            get { return this.area; }
            internal set { this.area = value; }
        }

        /// <summary>
        /// Blob's fullness, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>The property equals to blob's fullness, which is calculated
        /// as <b>Area / ( Width * Height )</b>. If it equals to <b>1</b>, then
        /// it means that entire blob's rectangle is filled by blob's pixel (no
        /// blank areas), which is true only for rectangles. If it equals to <b>0.5</b>,
        /// for example, then it means that only half of the bounding rectangle is filled
        /// by blob's pixels.</para></remarks>
        /// 
        public double Fullness
        {
            get { return this.fullness; }
            internal set { this.fullness = value; }
        }

        /// <summary>
        /// Blob's center of gravity point.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps center of gravity point, which is calculated as
        /// mean value of X and Y coordinates of blob's points.</para></remarks>
        /// 
        public Point CenterOfGravity
        {
            get { return this.cog; }
            internal set { this.cog = value; }
        }

        /// <summary>
        /// Blob's mean color.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps mean color of pixels comprising the blob.</para></remarks>
        /// 
        public Color ColorMean
        {
            get { return this.colorMean; }
            internal set { this.colorMean = value; }
        }

        /// <summary>
        /// Blob color's standard deviation.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps standard deviation of pixels' colors comprising the blob.</para></remarks>
        /// 
        public Color ColorStdDev
        {
            get { return this.colorStdDev; }
            internal set { this.colorStdDev = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blob"/> class.
        /// </summary>
        /// 
        /// <param name="id">Blob's ID in the original image.</param>
        /// <param name="rect">Blob's rectangle in the original image.</param>
        /// 
        /// <remarks><para>This constructor leaves <see cref="Image"/> property not initialized. The blob's
        /// image may be extracted later using <see cref="BlobCounterBase.ExtractBlobsImage( Bitmap, Blob, bool )"/>
        /// or <see cref="BlobCounterBase.ExtractBlobsImage( UnmanagedImage, Blob, bool )"/> method.</para></remarks>
        /// 
        public Blob( int id, Rectangle rect )
        {
            this.id   = id;
            this.rect = rect;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blob"/> class.
        /// </summary>
        /// 
        /// <param name="source">Source blob to copy.</param>
        /// 
        /// <remarks><para>This copy constructor leaves <see cref="Image"/> property not initialized. The blob's
        /// image may be extracted later using <see cref="BlobCounterBase.ExtractBlobsImage( Bitmap, Blob, bool )"/>
        /// or <see cref="BlobCounterBase.ExtractBlobsImage( UnmanagedImage, Blob, bool )"/> method.</para></remarks>
        /// 
        public Blob( Blob source )
        {
            // copy everything except image
            this.id   = source.id;
            this.rect = source.rect;
            this.cog  = source.cog;
            this.area = source.area;
            this.fullness = source.fullness;
            this.colorMean = source.colorMean;
            this.colorStdDev = source.colorStdDev;
        }
    }
}
