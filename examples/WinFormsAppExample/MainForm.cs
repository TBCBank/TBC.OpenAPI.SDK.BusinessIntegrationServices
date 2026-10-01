// Copyright (C) TBC Bank. All Rights Reserved.

using WinFormsAppExample.Infrastructure;
using WinFormsAppExample.Tabs;

namespace WinFormsAppExample
{
    public partial class MainForm : Form
    {
        private const string TestUrl = "https://test-api.tbcbank.ge/";
        private const string ProductionUrl = "https://api.tbcbank.ge/";
        private const string CustomEnvironment = "Custom";

        private static readonly (string Name, string Url)[] Environments =
        [
            ("Test", TestUrl),
            ("Production", ProductionUrl),
            (CustomEnvironment, string.Empty)
        ];

        private readonly ClientHolder _holder = new();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            var defaults = ConfigDefaults.Load();

            environmentComboBox.Items.AddRange(Environments.Select(x => (object)x.Name).ToArray());
            baseUrlTextBox.Text = string.IsNullOrWhiteSpace(defaults.BaseUrl) ? TestUrl : ClientHolder.NormalizeBaseUrl(defaults.BaseUrl);
            apiKeyTextBox.Text = defaults.ApiKey ?? string.Empty;
            secretTextBox.Text = defaults.ClientSecret ?? string.Empty;

            var match = Array.FindIndex(Environments, x => string.Equals(x.Url, baseUrlTextBox.Text, StringComparison.OrdinalIgnoreCase));
            environmentComboBox.SelectedIndex = match >= 0 ? match : Environments.Length - 1;

            AddTab("Statement", new StatementTab(_holder));
            AddTab("Movements", new MovementsTab(_holder));
            AddTab("Movement by id", new MovementByIdTab(_holder));
            AddTab("Balances", new BalancesTab(_holder));
            AddTab("Import single transfers", new ImportSingleTransfersTab(_holder));
            AddTab("Import batch transfer", new ImportBatchTransferTab(_holder));
            AddTab("Transfer status", new TransferStatusTab(_holder));
            AddTab("Transfer id lookup", new TransferIdLookupTab(_holder));
        }

        private void AddTab(string title, Control content)
        {
            content.Dock = DockStyle.Fill;

            var page = new TabPage(title);
            page.Controls.Add(content);
            operationsTabControl.TabPages.Add(page);
        }

        private void environmentComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var selected = Environments[environmentComboBox.SelectedIndex];

            if (selected.Name != CustomEnvironment)
            {
                baseUrlTextBox.Text = selected.Url;
            }

            baseUrlTextBox.ReadOnly = selected.Name != CustomEnvironment;
        }

        private void connectButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(baseUrlTextBox.Text)
                || string.IsNullOrWhiteSpace(apiKeyTextBox.Text)
                || string.IsNullOrWhiteSpace(secretTextBox.Text))
            {
                MessageBox.Show(this, "Base URL, API key and client secret are required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                baseUrlTextBox.Text = ClientHolder.NormalizeBaseUrl(baseUrlTextBox.Text);
                _holder.Connect(baseUrlTextBox.Text, apiKeyTextBox.Text.Trim(), secretTextBox.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, JsonFormatter.FormatException(ex), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            operationsTabControl.Enabled = true;
            connectionStatusLabel.Text = $"Connected to {baseUrlTextBox.Text.Trim()} (token is requested on first call)";
        }
    }
}
