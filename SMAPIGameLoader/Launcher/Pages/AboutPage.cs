using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using System;
using Xamarin.Essentials;

namespace SMAPIGameLoader.Launcher.Pages;

internal sealed class AboutPage : LinearLayout
{
    private const string UpstreamUrl = "https://github.com/NRTnarathip/SMAPILoader";
    private const string ForkUrl = "https://github.com/ncxxi/SMAPILoader-Kitsune";
    private const string LicenseUrl = "https://www.gnu.org/licenses/gpl-3.0.html";

    private readonly Context _context;

    public AboutPage(Context context) : base(context)
    {
        _context = context;
        Orientation = Orientation.Vertical;
        LayoutParameters = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent);

        AddHeader();
        AddContent();
    }

    private void AddHeader()
    {
        var headerBox = new LinearLayout(_context) { Orientation = Orientation.Vertical };
        headerBox.SetBackgroundColor(Color.ParseColor("#1A1D24"));
        headerBox.Elevation = Ui.Dp(_context, 6);
        headerBox.SetPadding(
            Ui.Dp(_context, 20),
            Ui.Dp(_context, 48),
            Ui.Dp(_context, 20),
            Ui.Dp(_context, 16));

        var header = new TextView(_context) { Text = "About" };
        header.SetTextSize(ComplexUnitType.Sp, 20);
        header.SetTextColor(Color.White);
        header.SetTypeface(null, TypefaceStyle.Bold);
        headerBox.AddView(header);

        var subtitle = new TextView(_context) { Text = $"SMAPI Kitsune {AppInfo.VersionString}" };
        subtitle.SetTextSize(ComplexUnitType.Sp, 14);
        subtitle.SetTextColor(Color.ParseColor("#9AA0A6"));
        headerBox.AddView(subtitle, new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent)
        { TopMargin = Ui.Dp(_context, 4) });

        AddView(headerBox, new LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent));
    }

    private void AddContent()
    {
        var scroll = new ScrollView(_context) { FillViewport = true };
        AddView(scroll, new LayoutParams(
            ViewGroup.LayoutParams.MatchParent, 0, 1f));

        var body = new LinearLayout(_context) { Orientation = Orientation.Vertical };
        body.SetPadding(
            Ui.Dp(_context, 16),
            Ui.Dp(_context, 16),
            Ui.Dp(_context, 16),
            Ui.Dp(_context, 24));
        scroll.AddView(body, new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent));

        AddIntroduction(body);
        AddProjectLinks(body);
        AddLicenseNotice(body);
    }

    private void AddIntroduction(LinearLayout body)
    {
        var card = CreateCard();
        var content = CreateCardContent();
        card.AddView(content);

        var title = new TextView(_context) { Text = "SMAPI Kitsune" };
        title.SetTextSize(ComplexUnitType.Sp, 18);
        title.SetTextColor(Color.White);
        title.SetTypeface(null, TypefaceStyle.Bold);
        content.AddView(title);

        var version = new TextView(_context)
        {
            Text = $"Version {AppInfo.VersionString} ({AppInfo.BuildString})"
        };
        version.SetTextSize(ComplexUnitType.Sp, 13);
        version.SetTextColor(Color.ParseColor("#9AA0A6"));
        content.AddView(version, new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent)
        { TopMargin = Ui.Dp(_context, 4) });

        var description = new TextView(_context)
        {
            Text = "Unofficial Android loader for Stardew Valley, forked from SMAPILoader."
        };
        description.SetTextSize(ComplexUnitType.Sp, 14);
        description.SetTextColor(Color.ParseColor("#D0D4DA"));
        description.SetLineSpacing(0, 1.15f);
        content.AddView(description, new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent)
        { TopMargin = Ui.Dp(_context, 12) });

        body.AddView(card, CardLayoutParams());
    }

    private void AddProjectLinks(LinearLayout body)
    {
        var card = CreateCard();
        var content = new LinearLayout(_context) { Orientation = Orientation.Vertical };
        content.SetPadding(0, Ui.Dp(_context, 16), 0, Ui.Dp(_context, 8));
        card.AddView(content);

        var label = new TextView(_context) { Text = "PROJECT" };
        label.SetTextSize(ComplexUnitType.Sp, 12);
        label.SetTextColor(Color.ParseColor("#9AA0A6"));
        label.SetTypeface(null, TypefaceStyle.Bold);
        label.SetPadding(
            Ui.Dp(_context, 20), 0,
            Ui.Dp(_context, 20), Ui.Dp(_context, 4));
        content.AddView(label);

        AddLinkRow(content, "Upstream project", UpstreamUrl, false);
        AddLinkRow(content, "Fork repository", ForkUrl, true);
        AddLinkRow(content, "License", "GNU General Public License v3.0", LicenseUrl, true);

        body.AddView(card, CardLayoutParams());
    }

    private void AddLicenseNotice(LinearLayout body)
    {
        var card = CreateCard();
        var content = CreateCardContent();
        card.AddView(content);

        var title = new TextView(_context) { Text = "License" };
        title.SetTextSize(ComplexUnitType.Sp, 16);
        title.SetTextColor(Color.White);
        title.SetTypeface(null, TypefaceStyle.Bold);
        content.AddView(title);

        var notice = new TextView(_context)
        {
            Text = "SMAPI Kitsune and the upstream SMAPILoader project are free software " +
                "licensed under the GNU General Public License v3.0. The source code for " +
                "this fork is available at the repository linked above."
        };
        notice.SetTextSize(ComplexUnitType.Sp, 14);
        notice.SetTextColor(Color.ParseColor("#D0D4DA"));
        notice.SetLineSpacing(0, 1.15f);
        content.AddView(notice, new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent)
        { TopMargin = Ui.Dp(_context, 8) });

        var disclaimer = new TextView(_context)
        {
            Text = "Stardew Valley is a trademark of ConcernedApe. This project is not " +
                "affiliated with or endorsed by ConcernedApe or Chucklefish."
        };
        disclaimer.SetTextSize(ComplexUnitType.Sp, 12);
        disclaimer.SetTextColor(Color.ParseColor("#9AA0A6"));
        disclaimer.SetLineSpacing(0, 1.15f);
        content.AddView(disclaimer, new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent)
        { TopMargin = Ui.Dp(_context, 12) });

        body.AddView(card, CardLayoutParams());
    }

    private void AddLinkRow(LinearLayout parent, string titleText, string subtitleText, string url, bool divider)
    {
        if (divider)
            parent.AddView(CreateDivider());

        var row = new LinearLayout(_context) { Orientation = Orientation.Vertical };
        row.SetPadding(
            Ui.Dp(_context, 20),
            Ui.Dp(_context, 12),
            Ui.Dp(_context, 20),
            Ui.Dp(_context, 12));
        row.Clickable = true;
        row.Focusable = true;

        var background = new TypedValue();
        if (_context.Theme.ResolveAttribute(
                Android.Resource.Attribute.SelectableItemBackground, background, true))
            row.SetBackgroundResource(background.ResourceId);

        var title = new TextView(_context) { Text = titleText };
        title.SetTextSize(ComplexUnitType.Sp, 15);
        title.SetTextColor(Color.White);
        title.SetTypeface(null, TypefaceStyle.Bold);
        row.AddView(title);

        var subtitle = new TextView(_context) { Text = subtitleText };
        subtitle.SetTextSize(ComplexUnitType.Sp, 13);
        subtitle.SetTextColor(Color.ParseColor("#81C784"));
        subtitle.SetPadding(0, Ui.Dp(_context, 3), 0, 0);
        row.AddView(subtitle);

        row.Click += (s, e) => OpenLink(url);
        parent.AddView(row, new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent));
    }

    private View CreateDivider()
    {
        var divider = new View(_context);
        divider.SetBackgroundColor(Color.ParseColor("#22FFFFFF"));
        divider.LayoutParameters = new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            Ui.Dp(_context, 1))
        { LeftMargin = Ui.Dp(_context, 20) };
        return divider;
    }

    private CardView CreateCard()
    {
        var card = new CardView(_context)
        {
            Radius = Ui.Dp(_context, 16),
            CardElevation = Ui.Dp(_context, 2),
            UseCompatPadding = true
        };
        card.SetCardBackgroundColor(Color.ParseColor("#1A1D24"));
        return card;
    }

    private LinearLayout CreateCardContent()
    {
        var content = new LinearLayout(_context) { Orientation = Orientation.Vertical };
        content.SetPadding(
            Ui.Dp(_context, 20),
            Ui.Dp(_context, 16),
            Ui.Dp(_context, 20),
            Ui.Dp(_context, 16));
        return content;
    }

    private LinearLayout.LayoutParams CardLayoutParams() =>
        new(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
        {
            BottomMargin = Ui.Dp(_context, 16)
        };

    private async void OpenLink(string url)
    {
        try
        {
            await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception)
        {
            ToastNotifyTool.Notify("Could not open link");
        }
    }
}
