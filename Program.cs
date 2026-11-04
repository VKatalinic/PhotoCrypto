using PhotoCrypto.Forms;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Application.ThreadException += (_, args) =>
            MessageBox.Show(
                $"Neočekivana greška:\n{args.Exception.Message}",
                "PhotoCrypto – Greška",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

        Application.Run(new MainForm());
    }
}
