namespace PhotoCrypto.Crypto.Encryption;

public interface IEncryptionStrategy
{
    string Name { get; }

    string Description { get; }

    byte[] Encrypt(byte[] data, byte[] key);

    byte[] Decrypt(byte[] data, byte[] key);
}
