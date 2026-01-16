using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;

namespace spectrum_to_lut;

public static class SpectrumTools
{
    public static Image<Rgb24>
        GenerateReferenceCube(int size) // Creates a standard 3d spectrum of a given size
    {
        // create base 3d texture
        Image<Rgb24> cube = new Image<Rgb24>(size * size, size);

        // Main loop
        for (int z = 0; z < size; z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float r = ((1f / size) * x);
                    float g = ((1f / size) * y);
                    float b = ((1f / size) * z);

                    cube[((size * z) + x), y] = new Rgb(r, g, b);
                }
            }
        }
        Console.WriteLine("Creating reference cube...");
        return cube;
    }

    public static Image<Rgb24> GenerateReferenceSpectrum(int size) // Creates a standard 1d spectrum of a given size
    {
        // Init vars
        Image<Rgb24> spectrum = new Image<Rgb24>(size, 1);

        for (int x = 0; x < size; x++)
        {
            spectrum[x, 0] = ColorSpaceConverter.ToRgb(new Hsv(((360f / size) * x), 1f, 1f));
        }
        
        Console.WriteLine("Creating spectrum...");
        return spectrum;
    }

    public static Image<Rgb24> ApplySpectrum(Image<Rgb24> cubeRef, Image<Rgb24> spectrumRef, Image<Rgb24> spectrum,
        int yOffset) // Creates a 3d LUT from reference data and an input spectrum
    {
        Image<Rgb24> cube = new Image<Rgb24>(cubeRef.Width, cubeRef.Height); // Create base image

        int pass = 1; // used to track passes that are reported to the user 
        
        // Main loop
        Parallel.For(0, spectrumRef.Width, i =>
        {
            spectrum.ProcessPixelRows(spectrumRef, (spectrumAccessor, spectrumRefAccessor) =>
            {
                // Get Spectrum rows
                Span<Rgb24> spectrumRefRow = spectrumRefAccessor.GetRowSpan(0);
                Span<Rgb24> spectrumRow = spectrumAccessor.GetRowSpan(yOffset);
                
                // Get the Pixels of the spectrum to compare
                Rgb refPixel = spectrumRefRow[i];
                Rgb spectrumPixel = spectrumRow[i];

                // Convert pixels to HSV
                var refHsv = ColorSpaceConverter.ToHsv(in refPixel);
                var spectrumHsv = ColorSpaceConverter.ToHsv(in spectrumPixel);
                
                // Compare and calculate the required hue shift
                float hueShift = (spectrumHsv.H - refHsv.H);
                
                // Apply hue shift
                cube.ProcessPixelRows(cubeRef, (cubeAccessor, cubeRefAccessor) =>
                {
                    for (int h = 0; h < cubeRefAccessor.Height; h++)
                    {
                        // Get pixel rows of the cubes
                        Span<Rgb24> cubeRows = cubeAccessor.GetRowSpan(h);
                        Span<Rgb24> cubeRefRows = cubeRefAccessor.GetRowSpan(h);

                        for (int w = 0; w < cubeRefRows.Length; w++)
                        {
                            // Get the pixel we are working on and convert it to HSV
                            Rgb cubeRefPixel = cubeRefRows[w];
                            var cubeRefHsv = ColorSpaceConverter.ToHsv(in cubeRefPixel);

                            // Compare the hue in the reference cube to reference spectrum and alter the hue of the cube to the input spectrum
                            if (Math.Abs(refHsv.H - cubeRefHsv.H) < 0.2)
                            {
                                float saturationShift = Math.Clamp((spectrumHsv.S - cubeRefHsv.S), -1, 0);
                                float valueShift = (Math.Clamp((spectrumHsv.V - cubeRefHsv.V), -1, 0) * cubeRefHsv.S);

                                cubeRows[w] = ColorSpaceConverter.ToRgb(new Hsv((cubeRefHsv.H + hueShift),
                                    (cubeRefHsv.S + saturationShift), (cubeRefHsv.V + valueShift)));
                            }
                        }
                    }
                });
                Console.WriteLine("Pass[{0}/{1}]", pass++, spectrumRef.Width);
            });
        });
        return cube;
    }
}