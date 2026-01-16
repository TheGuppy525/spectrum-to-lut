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
            
            // Default args
            string inputPath = "input.png";
            string outputPath = "output.png";
            int yOffset = 0;
            int lutSize = 32;

            // Get args from user
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-yoffset":
                        yOffset =  Convert.ToInt32(args[i+1]);
                        break;
                    
                    case "-i":
                        inputPath = args[i+1];
                        break;
                    
                    case "-o":
                        outputPath = args[i+1];
                        break;
                    
                    case  "-size":
                        lutSize = Convert.ToInt32(args[i+1]);
                        break;
                }
            }

            // Load/generate required images
            Image<Rgb24> spectrum = Image.Load<Rgb24>(inputPath);
            var spectrumRef = SpectrumTools.GenerateReferenceSpectrum(spectrum.Width);
            var cubeRef = SpectrumTools.GenerateReferenceCube(lutSize);
            
            // Generate 3d texture lut
            var lut = SpectrumTools.ApplySpectrum(cubeRef, spectrumRef, spectrum, yOffset);
            
            lut.SaveAsPng(outputPath);
            Console.WriteLine("\nDone!");

        }
    }
} 