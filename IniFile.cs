using System;
using System.Runtime.InteropServices;
using System.Text;

public class IniFile
{
    private readonly string Path;

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern int GetPrivateProfileString(string? Section, string? Key, string? Default, 
        StringBuilder RetVal, int Size, string FilePath);

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern long WritePrivateProfileString(string? Section, string? Key, 
        string? Value, string FilePath);

    public IniFile(string iniPath)
    {
        Path = iniPath;
    }

    public string ReadValue(string Section, string Key, string DefaultValue = "")
    {
        StringBuilder temp = new StringBuilder(255);
        GetPrivateProfileString(Section, Key, DefaultValue, temp, 255, Path);
        return temp.ToString();
    }

    public bool WriteValue(string Section, string Key, string Value)
    {
        return WritePrivateProfileString(Section, Key, Value, Path) != 0;
    }

    public string[] ReadSection(string Section)
    {
        StringBuilder temp = new StringBuilder(8192);
        GetPrivateProfileString(Section, null, "", temp, 8192, Path);
        return temp.Length > 0 ? temp.ToString().Split('\0', StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();
    }

    public string[] GetSectionNames()
    {
        StringBuilder temp = new StringBuilder(8192);
        GetPrivateProfileString(null, null, "", temp, 8192, Path);
        return temp.Length > 0 ? temp.ToString().Split('\0', StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();
    }
} 