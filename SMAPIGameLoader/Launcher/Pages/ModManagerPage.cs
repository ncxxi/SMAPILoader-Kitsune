using Android.App.Job;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.FloatingActionButton;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.FloatingActionButton;
using System;
using System.IO;

namespace SMAPIGameLoader.Launcher.Pages;


internal sealed class ModManagerPage : LinearLayout
{

    private readonly Context _context;
    private readonly List<ModItemView> _mods = new();
    private readonly ModAdapter _adapter;
    private readonly TextView _countText;
    private readonly TextView _emptyText;
    private readonly RecyclerView _list;

    public FloatingActionButton InstallFab { get; }

    public ModManagerPage(Context context) : base(context)
    {
        _context = context;
        Orientation = Orientation.Vertical;
        LayoutParameters = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent);

        // ---- Header ----
        _countText = new TextView(context);
        _countText.SetTextSize(ComplexUnitType.Sp, 20);

        var header = new LinearLayout(context) { Orientation = Orientation.Horizontal };
        header.SetPadding(Ui.Dp(context, 16), Ui.Dp(context, 16), Ui.Dp(context, 16), Ui.Dp(context, 8));
        header.AddView(_countText, new LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f));
        AddView(header, new LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent));

        // ---- Body: list + empty state + FAB ซ้อนกันใน FrameLayout ----
        var body = new FrameLayout(context);
        AddView(body, new LayoutParams(
            ViewGroup.LayoutParams.MatchParent, 0, 1f));

        _adapter = new ModAdapter(context, _mods, OnToggledModItem, OnClickedModItem);
        _list = new RecyclerView(context);
        _list.SetLayoutManager(new LinearLayoutManager(context));
        _list.SetAdapter(_adapter);
        body.AddView(_list, new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent));

        _emptyText = new TextView(context)
        {
            Text = "No mods found.",
            Gravity = GravityFlags.Center
        };
        _emptyText.SetTextSize(ComplexUnitType.Sp, 16);
        body.AddView(_emptyText, new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent));

        InstallFab = new FloatingActionButton(context);
        InstallFab.SetImageResource(Android.Resource.Drawable.IcInputAdd);
        var fabLp = new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.WrapContent,
            ViewGroup.LayoutParams.WrapContent)
        {
            Gravity = GravityFlags.End | GravityFlags.Bottom
        };
        InstallFab.Click += InstallFab_Click;
        fabLp.SetMargins(
            Ui.Dp(context, 16), Ui.Dp(context, 16),
            Ui.Dp(context, 16), Ui.Dp(context, 16));
        body.AddView(InstallFab, fabLp);

        RefreshMods();
    }

    void RefreshMods()
    {
        //clear first
        _mods.Clear();

        try
        {
            var manifestFiles = new List<string>();
            Console.WriteLine("Start Refresh Mods..");
            ModTool.FindManifestFile(ModTool.ModsDir, manifestFiles);
            for (int i = 0; i < manifestFiles.Count; i++)
            {
                var mod = new ModItemView(manifestFiles[i], i);
                _mods.Add(mod);
            }
        }
        catch (Exception ex)
        {
            ErrorDialogTool.Show(ex);
        }

        //refresh
        _adapter.NotifyDataSetChanged();

        _countText.Text = $"Found Mods: {_mods.Count}";
        bool empty = _mods.Count == 0;
        _emptyText.Visibility = empty ? ViewStates.Visible : ViewStates.Gone;
        _list.Visibility = empty ? ViewStates.Gone : ViewStates.Visible;
    }

    private void OnClickedModItem(ModItemView mod, View anchor)
    {
        var popup = new Android.Widget.PopupMenu(_context, anchor);
        popup.Menu!.Add(0, 1, 0, "Open Folder");
        popup.Menu.Add(0, 2, 1, "Delete");

        popup.MenuItemClick += (s, e) =>
        {
            switch (e.Item!.ItemId)
            {
                case 1: OpenModFolder(mod); break;
                case 2: OnClickDeleteMod(mod); break;
            }
        };

        popup.Show();
    }

    private void OpenModFolder(ModItemView mod)
    {
        try
        {
            FileTool.OpenAppFiles(mod.modFolderPath);
        }
        catch (Exception)
        {
            Toast.MakeText(_context, "Failed to open mod folder", ToastLength.Short)!.Show();
        }
    }

    private void OnClickDeleteMod(ModItemView mod)
    {
        new AlertDialog.Builder(_context)
            .SetTitle("Delete Mod!")
            .SetMessage($"Do you want delete \"{mod.NameUIText}\"?")
            .SetPositiveButton("Yes", (s, e) =>
            {
                if (ModInstaller.TryDeleteMod(mod.modFolderPath))
                {
                    RefreshMods();
                }
            })
            .SetNegativeButton("No", (s, e) => { })
            .Show();
    }

    private void InstallFab_Click(object sender, System.EventArgs e)
    {
        ModInstaller.OnClickInstallMod(() =>
        {
            RefreshMods();
        });
    }

    private void OnToggledModItem(ModItemView mod)
    {
        mod.Enabled = !mod.Enabled;
    }
}
