using System;
using System.IO;

namespace BarrichCSSystem.Utils;

public static class FileUtils
{
    public static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    
    public static string GetCefCachePath(string subpath = "") {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var cachePath = Path.Combine(appData, "BarrichCSSystem", "CefCache", subpath);
        EnsureDirectoryExists(cachePath);
            
        return cachePath;
    }
    
    public static string GetCefRelativeCachePath(string uniqKey)
    {
        return string.IsNullOrEmpty(uniqKey) ? "sandbox" : $"sandbox_{uniqKey}";
    }
    
    public static string GetSQLiteDBPath() {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "BarrichCSSystem", "db.sqlite");
    }
}