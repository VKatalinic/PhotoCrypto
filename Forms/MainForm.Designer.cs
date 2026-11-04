namespace PhotoCrypto.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel mainLayout = null!;

    private GroupBox grpImage = null!;
    private PictureBox picImage = null!;
    private Label lblImageInfo = null!;
    private Button btnLoadImage = null!;
    private GroupBox grpAnalysis = null!;
    private Label lblAnalysisResult = null!;

    private GroupBox grpKey = null!;
    private Label lblDerivMethod = null!;
    private ComboBox cmbDerivation = null!;
    private Label lblDerivationDesc = null!;
    private Button btnDeriveKey = null!;
    private Label lblKeyPreview = null!;

    private GroupBox grpEncryption = null!;
    private Label lblAlgorithm = null!;
    private ComboBox cmbEncryption = null!;
    private Label lblEncryptionDesc = null!;
    private Label lblInput = null!;
    private TextBox txtInput = null!;
    private FlowLayoutPanel pnlButtons = null!;
    private Button btnEncrypt = null!;
    private Button btnDecrypt = null!;
    private Button btnSwapIo = null!;
    private Button btnCopyOutput = null!;
    private Label lblOutput = null!;
    private TextBox txtOutput = null!;

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.Text = "Enkripcija fotografijom";
        this.Size = new Size(1500, 800);
        this.MinimumSize = new Size(1400, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Font = new Font("Segoe UI", 9.5f);

        mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 3,
            Padding = new Padding(8)
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28f));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28f));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44f));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f));

        var leftPanel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 65f));
        leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 35f));

        grpImage = MakeGroupBox("Fotografija");

        picImage = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.FromArgb(30, 30, 30),
            BorderStyle = BorderStyle.None
        };

        lblImageInfo = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 22,
            Text = "Fotografija nije učitana",
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleLeft
        };

        btnLoadImage = MakeButton("Učitaj fotografiju", Color.FromArgb(52, 120, 200));
        btnLoadImage.Dock = DockStyle.Bottom;
        btnLoadImage.Height = 34;
        btnLoadImage.Click += btnLoadImage_Click;

        grpImage.Controls.Add(picImage);
        grpImage.Controls.Add(lblImageInfo);
        grpImage.Controls.Add(btnLoadImage);

        grpAnalysis = MakeGroupBox("Analiza kvalitete fotografije");
        lblAnalysisResult = new Label
        {
            Dock = DockStyle.Fill,
            Text = "Učitaj fotografiju.",
            ForeColor = Color.FromArgb(180, 180, 180),
            AutoSize = false
        };
        grpAnalysis.Controls.Add(lblAnalysisResult);

        leftPanel.Controls.Add(grpImage, 0, 0);
        leftPanel.Controls.Add(grpAnalysis, 0, 1);

        grpKey = MakeGroupBox("Derivacija ključa iz fotografije");

        var keyInner = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 5,
            ColumnCount = 1,
            Padding = new Padding(4)
        };
        keyInner.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
        keyInner.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
        keyInner.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));
        keyInner.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
        keyInner.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        lblDerivMethod = MakeLabel("Metoda derivacije:");
        cmbDerivation = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbDerivation.SelectedIndexChanged += cmbDerivation_SelectedIndexChanged;

        lblDerivationDesc = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(160, 160, 160),
            AutoSize = false
        };

        btnDeriveKey = MakeButton("Generiraj ključ iz fotografije", Color.FromArgb(40, 160, 80));
        btnDeriveKey.Dock = DockStyle.Fill;
        btnDeriveKey.Click += btnDeriveKey_Click;

        lblKeyPreview = new Label
        {
            Dock = DockStyle.Fill,
            Text = "Ključ: (nije generiran)",
            ForeColor = Color.FromArgb(255, 200, 80),
            AutoSize = false,
            Font = new Font("Consolas", 8.5f)
        };

        keyInner.Controls.Add(lblDerivMethod, 0, 0);
        keyInner.Controls.Add(cmbDerivation, 0, 1);
        keyInner.Controls.Add(lblDerivationDesc,0, 2);
        keyInner.Controls.Add(btnDeriveKey, 0, 3);
        keyInner.Controls.Add(lblKeyPreview, 0, 4);

        grpKey.Controls.Add(keyInner);

        grpEncryption = MakeGroupBox("Enkripcija / Dekripcija podataka");

        var encInner = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 8,
            ColumnCount = 1,
            Padding = new Padding(4)
        };
        encInner.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
        encInner.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
        encInner.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
        encInner.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
        encInner.RowStyles.Add(new RowStyle(SizeType.Percent, 45f)); 
        encInner.RowStyles.Add(new RowStyle(SizeType.Absolute, 42f));
        encInner.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
        encInner.RowStyles.Add(new RowStyle(SizeType.Percent, 55f)); 

        lblAlgorithm = MakeLabel("Algoritam enkripcije:");
        cmbEncryption = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbEncryption.SelectedIndexChanged += cmbEncryption_SelectedIndexChanged;

        lblEncryptionDesc = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(160, 160, 160),
            AutoSize = false
        };

        lblInput = MakeLabel("Ulazni tekst (za enkripciju) ili Base64 šifrat (za dekripciju):");

        txtInput = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Consolas", 9.5f),
            BackColor = Color.FromArgb(28, 28, 28),
            ForeColor = Color.FromArgb(220, 220, 220)
        };

        pnlButtons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 4, 0, 0)
        };

        btnEncrypt = MakeButton("Enkriptiraj", Color.FromArgb(52, 120, 200));
        btnEncrypt.Width = 140;
        btnEncrypt.Click += btnEncrypt_Click;

        btnDecrypt = MakeButton("Dekriptiraj", Color.FromArgb(160, 80, 40));
        btnDecrypt.Width = 140;
        btnDecrypt.Click += btnDecrypt_Click;

        btnSwapIo = MakeButton("Zamijeni", Color.FromArgb(80, 80, 80));
        btnSwapIo.Width = 110;
        btnSwapIo.Click += btnSwapIo_Click;

        pnlButtons.Controls.AddRange(new Control[]
            { btnEncrypt, btnDecrypt, btnSwapIo, btnCopyOutput });

        lblOutput = MakeLabel("Izlaz (Base64 šifrat ili dekriptirani tekst):");

        txtOutput = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Consolas", 9.5f),
            BackColor = Color.FromArgb(18, 28, 18),
            ForeColor = Color.FromArgb(140, 220, 140)
        };

        encInner.Controls.Add(lblAlgorithm, 0, 0);
        encInner.Controls.Add(cmbEncryption, 0, 1);
        encInner.Controls.Add(lblEncryptionDesc,0,2);
        encInner.Controls.Add(lblInput, 0, 3);
        encInner.Controls.Add(txtInput, 0, 4);
        encInner.Controls.Add(pnlButtons, 0, 5);
        encInner.Controls.Add(lblOutput, 0, 6);
        encInner.Controls.Add(txtOutput, 0, 7);

        grpEncryption.Controls.Add(encInner);

        mainLayout.Controls.Add(leftPanel, 0, 0);
        mainLayout.Controls.Add(grpKey, 1, 0);
        mainLayout.Controls.Add(grpEncryption, 2, 0);

        this.Controls.Add(mainLayout);

        ApplyDarkTheme(this);

        this.ResumeLayout(false);
    }


    private static GroupBox MakeGroupBox(string title) => new GroupBox
    {
        Text = title,
        Dock = DockStyle.Fill,
        ForeColor = Color.FromArgb(200, 200, 200),
        Padding = new Padding(6)
    };

    private static Label MakeLabel(string text) => new Label
    {
        Text = text,
        Dock = DockStyle.Fill,
        AutoSize = false,
        ForeColor = Color.FromArgb(190, 190, 190),
        TextAlign = ContentAlignment.MiddleLeft
    };

    private static Button MakeButton(string text, Color backColor) => new Button
    {
        Text = text,
        BackColor = backColor,
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat,
        Height = 32,
        Cursor = Cursors.Hand
    };

    private static void ApplyDarkTheme(Control root)
    {
        root.BackColor = Color.FromArgb(30, 30, 30);
        root.ForeColor = Color.FromArgb(200, 200, 200);
        foreach (Control c in root.Controls)
        {
            if (c is GroupBox or TableLayoutPanel or Panel or FlowLayoutPanel)
                ApplyDarkTheme(c);
            if (c is ComboBox cmb)
                cmb.BackColor = Color.FromArgb(45, 45, 45);
        }
    }
}