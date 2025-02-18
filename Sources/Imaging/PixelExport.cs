// -----------------------------------------------------------------------------------------------------------
//   <copyright file="PixelExport.cs" company="Aspose Pty Ltd" author="Stanislav Popov" date="23.08.2024 16:05">
//      Copyright (c) 2001-2024 Aspose Pty Ltd. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------------------------------------

namespace Openize.AForge.Imaging.NetStandard
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public static class PixelExport
    {
        #region Methods

        public static unsafe UnmanagedImage PixelDataToImage(int[] data, int width, int height)
        {
            fixed (int* src = data)
            {
                var image = new UnmanagedImage((IntPtr)src, width, height, width * 4, PixelFormat.Format32bppArgb);
                return image;
            }
        }

        /// <summary>
        ///     Images to pixel data.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static unsafe int[] ImageToPixelData(UnmanagedImage image)
        {
            var size = image.Width * image.Height;
            var data = new int[size];
            using (var bitmap = image.ToManagedImage())
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    var src = (int*)bitmapData.Scan0.ToPointer();
                    fixed (int* dst = data)
                    {
                        for (var i = 0; i < size; i++)
                        {
                            dst[i] = src[i];
                        }
                    }
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
            }

            return data;
        }

        #endregion
    }
}