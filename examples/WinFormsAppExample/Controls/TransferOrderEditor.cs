// Copyright (C) TBC Bank. All Rights Reserved.

using System.Globalization;
using TBC.OpenAPI.SDK.BusinessIntegrationServices;

namespace WinFormsAppExample.Controls;

internal sealed class TransferOrderEditor : UserControl
{
    private static readonly TransferType[] AllTypes = Enum.GetValues<TransferType>();

    private static readonly TransferType[] WithCreditAccount =
    [
        TransferType.TransferToOwnAccount,
        TransferType.TransferWithinBank,
        TransferType.TransferToOtherBankNationalCurrency,
        TransferType.TransferToOtherBankForeignCurrency
    ];

    private readonly bool _batchMode;
    private readonly ComboBox _typeCombo = new();
    private readonly TableLayoutPanel _table = new();
    private readonly Dictionary<string, TextBox> _boxes = new();
    private readonly List<(Control Label, Control Input, Func<TransferType, bool> IsVisible)> _rows = [];

    public TransferOrderEditor(bool batchMode)
    {
        _batchMode = batchMode;

        _table.ColumnCount = 2;
        _table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _table.Dock = DockStyle.Top;
        _table.AutoSize = true;
        _table.AutoSizeMode = AutoSizeMode.GrowAndShrink;

        _typeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _typeCombo.Items.AddRange(AllTypes.Select(t => (object)t).ToArray());
        _typeCombo.SelectedIndex = 0;
        _typeCombo.SelectedIndexChanged += (_, _) => ApplyVisibility();
        AddRow("Transfer type", _typeCombo, _ => true);

        AddText("ExternalId", "External id", _ => true);
        AddText("DebitAccountNumber", "Debit account number", _ => !_batchMode);
        AddText("DebitCurrency", "Debit account currency", _ => !_batchMode);
        AddText("DocumentNumber", "Document number", _ => true);
        AddText("Amount", "Amount", _ => true);
        AddText("AmountCurrency", "Amount currency", _ => true);
        AddText("Position", "Position", _ => true);
        AddText("Description", "Description", _ => true);
        AddText("AdditionalDescription", "Additional description", _ => true);

        // Batch orders accept a credit account on every type; single treasury orders do not have one.
        Func<TransferType, bool> hasCredit = t => _batchMode || Array.IndexOf(WithCreditAccount, t) >= 0;
        AddText("CreditAccountNumber", "Credit account number", hasCredit);
        AddText("CreditCurrency", "Credit account currency", hasCredit);

        AddText("BeneficiaryName", "Beneficiary name", OneOf(TransferType.TransferWithinBank, TransferType.TransferToOtherBankNationalCurrency, TransferType.TransferToOtherBankForeignCurrency));
        AddText("BeneficiaryTaxCode", "Beneficiary tax code", OneOf(TransferType.TransferWithinBank, TransferType.TransferToOtherBankNationalCurrency));
        AddText("PersonalNumber", "Personal number", OneOf(TransferType.TransferWithinBank));

        var foreign = OneOf(TransferType.TransferToOtherBankForeignCurrency);
        AddText("BeneficiaryAddress", "Beneficiary address", foreign);
        AddText("BeneficiaryBankCode", "Beneficiary bank code", foreign);
        AddText("BeneficiaryBankName", "Beneficiary bank name", foreign);
        AddText("IntermediaryBankCode", "Intermediary bank code", foreign);
        AddText("IntermediaryBankName", "Intermediary bank name", foreign);
        AddText("ChargeDetails", "Charge details", foreign);

        var treasury = OneOf(TransferType.TreasuryTransfer);
        AddText("TaxpayerCode", "Taxpayer code", treasury);
        AddText("TaxpayerName", "Taxpayer name", treasury);
        AddText("TreasuryCode", "Treasury code", treasury);

        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Controls.Add(_table);

        ResetFields();
        ApplyVisibility();
    }

    public TransferType SelectedType => (TransferType)_typeCombo.SelectedItem!;

