using Android.App;
using Android.Content;
using SMAPIGameLoader.Launcher;
using System;

namespace SMAPIGameLoader;
internal static class EntryGame
{
    public static void LaunchGameActivity(Activity launcherActivity)
    {
        TaskTool.Run(launcherActivity, async () =>
        {
            LaunchGameActivityInternal(launcherActivity);
        });
    }

    static void LaunchGameActivityInternal(Activity launcherActivity)
    {
        //ToastNotifyTool.Notify("Starting Game..");
        //check game it's can launch with version

        try
        {
            CrashLog.Write("LaunchGameActivityInternal start");
            if (StardewApkTool.CurrentPackageInfo == null)
            {
                CrashLog.Write("Game package not found");
                ToastNotifyTool.Notify("Stardew Valley is not installed");
                return;
            }

            CrashLog.Write("Game package: " + StardewApkTool.CurrentPackageInfo.PackageName
                + ", version: " + StardewApkTool.CurrentGameVersion
                + ", base apk: " + StardewApkTool.BaseApkPath);

            if (GameCheckPolicy.RequireGameChecks && StardewApkTool.IsGameVersionSupport == false)
            {
                ToastNotifyTool.Notify("Not support game version: " + StardewApkTool.CurrentGameVersion + ", please update game");
                return;
            }

            if (SMAPIInstaller.IsInstalled is false)
            {
                CrashLog.Write("SMAPI is not installed");
                ToastNotifyTool.Notify("Please install SMAPI!!");
                return;
            }

            GameCloner.Setup();
            CrashLog.Write("GameCloner.Setup done");

#if DEBUG
            ToastNotifyTool.Notify("Error can't start game on Debug Mode");
            return;
#endif

            StartSMAPIActivity(launcherActivity);
            CrashLog.Write("SMAPIActivity started");
        }
        catch (Exception ex)
        {
            CrashLog.Write("LaunchGameActivityInternal failed: " + ex);
            ToastNotifyTool.Notify("Error:LaunchGameActivity: " + ex.ToString());
        }
    }
    //prevent Load Game Assembly in scope function LaunchGameActivityInternal()
    static void StartSMAPIActivity(Activity launcherActivity)
    {
        var intent = new Intent(launcherActivity, typeof(SMAPIActivity));
        intent.AddFlags(ActivityFlags.ClearTask);
        intent.AddFlags(ActivityFlags.NewTask);
        launcherActivity.StartActivity(intent);
        launcherActivity.Finish();
    }
}
