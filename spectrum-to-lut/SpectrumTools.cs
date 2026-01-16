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

    public static Image<Rgb24> ApplySpectrum(Image<Rgb24> cubeRef, Image<Rgb24> spectrumRef, Image<Rgb24> spectrum, int yOffset)
    {
        Image<Rgb24> cube = new Image<Rgb24>(cubeRef.Width, cubeRef.Height); // Create base image

        spectrum.ProcessPixelRows(spectrumRef, (spectrumAccessor, spectrumRefAccessor) =>
        {
            // Init vars
            float hueShift;
            
            // Get Spectrum rows
            Span<Rgb24> spectrumRefRow = spectrumRefAccessor.GetRowSpan(0);
            Span<Rgb24> spectrumRow = spectrumAccessor.GetRowSpan(yOffset);
            
            // Main loop
            for (int i = 0; i < spectrumRef.Width; i++)
            {
                // Get the Pixels of the spectrum to compare
                Rgb refPixel = spectrumRefRow[i];   
                Rgb spectrumPixel = spectrumRow[i];
                
                // Convert pixels to HSV
                var refHsv = ColorSpaceConverter.ToHsv(in refPixel);
                var spectrumHsv = ColorSpaceConverter.ToHsv(in spectrumPixel);
                
                // Apply hue shift
                cube.ProcessPixelRows(cubeRef, (cubeAccessor, cubeRefAccessor) =>
                {
                    // Compare and calculate the required hue shift
                    hueShift = (spectrumHsv.H - refHsv.H);
                    
                    for (int h = 0; h < cubeRefAccessor.Height; h++)
                    {
                        // Get pixel rows of the cubes
                        Span<Rgb24> cubeRows = cubeAccessor.GetRowSpan(h);
                        Span<Rgb24> cubeRefRows = cubeRefAccessor.GetRowSpan(h);
                        
                        for (int w = 0; w < cubeRefRows.Length; w++)
                        {
                            Rgb cubeRefPixel = cubeRefRows[w];
                            var cubeRefHsv = ColorSpaceConverter.ToHsv(in cubeRefPixel);
                        
                            if (Math.Abs(refHsv.H - cubeRefHsv.H) < 0.2)
                            {
                                float valueShift = 0;
                                float saturationShift = 0;
                                
                                if (cubeRefHsv.S != 0)
                                {
                                    saturationShift = Math.Clamp((spectrumHsv.S - cubeRefHsv.S), -1, 0);
                                    valueShift = (Math.Clamp((spectrumHsv.V - cubeRefHsv.V), -1, 0) * cubeRefHsv.S);
                                }

                                cubeRows[w] = ColorSpaceConverter.ToRgb(new Hsv((cubeRefHsv.H+hueShift), (cubeRefHsv.S+saturationShift), (cubeRefHsv.V+valueShift)));
                            }
                        }
                    }
                });
                
                Console.WriteLine("Pass[{0}/{1}]", i+1, spectrumRef.Width);
            }

        });
            
        return cube;
    }
}