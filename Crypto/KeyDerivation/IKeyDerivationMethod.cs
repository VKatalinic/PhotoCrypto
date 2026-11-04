using System.Drawing;

namespace PhotoCrypto.Crypto.KeyDerivation;

public interface IKeyDerivationMethod
{
    string Name { get; }
    string Description { get; }
    byte[] DeriveKey(Bitmap image);
}
