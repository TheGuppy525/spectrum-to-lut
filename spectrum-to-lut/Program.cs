using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;


namespace spectrum_to_lut
{
    class Program
    {
        static void Main(string[] args)
        {
            int WIDTH = 2048;
            
            string inputPath = "input.png";
            string outputPath = "output.png";
            string spectrumRefPath = "spectrumRef.png";
            string cubeRefPath = "cubeRef.png";
            if (args.Length == 2)
            { 
                inputPath = args[0];
                outputPath = args[1];
            }

            Image<Rgba32> spectrum = Image.Load<Rgba32>(inputPath);
            Image<Rgba32> spectrumRef = Image.Load<Rgba32>(spectrumRefPath);
            Image<Rgba32> cubeRef = Image.Load<Rgba32>(cubeRefPath);

            if (spectrumRef.Width != spectrum.Width)
            {
                Console.Write("Width mismatch between input and reference spectrum");
                return;
            }
            
            for (int i = 0; i < WIDTH; i++)
            {
                Rgb refPixel = spectrumRef[i, 0]; 
                Rgb spectrumPixel = spectrum[i, 0];
                
                var refHsv = ColorSpaceConverter.ToHsv(in refPixel);
                float refHue = refHsv.H;
                var spectrumHsv = ColorSpaceConverter.ToHsv(in spectrumPixel);
                float spectrumHue = spectrumHsv.H;

                float hueShift = (spectrumHue - refHue);

                for (int h = 0; h < cubeRef.Height; h++)
                {
                    for (int w = 0; w < cubeRef.Width; w++)
                    {
                        Rgb cubePixel = cubeRef[w, h];
                        var cubeHsv = ColorSpaceConverter.ToHsv(in cubePixel);
                        float cubeHue = cubeHsv.H;

                        if (Math.Abs(refHue - cubeHue) < 1)
                        {
                            cubeRef[w, h] = ColorSpaceConverter.ToRgb(new Hsv((cubeHue+hueShift), cubeHsv.V, cubeHsv.S));
                        }
                    }
                }
                Console.WriteLine(spectrumHue.ToString());
            }
            cubeRef.SaveAsPng(outputPath);

        }
    }
}