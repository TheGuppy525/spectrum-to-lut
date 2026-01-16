using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;



namespace spectrum_to_lut
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // Default args
            string inputPath = "input.png";
            string outputPath = "output";
            int yOffset = 0;
            int lutSize = 32;
            string format = "cube";

            // Set args
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-offset":
                        yOffset =  Convert.ToInt32(args[i+1]);
                        break;
                    
                    case "-i":
                        inputPath = args[i+1];
                        break;
                    
                    case "-o":
                        outputPath = args[i+1];
                        break;
                    
                    case  "-s":
                        lutSize = Convert.ToInt32(args[i+1]);
                        break;
                    
                    case "-f":
                        format = args[i+1];
                        break;
                    
                    case "-help":
                        Console.WriteLine($$"""
                                               usage: spectrum-to-lut [options]
                                               
                                               options:
                                                 -i, <path>         specifies input spectrum location
                                                 -o, <path>         specifies output LUT location
                                                 -s,                sets the size of the output lut
                                                 -offset,           sets the vertical offset in pixels 
                                                                    of the input spectrum top to bottom
                                                 -f,                sets the output format                                 
                                            """);
                        return;
                }
            }
            
            // Check if input file exists
            if (File.Exists(inputPath))
            {
                // Load/generate required images
                Image<Rgb24> spectrum = Image.Load<Rgb24>(inputPath);
                var spectrumRef = SpectrumTools.GenerateReferenceSpectrum(spectrum.Width);
                var cubeRef = SpectrumTools.GenerateReferenceCube(lutSize);
            
                // Generate 3d texture lut
                var lut = SpectrumTools.ApplySpectrum(cubeRef, spectrumRef, spectrum, yOffset);
                
                // Save LUT
                Console.WriteLine("Saving LUT...");
                switch (format)
                {
                    case "png":
                        lut.SaveAsPng(outputPath + ".png");
                        break;
                    
                    case "cube":
                        Cube.SaveAsCube(lut, lutSize, outputPath + ".cube");
                        break;
                    
                    case "bmp":
                        lut.SaveAsBmp(outputPath + ".bmp");
                        break;
                    
                    case "jpeg" or "jpg":
                        lut.SaveAsJpeg(outputPath + ".jpg");
                        break;
                    
                    case "tga":
                        lut.SaveAsTga(outputPath + ".tga");
                        break;
                    
                    case "tiff":
                        lut.SaveAsTiff(outputPath + ".tiff");
                        break;
                    
                    case "webp":
                        lut.SaveAsWebp(outputPath + ".webp");
                        break;
                    
                    case "Qoi":
                        lut.SaveAsQoi(outputPath + ".Qoi");
                        break;
                }
                Console.WriteLine("\nDone!");
            }
            else
            {
                Console.WriteLine("Input Spectrum not found!");
            }
        }
    }
} 