    public string ExternalId => Field("ExternalId") ?? string.Empty;

    public string AmountSummary => $"{Field("Amount")} {Field("AmountCurrency")}".Trim();

    // Keeps the type and debit account so several orders can be added in a row.
    public void ResetFields()
    {
        foreach (var key in new[]
        {
            "DocumentNumber", "Amount", "Position", "Description", "AdditionalDescription", "CreditAccountNumber",
            "BeneficiaryName", "BeneficiaryTaxCode", "PersonalNumber", "BeneficiaryAddress", "BeneficiaryBankCode",
            "BeneficiaryBankName", "IntermediaryBankCode", "IntermediaryBankName", "ChargeDetails",
            "TaxpayerCode", "TaxpayerName", "TreasuryCode"
        })
        {
            _boxes[key].Clear();
        }

        _boxes["ExternalId"].Text = Guid.NewGuid().ToString();

        if (_boxes["AmountCurrency"].TextLength == 0)
        {
            _boxes["AmountCurrency"].Text = "GEL";
        }

        if (_boxes["CreditCurrency"].TextLength == 0)
        {
            _boxes["CreditCurrency"].Text = "GEL";
        }

        if (_boxes["DebitCurrency"].TextLength == 0)
        {
            _boxes["DebitCurrency"].Text = "GEL";
        }
    }

