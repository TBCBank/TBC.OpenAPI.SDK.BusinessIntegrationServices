// Copyright (C) TBC Bank. All Rights Reserved.

namespace WinFormsAppExample;

internal static class Program
{
    [STAThread]
    internal static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
