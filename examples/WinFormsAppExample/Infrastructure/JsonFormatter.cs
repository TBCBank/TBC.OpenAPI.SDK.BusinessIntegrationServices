// Copyright (C) TBC Bank. All Rights Reserved.

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WinFormsAppExample.Infrastructure;

internal static class JsonFormatter
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static string Format(object? value)
        => value is null ? "null" : JsonSerializer.Serialize(value, value.GetType(), Options);

    public static string FormatException(Exception exception)
    {
        var builder = new StringBuilder();

        for (var current = exception; current is not null; current = current.InnerException!)
        {
            if (builder.Length > 0)
            {
                builder.AppendLine();
                builder.Append("  caused by: ");
            }

            builder.Append(current.GetType().Name).Append(": ").Append(current.Message);
        }

        return builder.ToString();
    }
}