    public string? GetValidationError()
    {
        var type = SelectedType;

        if (Field("ExternalId") is null)
        {
            return "External id is required.";
        }

        if (!_batchMode && (Field("DebitAccountNumber") is null || Field("DebitCurrency") is null))
        {
            return "Debit account number and currency are required.";
        }

        if (!TryDecimal(out _) || Field("AmountCurrency") is null)
        {
            return "A valid amount and amount currency are required.";
        }

        if (Field("DocumentNumber") is not null && !long.TryParse(Field("DocumentNumber"), NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
        {
            return "Document number must be a whole number.";
        }

        if (Field("Position") is not null && !int.TryParse(Field("Position"), NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
        {
            return "Position must be a whole number.";
        }

        var needsCredit = _batchMode ? type != TransferType.TreasuryTransfer : Array.IndexOf(WithCreditAccount, type) >= 0;
        if (needsCredit && (Field("CreditAccountNumber") is null || Field("CreditCurrency") is null))
        {
            return "Credit account number and currency are required.";
        }

        return null;
    }

    public SingleTransferOrder ToSingleOrder()
    {
        SingleTransferOrder order = SelectedType switch
        {
            TransferType.TransferToOwnAccount => new OwnAccountTransferOrder
            {
                CreditAccount = CreditAccount()
            },
            TransferType.TransferWithinBank => new WithinBankTransferOrder
            {
                CreditAccount = CreditAccount(),
                BeneficiaryName = Field("BeneficiaryName"),
                BeneficiaryTaxCode = Field("BeneficiaryTaxCode"),
                PersonalNumber = Field("PersonalNumber")
            },
            TransferType.TransferToOtherBankNationalCurrency => new OtherBankNationalCurrencyTransferOrder
            {
                CreditAccount = CreditAccount(),
                BeneficiaryName = Field("BeneficiaryName"),
                BeneficiaryTaxCode = Field("BeneficiaryTaxCode")
            },
            TransferType.TransferToOtherBankForeignCurrency => new OtherBankForeignCurrencyTransferOrder
            {
                CreditAccount = CreditAccount(),
                BeneficiaryName = Field("BeneficiaryName"),
                BeneficiaryAddress = Field("BeneficiaryAddress"),
                BeneficiaryBankCode = Field("BeneficiaryBankCode"),
                BeneficiaryBankName = Field("BeneficiaryBankName"),
                IntermediaryBankCode = Field("IntermediaryBankCode"),
                IntermediaryBankName = Field("IntermediaryBankName"),
                ChargeDetails = Field("ChargeDetails")
            },
            TransferType.TreasuryTransfer => new TreasuryTransferOrder
            {
                TaxpayerCode = Field("TaxpayerCode"),
                TaxpayerName = Field("TaxpayerName"),
                TreasuryCode = Field("TreasuryCode")
            },
            _ => throw new InvalidOperationException($"Unsupported transfer type {SelectedType}.")
        };

        order.TransferExternalId = Field("ExternalId");
        order.DebitAccount = new AccountIdentification
        {
            AccountNumber = Field("DebitAccountNumber"),
            AccountCurrencyCode = Field("DebitCurrency")
        };
        order.DocumentNumber = OptionalLong();
        order.Amount = Amount();
        order.Position = OptionalInt();
        order.Description = Field("Description");
        order.AdditionalDescription = Field("AdditionalDescription");

        return order;
    }

    public BatchTransferOrder ToBatchOrder()
        => new()
        {
            TransferType = SelectedType,
            TransferExternalId = Field("ExternalId"),
            CreditAccount = Field("CreditAccountNumber") is null ? null : CreditAccount(),
            DocumentNumber = OptionalLong(),
            Amount = Amount(),
            Position = OptionalInt(),
            Description = Field("Description"),
            AdditionalDescription = Field("AdditionalDescription"),
            PersonalNumber = Field("PersonalNumber"),
            BeneficiaryName = Field("BeneficiaryName"),
            BeneficiaryTaxCode = Field("BeneficiaryTaxCode"),
            BeneficiaryAddress = Field("BeneficiaryAddress"),
            BeneficiaryBankCode = Field("BeneficiaryBankCode"),
            BeneficiaryBankName = Field("BeneficiaryBankName"),
            IntermediaryBankCode = Field("IntermediaryBankCode"),
            IntermediaryBankName = Field("IntermediaryBankName"),
            ChargeDetails = Field("ChargeDetails"),
            TaxpayerCode = Field("TaxpayerCode"),
            TaxpayerName = Field("TaxpayerName"),
            TreasuryCode = Field("TreasuryCode")
        };

    private static Func<TransferType, bool> OneOf(params TransferType[] types)
        => t => Array.IndexOf(types, t) >= 0;

    private void AddText(string key, string label, Func<TransferType, bool> isVisible)
    {
        var box = new TextBox();
        _boxes[key] = box;
        AddRow(label, box, isVisible);
    }

    private void AddRow(string label, Control input, Func<TransferType, bool> isVisible)
    {
        var caption = new Label
        {
            Text = label,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(3, 6, 8, 6)
        };

        input.Dock = DockStyle.Fill;

        var row = _table.RowCount++;
        _table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _table.Controls.Add(caption, 0, row);
        _table.Controls.Add(input, 1, row);
        _rows.Add((caption, input, isVisible));
    }

    private void ApplyVisibility()
    {
        var type = SelectedType;

        _table.SuspendLayout();
        foreach (var (label, input, isVisible) in _rows)
        {
            var visible = isVisible(type);
            label.Visible = visible;
            input.Visible = visible;
        }

        _table.ResumeLayout();
    }

    private string? Field(string key)
    {
        var value = _boxes[key].Text.Trim();
        return value.Length == 0 ? null : value;
    }

    private bool TryDecimal(out decimal value)
        => decimal.TryParse(Field("Amount"), NumberStyles.Number, CultureInfo.InvariantCulture, out value);

    private Money Amount()
    {
        TryDecimal(out var amount);
        return new Money { Amount = amount, Currency = Field("AmountCurrency") };
    }

    private AccountIdentification CreditAccount()
        => new()
        {
            AccountNumber = Field("CreditAccountNumber"),
            AccountCurrencyCode = Field("CreditCurrency")
        };

    private long? OptionalLong()
        => Field("DocumentNumber") is { } text ? long.Parse(text, CultureInfo.InvariantCulture) : null;

    private int? OptionalInt()
        => Field("Position") is { } text ? int.Parse(text, CultureInfo.InvariantCulture) : null;
}
