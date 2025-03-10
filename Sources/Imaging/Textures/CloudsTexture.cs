// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � Andrew Kirillov, 2005-2010
// andrew.kirillov@aforgenet.com
//

namespace Openize.AForge.Imaging.NetStandard.Textures
{
    using System;
    using Math.NetStandard;

    /// <summary>
    /// Clouds texture.
    /// </summary>
    /// 
    /// <remarks><para>The texture generator creates textures with effect of clouds.</para>
    /// 
    /// <para>The generator is based on the <see cref="PerlinNoise">Perlin noise function</see>.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create texture generator
    /// CloudsTexture textureGenerator = new CloudsTexture( );
    /// // generate new texture
    /// float[,] texture = textureGenerator.Generate( 320, 240 );
    /// // convert it to image to visualize
    /// Bitmap textureImage = TextureTools.ToBitmap( texture );
    /// </code>
    ///
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/clouds_texture.jpg" width="320" height="240" />
    /// </remarks>
    /// 
    public class CloudsTexture : ITextureGenerator
    {
        // Perlin noise function used for texture generation
        private PerlinNoise noise = new PerlinNoise( 8, 0.5, 1.0 / 32, 1.0 );

        // randmom number generator
        private Random rand = new Random( );
        private int		r;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudsTexture"/> class.
        /// </summary>
        /// 
        public CloudsTexture( )
        {
            this.Reset( );
        }

        /// <summary>
        /// Generate texture.
        /// </summary>
        /// 
        /// <param name="width">Texture's width.</param>
        /// <param name="height">Texture's height.</param>
        /// 
        /// <returns>Two dimensional array of intensities.</returns>
        /// 
        /// <remarks>Generates new texture of the specified size.</remarks>
        ///  
        public float[,] Generate( int width, int height )
        {
            float[,] texture = new float[height, width];

            for ( int y = 0; y < height; y++ )
            {
                for ( int x = 0; x < width; x++ )
                {
                    texture[y, x] =
                        Math.Max( 0.0f, Math.Min( 1.0f,
                            (float) this.noise.Function2D( x + this.r, y + this.r ) * 0.5f + 0.5f
                        ) );

                }
            }
            return texture;
        }

        /// <summary>
        /// Reset generator.
        /// </summary>
        /// 
        /// <remarks>Regenerates internal random numbers.</remarks>
        /// 
        public void Reset( )
        {
            this.r = this.rand.Next( 5000 );
        }
    }
}
