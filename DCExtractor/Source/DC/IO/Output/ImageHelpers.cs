using DC.Types;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "ImageHelpers"
    /// 
    /// Purpose :   provides helper functions for saving image data.
    /// 
    /// We're really leveraging .NET here to save these pngs. Honestly, I'd prefer it over rolling my own.
    /// If you don't like PNG output, you could always write another exporter for a format that handles alpha differently(like targa).
    /// </summary>
    public static class ImageHelpers
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Constants.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public const string PNGExtension = ".png";


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves a PNG image from the tim2 image passed.
        /// </summary>
        /// <param name="tImage">The image to convert from.</param>
        public static void SavePNG(TIM2Image tImage)
        {
            for (int picture = 0; picture < tImage.pictures.Length; picture++)
            {
                //Grab a picture from the list.
                TIM2Picture tPicture = tImage.pictures[picture];

                //Make sure this image actually has data.
                if (tPicture.imageBytes == null || tPicture.imageBytes.Length == 0)
                    return;

                //Create a new .NET Bitmap.
                Bitmap tOutput = new Bitmap(tPicture.imageWidth, tPicture.imageHeight);
                {
                    //Get the pixels from the image.
                    Color[] tPixels = tPicture.GetPixels();

                    //Copy those pixels into the bitmap.
                    for (int x = 0; x < tOutput.Width; x++)
                    {
                        for (int y = 0; y < tOutput.Height; y++)
                        {
                            tOutput.SetPixel(x, y, tPixels[y * tPicture.imageWidth + x]);
                        }
                    }
                }

                //Create a new output path for this picture.
                string szNewPath = string.Empty;

                //If there is only one picture, just rename the output to png.
                if (tImage.pictures.Length == 1)
                    szNewPath = Path.ChangeExtension(tImage.filePath, ".png");
                //Otherwise, save them as subscripted file names.
                else
                    szNewPath = Path.Combine(Path.GetDirectoryName(tImage.filePath),
                    Path.GetFileNameWithoutExtension(tImage.filePath) + "_" + picture + ".png");

                //Save out the png version of this bitmap to the same folder with a different extension.
                tOutput.Save(szNewPath, ImageFormat.Png);
            }
        }
    }
}