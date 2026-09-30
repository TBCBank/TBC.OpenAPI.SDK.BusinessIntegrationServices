// Copyright (C) TBC Bank. All Rights Reserved.

namespace WinFormsAppExample
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            connectionPanel = new FlowLayoutPanel();
            environmentLabel = new Label();
            environmentComboBox = new ComboBox();
            baseUrlTextBox = new TextBox();
            apiKeyLabel = new Label();
            apiKeyTextBox = new TextBox();
            secretLabel = new Label();
            secretTextBox = new TextBox();
            connectButton = new Button();
            operationsTabControl = new TabControl();
            statusStrip = new StatusStrip();
            connectionStatusLabel = new ToolStripStatusLabel();
            connectionPanel.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // connectionPanel
            // 
            connectionPanel.AutoSize = true;
            connectionPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            connectionPanel.Controls.Add(environmentLabel);
            connectionPanel.Controls.Add(environmentComboBox);
            connectionPanel.Controls.Add(baseUrlTextBox);
            connectionPanel.Controls.Add(apiKeyLabel);
            connectionPanel.Controls.Add(apiKeyTextBox);
            connectionPanel.Controls.Add(secretLabel);
            connectionPanel.Controls.Add(secretTextBox);
            connectionPanel.Controls.Add(connectButton);
            connectionPanel.Dock = DockStyle.Top;
            connectionPanel.Name = "connectionPanel";
            connectionPanel.Padding = new Padding(8);
            // 
            // environmentLabel
            // 
            environmentLabel.Anchor = AnchorStyles.Left;
            environmentLabel.AutoSize = true;
            environmentLabel.Name = "environmentLabel";
            environmentLabel.Text = "Environment";
            // 
            // environmentComboBox
            // 
            environmentComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            environmentComboBox.Name = "environmentComboBox";
            environmentComboBox.Width = 110;
            environmentComboBox.SelectedIndexChanged += environmentComboBox_SelectedIndexChanged;
            // 
            // baseUrlTextBox
            // 
            baseUrlTextBox.Name = "baseUrlTextBox";
            baseUrlTextBox.Width = 240;
            // 
            // apiKeyLabel
            // 
            apiKeyLabel.Anchor = AnchorStyles.Left;
            apiKeyLabel.AutoSize = true;
            apiKeyLabel.Margin = new Padding(12, 0, 3, 0);
            apiKeyLabel.Name = "apiKeyLabel";
            apiKeyLabel.Text = "API key";
            // 
            // apiKeyTextBox
            // 
            apiKeyTextBox.Name = "apiKeyTextBox";
            apiKeyTextBox.Width = 180;
            // 
            // secretLabel
            // 
            secretLabel.Anchor = AnchorStyles.Left;
            secretLabel.AutoSize = true;
            secretLabel.Margin = new Padding(12, 0, 3, 0);
            secretLabel.Name = "secretLabel";
            secretLabel.Text = "Client secret";
            // 
            // secretTextBox
            // 
            secretTextBox.Name = "secretTextBox";
            secretTextBox.UseSystemPasswordChar = true;
            secretTextBox.Width = 180;
            // 
            // connectButton
            // 
            connectButton.AutoSize = true;
            connectButton.Margin = new Padding(12, 3, 3, 3);
            connectButton.Name = "connectButton";
            connectButton.Text = "Connect";
            connectButton.Click += connectButton_Click;
            // 
            // operationsTabControl
            // 
            operationsTabControl.Dock = DockStyle.Fill;
            operationsTabControl.Enabled = false;
            operationsTabControl.Name = "operationsTabControl";
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(20, 20);
            statusStrip.Items.AddRange(new ToolStripItem[] { connectionStatusLabel });
            statusStrip.Name = "statusStrip";
            // 
            // connectionStatusLabel
            // 
            connectionStatusLabel.Name = "connectionStatusLabel";
            connectionStatusLabel.Text = "Not connected";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1163, 676);
            Controls.Add(operationsTabControl);
            Controls.Add(connectionPanel);
            Controls.Add(statusStrip);
            MinimumSize = new Size(900, 500);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TBC OpenAPI Business Integration Services Example App";
            Load += MainForm_Load;
            connectionPanel.ResumeLayout(false);
            connectionPanel.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel connectionPanel;
        private Label environmentLabel;
        private ComboBox environmentComboBox;
        private TextBox baseUrlTextBox;
        private Label apiKeyLabel;
        private TextBox apiKeyTextBox;
        private Label secretLabel;
        private TextBox secretTextBox;
        private Button connectButton;
        private TabControl operationsTabControl;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel connectionStatusLabel;
    }
}
