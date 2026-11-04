using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PhotoCrypto.Analysis;

public class ImageAnalyzer
{
    public record AnalysisResult(
        double ShannonEntropy,
        double IntensityVariance,
        double Score,
        string Recommendation
    );

    public AnalysisResult Analyze(Bitmap image)
    {
        byte[] grayscale = ToGrayscaleBytes(image);

        double entropy = ComputeShannonEntropy(grayscale);
        double variance = ComputeVariance(grayscale);

        double score = ComputeScore(entropy, variance);
        string recommendation = BuildRecommendation(score, entropy, variance);

        return new AnalysisResult(entropy, variance, score, recommendation);
    }

    private static double ComputeShannonEntropy(byte[] pixels)
    {
        long[] histogram = new long[256];
        foreach (byte b in pixels)
            histogram[b]++;

        double entropy = 0.0;
        double n = pixels.Length;

        for (int i = 0; i < 256; i++)
        {
            if (histogram[i] == 0) continue;
            double p = histogram[i] / n;
            entropy -= p * Math.Log2(p);
        }

        return entropy;
    }

    private static double ComputeVariance(byte[] pixels)
    {
        double mean = 0.0;
        foreach (byte b in pixels)
            mean += b;
        mean /= pixels.Length;

        double variance = 0.0;
        foreach (byte b in pixels)
        {
            double diff = b - mean;
            variance += diff * diff;
        }
        return variance / pixels.Length;
    }

    private static double ComputeScore(double entropy, double variance)
    {
        double entropyScore = (entropy / 8.0) * 100.0;

        double varianceScore = Math.Min((variance / 5000.0) * 100.0, 100.0);

        return entropyScore * 0.6 + varianceScore * 0.4;
    }

    private static string BuildRecommendation(double score, double entropy, double variance)
    {
        string grade = score switch
        {
            >= 70 => "Odlično",
            >= 50 => "Dobro",
            >= 30 => "Osrednje",
            _ => "Loše"
        };

        string entropyComment = entropy switch
        {
            >= 7.0 => "Visoka entropija – slika je informacijski bogata",
            >= 5.0 => "Srednja entropija",
            _ => "Niska entropija – slika je previše jednolična"
        };

        string varianceComment = variance switch
        {
            >= 3000 => "Visoka varijanca – dobra rasprostranjenost svjetline",
            >= 1000 => "Srednja varijanca",
            _ => "Niska varijanca – slika je previše tamna ili previše svijetla"
        };

        return $"{grade} (ocjena: {score:F1}/100)\n{entropyComment}\n{varianceComment}";
    }

    private static byte[] ToGrayscaleBytes(Bitmap bmp)
    {
        Rectangle rect = new(0, 0, bmp.Width, bmp.Height);
        BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            int stride = Math.Abs(bmpData.Stride);
            byte[] rawPixels = new byte[stride * bmp.Height];
            Marshal.Copy(bmpData.Scan0, rawPixels, 0, rawPixels.Length);

            int totalPixels = bmp.Width * bmp.Height;
            byte[] gray = new byte[totalPixels];

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    int offset = y * stride + x * 4;
                    byte b = rawPixels[offset];
                    byte g = rawPixels[offset + 1];
                    byte r = rawPixels[offset + 2];

                    gray[y * bmp.Width + x] =
                        (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                }
            }

            return gray;
        }
        finally
        {
            bmp.UnlockBits(bmpData);
        }
    }
}