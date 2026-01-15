using System;
using System.Numerics;
using System.Runtime;
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
            string inputPath = "input.png";
            string outputPath = "output.png";
            string spectrumRefPath = "spectrumRef.png";
            string cubeRefPath = "cubeRef.png";
            
           /* if (args.Length == 2)
            { 
                inputPath = args[0];
                outputPath = args[1];
            } */

            int yOffset = 0;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-yoffset":
                        yOffset =  Convert.ToInt32(args[i+1]);
                        break;
                }
            }

            Image<Rgb24> spectrum = Image.Load<Rgb24>(inputPath);
            Image<Rgb24> spectrumRef = Image.Load<Rgb24>(spectrumRefPath);
            Image<Rgb24> cubeRef = Image.Load<Rgb24>(cubeRefPath);

            if (spectrumRef.Width != spectrum.Width)
            {
                Console.Write("Width mismatch between input and reference spectrum");
                return;
            }
            
            Image<Rgb24> cube = new Image<Rgb24>(1024, 32);
            
            for (int i = 0; i < spectrumRef.Width; i++)
            {
                Rgb refPixel = spectrumRef[i, 0];   
                Rgb spectrumPixel = spectrum[i, yOffset];
                
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
                        
                        if (Math.Abs(refHue - cubeHue) < 0.2)
                        {
                            
                            float valueShift = 0;
                            float saturationShift = 0;
                            if (cubeHsv.S != 0)
                            {
                                saturationShift = Math.Clamp((spectrumHsv.S - cubeHsv.S), -1, 0);
                                valueShift = (Math.Clamp((spectrumHsv.V - cubeHsv.V), -1, 0) * cubeHsv.S);
                            }

                            cube[w, h] = ColorSpaceConverter.ToRgb(new Hsv((cubeHue+hueShift), (cubeHsv.S+saturationShift), (cubeHsv.V+valueShift)));
                        }
                    }
                }
            }
            cube.SaveAsPng(outputPath);

        }
    }
} 