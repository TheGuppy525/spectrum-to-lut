using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;

namespace spectrum_to_lut;

public static class Cube
{
    public static void SaveAsCube(Image<Rgb24> image, int size, string outputPath) // Saves an image as a .cube file | This was made with resonites formating in mind
    {
        using (var sw = new StreamWriter(outputPath))
        {
            sw.WriteLine("TITLE \"spectrum-to-lut made with love\"");
            sw.WriteLine("LUT_3D_SIZE {0}", size);
            sw.WriteLine("DOMAIN_MIN 0 0 0");
            sw.WriteLine("DOMAIN_MAX 1 1 1");

            for (int z = 0; z < size; z++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        Rgb pixel = image[x+(size*z), y];
                    
                        sw.WriteLine("{0} {1} {2}", pixel.R, pixel.G, pixel.B);
                    }
                }
            }
            
        }
    }
}