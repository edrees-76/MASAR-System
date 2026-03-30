using System;
using System.IO;

namespace MASAR.Helpers;

public static class SettingsHelper
{
    private static readonly string SettingsDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MASAR");
    private static readonly string SettingsFile;

    static SettingsHelper()
    {
        Directory.CreateDirectory(SettingsDir);
        SettingsFile = Path.Combine(SettingsDir, "settings.ini");

        // Migrate: if settings exist in old location (beside exe), copy once
        var oldFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.ini");
        if (!File.Exists(SettingsFile) && File.Exists(oldFile))
        {
            try { File.Copy(oldFile, SettingsFile); } catch { }
        }
    }

    public static bool IsDarkMode
    {
        get => Read("Theme") == "Dark";
        set => Write("Theme", value ? "Dark" : "Light");
    }

    public static string Language
    {
        get => Read("Language") ?? "ar";
        set => Write("Language", value);
    }

    private static string? Read(string key)
    {
        if (!File.Exists(SettingsFile)) return null;
        foreach (var line in File.ReadAllLines(SettingsFile))
        {
            var parts = line.Split('=', 2);
            if (parts.Length == 2 && parts[0].Trim() == key)
                return parts[1].Trim();
        }
        return null;
    }

    private static void Write(string key, string value)
    {
        var lines = File.Exists(SettingsFile) ? File.ReadAllLines(SettingsFile).ToList() : new List<string>();
        var found = false;
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].StartsWith(key + "="))
            {
                lines[i] = $"{key}={value}";
                found = true;
                break;
            }
        }
        if (!found) lines.Add($"{key}={value}");
        File.WriteAllLines(SettingsFile, lines);
    }
}
