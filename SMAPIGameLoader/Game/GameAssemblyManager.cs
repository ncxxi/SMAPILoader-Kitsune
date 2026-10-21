using Android.App;
using MonoGame.Framework.Utilities;
using SMAPIGameLoader.Game;
using SMAPIGameLoader.Launcher;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Xamarin.Android.AssemblyStore;

namespace SMAPIGameLoader;

internal class GameAssemblyManager
{
    public const string AssembliesDirName = "Stardew Assemblies";
    public static string AssembliesDirPath => Path.Combine(FileTool.ExternalFilesDir, AssembliesDirName);
    public const string StardewDllName = "StardewValley.dll";
    public const string MonoGameDLLFileName = "MonoGame.Framework.dll";
    public static string StardewValleyFilePath => Path.Combine(AssembliesDirPath, StardewDllName);

    static void LogAssemblyStore(AssemblyStoreExplorerLogLevel level, string message)
    {
        if (level == AssemblyStoreExplorerLogLevel.Warning)
            return;
        Console.WriteLine($"{level}: {message}");
    }

    public static void VerifyAssemblies()
    {
        Console.WriteLine("Verify Assemblies");
        CrashLog.Write("VerifyAssemblies start");
        var assembliesOutputDirPath = AssembliesDirPath;
        Directory.CreateDirectory(assembliesOutputDirPath);

        {
            Console.WriteLine("try clone stardew assemblies");
            //clone dlls Stardew Valley - try base APK first, then arm64 split APK for .NET 9+
            CrashLog.Write("Reading game assembly store: " + StardewApkTool.BaseApkPath);
            var store = new AssemblyStoreExplorer(StardewApkTool.BaseApkPath, LogAssemblyStore, keepStoreInMemory: true);
            if (store.Assemblies.Count == 0 && StardewApkTool.Arm64ApkPath != null)
            {
                Console.WriteLine("No assemblies in base APK, trying arm64 split APK");
                CrashLog.Write("Reading game arm64 assembly store: " + StardewApkTool.Arm64ApkPath);
                store = new AssemblyStoreExplorer(StardewApkTool.Arm64ApkPath, LogAssemblyStore, keepStoreInMemory: true);
            }

            CrashLog.Write("Game assemblies found: " + store.Assemblies.Count);
            foreach (var asm in store.Assemblies)
            {
                asm.ExtractImage(assembliesOutputDirPath);
            }
            CrashLog.Write("Game assemblies extracted");
            Console.WriteLine("done clone stardew assemblies");
        }

        {
            Console.WriteLine("try clone SMAPI Game Loader Assemblies");
            //clone dll & no trimming from this app 
            var appInfo = Application.Context.ApplicationInfo;
            CrashLog.Write("Reading loader assembly store: " + appInfo.PublicSourceDir);
            var store = new AssemblyStoreExplorer(appInfo.PublicSourceDir, LogAssemblyStore, keepStoreInMemory: true);
            CrashLog.Write("Loader assemblies found: " + store.Assemblies.Count);
            foreach (var asm in store.Assemblies)
            {
                // This includes the loader's MonoGame.Framework.dll. It must replace the
                // game's copy because Java.Interop generated the Java peers for this
                // exact assembly during the build.
                asm.ExtractImage(assembliesOutputDirPath);
            }
            CrashLog.Write("Loader assemblies extracted");
            Console.WriteLine("done clone SMAPI Game Loader Assemblies");
        }

        CrashLog.Write("VerifyAssemblies done");
    }
    public static Assembly LoadAssembly(string dllFileName)
    {
        return Assembly.LoadFrom(Path.Combine(AssembliesDirPath, dllFileName));
    }

    static string LibDirPath => Path.Combine(FileTool.ExternalFilesDir, "lib");
    internal static void VerifyLibs()
    {
        Console.WriteLine("try setup libs");
        //clean lib first
        Console.WriteLine("clean up lib dir");
        if (Directory.Exists(LibDirPath))
            Directory.Delete(LibDirPath, true);
        Console.WriteLine("done setup libs");
    }
}
