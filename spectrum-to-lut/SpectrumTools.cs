using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;

namespace spectrum_to_lut;

public static class SpectrumTools
{
    public static Image<Rgb24> GenerateReferenceCube(int size) // Creates a reference 3d texture of the given size with a standard spectrum
    {
        
        // Init vars
        Image<Rgb24> cube = new Image<Rgb24>(size*size, size);
        float r = 0;
        float g = 0;
        float b = 0;
        
        // Main loop
        for (int z = 0; z < size; z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    r = ((1f / size) * x);
                    g = ((1f / size) * y);
                    b = ((1f / size) * z);
                    
                    cube[((size*z) + x), y] = new Rgb(r, g, b);
                }
            }
        }
        
        return cube;
    }

    public static Image<Rgb24> GenerateReferenceSpectrum(int size)
    {
        // Init vars
        Image<Rgb24> spectrum = new Image<Rgb24>(size, 1);

        for (int x = 0; x < size; x++)
        {
            spectrum[x, 0] = ColorSpaceConverter.ToRgb(new  Hsv(((360f / size) * x), 1f, 1f));
        }
        
        return spectrum;

    }
}