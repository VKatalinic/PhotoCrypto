using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace PhotoCrypto.Crypto.KeyDerivation;

public class HashKeyDerivation : IKeyDerivationMethod
{
    public enum HashAlgorithmType { SHA256, SHA512 }

    private readonly HashAlgorithmType algorithm;

    public HashKeyDerivation(HashAlgorithmType algorithm = HashAlgorithmType.SHA256) => this.algorithm = algorithm;

    public string Name => $"Hash ({algorithm})";
    public string Description =>
        $"Izračunava {algorithm} sažetak svih piksel-bajta fotografije. " +
        "Deterministički – isti ključ uvijek za istu sliku.";

    public byte[] DeriveKey(Bitmap image)
    {
        byte[] pixelBytes = ExtractPixelBytes(image);

        return algorithm switch
        {
            HashAlgorithmType.SHA512 => SHA512.HashData(pixelBytes),
            _ => SHA256.HashData(pixelBytes)
        };
    }

    private static byte[] ExtractPixelBytes(Bitmap bmp)
    {
        Rectangle rect = new(0, 0, bmp.Width, bmp.Height);
        BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            int byteCount = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] pixels = new byte[byteCount];
            Marshal.Copy(bmpData.Scan0, pixels, 0, byteCount);
            return pixels;
        }
        finally
        {
            bmp.UnlockBits(bmpData);
        }
    }
}
