// Copyright (C) TBC Bank. All Rights Reserved.

using System.Diagnostics;
using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using WinFormsAppExample.Infrastructure;

namespace WinFormsAppExample.Controls;

internal abstract class OperationTabBase : UserControl
{
    private readonly ClientHolder _holder;
    private readonly SplitContainer _split = new();
    private readonly Panel _inputScroll = new();
    private readonly TableLayoutPanel _inputTable = new();
    private readonly Button _executeButton = new();
    private readonly Button _cancelButton = new();
    private readonly Label _statusLabel = new();
    private readonly TextBox _responseBox = new();
    private CancellationTokenSource? _cts;
    private bool _splitInitialized;

    protected OperationTabBase(ClientHolder holder, string executeText)
    {
        _holder = holder;

        _split.Dock = DockStyle.Fill;
        _split.Orientation = Orientation.Vertical;

        BuildInputPane(executeText);
        BuildResponsePane();

        Controls.Add(_split);
    }

    protected abstract Task<object?> ExecuteAsync(IBusinessIntegrationServicesClient client, CancellationToken cancellationToken);

    protected virtual string? GetValidationError() => null;

    protected void AddRow(string label, Control control)
    {
        var row = _inputTable.RowCount++;
        _inputTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _inputTable.Controls.Add(new Label
        {
            Text = label,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(3, 6, 8, 6)
        }, 0, row);

        control.Dock = DockStyle.Fill;
        _inputTable.Controls.Add(control, 1, row);
    }

    protected void AddWide(Control control)
    {
        var row = _inputTable.RowCount++;
        _inputTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        control.Dock = DockStyle.Fill;
        _inputTable.Controls.Add(control, 0, row);
        _inputTable.SetColumnSpan(control, 2);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Min sizes are applied here because they throw while the control still has its default width.
        if (!_splitInitialized && Width > 600)
        {
            _split.Panel1MinSize = 250;
            _split.Panel2MinSize = 250;
            _split.SplitterDistance = Width * 45 / 100;
            _splitInitialized = true;
        }
    }

    private void BuildInputPane(string executeText)
    {
        _inputTable.ColumnCount = 2;
        _inputTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _inputTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _inputTable.Dock = DockStyle.Top;
        _inputTable.AutoSize = true;
        _inputTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _inputTable.Padding = new Padding(8);

        _inputScroll.Dock = DockStyle.Fill;
        _inputScroll.AutoScroll = true;
        _inputScroll.Controls.Add(_inputTable);

        _executeButton.Text = executeText;
        _executeButton.AutoSize = true;
        _executeButton.Click += OnExecuteClick;

        _cancelButton.Text = "Cancel";
        _cancelButton.AutoSize = true;
        _cancelButton.Enabled = false;
        _cancelButton.Click += (_, _) => _cts?.Cancel();

        _statusLabel.AutoSize = true;
        _statusLabel.Anchor = AnchorStyles.Left;
        _statusLabel.Margin = new Padding(12, 8, 3, 3);
        _statusLabel.Text = "Ready";

        var actionBar = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(8),
            WrapContents = false
        };
        actionBar.Controls.AddRange([_executeButton, _cancelButton, _statusLabel]);

        _split.Panel1.Controls.Add(_inputScroll);
        _split.Panel1.Controls.Add(actionBar);
    }

    private void BuildResponsePane()
    {
        _responseBox.Multiline = true;
        _responseBox.ReadOnly = true;
        _responseBox.ScrollBars = ScrollBars.Both;
        _responseBox.WordWrap = false;
        _responseBox.Dock = DockStyle.Fill;
        _responseBox.Font = new Font("Consolas", 10F);
        _responseBox.BackColor = SystemColors.Window;

        var copyButton = new Button { Text = "Copy", AutoSize = true };
        copyButton.Click += (_, _) =>
        {
            if (_responseBox.TextLength > 0)
            {
                Clipboard.SetText(_responseBox.Text);
            }
        };

        var clearButton = new Button { Text = "Clear", AutoSize = true };
        clearButton.Click += (_, _) => _responseBox.Clear();

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(4)
        };
        toolbar.Controls.AddRange([new Label { Text = "Response", AutoSize = true, Margin = new Padding(3, 8, 12, 3) }, copyButton, clearButton]);

        _split.Panel2.Controls.Add(_responseBox);
        _split.Panel2.Controls.Add(toolbar);
    }

    private async void OnExecuteClick(object? sender, EventArgs e)
    {
        if (!_holder.IsConnected)
        {
            SetStatus("Not connected. Press Connect first.", isError: true);
            return;
        }

        var validationError = GetValidationError();
        if (validationError is not null)
        {
            SetStatus(validationError, isError: true);
            return;
        }

        using var cts = new CancellationTokenSource();
        _cts = cts;
        SetBusy(true);
        SetStatus("Running...", isError: false);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await ExecuteAsync(_holder.Client, cts.Token);
            _responseBox.ForeColor = SystemColors.WindowText;
            _responseBox.Text = JsonFormatter.Format(result);
            SetStatus($"Succeeded in {stopwatch.ElapsedMilliseconds} ms", isError: false);
        }
        catch (Exception ex)
        {
            var cancelled = cts.IsCancellationRequested;
            _responseBox.ForeColor = Color.Firebrick;
            _responseBox.Text = cancelled ? "Cancelled." : JsonFormatter.FormatException(ex);
            SetStatus(cancelled ? "Cancelled" : "Failed", isError: !cancelled);
        }
        finally
        {
            _cts = null;
            SetBusy(false);
        }
    }

    private void SetBusy(bool busy)
    {
        _executeButton.Enabled = !busy;
        _cancelButton.Enabled = busy;
        _inputScroll.Enabled = !busy;
    }

    private void SetStatus(string text, bool isError)
    {
        _statusLabel.Text = text;
        _statusLabel.ForeColor = isError ? Color.Firebrick : SystemColors.ControlText;
    }
}
