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
                    
                    case  "-size":
                        lutSize = Convert.ToInt32(args[i+1]);
                        break;
                    
                    case "-f":
                        format = args[i+1];
                        break;
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

                switch (format)
                {
                    case "png":
                        lut.SaveAsPng(outputPath + ".png");
                        break;
                    
                    case "cube":
                        Cube.SaveAsCube(lut, lutSize, outputPath + ".cube");
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