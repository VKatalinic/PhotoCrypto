using System.Security.Cryptography;

namespace PhotoCrypto.Crypto.Encryption;

public class AesCbcEncryptionStrategy : IEncryptionStrategy
{
    public string Name => "AES-256-CBC";
    public string Description =>
        "Industrijski standard – blok-šifar s 256-bitnim ključem i CBC načinom rada. " +
        "Slučajni IV pripojen je šifriranoj poruci (prvih 16 B).";

    private const int KeySize = 32;
    private const int BlockSize = 16;

    public byte[] Encrypt(byte[] data, byte[] key)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("Podaci za enkripciju ne smiju biti prazni.", nameof(data));
        if (key == null || key.Length == 0)
            throw new ArgumentException("Ključ ne smije biti prazan.", nameof(key));

        byte[] aesKey = NormalizeKey(key);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = aesKey;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        byte[] ciphertext = encryptor.TransformFinalBlock(data, 0, data.Length);

        byte[] output = new byte[BlockSize + ciphertext.Length];
        Buffer.BlockCopy(aes.IV, 0, output, 0, BlockSize);
        Buffer.BlockCopy(ciphertext, 0, output, BlockSize, ciphertext.Length);
        return output;
    }

    public byte[] Decrypt(byte[] data, byte[] key)
    {
        if (data == null || data.Length <= BlockSize)
            throw new ArgumentException(
                $"Šifrirani podaci moraju biti duži od {BlockSize} B (IV + ciphertext).", nameof(data));
        if (key == null || key.Length == 0)
            throw new ArgumentException("Ključ ne smije biti prazan.", nameof(key));

        byte[] aesKey = NormalizeKey(key);

        byte[] iv = new byte[BlockSize];
        byte[] ciphertext = new byte[data.Length - BlockSize];
        Buffer.BlockCopy(data, 0, iv, 0, BlockSize);
        Buffer.BlockCopy(data, BlockSize, ciphertext, 0, ciphertext.Length);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = aesKey;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
    }

    private static byte[] NormalizeKey(byte[] key)
    {
        if (key.Length == KeySize) return key;
        return SHA256.HashData(key);
    }
}
