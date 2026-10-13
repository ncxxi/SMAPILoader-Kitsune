using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace SMAPIGameLoader.Launcher;

public class ModItemView
{
    public string NameUIText = "Unknow";
    public string VersionUIText = "Unknow";
    public string FolderPathUIText = "Unknow";

    public readonly string modName = "unknow";
    public readonly string modVersion = "unknow";
    public string modFolderPath => GetModFolderPath();
    public readonly string modManifestPath;
    public string GetModFolderPath()
    {
        return Path.GetDirectoryName(modManifestPath);
    }
    public bool Enabled
    {
        get => IsEnable();
        set => TrySetActive(value);
    }
    private bool IsEnable()
    {
        var folderName = Path.GetFileName(GetModFolderPath());
        return !string.IsNullOrEmpty(folderName) && !folderName.StartsWith('.');
    }

    public ModItemView(string manifestFilePath, int modListIndex)
    {
        try
        {
            modManifestPath = manifestFilePath;

            var manifestText = File.ReadAllText(manifestFilePath);
            var manifest = JObject.Parse(manifestText);

            this.modName = manifest["Name"].ToString();
            this.modVersion = manifest["Version"].ToString();

            this.NameUIText = $"[{modListIndex + 1}]: {modName}";
            this.VersionUIText = $"Version: {modVersion}";

            var relativeModDir = modFolderPath.Substring(modFolderPath.IndexOf("/Mods") + 5);
            FolderPathUIText = $"Folder: {relativeModDir}";
        }
        catch (Exception ex)
        {
            FolderPathUIText = modFolderPath;
            ErrorDialogTool.Show(ex, "Error try parser mod folder path: " + this.modFolderPath);
        }

        this.NameUIText = $"[{modListIndex + 1}]: {modName}";
        this.VersionUIText = $"Version: {modVersion}";
    }

    void TrySetActive(bool newStatus)
    {
        if (newStatus == Enabled) return;

        try
        {

            var oldPath = modFolderPath;
            var parent = Path.GetDirectoryName(oldPath)!;
            var folderName = Path.GetFileName(oldPath).TrimStart('.');
            var newName = newStatus ? folderName : "." + folderName;
            var newPath = Path.Combine(parent, newName);
            Directory.Move(oldPath, newPath);
        }
        catch (Exception ex)
        {
            ErrorDialogTool.Show(ex, "Error try set mod active: " + this.modFolderPath);
        }
    }
}
