
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.IO;
using Xamarin.Essentials;

namespace SMAPIGameLoader.Launcher.Pages;

public class HomePage : LinearLayout
{
    public Button PlayButton { get; }
    public Button LogButton { get; }
    public SMAPIInfoCard SmapiCard { get; }
    public GameInfoCard GameInfoCard { get; }


    public HomePage(Context context) : base(context)
    {
        Orientation = Orientation.Vertical;
        SetPadding(0, 0, 0, Ui.Dp(context, 16));
        LayoutParameters = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent);

        // ===== Header =====
        var header = new TextView(context) { Text = "SMAPI Kitsune" };
        header.SetTextSize(ComplexUnitType.Sp, 20);
        header.SetTextColor(Color.White);
        header.SetTypeface(null, TypefaceStyle.Bold);
        header.SetPadding(Ui.Dp(context, 20), Ui.Dp(context, 48), Ui.Dp(context, 20), Ui.Dp(context, 16));
        header.Elevation = Ui.Dp(context, 6);
        AddView(header, new LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent));

        // ===== Body =====
        var body = new LinearLayout(context) { Orientation = Orientation.Vertical };
        body.SetPadding(Ui.Dp(context, 16), Ui.Dp(context, 16), Ui.Dp(context, 16), 0);
        AddView(body, new LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent));

        // game is ready!


        var cardLayoutParam = new LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent)
        {
            BottomMargin = Ui.Dp(context, 16)
        };

        GameInfoCard = new(context);
        body.AddView(GameInfoCard, cardLayoutParam);

        SmapiCard = new SMAPIInfoCard(context);
        body.AddView(SmapiCard, cardLayoutParam);

        LogButton = CreateButton(context, "Share Crash Log", "#37474F");
        LogButton.Click += LogButton_Click;
        body.AddView(LogButton);

        PlayButton = CreateButton(context, "Launch Game", "#1565C0");
        PlayButton.Click += PlayButton_Click;
        body.AddView(PlayButton);
    }

    // ===== Helper =====
    static Button CreateButton(Context context, string text, string colorHex)
    {
        var btn = new Button(context) { Text = text };
        btn.SetAllCaps(false);
        btn.SetTextColor(Color.White);
        btn.SetTextSize(ComplexUnitType.Sp, 16);
        btn.SetTypeface(null, TypefaceStyle.Bold);

        var bg = new GradientDrawable();
        bg.SetShape(ShapeType.Rectangle);
        bg.SetCornerRadius(Ui.Dp(context, 12));
        bg.SetColor(Color.ParseColor(colorHex));
        btn.Background = bg;

        btn.SetPadding(0, Ui.Dp(context, 14), 0, Ui.Dp(context, 14));
        var lp = new LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent)
        {
            BottomMargin = Ui.Dp(context, 12)
        };
        btn.LayoutParameters = lp;
        return btn;
    }



    private void InstallSMAPIButton_Click(object sender, EventArgs e)
    {
        SMAPIInstaller.OnInstalledSMAPI = () =>
        {
            SmapiCard.Refresh();
        };
        SMAPIInstaller.OnClickInstallSMAPIZip(sender, e);
    }

    private void PlayButton_Click(object sender, System.EventArgs e)
    {
        LauncherActivity.Instance.OnClickStartGame();
    }

    private async void LogButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (File.Exists(CrashLog.LogFilePath) is false)
            {
                ToastNotifyTool.Notify("No crash log yet");
                return;
            }

            await Clipboard.SetTextAsync(CrashLog.ReadTail());
            ToastNotifyTool.Notify("Crash log copied to clipboard");

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "SMAPI Kitsune crash log",
                File = new ShareFile(CrashLog.LogFilePath)
            });
        }
        catch (Exception ex)
        {
            ErrorDialogTool.Show(ex);
        }
    }
}
