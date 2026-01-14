using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace spectrum_to_lut
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputPath = "input.png";
            string outputPath = "output.cube";
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
            
            


        }
    }
}