using System.Security.Cryptography;
using System.Text;
using PhotoCrypto.Analysis;
using PhotoCrypto.Crypto.Encryption;
using PhotoCrypto.Crypto.KeyDerivation;

namespace PhotoCrypto.Forms;

public partial class MainForm : Form
{
    private Bitmap? loadedImage;
    private byte[]? derivedKey;

    private readonly List<IEncryptionStrategy>  encryptionStrategies;
    private readonly List<IKeyDerivationMethod> derivationMethods;
    private readonly ImageAnalyzer analyzer;

    public MainForm()
    {
        InitializeComponent();

        analyzer = new ImageAnalyzer();

        encryptionStrategies = new List<IEncryptionStrategy>
        {
            new AesCbcEncryptionStrategy()
        };

        derivationMethods = new List<IKeyDerivationMethod>
        {
            new HashKeyDerivation(HashKeyDerivation.HashAlgorithmType.SHA256),
            new HashKeyDerivation(HashKeyDerivation.HashAlgorithmType.SHA512),
        };

        PopulateComboBoxes();
        SetInitialState();
    }

    private void PopulateComboBoxes()
    {
        cmbEncryption.Items.Clear();
        foreach (var s in encryptionStrategies)
            cmbEncryption.Items.Add(s.Name);
        cmbEncryption.SelectedIndex = 0;

        cmbDerivation.Items.Clear();
        foreach (var m in derivationMethods)
            cmbDerivation.Items.Add(m.Name);
        cmbDerivation.SelectedIndex = 0;
    }

    private void SetInitialState()
    {
        btnDeriveKey.Enabled = false;
        btnEncrypt.Enabled = false;
        btnDecrypt.Enabled = false;
        lblKeyPreview.Text = "Ključ:";
        lblAnalysisResult.Text = "Analiza:";
    }

    private void btnLoadImage_Click(object sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            Title = "Odaberi fotografiju za kriptografski ključ",
            Filter = "Slike (*.png;*.bmp;*.jpg;*.jpeg)|*.png;*.bmp;*.jpg;*.jpeg|Sve datoteke|*.*"
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;

        try
        {
            loadedImage?.Dispose();
            loadedImage = new Bitmap(dlg.FileName);

            picImage.Image = loadedImage;
            picImage.SizeMode = PictureBoxSizeMode.Zoom;

            lblImageInfo.Text =
                $"Datoteka: {Path.GetFileName(dlg.FileName)}   " +
                $"Dimenzije: {loadedImage.Width}×{loadedImage.Height} px";

            ImageAnalysis();

            btnDeriveKey.Enabled = true;
            derivedKey = null;
            btnEncrypt.Enabled = false;
            btnDecrypt.Enabled = false;
            lblKeyPreview.Text = "Ključ:";
        }
        catch (Exception ex)
        {
            ShowError("Greška pri učitavanju fotografije", ex);
        }
    }

    private void ImageAnalysis()
    {
        if (loadedImage == null) return;

        try
        {
            Cursor = Cursors.WaitCursor;
            var result = analyzer.Analyze(loadedImage);

            lblAnalysisResult.Text =
                $"Shannonova entropija: {result.ShannonEntropy:F4} bit/px   " +
                $"Varijanca: {result.IntensityVariance:F1}\n" +
                result.Recommendation;
        }
        catch (Exception ex)
        {
            lblAnalysisResult.Text = $"Greška analize: {ex.Message}";
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void cmbDerivation_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblDerivationDesc.Text = derivationMethods[cmbDerivation.SelectedIndex].Description;
    }

    private void cmbEncryption_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblEncryptionDesc.Text = encryptionStrategies[cmbEncryption.SelectedIndex].Description;
    }

    private void btnDeriveKey_Click(object sender, EventArgs e)
    {
        if (loadedImage == null) return;

        try
        {
            Cursor = Cursors.WaitCursor;

            var method = derivationMethods[cmbDerivation.SelectedIndex];

            IKeyDerivationMethod activeMethod = method;

            derivedKey = activeMethod.DeriveKey(loadedImage);

            string hex  = Convert.ToHexString(derivedKey);
            string preview = hex.Length > 64 ? hex[..64] + "…" : hex;
            lblKeyPreview.Text = $"Ključ ({derivedKey.Length * 8} bita): {preview}";

            btnEncrypt.Enabled = true;
            btnDecrypt.Enabled = true;
        }
        catch (Exception ex)
        {
            ShowError("Greška pri derivaciji ključa", ex);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void btnEncrypt_Click(object sender, EventArgs e)
    {
        if (derivedKey == null) return;

        string plaintext = txtInput.Text;
        if (string.IsNullOrEmpty(plaintext))
        {
            MessageBox.Show("Unesite tekst za enkripciju.", "Upozorenje",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Cursor = Cursors.WaitCursor;

            var strategy = encryptionStrategies[cmbEncryption.SelectedIndex];
            byte[] data = Encoding.UTF8.GetBytes(plaintext);
            byte[] cipher = strategy.Encrypt(data, derivedKey);

            txtOutput.Text = Convert.ToBase64String(cipher);
        }
        catch (Exception ex)
        {
            ShowError("Greška pri enkripciji", ex);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void btnDecrypt_Click(object sender, EventArgs e)
    {
        if (derivedKey == null) return;

        string cipherBase64 = txtInput.Text.Trim();
        if (string.IsNullOrEmpty(cipherBase64))
        {
            MessageBox.Show("Zalijepite Base64 šifrirani tekst u polje za unos.", "Upozorenje",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Cursor = Cursors.WaitCursor;

            byte[]? cipher;
            if (!TryParseBase64(cipherBase64, out cipher))
            {
                MessageBox.Show("Unos nije valjani Base64 niz.", "Greška formata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var strategy = encryptionStrategies[cmbEncryption.SelectedIndex];
            byte[] plain = strategy.Decrypt(cipher!, derivedKey);

            txtOutput.Text = Encoding.UTF8.GetString(plain);
        }
        catch (CryptographicException)
        {
            MessageBox.Show(
                "Dekripcija nije uspjela.\nProvjerite:\n" +
                "  • Je li učitana ista fotografija?\n" +
                "  • Je li odabrana ista metoda derivacije i algoritam?\n",
                "Greška dekripcije",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            ShowError("Greška pri dekripciji", ex);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void btnSwapIo_Click(object sender, EventArgs e)
    {
        (txtInput.Text, txtOutput.Text) = (txtOutput.Text, txtInput.Text);
    }

    private static bool TryParseBase64(string s, out byte[]? result)
    {
        try { result = Convert.FromBase64String(s); return true; }
        catch { result = null; return false; }
    }

    private static void ShowError(string title, Exception ex) =>
        MessageBox.Show($"{ex.Message}", title, MessageBoxButtons.OK, MessageBoxIcon.Error);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            loadedImage?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}