// Copyright (C) TBC Bank. All Rights Reserved.

namespace WinFormsAppExample.Controls;

internal sealed class TransferOrderList : UserControl
{
    private readonly TransferOrderEditor _editor;
    private readonly bool _batchMode;
    private readonly ListView _list = new();
    private readonly Label _message = new();

    public TransferOrderList(bool batchMode)
    {
        _batchMode = batchMode;
        _editor = new TransferOrderEditor(batchMode) { Dock = DockStyle.Top };

        var addButton = new Button { Text = "Add order to list", AutoSize = true };
        addButton.Click += OnAddClick;

        _message.AutoSize = true;
        _message.ForeColor = Color.Firebrick;
        _message.Margin = new Padding(12, 8, 3, 3);

        var addBar = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = false };
        addBar.Controls.AddRange([addButton, _message]);

        _list.View = View.Details;
        _list.FullRowSelect = true;
        _list.HideSelection = false;
        _list.Dock = DockStyle.Fill;
        _list.Columns.Add("External id", 220);
        _list.Columns.Add("Type", 220);
        _list.Columns.Add("Amount", 100);

        var removeButton = new Button { Text = "Remove selected", AutoSize = true };
        removeButton.Click += (_, _) =>
        {
            foreach (var item in _list.SelectedItems.Cast<ListViewItem>().ToList())
            {
                _list.Items.Remove(item);
            }
        };

        var clearButton = new Button { Text = "Clear list", AutoSize = true };
        clearButton.Click += (_, _) => _list.Items.Clear();

        var listBar = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true };
        listBar.Controls.AddRange([removeButton, clearButton]);

        var listPanel = new Panel { Dock = DockStyle.Top, Height = 170 };
        listPanel.Controls.Add(_list);
        listPanel.Controls.Add(listBar);

        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;

        // Docked controls are laid out bottom-up, so add in reverse visual order.
        Controls.Add(listPanel);
        Controls.Add(addBar);
        Controls.Add(_editor);
    }

    public int Count => _list.Items.Count;

    public IReadOnlyList<T> Orders<T>()
        => _list.Items.Cast<ListViewItem>().Select(i => (T)i.Tag!).ToList();

    private void OnAddClick(object? sender, EventArgs e)
    {
        var error = _editor.GetValidationError();
        _message.Text = error ?? string.Empty;

        if (error is not null)
        {
            return;
        }

        object order = _batchMode ? _editor.ToBatchOrder() : _editor.ToSingleOrder();

        _list.Items.Add(new ListViewItem([_editor.ExternalId, _editor.SelectedType.ToString(), _editor.AmountSummary]) { Tag = order });
        _editor.ResetFields();
    }
}
